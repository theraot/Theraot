#if NET20 || NET30 || NET35

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
#if NET35
    using System.Collections.Generic;

    // Code by Matt Warren from "LINQ: Building an IQueryable Provider - Part II"

    public abstract partial class ExpressionVisitor
    {
        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        public virtual Expression Visit(Expression node)
        {
            if (node == null)
                return null;
            switch (node.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)node);

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)node);

                case ExpressionType.TypeIs:
                    return VisitTypeBinary((TypeBinaryExpression)node);

                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)node);

                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)node);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)node);

                case ExpressionType.MemberAccess:
                    return VisitMember((MemberExpression)node);

                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)node);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)node);

                case ExpressionType.New:
                    return VisitNew((NewExpression)node);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)node);

                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)node);

                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)node);

                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)node);

                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", node.NodeType));
            }
        }

        /// <summary>
        /// Visits the children of the <see cref="BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var conversion = VisitAndConvert(node.Conversion, "VisitBinary");
            var right = Visit(node.Right);
            if (left == node.Left && right == node.Right && conversion == node.Conversion)
            {
                return node;
            }
            if (node.NodeType == ExpressionType.Coalesce && node.Conversion != null)
            {
                return Expression.Coalesce(left, right, conversion);
            }
            return Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
        }

        /// <summary>
        /// Visits the children of the <see cref="ConditionalExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitConditional(ConditionalExpression node)
        {
            var test = Visit(node.Test);
            var ifTrue = Visit(node.IfTrue);
            var ifFalse = Visit(node.IfFalse);
            if (test != node.Test || ifTrue != node.IfTrue || ifFalse != node.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="InvocationExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitInvocation(InvocationExpression node)
        {
            var args = VisitExpressionList(node.Arguments);
            var expr = Visit(node.Expression);
            if (args != node.Arguments || expr != node.Expression)
            {
                return Expression.Invoke(expr, args);
            }
            return node;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var p = Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (var j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return original;
        }

        /// <summary>
        /// Visits the children of the <see cref="LambdaExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitLambda(LambdaExpression node)
        {
            var body = Visit(node.Body);
            if (body != node.Body)
            {
                return Expression.Lambda(node.Type, body, node.Parameters);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitMember(MemberExpression node)
        {
            var exp = Visit(node.Expression);
            if (exp != node.Expression)
            {
                return Expression.MakeMemberAccess(exp, node.Member);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            var obj = Visit(node.Object);
            var args = VisitExpressionList(node.Arguments);
            if (obj != node.Object || args != node.Arguments)
            {
                return Expression.Call(obj, node.Method, args);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="NewArrayExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitNewArray(NewArrayExpression node)
        {
            var exprs = VisitExpressionList(node.Expressions);
            if (exprs != node.Expressions)
            {
                if (node.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(node.Type.GetElementType(), exprs);
                }
                return Expression.NewArrayBounds(node.Type.GetElementType(), exprs);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="NewExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitNew(NewExpression node)
        {
            var args = VisitExpressionList(node.Arguments);
            if (args != node.Arguments)
            {
                if (node.Members != null)
                {
                    return Expression.New(node.Constructor, args, node.Members);
                }
                return Expression.New(node.Constructor, args);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="TypeBinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            var expr = Visit(node.Expression);
            if (expr != node.Expression)
            {
                return Expression.TypeIs(expr, node.TypeOperand);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="UnaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand);
            if (operand != node.Operand)
            {
                return Expression.MakeUnary(node.NodeType, operand, node.Type, node.Method);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitMemberInit(MemberInitExpression node)
        {
            var n = (NewExpression)VisitNew(node.NewExpression);
            var bindings = VisitBindingList(node.Bindings);
            if (n != node.NewExpression || bindings != node.Bindings)
            {
                return Expression.MemberInit(n, bindings);
            }
            return node;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var b = VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);

                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);

                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);

                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// Visits the children of the <see cref="ListInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitListInit(ListInitExpression node)
        {
            var n = (NewExpression)VisitNew(node.NewExpression);
            var initializers = VisitElementInitializerList(node.Initializers);
            if (n != node.NewExpression || initializers != node.Initializers)
            {
                return Expression.ListInit(n, initializers);
            }
            return node;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                var init = VisitElementInit(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
            {
                return list;
            }
            return original;
        }

        /// <summary>
        /// Visits the children of the <see cref="ElementInit" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual ElementInit VisitElementInit(ElementInit node)
        {
            var arguments = VisitExpressionList(node.Arguments);
            if (arguments != node.Arguments)
            {
                return Expression.ElementInit(node.AddMethod, arguments);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberAssignment" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            var e = Visit(node.Expression);
            if (e != node.Expression)
            {
                return Expression.Bind(node.Member, e);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberMemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            var bindings = VisitBindingList(node.Bindings);
            if (bindings != node.Bindings)
            {
                return Expression.MemberBind(node.Member, bindings);
            }
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberListBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            var initializers = VisitElementInitializerList(node.Initializers);
            if (initializers != node.Initializers)
            {
                return Expression.ListBind(node.Member, initializers);
            }
            return node;
        }
    }

#else

    public abstract partial class ExpressionVisitor
    {
        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        public virtual Expression Visit(Expression node)
        {
            if (node == null)
            {
                return null;
            }

            return node.Accept(this);
        }

        /// <summary>
        /// Visits the children of the <see cref="BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitBinary(BinaryExpression node)
        {
            // Walk children in evaluation order: left, conversion, right
            return ValidateBinary(
                node,
                node.Update(
                    Visit(node.Left),
                    VisitAndConvert(node.Conversion, "VisitBinary"),
                    Visit(node.Right)
                )
            );
        }

        /// <summary>
        /// Visits the children of the <see cref="BlockExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitBlock(BlockExpression node)
        {
            var count = node.ExpressionCount;
            Expression[] nodes = null;
            for (int i = 0; i < count; i++)
            {
                var oldNode = node.GetExpression(i);
                var newNode = Visit(oldNode);

                if (oldNode != newNode)
                {
                    if (nodes == null)
                    {
                        nodes = new Expression[count];
                    }
                    nodes[i] = newNode;
                }
            }
            var v = VisitAndConvert(node.Variables, "VisitBlock");

            if (v == node.Variables && nodes == null)
            {
                return node;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (nodes[i] == null)
                    {
                        nodes[i] = node.GetExpression(i);
                    }
                }
            }

            return node.Rewrite(v, nodes);
        }

        /// <summary>
        /// Visits the children of the <see cref="ConditionalExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitConditional(ConditionalExpression node)
        {
            return node.Update(Visit(node.Test), Visit(node.IfTrue), Visit(node.IfFalse));
        }

        /// <summary>
        /// Visits the <see cref="DebugInfoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return node;
        }

        /// <summary>
        /// Visits the <see cref="DefaultExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitDefault(DefaultExpression node)
        {
            return node;
        }

        /// <summary>
        /// Visits the children of the extension expression.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        /// <remarks>
        /// This can be overridden to visit or rewrite specific extension nodes.
        /// If it is not overridden, this method will call <see cref="Expression.VisitChildren" />,
        /// which gives the node a chance to walk its children. By default,
        /// <see cref="Expression.VisitChildren" /> will try to reduce the node.
        /// </remarks>
        protected internal virtual Expression VisitExtension(Expression node)
        {
            return node.VisitChildren(this);
        }

        /// <summary>
        /// Visits the children of the <see cref="GotoExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitGoto(GotoExpression node)
        {
            return node.Update(VisitLabelTarget(node.Target), Visit(node.Value));
        }

        /// <summary>
        /// Visits the children of the <see cref="InvocationExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitInvocation(InvocationExpression node)
        {
            var e = Visit(node.Expression);
            var a = VisitArguments(node);
            if (e == node.Expression && a == null)
            {
                return node;
            }
            return node.Rewrite(e, a);
        }

        /// <summary>
        /// Visits the children of the <see cref="LabelExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitLabel(LabelExpression node)
        {
            return node.Update(VisitLabelTarget(node.Target), Visit(node.DefaultValue));
        }

        /// <summary>
        /// Visits the children of the <see cref="LambdaExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitLambda(LambdaExpression node)
        {
            return node.Update(Visit(node.Body), VisitAndConvert(node.Parameters, "VisitLambda"));
        }

        /// <summary>
        /// Visits the children of the <see cref="LoopExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitLoop(LoopExpression node)
        {
            return node.Update(VisitLabelTarget(node.BreakLabel), VisitLabelTarget(node.ContinueLabel), Visit(node.Body));
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitMember(MemberExpression node)
        {
            return node.Update(Visit(node.Expression));
        }

        /// <summary>
        /// Visits the children of the <see cref="IndexExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitIndex(IndexExpression node)
        {
            var o = Visit(node.Object);
            var a = VisitArguments(node);
            if (o == node.Object && a == null)
            {
                return node;
            }

            return node.Rewrite(o, a);
        }

        /// <summary>
        /// Visits the children of the <see cref="MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            var o = Visit(node.Object);
            var a = VisitArguments(node);
            if (o == node.Object && a == null)
            {
                return node;
            }

            return node.Rewrite(o, a);
        }

        /// <summary>
        /// Visits the children of the <see cref="NewArrayExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitNewArray(NewArrayExpression node)
        {
            return node.Update(Visit(node.Expressions));
        }

        /// <summary>
        /// Visits the children of the <see cref="NewExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitNew(NewExpression node)
        {
            return node.Update(Visit(node.Arguments));
        }

        /// <summary>
        /// Visits the children of the <see cref="RuntimeVariablesExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return node.Update(VisitAndConvert(node.Variables, "VisitRuntimeVariables"));
        }

        /// <summary>
        /// Visits the children of the <see cref="SwitchCase" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return node.Update(Visit(node.TestValues), Visit(node.Body));
        }

        /// <summary>
        /// Visits the children of the <see cref="SwitchExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitSwitch(SwitchExpression node)
        {
            return ValidateSwitch(
                node,
                node.Update(
                    Visit(node.SwitchValue),
                    Visit(node.Cases, VisitSwitchCase),
                    Visit(node.DefaultBody)
                )
            );
        }

        /// <summary>
        /// Visits the children of the <see cref="CatchBlock" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return node.Update(VisitAndConvert(node.Variable, "VisitCatchBlock"), Visit(node.Filter), Visit(node.Body));
        }

        /// <summary>
        /// Visits the children of the <see cref="TryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitTry(TryExpression node)
        {
            return node.Update(
                Visit(node.Body),
                Visit(node.Handlers, VisitCatchBlock),
                Visit(node.Finally),
                Visit(node.Fault)
            );
        }

        /// <summary>
        /// Visits the children of the <see cref="TypeBinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return node.Update(Visit(node.Expression));
        }

        /// <summary>
        /// Visits the children of the <see cref="UnaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitUnary(UnaryExpression node)
        {
            return ValidateUnary(node, node.Update(Visit(node.Operand)));
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitMemberInit(MemberInitExpression node)
        {
            return node.Update(
                VisitAndConvert(node.NewExpression, "VisitMemberInit"),
                Visit(node.Bindings, VisitMemberBinding)
            );
        }

        /// <summary>
        /// Visits the children of the <see cref="ListInitExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitListInit(ListInitExpression node)
        {
            return node.Update(
                VisitAndConvert(node.NewExpression, "VisitListInit"),
                Visit(node.Initializers, VisitElementInit)
            );
        }

        /// <summary>
        /// Visits the children of the <see cref="ElementInit" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual ElementInit VisitElementInit(ElementInit node)
        {
            return node.Update(Visit(node.Arguments));
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberAssignment" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return node.Update(Visit(node.Expression));
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberMemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return node.Update(Visit(node.Bindings, VisitMemberBinding));
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberListBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return node.Update(Visit(node.Initializers, VisitElementInit));
        }

        // We wouldn't need this if switch didn't infer the method.
        private static SwitchExpression ValidateSwitch(SwitchExpression before, SwitchExpression after)
        {
            GC.KeepAlive(before);
            // If we did not have a method, we don't want to bind to one,
            // it might not be the right thing.
            if (before.Comparison == null && after.Comparison != null)
            {
                throw Error.MustRewriteWithoutMethod(after.Comparison, "VisitSwitch");
            }
            return after;
        }
    }

#endif

    /// <summary>
    /// Represents a visitor or rewriter for expression trees.
    /// </summary>
    /// <remarks>
    /// This class is designed to be inherited to create more specialized
    /// classes whose functionality requires traversing, examining or copying
    /// an expression tree.
    /// </remarks>
    public abstract partial class ExpressionVisitor
    {
        /// <summary>
        /// Dispatches the list of expressions to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="nodes">The expressions to visit.</param>
        /// <returns>The modified expression list, if any of the elements were modified;
        /// otherwise, returns the original expression list.</returns>
        public ReadOnlyCollection<Expression> Visit(ReadOnlyCollection<Expression> nodes)
        {
            Expression[] newNodes = null;
            var n = nodes.Count;
            for (int i = 0; i < n; i++)
            {
                var node = Visit(nodes[i]);

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (!ReferenceEquals(node, nodes[i]))
                {
                    newNodes = new Expression[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes[j];
                    }
                    newNodes[i] = node;
                }
            }
            if (newNodes == null)
            {
                return nodes;
            }
            return new TrueReadOnlyCollection<Expression>(newNodes);
        }

        private Expression[] VisitArguments(IArgumentProvider nodes)
        {
            return ExpressionVisitorUtils.VisitArguments(this, nodes);
        }

        /// <summary>
        /// Visits all nodes in the collection using a specified element visitor.
        /// </summary>
        /// <typeparam name="T">The type of the nodes.</typeparam>
        /// <param name="nodes">The nodes to visit.</param>
        /// <param name="elementVisitor">A delegate that visits a single element,
        /// optionally replacing it with a new element.</param>
        /// <returns>The modified node list, if any of the elements were modified;
        /// otherwise, returns the original node list.</returns>
        internal static ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor)
        {
            // NOTICE this method has no null check
            T[] newNodes = null;
            var n = nodes.Count;
            for (int i = 0; i < n; i++)
            {
                var node = elementVisitor(nodes[i]);
                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (!ReferenceEquals(node, nodes[i]))
                {
                    newNodes = new T[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes[j];
                    }
                    newNodes[i] = node;
                }
            }
            if (newNodes == null)
            {
                return nodes;
            }
            return new TrueReadOnlyCollection<T>(newNodes);
        }

        /// <summary>
        /// Visits an expression, casting the result back to the original expression type.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        /// <exception cref="InvalidOperationException">The visit method for this node returned a different type.</exception>
        public T VisitAndConvert<T>(T node, string callerName) where T : Expression
        {
            if (node == null)
            {
                return null;
            }
            node = Visit(node) as T;
            if (node == null)
            {
                throw Error.MustRewriteToSameNode(callerName, typeof(T), callerName);
            }
            return node;
        }

        /// <summary>
        /// Visits an expression, casting the result back to the original expression type.
        /// </summary>
        /// <typeparam name="T">The type of the expression.</typeparam>
        /// <param name="nodes">The expression to visit.</param>
        /// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        /// <exception cref="InvalidOperationException">The visit method for this node returned a different type.</exception>
        public ReadOnlyCollection<T> VisitAndConvert<T>(ReadOnlyCollection<T> nodes, string callerName) where T : Expression
        {
            T[] newNodes = null;
            var n = nodes.Count;
            for (int i = 0; i < n; i++)
            {
                var node = Visit(nodes[i]) as T;
                if (node == null)
                {
                    throw Error.MustRewriteToSameNode(callerName, typeof(T), callerName);
                }

                if (newNodes != null)
                {
                    newNodes[i] = node;
                }
                else if (!ReferenceEquals(node, nodes[i]))
                {
                    newNodes = new T[n];
                    for (int j = 0; j < i; j++)
                    {
                        newNodes[j] = nodes[j];
                    }
                    newNodes[i] = node;
                }
            }
            if (newNodes == null)
            {
                return nodes;
            }
            return new TrueReadOnlyCollection<T>(newNodes);
        }

        /// <summary>
        /// Visits the <see cref="ConstantExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        /// <summary>
        /// Visits the <see cref="LabelTarget" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="Expression&lt;T&gt;" />.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitLambda<T>(Expression<T> node)
        {
            return VisitLambda((LambdaExpression)node);
        }

        /// <summary>
        /// Visits the <see cref="ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected internal virtual Expression VisitParameter(ParameterExpression node)
        {
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberBinding" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected virtual MemberBinding VisitMemberBinding(MemberBinding node)
        {
            switch (node.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)node);

                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)node);

                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)node);

                default:
                    throw Error.UnhandledBindingType(node.BindingType);
            }
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
            if (before != after && before.Method == null)
            {
                if (after.Method != null)
                {
                    throw Error.MustRewriteWithoutMethod(after.Method, "VisitUnary");
                }

                // rethrow has null operand
                if (before.Operand != null && after.Operand != null)
                {
                    ValidateChildType(before.Operand.Type, after.Operand.Type, "VisitUnary");
                }
            }
            return after;
        }

        private static BinaryExpression ValidateBinary(BinaryExpression before, BinaryExpression after)
        {
            if (before != after && before.Method == null)
            {
                if (after.Method != null)
                {
                    throw Error.MustRewriteWithoutMethod(after.Method, "VisitBinary");
                }

                ValidateChildType(before.Left.Type, after.Left.Type, "VisitBinary");
                ValidateChildType(before.Right.Type, after.Right.Type, "VisitBinary");
            }
            return after;
        }

        // Value types must stay as the same type, otherwise it's now a
        // different operation, e.g. adding two doubles vs adding two ints.
        private static void ValidateChildType(Type before, Type after, string methodName)
        {
            if (before.IsValueType)
            {
                if (before == after)
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
            throw Error.MustRewriteChildToSameType(before, after, methodName);
        }
    }
}

#endif