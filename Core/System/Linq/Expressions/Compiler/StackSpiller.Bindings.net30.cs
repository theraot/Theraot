#if NET20 || NET30
#define FEATURE_CORECLR
#if FEATURE_CORECLR
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Compiler
{
    internal partial class StackSpiller
    {
        private abstract class BindingRewriter
        {
            protected readonly MemberBinding MemberBinding;
            protected RewriteAction RewriteAction;
            protected readonly StackSpiller StackSpiller;

            protected BindingRewriter(MemberBinding memberBinding, StackSpiller stackSpiller)
            {
                MemberBinding = memberBinding;
                StackSpiller = stackSpiller;
            }

            internal RewriteAction Action
            {
                get { return RewriteAction; }
            }

            internal abstract MemberBinding AsBinding();

            internal abstract Expression AsExpression(Expression target);

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
                }
                throw Error.UnhandledBinding();
            }
        }

        private class MemberMemberBindingRewriter : BindingRewriter
        {
            private readonly ReadOnlyCollection<MemberBinding> _bindings;
            private readonly BindingRewriter[] _bindingRewriters;

            internal MemberMemberBindingRewriter(MemberMemberBinding memberBinding, StackSpiller stackSpiller, Stack stack) :
                base(memberBinding, stackSpiller)
            {
                _bindings = memberBinding.Bindings;
                _bindingRewriters = new BindingRewriter[_bindings.Count];
                for (int i = 0; i < _bindings.Count; i++)
                {
                    var br = Create(_bindings[i], stackSpiller, stack);
                    RewriteAction |= br.Action;
                    _bindingRewriters[i] = br;
                }
            }

            internal override MemberBinding AsBinding()
            {
                switch (RewriteAction)
                {
                    case RewriteAction.None:
                        return MemberBinding;

                    case RewriteAction.Copy:
                        var newBindings = new MemberBinding[_bindings.Count];
                        for (int i = 0; i < _bindings.Count; i++)
                        {
                            newBindings[i] = _bindingRewriters[i].AsBinding();
                        }
                        return Expression.MemberBind(MemberBinding.Member, new TrueReadOnlyCollection<MemberBinding>(newBindings));
                }
                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                if (target.Type.IsValueType && MemberBinding.Member is Reflection.PropertyInfo)
                {
                    throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(MemberBinding.Member);
                }
                RequireNotRefInstance(target);

                var member = Expression.MakeMemberAccess(target, MemberBinding.Member);
                var memberTemp = StackSpiller.MakeTemp(member.Type);

                var block = new Expression[_bindings.Count + 2];
                block[0] = Expression.Assign(memberTemp, member);

                for (int i = 0; i < _bindings.Count; i++)
                {
                    var br = _bindingRewriters[i];
                    block[i + 1] = br.AsExpression(memberTemp);
                }

                // We need to copy back value types
                if (memberTemp.Type.IsValueType)
                {
                    block[_bindings.Count + 1] = Expression.Block(
                        typeof(void),
                        Expression.Assign(Expression.MakeMemberAccess(target, MemberBinding.Member), memberTemp)
                    );
                }
                else
                {
                    block[_bindings.Count + 1] = Expression.Empty();
                }
                return MakeBlock(block);
            }
        }

        private class ListBindingRewriter : BindingRewriter
        {
            private readonly ReadOnlyCollection<ElementInit> _inits;
            private readonly ChildRewriter[] _childRewriters;

            internal ListBindingRewriter(MemberListBinding memberBinding, StackSpiller stackSpiller, Stack stack) :
                base(memberBinding, stackSpiller)
            {
                _inits = memberBinding.Initializers;

                _childRewriters = new ChildRewriter[_inits.Count];
                for (int i = 0; i < _inits.Count; i++)
                {
                    var init = _inits[i];

                    var cr = new ChildRewriter(stackSpiller, stack, init.Arguments.Count);
                    cr.Add(init.Arguments);

                    RewriteAction |= cr.Action;
                    _childRewriters[i] = cr;
                }
            }

            internal override MemberBinding AsBinding()
            {
                switch (RewriteAction)
                {
                    case RewriteAction.None:
                        return MemberBinding;

                    case RewriteAction.Copy:
                        var newInits = new ElementInit[_inits.Count];
                        for (int i = 0; i < _inits.Count; i++)
                        {
                            var cr = _childRewriters[i];
                            if (cr.Action == RewriteAction.None)
                            {
                                newInits[i] = _inits[i];
                            }
                            else
                            {
                                newInits[i] = Expression.ElementInit(_inits[i].AddMethod, cr[0, -1]);
                            }
                        }
                        return Expression.ListBind(MemberBinding.Member, new TrueReadOnlyCollection<ElementInit>(newInits));
                }
                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                if (target.Type.IsValueType && MemberBinding.Member is Reflection.PropertyInfo)
                {
                    throw Error.CannotAutoInitializeValueTypeElementThroughProperty(MemberBinding.Member);
                }
                RequireNotRefInstance(target);

                var member = Expression.MakeMemberAccess(target, MemberBinding.Member);
                var memberTemp = StackSpiller.MakeTemp(member.Type);

                var block = new Expression[_inits.Count + 2];
                block[0] = Expression.Assign(memberTemp, member);

                for (int i = 0; i < _inits.Count; i++)
                {
                    var cr = _childRewriters[i];
                    var add = cr.Finish(Expression.Call(memberTemp, _inits[i].AddMethod, cr[0, -1]));
                    block[i + 1] = add.Node;
                }

                // We need to copy back value types
                if (memberTemp.Type.IsValueType)
                {
                    block[_inits.Count + 1] = Expression.Block(
                        typeof(void),
                        Expression.Assign(Expression.MakeMemberAccess(target, MemberBinding.Member), memberTemp)
                    );
                }
                else
                {
                    block[_inits.Count + 1] = Expression.Empty();
                }
                return MakeBlock(block);
            }
        }

        private class MemberAssignmentRewriter : BindingRewriter
        {
            private readonly Expression _rhs;

            internal MemberAssignmentRewriter(MemberAssignment memberBinding, StackSpiller stackSpiller, Stack stack) :
                base(memberBinding, stackSpiller)
            {
                var result = stackSpiller.RewriteExpression(memberBinding.Expression, stack);
                RewriteAction = result.Action;
                _rhs = result.Node;
            }

            internal override MemberBinding AsBinding()
            {
                switch (RewriteAction)
                {
                    case RewriteAction.None:
                        return MemberBinding;

                    case RewriteAction.Copy:
                        return Expression.Bind(MemberBinding.Member, _rhs);
                }
                throw ContractUtils.Unreachable;
            }

            internal override Expression AsExpression(Expression target)
            {
                RequireNotRefInstance(target);

                var member = Expression.MakeMemberAccess(target, MemberBinding.Member);
                var memberTemp = StackSpiller.MakeTemp(member.Type);

                return MakeBlock(
                    Expression.Assign(memberTemp, _rhs),
                    Expression.Assign(member, memberTemp),
                    Expression.Empty()
                );
            }
        }
    }
}

#endif
#endif