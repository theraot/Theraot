#if LESSTHAN_NET35

#pragma warning disable CA1812 // Avoid uninstantiated internal classes

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AstUtils = System.Linq.Expressions.Utils;

namespace System.Dynamic
{
    /// <summary>
    ///     Represents a set of binding restrictions on the <see cref="DynamicMetaObject" /> under which the dynamic binding is
    ///     valid.
    /// </summary>
    [DebuggerTypeProxy(typeof(BindingRestrictionsProxy))]
    [DebuggerDisplay("{DebugView}")]
    public abstract class BindingRestrictions
    {

        /// <summary>
        ///     Represents an empty set of binding restrictions. This field is read-only.
        /// </summary>
        public static readonly BindingRestrictions Empty = new CustomRestriction(AstUtils.Constant(true));
        private const int _customRestrictionHash = 613566756;
        private const int _instanceRestrictionHash = -1840700270;
        private const int _typeRestrictionHash = 1227133513; // 00100 1001 0010 0100 1001 0010 0100 1001₂
        // 01001 0010 0100 1001 0010 0100 1001 0010₂
        // 10010 0100 1001 0010 0100 1001 0010 0100₂

        private BindingRestrictions()
        {
        }

        private string DebugView => ToExpression().ToString();

        /// <summary>
        ///     Combines binding restrictions from the list of <see cref="DynamicMetaObject" /> instances into one set of
        ///     restrictions.
        /// </summary>
        /// <param name="contributingObjects">
        ///     The list of <see cref="DynamicMetaObject" /> instances from which to combine
        ///     restrictions.
        /// </param>
        /// <returns>The new set of binding restrictions.</returns>
        public static BindingRestrictions Combine(IList<DynamicMetaObject> contributingObjects)
        {
            var res = Empty;
            return contributingObjects == null ? res : contributingObjects.Where(mo => mo != null).Aggregate(res, (current, mo) => current.Merge(mo.Restrictions));
        }

        /// <summary>
        ///     Creates the binding restriction that checks the expression for arbitrary immutable properties.
        /// </summary>
        /// <param name="expression">The expression expressing the restrictions.</param>
        /// <returns>The new binding restrictions.</returns>
        /// <remarks>
        ///     By convention, the general restrictions created by this method must only test
        ///     immutable object properties.
        /// </remarks>
        public static BindingRestrictions GetExpressionRestriction(Expression expression)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ContractUtils.Requires(expression.Type == typeof(bool), nameof(expression));
            return new CustomRestriction(expression);
        }

        /// <summary>
        ///     Creates the binding restriction that checks the expression for object instance identity.
        /// </summary>
        /// <param name="expression">The expression to test.</param>
        /// <param name="instance">The exact object instance to test.</param>
        /// <returns>The new binding restrictions.</returns>
        public static BindingRestrictions GetInstanceRestriction(Expression expression, object instance)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));

            return new InstanceRestriction(expression, instance);
        }

        /// <summary>
        ///     Creates the binding restriction that check the expression for runtime type identity.
        /// </summary>
        /// <param name="expression">The expression to test.</param>
        /// <param name="type">The exact type to test.</param>
        /// <returns>The new binding restrictions.</returns>
        public static BindingRestrictions GetTypeRestriction(Expression expression, Type type)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));

            return new TypeRestriction(expression, type);
        }

        /// <summary>
        ///     Merges the set of binding restrictions with the current binding restrictions.
        /// </summary>
        /// <param name="restrictions">The set of restrictions with which to merge the current binding restrictions.</param>
        /// <returns>The new set of binding restrictions.</returns>
        public BindingRestrictions Merge(BindingRestrictions restrictions)
        {
            ContractUtils.RequiresNotNull(restrictions, nameof(restrictions));
            if (this == Empty)
            {
                return restrictions;
            }

            return restrictions == Empty ? this : new MergedRestriction(this, restrictions);
        }

        /// <summary>
        ///     Creates the <see cref="Expression" /> representing the binding restrictions.
        /// </summary>
        /// <returns>The expression tree representing the restrictions.</returns>
        public Expression ToExpression()
        {
            return GetExpression();
        }

        internal static BindingRestrictions GetTypeRestriction(DynamicMetaObject obj)
        {
            Debug.Assert(obj != null);
            if (obj.Value == null && obj.HasValue)
            {
                return GetInstanceRestriction(obj.Expression, null);
            }

            return GetTypeRestriction(obj.Expression, obj.LimitType);
        }

        // Overridden by specialized subclasses
        internal abstract Expression GetExpression();

        private sealed class BindingRestrictionsProxy
        {
            private readonly BindingRestrictions _node;

            public BindingRestrictionsProxy(BindingRestrictions node)
            {
                ContractUtils.RequiresNotNull(node, nameof(node));
                _node = node;
            }

            // To prevent fxcop warning about this field
            public override string ToString()
            {
                return _node.DebugView;
            }
        }

        private sealed class CustomRestriction : BindingRestrictions
        {
            private readonly Expression _expression;

            internal CustomRestriction(Expression expression)
            {
                Debug.Assert(expression != null);
                _expression = expression;
            }

            public override bool Equals(object obj)
            {
                var other = obj as CustomRestriction;
                return other?._expression == _expression;
            }

            public override int GetHashCode()
            {
                return _customRestrictionHash ^ _expression.GetHashCode();
            }

            internal override Expression GetExpression()
            {
                return _expression;
            }
        }

        private sealed class InstanceRestriction : BindingRestrictions
        {
            private readonly Expression _expression;
            private readonly object _instance;

            internal InstanceRestriction(Expression parameter, object instance)
            {
                Debug.Assert(parameter != null);
                _expression = parameter;
                _instance = instance;
            }

            public override bool Equals(object obj)
            {
                return obj is InstanceRestriction other && other._expression == _expression && other._instance == _instance;
            }

            public override int GetHashCode()
            {
                return _instanceRestrictionHash ^ RuntimeHelpers.GetHashCode(_instance) ^ _expression.GetHashCode();
            }

            internal override Expression GetExpression()
            {
                if (_instance == null)
                {
                    return Expression.Equal
                    (
                        Expression.Convert(_expression, typeof(object)),
                        AstUtils.Null
                    );
                }

                var temp = Expression.Parameter(typeof(object), null);
                return Expression.Block
                (
                    ReadOnlyCollectionEx.Create(temp),
                    ReadOnlyCollectionEx.Create<Expression>
                    (
#if ENABLEDYNAMICPROGRAMMING
                        Expression.Assign(
                            temp,
                            Expression.Property(
                                Expression.Constant(new WeakReference(_instance)),
                                typeof(WeakReference).GetProperty("Target")
                            )
                        ),
#else
                        Expression.Assign
                        (
                            temp,
                            Expression.Constant(_instance, typeof(object))
                        ),
#endif
                        Expression.AndAlso
                        (
                            //check that WeakReference was not collected.
                            Expression.NotEqual(temp, AstUtils.Null),
                            Expression.Equal
                            (
                                Expression.Convert(_expression, typeof(object)),
                                temp
                            )
                        )
                    )
                );
            }
        }

        private sealed class MergedRestriction : BindingRestrictions
        {
            private readonly BindingRestrictions _left;
            private readonly BindingRestrictions _right;

            internal MergedRestriction(BindingRestrictions left, BindingRestrictions right)
            {
                _left = left;
                _right = right;
            }

            internal override Expression GetExpression()
            {
                // We could optimize this better, e.g. common subexpression elimination
                // But for now, it's good enough.

                var testBuilder = new TestBuilder();

                // Visit the tree, left to right.
                // Use an explicit stack so we don't stack overflow.
                //
                // Left-most node is on top of the stack, so we always expand the
                // left most node each iteration.
                var stack = new Stack<BindingRestrictions>();
                BindingRestrictions top = this;
                for (; ; )
                {
                    if (top is MergedRestriction m)
                    {
                        stack.Push(m._right);
                        top = m._left;
                    }
                    else
                    {
                        testBuilder.Append(top);
                        if (stack.Count == 0)
                        {
                            return testBuilder.ToExpression();
                        }

                        top = stack.Pop();
                    }
                }
            }
        }

        /// <summary>
        ///     Builds a balanced tree of AndAlso nodes.
        ///     We do this so the compiler won't stack overflow if we have many
        ///     restrictions.
        /// </summary>
        private sealed class TestBuilder
        {
            private readonly Stack<AndNode> _tests = new Stack<AndNode>();
            private readonly HashSet<BindingRestrictions> _unique = new HashSet<BindingRestrictions>();

            internal void Append(BindingRestrictions restrictions)
            {
                if (_unique.Add(restrictions))
                {
                    Push(restrictions.GetExpression(), 0);
                }
            }

            internal Expression ToExpression()
            {
                var result = _tests.Pop().Node;
                while (_tests.Count > 0)
                {
                    result = Expression.AndAlso(_tests.Pop().Node, result);
                }

                return result;
            }

            private void Push(Expression node, int depth)
            {
                while (_tests.Count > 0 && _tests.Peek().Depth == depth)
                {
                    node = Expression.AndAlso(_tests.Pop().Node, node);
                    depth++;
                }

                _tests.Push(new AndNode { Node = node, Depth = depth });
            }

            private struct AndNode
            {
                internal int Depth;
                internal Expression Node;
            }
        }

        private sealed class TypeRestriction : BindingRestrictions
        {
            private readonly Expression _expression;
            private readonly Type _type;

            internal TypeRestriction(Expression parameter, Type type)
            {
                Debug.Assert(parameter != null);
                Debug.Assert(type != null);
                _expression = parameter;
                _type = type;
            }

            public override bool Equals(object obj)
            {
                return obj is TypeRestriction other && other._expression == _expression && TypeUtils.AreEquivalent(other._type, _type);
            }

            public override int GetHashCode()
            {
                return _typeRestrictionHash ^ _expression.GetHashCode() ^ _type.GetHashCode();
            }

#if NET35
            internal override Expression GetExpression()
            {
                return Expression.TypeIs(_expression, _type);
            }
#else

            internal override Expression GetExpression()
            {
                return Expression.TypeEqual(_expression, _type);
            }

#endif
        }
    }
}

#endif