#if NET20 || NET30 || NET35

using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
    internal abstract class ExpressionVisitor
    {
        protected virtual void Visit(Expression expression)
        {
            if (expression != null)
            {
                switch (expression.NodeType)
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
                        VisitUnary((UnaryExpression)expression);
                        break;

                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                    case ExpressionType.Power:
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
                        VisitBinary((BinaryExpression)expression);
                        break;

                    case ExpressionType.TypeIs:
                        VisitTypeIs((TypeBinaryExpression)expression);
                        break;

                    case ExpressionType.Conditional:
                        VisitConditional((ConditionalExpression)expression);
                        break;

                    case ExpressionType.Constant:
                        VisitConstant((ConstantExpression)expression);
                        break;

                    case ExpressionType.Parameter:
                        VisitParameter((ParameterExpression)expression);
                        break;

                    case ExpressionType.MemberAccess:
                        VisitMemberAccess((MemberExpression)expression);
                        break;

                    case ExpressionType.Call:
                        VisitMethodCall((MethodCallExpression)expression);
                        break;

                    case ExpressionType.Lambda:
                        VisitLambda((LambdaExpression)expression);
                        break;

                    case ExpressionType.New:
                        VisitNew((NewExpression)expression);
                        break;

                    case ExpressionType.NewArrayInit:
                    case ExpressionType.NewArrayBounds:
                        VisitNewArray((NewArrayExpression)expression);
                        break;

                    case ExpressionType.Invoke:
                        VisitInvocation((InvocationExpression)expression);
                        break;

                    case ExpressionType.MemberInit:
                        VisitMemberInit((MemberInitExpression)expression);
                        break;

                    case ExpressionType.ListInit:
                        VisitListInit((ListInitExpression)expression);
                        break;

                    default:
                        throw new ArgumentException(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
                }
            }
        }

        protected virtual void VisitBinary(BinaryExpression binary)
        {
            Visit(binary.Left);
            Visit(binary.Right);
            Visit(binary.Conversion);
        }

        protected virtual void VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    VisitMemberAssignment((MemberAssignment)binding);
                    break;

                case MemberBindingType.MemberBinding:
                    VisitMemberMemberBinding((MemberMemberBinding)binding);
                    break;

                case MemberBindingType.ListBinding:
                    VisitMemberListBinding((MemberListBinding)binding);
                    break;

                default:
                    throw new ArgumentException(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual void VisitBindingList(ReadOnlyCollection<MemberBinding> list)
        {
            VisitList(list, VisitBinding);
        }

        protected virtual void VisitConditional(ConditionalExpression conditional)
        {
            Visit(conditional.Test);
            Visit(conditional.IfTrue);
            Visit(conditional.IfFalse);
        }

        protected virtual void VisitConstant(ConstantExpression constant)
        {
            //Empty
        }

        protected virtual void VisitElementInitializer(ElementInit initializer)
        {
            VisitExpressionList(initializer.Arguments);
        }

        protected virtual void VisitElementInitializerList(ReadOnlyCollection<ElementInit> list)
        {
            VisitList(list, VisitElementInitializer);
        }

        protected virtual void VisitExpressionList(ReadOnlyCollection<Expression> list)
        {
            VisitList(list, Visit);
        }

        protected virtual void VisitInvocation(InvocationExpression invocation)
        {
            VisitExpressionList(invocation.Arguments);
            Visit(invocation.Expression);
        }

        protected virtual void VisitLambda(LambdaExpression lambda)
        {
            Visit(lambda.Body);
        }

        protected virtual void VisitList<T>(ReadOnlyCollection<T> list, Action<T> visitor)
        {
            foreach (T element in list)
            {
                visitor(element);
            }
        }

        protected virtual void VisitListInit(ListInitExpression init)
        {
            VisitNew(init.NewExpression);
            VisitElementInitializerList(init.Initializers);
        }

        protected virtual void VisitMemberAccess(MemberExpression member)
        {
            Visit(member.Expression);
        }

        protected virtual void VisitMemberAssignment(MemberAssignment assignment)
        {
            Visit(assignment.Expression);
        }

        protected virtual void VisitMemberInit(MemberInitExpression init)
        {
            VisitNew(init.NewExpression);
            VisitBindingList(init.Bindings);
        }

        protected virtual void VisitMemberListBinding(MemberListBinding binding)
        {
            VisitElementInitializerList(binding.Initializers);
        }

        protected virtual void VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            VisitBindingList(binding.Bindings);
        }

        protected virtual void VisitMethodCall(MethodCallExpression methodCall)
        {
            Visit(methodCall.Object);
            VisitExpressionList(methodCall.Arguments);
        }

        protected virtual void VisitNew(NewExpression nex)
        {
            VisitExpressionList(nex.Arguments);
        }

        protected virtual void VisitNewArray(NewArrayExpression newArray)
        {
            VisitExpressionList(newArray.Expressions);
        }

        protected virtual void VisitParameter(ParameterExpression parameter)
        {
            //Empty
        }

        protected virtual void VisitTypeIs(TypeBinaryExpression type)
        {
            Visit(type.Expression);
        }

        protected virtual void VisitUnary(UnaryExpression unary)
        {
            Visit(unary.Operand);
        }
    }
}

#endif