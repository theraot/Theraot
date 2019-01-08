#if LESSTHAN_NET40

// ExpressionTransformer.cs
//
// Authors:
//  Roei Erez (roeie@mainsoft.com)
//  Jb Evain (jbevain@novell.com)
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
    internal abstract class ExpressionTransformer
    {
        public Expression Transform(Expression expression)
        {
            return Visit(expression);
        }

        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return VisitUnary((UnaryExpression)exp);

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Power:
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
                    return VisitBinary((BinaryExpression)exp);

                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp);

                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp);

                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp);

                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp);

                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp);

                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp);

                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp);

                case ExpressionType.New:
                    return VisitNew((NewExpression)exp);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp);

                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);

                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp);

                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp);

                default:
                    throw new Exception($"Unhandled expression type: '{exp.NodeType}'");
            }
        }

        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            var left = Visit(b.Left);
            var right = Visit(b.Right);
            var conversion = Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                return b.NodeType == ExpressionType.Coalesce && b.Conversion != null ? Expression.Coalesce(left, right, conversion as LambdaExpression) : Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
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
                    throw new Exception($"Unhandled binding type '{binding.BindingType}'");
            }
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            return VisitList(original, VisitBinding);
        }

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            var test = Visit(c.Test);
            var ifTrue = Visit(c.IfTrue);
            var ifFalse = Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        protected virtual Expression VisitConstant(ConstantExpression constant)
        {
            return constant;
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.Arguments);
            return arguments != initializer.Arguments ? Expression.ElementInit(initializer.AddMethod, arguments) : initializer;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            return VisitList(original, VisitElementInitializer);
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            var list = VisitList(original, Visit);
            return list == null ? original : new ReadOnlyCollection<Expression>(list);
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            var args = VisitExpressionList(iv.Arguments);
            var expr = Visit(iv.Expression);
            return args != iv.Arguments || expr != iv.Expression ? Expression.Invoke(expr, args) : iv;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            var body = Visit(lambda.Body);
            return body != lambda.Body ? Expression.Lambda(lambda.Type, body, lambda.Parameters) : lambda;
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            var n = VisitNew(init.NewExpression);
            var initializers = VisitElementInitializerList(init.Initializers);
            return n != init.NewExpression || initializers != init.Initializers ? Expression.ListInit(n, initializers) : init;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            var exp = Visit(m.Expression);
            if (exp != m.Expression)
            {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            var e = Visit(assignment.Expression);
            return e != assignment.Expression ? Expression.Bind(assignment.Member, e) : assignment;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            var n = VisitNew(init.NewExpression);
            var bindings = VisitBindingList(init.Bindings);
            return n != init.NewExpression || bindings != init.Bindings ? Expression.MemberInit(n, bindings) : init;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.Initializers);
            return initializers != binding.Initializers ? Expression.ListBind(binding.Member, initializers) : binding;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.Bindings);
            return bindings != binding.Bindings ? Expression.MemberBind(binding.Member, bindings) : binding;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            var obj = Visit(methodCall.Object);
            var args = VisitExpressionList(methodCall.Arguments);
            if (obj != methodCall.Object || args != methodCall.Arguments)
            {
                return Expression.Call(obj, methodCall.Method, args);
            }
            return methodCall;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            var args = VisitExpressionList(nex.Arguments);
            return args != nex.Arguments ? nex.Members != null
                ? Expression.New(nex.Constructor, args, nex.Members)
                : Expression.New(nex.Constructor, args) : nex;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            var expressionList = VisitExpressionList(na.Expressions);
            if (expressionList != na.Expressions)
            {
                return na.NodeType == ExpressionType.NewArrayInit ? Expression.NewArrayInit(na.Type.GetElementType(), expressionList) : Expression.NewArrayBounds(na.Type.GetElementType(), expressionList);
            }
            return na;
        }

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            var expr = Visit(b.Expression);
            return expr != b.Expression ? Expression.TypeIs(expr, b.TypeOperand) : b;
        }

        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            var operand = Visit(u.Operand);
            return operand != u.Operand ? Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method) : u;
        }

        private static IList<TElement> VisitList<TElement>(ReadOnlyCollection<TElement> original, Func<TElement, TElement> visit)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (visit == null)
            {
                throw new ArgumentNullException(nameof(visit));
            }
#endif
            List<TElement> list = null;
            var count = original.Count;
            for (var index = 0; index < count; index++)
            {
                var element = visit(original[index]);
                if (list != null)
                {
                    list.Add(element);
                }
                else if (!EqualityComparer<TElement>.Default.Equals(element, original[index]))
                {
                    list = new List<TElement>(count);
                    for (var subIndex = 0; subIndex < index; subIndex++)
                    {
                        list.Add(original[subIndex]);
                    }
                    list.Add(element);
                }
            }
            return (IList<TElement>)list ?? original;
        }
    }
}

#endif