﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>Creates a <see cref="MemberInitExpression"/>.</summary>
        /// <returns>A <see cref="MemberInitExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.MemberInit"/> and the <see cref="MemberInitExpression.NewExpression"/> and <see cref="MemberInitExpression.Bindings"/> properties set to the specified values.</returns>
        /// <param name="newExpression">A <see cref="NewExpression"/> to set the <see cref="MemberInitExpression.NewExpression"/> property equal to.</param>
        /// <param name="bindings">An array of <see cref="MemberBinding"/> objects to use to populate the <see cref="MemberInitExpression.Bindings"/> collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="newExpression"/> or <paramref name="bindings"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type that <paramref name="newExpression"/>.Type represents.</exception>
        public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
        {
            return MemberInit(newExpression, (IEnumerable<MemberBinding>)bindings);
        }

        /// <summary>Creates a <see cref="MemberInitExpression"/>.</summary>
        /// <returns>A <see cref="MemberInitExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.MemberInit"/> and the <see cref="MemberInitExpression.NewExpression"/> and <see cref="MemberInitExpression.Bindings"/> properties set to the specified values.</returns>
        /// <param name="newExpression">A <see cref="NewExpression"/> to set the <see cref="MemberInitExpression.NewExpression"/> property equal to.</param>
        /// <param name="bindings">An <see cref="IEnumerable{T}"/> that contains <see cref="MemberBinding"/> objects to use to populate the <see cref="MemberInitExpression.Bindings"/> collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="newExpression"/> or <paramref name="bindings"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="MemberBinding.Member"/> property of an element of <paramref name="bindings"/> does not represent a member of the type that <paramref name="newExpression"/>.Type represents.</exception>
        public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            ContractUtils.RequiresNotNull(newExpression, nameof(newExpression));
            ContractUtils.RequiresNotNull(bindings, nameof(bindings));
            var bindingsArray = Theraot.Collections.Extensions.AsArrayInternal(bindings);
            ValidateMemberInitArgs(newExpression.Type, bindingsArray);
            return new MemberInitExpression(newExpression, bindingsArray);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents calling a constructor and initializing one or more members of the new object.
    /// </summary>
    [DebuggerTypeProxy(typeof(MemberInitExpressionProxy))]
    public sealed class MemberInitExpression : Expression
    {
        private readonly MemberBinding[] _bindings;
        private readonly ReadOnlyCollectionEx<MemberBinding> _bindingsAsReadOnlyCollection;

        internal MemberInitExpression(NewExpression newExpression, MemberBinding[] bindings)
        {
            NewExpression = newExpression;
            _bindings = bindings;
            _bindingsAsReadOnlyCollection = ReadOnlyCollectionEx.Create(_bindings);
        }

        /// <summary>Gets the bindings that describe how to initialize the members of the newly created object.</summary>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of <see cref="MemberBinding"/> objects which describe how to initialize the members.</returns>
        public ReadOnlyCollection<MemberBinding> Bindings => _bindingsAsReadOnlyCollection;

        /// <inheritdoc />
        /// <summary>
        /// Gets a value that indicates whether the expression tree node can be reduced.
        /// </summary>
        public override bool CanReduce => true;

        /// <summary>Gets the expression that represents the constructor call.</summary>
        /// <returns>A <see cref="Expressions.NewExpression"/> that represents the constructor call.</returns>
        public NewExpression NewExpression { get; }

        /// <inheritdoc />
        /// <summary>
        /// Returns the node type of this Expression. Extension nodes should return
        /// ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
        public override ExpressionType NodeType => ExpressionType.MemberInit;

        /// <inheritdoc />
        /// <summary>
        /// Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.
        /// </summary>
        /// <returns>The <see cref="T:System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type => NewExpression.Type;

        /// <inheritdoc />
        /// <summary>
        /// Reduces the <see cref="T:System.Linq.Expressions.MemberInitExpression" /> to a simpler expression.
        /// If CanReduce returns true, this should return a valid expression.
        /// This method is allowed to return another node which itself
        /// must be reduced.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            return ReduceMemberInit(NewExpression, Bindings, true);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="newExpression">The <see cref="NewExpression"/> property of the result.</param>
        /// <param name="bindings">The <see cref="Bindings"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public MemberInitExpression Update(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            if (newExpression == NewExpression && bindings != null && ExpressionUtils.SameElements(ref bindings, _bindings))
            {
                return this;
            }

            return MemberInit(newExpression, bindings);
        }

        internal static Expression ReduceListInit(
                    Expression listExpression, ReadOnlyCollection<ElementInit> initializers, bool keepOnStack)
        {
            var listVar = Variable(listExpression.Type);
            var count = initializers.Count;
            var block = new Expression[count + 2];
            block[0] = Assign(listVar, listExpression);
            for (var i = 0; i < count; i++)
            {
                var element = initializers[i];
                block[i + 1] = Call(listVar, element.AddMethod, element.Arguments);
            }

            block[count + 1] = keepOnStack ? (Expression)listVar : Utils.Empty;
            return Block(new[] { listVar }, block);
        }

        internal static Expression ReduceMemberBinding(ParameterExpression objVar, MemberBinding binding)
        {
            var member = MakeMemberAccess(objVar, binding.Member);
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return Assign(member, ((MemberAssignment)binding).Expression);

                case MemberBindingType.ListBinding:
                    return ReduceListInit(member, ((MemberListBinding)binding).Initializers, false);

                case MemberBindingType.MemberBinding:
                    return ReduceMemberInit(member, ((MemberMemberBinding)binding).Bindings, false);

                default: throw ContractUtils.Unreachable;
            }
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitMemberInit(this);
        }

        private static Expression ReduceMemberInit(
            Expression objExpression, ReadOnlyCollection<MemberBinding> bindings, bool keepOnStack)
        {
            var objVar = Variable(objExpression.Type);
            var count = bindings.Count;
            var block = new Expression[count + 2];
            block[0] = Assign(objVar, objExpression);
            for (var i = 0; i < count; i++)
            {
                block[i + 1] = ReduceMemberBinding(objVar, bindings[i]);
            }

            block[count + 1] = keepOnStack ? (Expression)objVar : Utils.Empty;
            return Block(new[] { objVar }, block);
        }
    }
}

#endif