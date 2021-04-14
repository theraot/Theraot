#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal partial class StackSpiller
    {
        private abstract class BindingRewriter
        {
            protected readonly MemberBinding Binding;
            protected readonly StackSpiller Spiller;

            protected BindingRewriter(MemberBinding binding, StackSpiller spiller)
            {
                Binding = binding;
                Spiller = spiller;
            }

            protected internal RewriteAction Action { get; protected set; }

            internal static BindingRewriter Create(MemberBinding binding, StackSpiller spiller, Stack stack)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        var assign = (MemberAssignment)binding;
                        return new MemberAssignmentRewriter(assign, spiller, stack);

                    case MemberBindingType.ListBinding:
                        var list = (MemberListBinding)binding;
                        return new ListBindingRewriter(list, spiller, stack);

                    case MemberBindingType.MemberBinding:
                        var member = (MemberMemberBinding)binding;
                        return new MemberMemberBindingRewriter(member, spiller, stack);

                    default:
                        break;
                }

                throw new ArgumentException("Unhandled binding", string.Empty);
            }

            internal abstract MemberBinding AsBinding();

            internal abstract Expression AsExpression(Expression target);

            protected void RequireNoValueProperty()
            {
                if (Binding.Member is PropertyInfo property && property.PropertyType.IsValueType)
                {
                    throw new InvalidOperationException($"Cannot auto initialize members of value type through property '{property}', use assignment instead");
                }
            }
        }

        private sealed class ListBindingRewriter : BindingRewriter
        {
            private readonly ChildRewriter[] _childRewriters;
            private readonly ReadOnlyCollection<ElementInit> _initializers;

            internal ListBindingRewriter(MemberListBinding binding, StackSpiller spiller, Stack stack) :
                base(binding, spiller)
            {
                _initializers = binding.Initializers;

                var count = _initializers.Count;
                _childRewriters = new ChildRewriter[count];

                for (var i = 0; i < count; i++)
                {
                    var init = _initializers[i];

                    var cr = new ChildRewriter(spiller, stack, init.Arguments.Count);
                    cr.Add(init.Arguments);

                    Action |= cr.Action;
                    _childRewriters[i] = cr;
                }
            }

            internal override MemberBinding AsBinding()
            {
                switch (Action)
                {
                    case RewriteAction.None:
                        return Binding;

                    case RewriteAction.Copy:
                        var count = _initializers.Count;
                        var newInitializer = new ElementInit[count];
                        for (var i = 0; i < count; i++)
                        {
                            var cr = _childRewriters[i];
                            if (cr.Action == RewriteAction.None)
                            {
                                newInitializer[i] = _initializers[i];
                            }
                            else
                            {
                                newInitializer[i] = new ElementInit(_initializers[i].AddMethod, cr[0, -1]);
                            }
                        }

                        return new MemberListBinding(Binding.Member, ReadOnlyCollectionEx.Create(newInitializer));

                    default:
                        break;
                }

                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                RequireNoValueProperty();

                Expression member = MemberExpression.Make(target, Binding.Member);
                Expression memberTemp = Spiller.MakeTemp(member.Type);

                var count = _initializers.Count;
                var block = new Expression[count + 2];
                block[0] = new AssignBinaryExpression(memberTemp, member);

                for (var i = 0; i < count; i++)
                {
                    var cr = _childRewriters[i];
                    var add = cr.Finish(new InstanceMethodCallExpressionN(_initializers[i].AddMethod, memberTemp, cr[0, -1]));
                    block[i + 1] = add.Node;
                }

                // We need to copy back value types
                if (memberTemp.Type.IsValueType)
                {
                    block[count + 1] = Expression.Block
                    (
                        typeof(void),
                        new AssignBinaryExpression(MemberExpression.Make(target, Binding.Member), memberTemp)
                    );
                }
                else
                {
                    block[count + 1] = Utils.Empty;
                }

                return MakeBlock(block);
            }
        }

        private sealed class MemberAssignmentRewriter : BindingRewriter
        {
            private readonly Expression _rhs;

            internal MemberAssignmentRewriter(MemberAssignment binding, StackSpiller spiller, Stack stack) :
                base(binding, spiller)
            {
                var result = spiller.RewriteExpression(binding.Expression, stack);
                Action = result.Action;
                _rhs = result.Node;
            }

            internal override MemberBinding AsBinding()
            {
                switch (Action)
                {
                    case RewriteAction.None:
                        return Binding;

                    case RewriteAction.Copy:
                        return new MemberAssignment(Binding.Member, _rhs);

                    default:
                        break;
                }

                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                Expression member = MemberExpression.Make(target, Binding.Member);
                Expression memberTemp = Spiller.MakeTemp(member.Type);

                return MakeBlock
                (
                    new AssignBinaryExpression(memberTemp, _rhs),
                    new AssignBinaryExpression(member, memberTemp),
                    Utils.Empty
                );
            }
        }

        private sealed class MemberMemberBindingRewriter : BindingRewriter
        {
            private readonly BindingRewriter[] _bindingRewriters;
            private readonly ReadOnlyCollection<MemberBinding> _bindings;

            internal MemberMemberBindingRewriter(MemberMemberBinding binding, StackSpiller spiller, Stack stack) :
                base(binding, spiller)
            {
                _bindings = binding.Bindings;

                var count = _bindings.Count;
                _bindingRewriters = new BindingRewriter[count];

                for (var i = 0; i < count; i++)
                {
                    var br = Create(_bindings[i], spiller, stack);
                    Action |= br.Action;
                    _bindingRewriters[i] = br;
                }
            }

            internal override MemberBinding AsBinding()
            {
                switch (Action)
                {
                    case RewriteAction.None:
                        return Binding;

                    case RewriteAction.Copy:
                        var count = _bindings.Count;
                        var newBindings = new MemberBinding[count];
                        for (var i = 0; i < count; i++)
                        {
                            newBindings[i] = _bindingRewriters[i].AsBinding();
                        }

                        return new MemberMemberBinding(Binding.Member, newBindings);

                    default:
                        break;
                }

                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                RequireNoValueProperty();

                Expression member = MemberExpression.Make(target, Binding.Member);
                Expression memberTemp = Spiller.MakeTemp(member.Type);

                var count = _bindings.Count;
                var block = new Expression[count + 2];
                block[0] = new AssignBinaryExpression(memberTemp, member);

                for (var i = 0; i < count; i++)
                {
                    var br = _bindingRewriters[i];
                    block[i + 1] = br.AsExpression(memberTemp);
                }

                // We need to copy back value types.
                if (memberTemp.Type.IsValueType)
                {
                    block[count + 1] = Expression.Block
                    (
                        typeof(void),
                        new AssignBinaryExpression(MemberExpression.Make(target, Binding.Member), memberTemp)
                    );
                }
                else
                {
                    block[count + 1] = Utils.Empty;
                }

                return MakeBlock(block);
            }
        }
    }
}

#endif