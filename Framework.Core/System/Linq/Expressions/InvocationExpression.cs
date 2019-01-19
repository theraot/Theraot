#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using Theraot.Collections;
using Theraot.Collections.Specialized;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression to the provided arguments.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <param name="arguments">
        /// An array of <see cref="Expression"/> objects
        /// that represent the arguments that the delegate or lambda expression is applied to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.-or-The <see cref="Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="arguments"/> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        public static InvocationExpression Invoke(Expression expression, params Expression[] arguments)
        {
            return Invoke(expression, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression to a list of argument expressions.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression to the provided arguments.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <param name="arguments">
        /// An <see cref="Collections.Generic.IEnumerable{TDelegate}"/> of <see cref="Expression"/> objects
        /// that represent the arguments that the delegate or lambda expression is applied to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.-or-The <see cref="Type"/> property of an element of <paramref name="arguments"/> is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="arguments"/> does not contain the same number of elements as the list of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
        {
            var argumentList = Theraot.Collections.Extensions.AsArray(arguments);

            switch (argumentList.Length)
            {
                case 0:
                    return Invoke(expression);

                case 1:
                    return Invoke(expression, argumentList[0]);

                case 2:
                    return Invoke(expression, argumentList[0], argumentList[1]);

                case 3:
                    return Invoke(expression, argumentList[0], argumentList[1], argumentList[2]);

                case 4:
                    return Invoke(expression, argumentList[0], argumentList[1], argumentList[2], argumentList[3]);

                case 5:
                    return Invoke(expression, argumentList[0], argumentList[1], argumentList[2], argumentList[3], argumentList[4]);
                default:
                    break;
            }

            ExpressionUtils.RequiresCanRead(expression, nameof(expression));

            var mi = GetInvokeMethod(expression);
            ValidateArgumentTypes(mi, ExpressionType.Invoke, ref argumentList, nameof(expression));
            return new InvocationExpressionN(expression, argumentList, mi.ReturnType);
        }

        /// <summary>
        /// Gets the delegate's Invoke method; used by InvocationExpression.
        /// </summary>
        /// <param name="expression">The expression to be invoked.</param>
        internal static MethodInfo GetInvokeMethod(Expression expression)
        {
            var delegateType = expression.Type;
            if (!expression.Type.IsSubclassOf(typeof(MulticastDelegate)))
            {
                var exprType = TypeUtils.FindGenericType(typeof(Expression<>), expression.Type);
                if (exprType == null)
                {
                    throw new ArgumentException($"Expression of type '{expression.Type}' cannot be invoked", nameof(expression));
                }
                delegateType = exprType.GetGenericArguments()[0];
            }

            return delegateType.GetInvokeMethod();
        }

        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression with no arguments.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        internal static InvocationExpression Invoke(Expression expression)
        {
            // COMPAT: This method is marked as non-public to avoid a gap between a 0-ary and 2-ary overload (see remark for the unary case below).

            ExpressionUtils.RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 0, pis);

            return new InvocationExpression0(expression, method.ReturnType);
        }

        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression to one argument expression.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression to the provided arguments.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="Expression"/> that represents the first argument.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.-or-The <see cref="Type"/> property of an argument expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0)
        {
            // COMPAT: This method is marked as non-public to ensure compile-time compatibility for Expression.Invoke(e, null).

            ExpressionUtils.RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 1, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0], nameof(expression), nameof(arg0));

            return new InvocationExpression1(expression, method.ReturnType, arg0);
        }

        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression to two argument expressions.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression to the provided arguments.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="Expression"/> that represents the first argument.
        /// </param>
        /// <param name="arg1">
        /// The <see cref="Expression"/> that represents the second argument.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.-or-The <see cref="Type"/> property of an argument expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 2, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0], nameof(expression), nameof(arg0));
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1], nameof(expression), nameof(arg1));

            return new InvocationExpression2(expression, method.ReturnType, arg0, arg1);
        }

        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression to three argument expressions.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression to the provided arguments.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="Expression"/> that represents the first argument.
        /// </param>
        /// <param name="arg1">
        /// The <see cref="Expression"/> that represents the second argument.
        /// </param>
        /// <param name="arg2">
        /// The <see cref="Expression"/> that represents the third argument.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.-or-The <see cref="Type"/> property of an argument expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.

            ExpressionUtils.RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 3, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0], nameof(expression), nameof(arg0));
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1], nameof(expression), nameof(arg1));
            arg2 = ValidateOneArgument(method, ExpressionType.Invoke, arg2, pis[2], nameof(expression), nameof(arg2));

            return new InvocationExpression3(expression, method.ReturnType, arg0, arg1, arg2);
        }

        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression to four argument expressions.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression to the provided arguments.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="Expression"/> that represents the first argument.
        /// </param>
        /// <param name="arg1">
        /// The <see cref="Expression"/> that represents the second argument.
        /// </param>
        /// <param name="arg2">
        /// The <see cref="Expression"/> that represents the third argument.
        /// </param>
        /// <param name="arg3">
        /// The <see cref="Expression"/> that represents the fourth argument.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.-or-The <see cref="Type"/> property of an argument expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.

            ExpressionUtils.RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 4, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0], nameof(expression), nameof(arg0));
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1], nameof(expression), nameof(arg1));
            arg2 = ValidateOneArgument(method, ExpressionType.Invoke, arg2, pis[2], nameof(expression), nameof(arg2));
            arg3 = ValidateOneArgument(method, ExpressionType.Invoke, arg3, pis[3], nameof(expression), nameof(arg3));

            return new InvocationExpression4(expression, method.ReturnType, arg0, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates an <see cref="InvocationExpression"/> that
        /// applies a delegate or lambda expression to five argument expressions.
        /// </summary>
        /// <returns>
        /// An <see cref="InvocationExpression"/> that
        /// applies the specified delegate or lambda expression to the provided arguments.
        /// </returns>
        /// <param name="expression">
        /// An <see cref="Expression"/> that represents the delegate
        /// or lambda expression to be applied.
        /// </param>
        /// <param name="arg0">
        /// The <see cref="Expression"/> that represents the first argument.
        /// </param>
        /// <param name="arg1">
        /// The <see cref="Expression"/> that represents the second argument.
        /// </param>
        /// <param name="arg2">
        /// The <see cref="Expression"/> that represents the third argument.
        /// </param>
        /// <param name="arg3">
        /// The <see cref="Expression"/> that represents the fourth argument.
        /// </param>
        /// <param name="arg4">
        /// The <see cref="Expression"/> that represents the fifth argument.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="expression"/>.Type does not represent a delegate type or an <see cref="Expression{TDelegate}"/>.-or-The <see cref="Type"/> property of an argument expression is not assignable to the type of the corresponding parameter of the delegate represented by <paramref name="expression"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// The number of arguments does not contain match the number of parameters for the delegate represented by <paramref name="expression"/>.</exception>
        internal static InvocationExpression Invoke(Expression expression, Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
        {
            // NB: This method is marked as non-public to avoid public API additions at this point.

            ExpressionUtils.RequiresCanRead(expression, nameof(expression));

            var method = GetInvokeMethod(expression);

            var pis = GetParametersForValidation(method, ExpressionType.Invoke);

            ValidateArgumentCount(method, ExpressionType.Invoke, 5, pis);

            arg0 = ValidateOneArgument(method, ExpressionType.Invoke, arg0, pis[0], nameof(expression), nameof(arg0));
            arg1 = ValidateOneArgument(method, ExpressionType.Invoke, arg1, pis[1], nameof(expression), nameof(arg1));
            arg2 = ValidateOneArgument(method, ExpressionType.Invoke, arg2, pis[2], nameof(expression), nameof(arg2));
            arg3 = ValidateOneArgument(method, ExpressionType.Invoke, arg3, pis[3], nameof(expression), nameof(arg3));
            arg4 = ValidateOneArgument(method, ExpressionType.Invoke, arg4, pis[4], nameof(expression), nameof(arg4));

            return new InvocationExpression5(expression, method.ReturnType, arg0, arg1, arg2, arg3, arg4);
        }
    }

    /// <summary>
    /// Represents an expression that applies a delegate or lambda expression to a list of argument expressions.
    /// </summary>
    [DebuggerTypeProxy(typeof(InvocationExpressionProxy))]
    public class InvocationExpression : Expression, IArgumentProvider
    {
        internal InvocationExpression(Expression expression, Type returnType)
        {
            Expression = expression;
            Type = returnType;
        }

        /// <summary>
        /// Gets the number of argument expressions of the node.
        /// </summary>
        public virtual int ArgumentCount => throw ContractUtils.Unreachable;

        /// <summary>
        /// Gets the arguments that the delegate or lambda expression is applied to.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments => GetOrMakeArguments();

        /// <summary>
        /// Gets the delegate or lambda expression to be applied.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Returns the node type of this Expression. Extension nodes should return
        /// ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> of the expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.Invoke;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression"/> represents.
        /// </summary>
        /// <returns>The <see cref="System.Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type { get; }

        internal LambdaExpression LambdaOperand => Expression.NodeType == ExpressionType.Quote
            ? (LambdaExpression)((UnaryExpression)Expression).Operand
            : Expression as LambdaExpression;

        /// <summary>
        /// Gets the argument expression with the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the argument expression to get.</param>
        /// <returns>The expression representing the argument at the specified <paramref name="index"/>.</returns>
        public virtual Expression GetArgument(int index)
        {
            throw ContractUtils.Unreachable;
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> property of the result.</param>
        /// <param name="arguments">The <see cref="Arguments"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public InvocationExpression Update(Expression expression, IEnumerable<Expression> arguments)
        {
            if (expression == Expression && arguments != null && ExpressionUtils.SameElements(ref arguments, Theraot.Collections.Extensions.AsArray(Arguments)))
            {
                return this;
            }

            return Invoke(expression, arguments);
        }

        internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            throw ContractUtils.Unreachable;
        }

        internal virtual InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            throw ContractUtils.Unreachable;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitInvocation(this);
        }
    }

    internal sealed class InvocationExpression0 : InvocationExpression
    {
        public InvocationExpression0(Expression lambda, Type returnType)
            : base(lambda, returnType)
        {
        }

        public override int ArgumentCount => 0;

        public override Expression GetArgument(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return EmptyCollection<Expression>.Instance;
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 0);

            return Invoke(lambda);
        }
    }

    internal sealed class InvocationExpression1 : InvocationExpression
    {
        private object _arg0;       // storage for the 1st argument or a read-only collection.  See IArgumentProvider

        public InvocationExpression1(Expression lambda, Type returnType, Expression arg0)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
        }

        public override int ArgumentCount => 1;

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<Expression>(_arg0);
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ExpressionUtils.ReturnReadOnly(this, ref _arg0);
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 1);

            if (arguments != null)
            {
                return Invoke(lambda, arguments[0]);
            }
            return Invoke(lambda, ExpressionUtils.ReturnObject<Expression>(_arg0));
        }
    }

    internal sealed class InvocationExpression2 : InvocationExpression
    {
        private readonly Expression _arg1;
        private object _arg0;               // storage for the 1st argument or a read-only collection.  See IArgumentProvider
                                            // storage for the 2nd argument

        public InvocationExpression2(Expression lambda, Type returnType, Expression arg0, Expression arg1)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
        }

        public override int ArgumentCount => 2;

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ExpressionUtils.ReturnReadOnly(this, ref _arg0);
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 2);

            if (arguments != null)
            {
                return Invoke(lambda, arguments[0], arguments[1]);
            }
            return Invoke(lambda, ExpressionUtils.ReturnObject<Expression>(_arg0), _arg1);
        }
    }

    internal sealed class InvocationExpression3 : InvocationExpression
    {
        private readonly Expression _arg1;

        // storage for the 2nd argument
        private readonly Expression _arg2;

        private object _arg0;               // storage for the 1st argument or a read-only collection.  See IArgumentProvider
                                            // storage for the 3rd argument

        public InvocationExpression3(Expression lambda, Type returnType, Expression arg0, Expression arg1, Expression arg2)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        public override int ArgumentCount => 3;

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                case 2: return _arg2;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ExpressionUtils.ReturnReadOnly(this, ref _arg0);
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 3);

            if (arguments != null)
            {
                return Invoke(lambda, arguments[0], arguments[1], arguments[2]);
            }
            return Invoke(lambda, ExpressionUtils.ReturnObject<Expression>(_arg0), _arg1, _arg2);
        }
    }

    internal sealed class InvocationExpression4 : InvocationExpression
    {
        private readonly Expression _arg1;

        // storage for the 2nd argument
        private readonly Expression _arg2;

        // storage for the 3rd argument
        private readonly Expression _arg3;

        private object _arg0;               // storage for the 1st argument or a read-only collection.  See IArgumentProvider
                                            // storage for the 4th argument

        public InvocationExpression4(Expression lambda, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        public override int ArgumentCount => 4;

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                case 2: return _arg2;
                case 3: return _arg3;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ExpressionUtils.ReturnReadOnly(this, ref _arg0);
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 4);

            if (arguments != null)
            {
                return Invoke(lambda, arguments[0], arguments[1], arguments[2], arguments[3]);
            }
            return Invoke(lambda, ExpressionUtils.ReturnObject<Expression>(_arg0), _arg1, _arg2, _arg3);
        }
    }

    internal sealed class InvocationExpression5 : InvocationExpression
    {
        private readonly Expression _arg1;

        // storage for the 2nd argument
        private readonly Expression _arg2;

        // storage for the 3rd argument
        private readonly Expression _arg3;

        // storage for the 4th argument
        private readonly Expression _arg4;

        private object _arg0;               // storage for the 1st argument or a read-only collection.  See IArgumentProvider
                                            // storage for the 5th argument

        public InvocationExpression5(Expression lambda, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3, Expression arg4)
            : base(lambda, returnType)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _arg4 = arg4;
        }

        public override int ArgumentCount => 5;

        public override Expression GetArgument(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<Expression>(_arg0);
                case 1: return _arg1;
                case 2: return _arg2;
                case 3: return _arg3;
                case 4: return _arg4;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return ExpressionUtils.ReturnReadOnly(this, ref _arg0);
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == 5);

            if (arguments != null)
            {
                return Invoke(lambda, arguments[0], arguments[1], arguments[2], arguments[3], arguments[4]);
            }
            return Invoke(lambda, ExpressionUtils.ReturnObject<Expression>(_arg0), _arg1, _arg2, _arg3, _arg4);
        }
    }

    internal sealed class InvocationExpressionN : InvocationExpression
    {
        private readonly Expression[] _arguments;
        private readonly ArrayReadOnlyCollection<Expression> _argumentsAsReadOnly;

        public InvocationExpressionN(Expression lambda, Expression[] arguments, Type returnType)
            : base(lambda, returnType)
        {
            _arguments = arguments;
            _argumentsAsReadOnly = ArrayReadOnlyCollection.Create(_arguments);
        }

        public override int ArgumentCount => _arguments.Length;

        public override Expression GetArgument(int index) => _arguments[index];

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return _argumentsAsReadOnly;
        }

        internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
        {
            Debug.Assert(lambda != null);
            Debug.Assert(arguments == null || arguments.Length == _arguments.Length);

            return Invoke(lambda, arguments ?? _arguments);
        }
    }
}

#endif