#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;

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
                        MemberAssignment assign = (MemberAssignment)binding;
                        return new MemberAssignmentRewriter(assign, spiller, stack);

                    case MemberBindingType.ListBinding:
                        MemberListBinding list = (MemberListBinding)binding;
                        return new ListBindingRewriter(list, spiller, stack);

                    case MemberBindingType.MemberBinding:
                        MemberMemberBinding member = (MemberMemberBinding)binding;
                        return new MemberMemberBindingRewriter(member, spiller, stack);
                }
                throw Error.UnhandledBinding();
            }

            internal abstract MemberBinding AsBinding();

            internal abstract Expression AsExpression(Expression target);

            protected void RequireNoValueProperty()
            {
                if (Binding.Member is PropertyInfo property && property.PropertyType.IsValueType)
                {
                    throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(property);
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

                int count = _initializers.Count;
                _childRewriters = new ChildRewriter[count];

                for (int i = 0; i < count; i++)
                {
                    ElementInit init = _initializers[i];

                    ChildRewriter cr = new ChildRewriter(spiller, stack, init.Arguments.Count);
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
                        int count = _initializers.Count;
                        ElementInit[] newInitializer = new ElementInit[count];
                        for (int i = 0; i < count; i++)
                        {
                            ChildRewriter cr = _childRewriters[i];
                            if (cr.Action == RewriteAction.None)
                            {
                                newInitializer[i] = _initializers[i];
                            }
                            else
                            {
                                newInitializer[i] = new ElementInit(_initializers[i].AddMethod, cr[0, -1]);
                            }
                        }
                        return new MemberListBinding(Binding.Member, new TrueReadOnlyCollection<ElementInit>(newInitializer));
                }
                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                RequireNoValueProperty();

                Expression member = MemberExpression.Make(target, Binding.Member);
                Expression memberTemp = Spiller.MakeTemp(member.Type);

                int count = _initializers.Count;
                Expression[] block = new Expression[count + 2];
                block[0] = new AssignBinaryExpression(memberTemp, member);

                for (int i = 0; i < count; i++)
                {
                    ChildRewriter cr = _childRewriters[i];
                    Result add = cr.Finish(new InstanceMethodCallExpressionN(_initializers[i].AddMethod, memberTemp, cr[0, -1]));
                    block[i + 1] = add.Node;
                }

                // We need to copy back value types
                if (memberTemp.Type.IsValueType)
                {
                    block[count + 1] = Expression.Block(
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
                Result result = spiller.RewriteExpression(binding.Expression, stack);
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
                }
                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                Expression member = MemberExpression.Make(target, Binding.Member);
                Expression memberTemp = Spiller.MakeTemp(member.Type);

                return MakeBlock(
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

                int count = _bindings.Count;
                _bindingRewriters = new BindingRewriter[count];

                for (int i = 0; i < count; i++)
                {
                    BindingRewriter br = Create(_bindings[i], spiller, stack);
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
                        int count = _bindings.Count;
                        MemberBinding[] newBindings = new MemberBinding[count];
                        for (int i = 0; i < count; i++)
                        {
                            newBindings[i] = _bindingRewriters[i].AsBinding();
                        }
                        return new MemberMemberBinding(Binding.Member, newBindings);
                }
                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                RequireNoValueProperty();

                Expression member = MemberExpression.Make(target, Binding.Member);
                Expression memberTemp = Spiller.MakeTemp(member.Type);

                int count = _bindings.Count;
                Expression[] block = new Expression[count + 2];
                block[0] = new AssignBinaryExpression(memberTemp, member);

                for (int i = 0; i < count; i++)
                {
                    BindingRewriter br = _bindingRewriters[i];
                    block[i + 1] = br.AsExpression(memberTemp);
                }

                // We need to copy back value types.
                if (memberTemp.Type.IsValueType)
                {
                    block[count + 1] = Expression.Block(
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