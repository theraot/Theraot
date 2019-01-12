#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using static System.Linq.Expressions.CachedReflectionInfo;

namespace System.Runtime.CompilerServices
{
    public partial class RuntimeOps
    {
        /// <summary>
        /// Combines two runtime variable lists and returns a new list.
        /// </summary>
        /// <param name="first">The first list.</param>
        /// <param name="second">The second list.</param>
        /// <param name="indexes">The index array indicating which list to get variables from.</param>
        /// <returns>The merged runtime variables.</returns>
        // [Obsolete("do not use this method", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static IRuntimeVariables MergeRuntimeVariables(IRuntimeVariables first, IRuntimeVariables second, int[] indexes)
        {
            return new MergedRuntimeVariables(first, second, indexes);
        }

        /// <summary>
        /// Quotes the provided expression tree.
        /// </summary>
        /// <param name="expression">The expression to quote.</param>
        /// <param name="hoistedLocals">The hoisted local state provided by the compiler.</param>
        /// <param name="locals">The actual hoisted local values.</param>
        /// <returns>The quoted expression.</returns>
        // [Obsolete("do not use this method", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static Expression Quote(Expression expression, object hoistedLocals, object[] locals)
        {
            Debug.Assert(hoistedLocals != null && locals != null);
            var quoter = new ExpressionQuoter((HoistedLocals)hoistedLocals, locals);
            return quoter.Visit(expression);
        }

        // Modifies a quoted Expression instance by changing hoisted variables and
        // parameters into hoisted local references. The variable's StrongBox is
        // burned as a constant, and all hoisted variables/parameters are rewritten
        // as indexing expressions.
        //
        // The behavior of Quote is intended to be like C# and VB expression quoting
        private sealed class ExpressionQuoter : ExpressionVisitor
        {
            private readonly object[] _locals;
            private readonly HoistedLocals _scope;

            // A stack of variables that are defined in nested scopes. We search
            // this first when resolving a variable in case a nested scope shadows
            // one of our variable instances.
            private readonly Stack<HashSet<ParameterExpression>> _shadowedVars = new Stack<HashSet<ParameterExpression>>();

            internal ExpressionQuoter(HoistedLocals scope, object[] locals)
            {
                _scope = scope;
                _locals = locals;
            }

            protected internal override Expression VisitBlock(BlockExpression node)
            {
                if (node.Variables.Count > 0)
                {
                    _shadowedVars.Push(new HashSet<ParameterExpression>(node.Variables));
                }
                var b = ExpressionVisitorUtils.VisitBlockExpressions(this, node);
                if (node.Variables.Count > 0)
                {
                    _shadowedVars.Pop();
                }
                if (b == null)
                {
                    return node;
                }
                return node.Rewrite(node.Variables, b);
            }

            protected internal override Expression VisitLambda<T>(Expression<T> node)
            {
                if (node.ParameterCount > 0)
                {
                    var parameters = new HashSet<ParameterExpression>();

                    for (int i = 0, n = node.ParameterCount; i < n; i++)
                    {
                        parameters.Add(node.GetParameter(i));
                    }

                    _shadowedVars.Push(parameters);
                }
                var b = Visit(node.Body);
                if (node.ParameterCount > 0)
                {
                    _shadowedVars.Pop();
                }
                if (b == node.Body)
                {
                    return node;
                }
                return node.Rewrite(b, parameters: null);
            }

            protected internal override Expression VisitParameter(ParameterExpression node)
            {
                var box = GetBox(node);
                if (box == null)
                {
                    return node;
                }
                return Expression.Field(Expression.Constant(box), "Value");
            }

            protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                var count = node.Variables.Count;
                var boxes = new List<IStrongBox>();
                var vars = new List<ParameterExpression>();
                var indexes = new int[count];
                for (var i = 0; i < indexes.Length; i++)
                {
                    var box = GetBox(node.Variables[i]);
                    if (box == null)
                    {
                        indexes[i] = vars.Count;
                        vars.Add(node.Variables[i]);
                    }
                    else
                    {
                        indexes[i] = -1 - boxes.Count;
                        boxes.Add(box);
                    }
                }

                // No variables were rewritten. Just return the original node
                if (boxes.Count == 0)
                {
                    return node;
                }

                var boxesConst = Expression.Constant(new RuntimeVariables(boxes.ToArray()), typeof(IRuntimeVariables));
                // All of them were rewritten. Just return the array as a constant
                if (vars.Count == 0)
                {
                    return boxesConst;
                }

                // Otherwise, we need to return an object that merges them
                return Expression.Call(
                    RuntimeOpsMergeRuntimeVariables,
                    Expression.RuntimeVariables(new TrueReadOnlyCollection<ParameterExpression>(vars.ToArray())),
                    boxesConst,
                    Expression.Constant(indexes)
                );
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Variable != null)
                {
                    _shadowedVars.Push(new HashSet<ParameterExpression> { node.Variable });
                }
                var b = Visit(node.Body);
                var f = Visit(node.Filter);
                if (node.Variable != null)
                {
                    _shadowedVars.Pop();
                }
                if (b == node.Body && f == node.Filter)
                {
                    return node;
                }
                return Expression.MakeCatchBlock(node.Test, node.Variable, b, f);
            }

            private IStrongBox GetBox(ParameterExpression variable)
            {
                // Skip variables that are shadowed by a nested scope/lambda
                foreach (var hidden in _shadowedVars)
                {
                    if (hidden.Contains(variable))
                    {
                        return null;
                    }
                }

                var scope = _scope;
                var locals = _locals;
                while (true)
                {
                    if (scope.Indexes.TryGetValue(variable, out var hoistIndex))
                    {
                        return (IStrongBox)locals[hoistIndex];
                    }
                    scope = scope.Parent;
                    if (scope == null)
                    {
                        break;
                    }
                    locals = HoistedLocals.GetParent(locals);
                }

                // Unbound variable: an error should've been thrown already
                // from VariableBinder
                throw ContractUtils.Unreachable;
            }
        }
    }
}

#endif