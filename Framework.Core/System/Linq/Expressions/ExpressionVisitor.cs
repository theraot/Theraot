#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
    /// <summary>
    ///     Represents a visitor or rewriter for expression trees.
    /// </summary>
    /// <remarks>
    ///     This class is designed to be inherited to create more specialized
    ///     classes whose functionality requires traversing, examining or copying
    ///     an expression tree.
    /// </remarks>
    public abstract class ExpressionVisitor
    {
        /// <summary>
        ///     Visits all nodes in the collection using a specified element visitor.
        /// </summary>
        /// <typeparam name="T">The type of the nodes.</typeparam>
        /// <param name="nodes">The nodes to visit.</param>
        /// <param name="elementVisitor">
        ///     A delegate that visits a single element,
        ///     optionally replacing it with a new element.
        /// </param>
        /// <returns>
        ///     The modified node list, if any of the elements were modified;
        ///     otherwise, returns the original node list.
        /// </returns>
        public static ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor)
        {
            ContractUtils.RequiresNotNull(nodes, nameof(nodes));
            if (elementVisitor == null)
            {
                throw new ArgumentNullException(nameof(elementVisitor));
            }

            T[]? newNodes = null;
            for (int i = 0, n = nodes.Count; i < n; i++)
            {
                var node = elementVisitor(nodes[i]);
                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (!ReferenceEquals(node, nodes[i]))
                {
                    newNodes = new T[n];
                    for (var j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes[j];
                    }

                    newNodes[i] = node;
                }
            }

            return newNodes == null ? nodes : ReadOnlyCollectionEx.Create(newNodes);
        }

        /// <summary>
        ///     Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        [return: NotNullIfNotNull("node")]
        public virtual Expression? Visit(Expression? node)
        {
            return node?.Accept(this);
        }

        /// <summary>
        ///     Dispatches the list of expressions to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="nodes">The expressions to visit.</param>
        /// <returns>
        ///     The modified expression list, if any of the elements were modified;
        ///     otherwise, returns the original expression list.
        /// </returns>
        public ReadOnlyCollection<Expression> Visit(ReadOnlyCollection<Expression> nodes)
        {
            ContractUtils.RequiresNotNull(nodes, nameof(nodes));
            Expression[]? newNodes = null;
            for (int i = 0, n = nodes.Count; i < n; i++)
            {
                var node = Visit(nodes[i]);

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (node != nodes[i])
                {
                    newNodes = new Expression[n];
                    for (var j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes[j];
                    }

                    newNodes[i] = node;
                }
            }

            return newNodes == null ? nodes : ReadOnlyCollectionEx.Create(newNodes);
        }

        /// <summary>
        ///     Visits an expression, casting the result back to the original expression type.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        /// <exception cref="InvalidOperationException">The visit method for this node returned a different type.</exception>
        public T VisitAndConvert<T>([DisallowNull] T node, string callerName)
            where T : Expression
        {
            if (Visit(node) is T nodeAsT)
            {
                return nodeAsT;
            }

            throw new InvalidOperationException($"When called from '{callerName}', rewriting a node of type '{typeof(T)}' must return a non-null value of the same type. Alternatively, override '{callerName}' and change it to not visit children of this type.");
        }

        /// <summary>
        ///     Visits an expression, casting the result back to the original expression type.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="nodes">The expression to visit.</param>
        /// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        /// <exception cref="InvalidOperationException">The visit method for this node returned a different type.</exception>
        public ReadOnlyCollection<T> VisitAndConvert<T>(ReadOnlyCollection<T> nodes, string callerName) where T : Expression
        {
            ContractUtils.RequiresNotNull(nodes, nameof(nodes));
            T[]? newNodes = null;
            for (int i = 0, n = nodes.Count; i < n; i++)
            {
                if (!(Visit(nodes[i]) is T node))
                {
                    throw new InvalidOperationException($"When called from '{callerName}', rewriting a node of type '{typeof(T)}' must return a non-null value of the same type. Alternatively, override '{callerName}' and change it to not visit children of this type.");
                }

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (!ReferenceEquals(node, nodes[i]))
                {
                    newNodes = new T[n];
                    for (var j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes[j];
                    }

                    newNodes[i] = node;
                }
            }

            return newNodes == null ? nodes : ReadOnlyCollectionEx.Create(newNodes);
        }

        /// <summary>
        ///     Visits the children of the <see cref="BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitBinary(BinaryExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            // Walk children in evaluation order: left, conversion, right
            return VisitBinaryExtracted(node);
        }

        private Expression VisitBinaryExtracted(BinaryExpression node)
        {
            return ValidateBinary
            (
                node,
                node.Update
                (
                    Visit(node.Left),
                    node.Conversion == null ? null : VisitAndConvert(node.Conversion, nameof(VisitBinary)),
                    Visit(node.Right)
                )
            );
        }

        /// <summary>
        ///     Visits the children of the <see cref="BlockExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitBlock(BlockExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return VisitBlockExtracted(node);
        }

        private Expression VisitBlockExtracted(BlockExpression node)
        {
            var nodes = ExpressionVisitorUtils.VisitBlockExpressions(this, node);
            var v = VisitAndConvert(node.Variables, nameof(VisitBlock));

            if (v == node.Variables && nodes == null)
            {
                return node;
            }

            return node.Rewrite(v, nodes);
        }

        /// <summary>
        ///     Visits the children of the <see cref="ConditionalExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitConditional(ConditionalExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Test), Visit(node.IfTrue), Visit(node.IfFalse));
        }

        /// <summary>
        ///     Visits the <see cref="ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        /// <summary>
        ///     Visits the <see cref="DebugInfoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return node;
        }

        /// <summary>
        ///     Visits the <see cref="DefaultExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitDefault(DefaultExpression node)
        {
            return node;
        }

        /// <summary>
        ///     Visits the children of the <see cref="DynamicExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitDynamic(DynamicExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var a = VisitArguments(node);
            return a == null ? node : node.Rewrite(a);
        }

        /// <summary>
        ///     Visits the children of the extension expression.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        /// <remarks>
        ///     This can be overridden to visit or rewrite specific extension nodes.
        ///     If it is not overridden, this method will call <see cref="Expression.VisitChildren" />,
        ///     which gives the node a chance to walk its children. By default,
        ///     <see cref="Expression.VisitChildren" /> will try to reduce the node.
        /// </remarks>
        protected internal virtual Expression VisitExtension(Expression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.VisitChildren(this);
        }

        /// <summary>
        ///     Visits the children of the <see cref="GotoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitGoto(GotoExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(VisitLabelTarget(node.Target), Visit(node.Value));
        }

        /// <summary>
        ///     Visits the children of the <see cref="IndexExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitIndex(IndexExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var o = Visit(node.Object);
            var a = VisitArguments(node);
            if (o == node.Object && a == null)
            {
                return node;
            }

            return node.Rewrite(o, a);
        }

        /// <summary>
        ///     Visits the children of the <see cref="InvocationExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitInvocation(InvocationExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var e = Visit(node.Expression);
            var a = VisitArguments(node);
            if (e == node.Expression && a == null)
            {
                return node;
            }

            return node.Rewrite(e, a);
        }

        /// <summary>
        ///     Visits the children of the <see cref="LabelExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitLabel(LabelExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(VisitLabelTarget(node.Target), Visit(node.DefaultValue));
        }

        /// <summary>
        ///     Visits the children of the <see cref="Expression{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitLambda<T>(Expression<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var body = Visit(node.Body);
            var parameters = VisitParameters(node, nameof(VisitLambda));

            if (body == node.Body && parameters == null)
            {
                return node;
            }

            return node.Rewrite(body, parameters);
        }

        /// <summary>
        ///     Visits the children of the <see cref="ListInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitListInit(ListInitExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update
            (
                VisitAndConvert(node.NewExpression, nameof(VisitListInit)),
                Visit(node.Initializers, VisitElementInit)
            );
        }

        /// <summary>
        ///     Visits the children of the <see cref="LoopExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitLoop(LoopExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(VisitLabelTarget(node.BreakLabel), VisitLabelTarget(node.ContinueLabel), Visit(node.Body));
        }

        /// <summary>
        ///     Visits the children of the <see cref="MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitMember(MemberExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Expression));
        }

        /// <summary>
        ///     Visits the children of the <see cref="MemberInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitMemberInit(MemberInitExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update
            (
                VisitAndConvert(node.NewExpression, nameof(VisitMemberInit)),
                Visit(node.Bindings, VisitMemberBinding)
            );
        }

        /// <summary>
        ///     Visits the children of the <see cref="MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var o = Visit(node.Object);
            var a = VisitArguments(node);
            if (o == node.Object && a == null)
            {
                return node;
            }

            return node.Rewrite(o, a);
        }

        /// <summary>
        ///     Visits the children of the <see cref="NewExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitNew(NewExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var a = VisitArguments(node);
            return a == null ? node : node.Update(a);
        }

        /// <summary>
        ///     Visits the children of the <see cref="NewArrayExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitNewArray(NewArrayExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Expressions));
        }

        /// <summary>
        ///     Visits the <see cref="ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitParameter(ParameterExpression node)
        {
            return node;
        }

        /// <summary>
        ///     Visits the children of the <see cref="RuntimeVariablesExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(VisitAndConvert(node.Variables, nameof(VisitRuntimeVariables)));
        }

        /// <summary>
        ///     Visits the children of the <see cref="SwitchExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitSwitch(SwitchExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return VisitSwitchExtracted(node);
        }

        private Expression VisitSwitchExtracted(SwitchExpression node)
        {
            var visitedSwitchValue = node.SwitchValue;
            var visitedCases = Visit(node.Cases, VisitSwitchCase);
            var visitedDefaultBody = Visit(node.DefaultBody);
            var updated = node.Update(visitedSwitchValue, visitedCases, visitedDefaultBody);
            return ValidateSwitch(node, updated);
        }

        /// <summary>
        ///     Visits the children of the <see cref="TryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitTry(TryExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update
            (
                Visit(node.Body),
                Visit(node.Handlers, VisitCatchBlock),
                Visit(node.Finally),
                Visit(node.Fault)
            );
        }

        /// <summary>
        ///     Visits the children of the <see cref="TypeBinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Expression));
        }

        /// <summary>
        ///     Visits the children of the <see cref="UnaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected internal virtual Expression VisitUnary(UnaryExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return VisitUnaryExtracted(node);
        }

        private Expression VisitUnaryExtracted(UnaryExpression node)
        {
            var visited = Visit(node.Operand);
            var updated = node.Update(visited);
            return ValidateUnary(node, updated);
        }

        /// <summary>
        ///     Visits the children of the <see cref="CatchBlock" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual CatchBlock VisitCatchBlock(CatchBlock node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return VisitCatchBlockExtracted(node);
        }

        private CatchBlock VisitCatchBlockExtracted(CatchBlock node)
        {
            return node.Update(node.Variable == null ? null : VisitAndConvert(node.Variable, nameof(VisitCatchBlock)), Visit(node.Filter), Visit(node.Body));
        }

        /// <summary>
        ///     Visits the children of the <see cref="ElementInit" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual ElementInit VisitElementInit(ElementInit node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Arguments));
        }

        /// <summary>
        ///     Visits the <see cref="LabelTarget" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return node;
        }

        /// <summary>
        ///     Visits the children of the <see cref="MemberAssignment" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Expression));
        }

        /// <summary>
        ///     Visits the children of the <see cref="MemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual MemberBinding VisitMemberBinding(MemberBinding node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            switch (node.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)node);

                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)node);

                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)node);

                default:
                    throw new ArgumentException($"Unhandled Binding Type: {node.BindingType}");
            }
        }

        /// <summary>
        ///     Visits the children of the <see cref="MemberListBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Initializers, VisitElementInit));
        }

        /// <summary>
        ///     Visits the children of the <see cref="MemberMemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.Bindings, VisitMemberBinding));
        }

        /// <summary>
        ///     Visits the children of the <see cref="SwitchCase" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        protected virtual SwitchCase VisitSwitchCase(SwitchCase node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Update(Visit(node.TestValues), Visit(node.Body));
        }

        private static BinaryExpression ValidateBinary(BinaryExpression before, BinaryExpression after)
        {
            if (before == after || before.Method != null)
            {
                return after;
            }

            if (after.Method != null)
            {
                throw new InvalidOperationException($"Rewritten expression calls operator method '{after.Method}', but the original node had no operator method. If this is is intentional, override '{nameof(VisitBinary)}' and change it to allow this rewrite.");
            }

            ValidateChildType(before.Left.Type, after.Left.Type, nameof(VisitBinary));
            ValidateChildType(before.Right.Type, after.Right.Type, nameof(VisitBinary));
            return after;
        }

        // Value types must stay as the same type, otherwise it's now a
        // different operation, e.g. adding two doubles vs adding two ints.
        private static void ValidateChildType(Type before, Type after, string methodName)
        {
            if (before.IsValueType)
            {
                if (TypeUtils.AreEquivalent(before, after))
                {
                    // types are the same value type
                    return;
                }
            }
            else if (!after.IsValueType)
            {
                // both are reference types
                return;
            }

            // Otherwise, it's an invalid type change.
            throw new InvalidOperationException($"Rewriting child expression from type '{before}' to type '{after}' is not allowed, because it would change the meaning of the operation. If this is intentional, override '{methodName}' and change it to allow this rewrite.");
        }

        // We wouldn't need this if switch didn't infer the method.
        private static SwitchExpression ValidateSwitch(SwitchExpression before, SwitchExpression after)
        {
            // If we did not have a method, we don't want to bind to one,
            // it might not be the right thing.
            if (before.Comparison == null && after.Comparison != null)
            {
                throw new InvalidOperationException($"Rewritten expression calls operator method '{after.Comparison}', but the original node had no operator method. If this is is intentional, override '{nameof(VisitSwitch)}' and change it to allow this rewrite.");
            }

            return after;
        }

        //
        // Prevent some common cases of invalid rewrites.
        //
        // Essentially, we don't want the rewritten node to be semantically
        // bound by the factory, which may do the wrong thing. Instead we
        // require derived classes to be explicit about what they want to do if
        // types change.
        //
        private static UnaryExpression ValidateUnary(UnaryExpression before, UnaryExpression after)
        {
            if (before == after || before.Method != null)
            {
                return after;
            }

            if (after.Method != null)
            {
                throw new InvalidOperationException($"Rewritten expression calls operator method '{after.Method}', but the original node had no operator method. If this is is intentional, override '{nameof(VisitUnary)}' and change it to allow this rewrite.");
            }

            // rethrow has null operand
            if (before.Operand != null && after.Operand != null)
            {
                ValidateChildType(before.Operand.Type, after.Operand.Type, nameof(VisitUnary));
            }

            return after;
        }

        private Expression[]? VisitArguments(IArgumentProvider nodes)
        {
            return ExpressionVisitorUtils.VisitArguments(this, nodes);
        }

        private ParameterExpression[]? VisitParameters(IParameterProvider nodes, string callerName)
        {
            return ExpressionVisitorUtils.VisitParameters(this, nodes, callerName);
        }
    }
}

#endif

