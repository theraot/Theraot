#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions.Compiler;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Reflection;
using DelegateHelpers = System.Linq.Expressions.Compiler.DelegateHelpers;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Defines a <see cref="Expression{TDelegate}"/> node.
    /// This captures a block of code that is similar to a .NET method body.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <remarks>
    /// Lambda expressions take input through parameters and are expected to be fully bound.
    /// </remarks>
    public class Expression<TDelegate> : LambdaExpression
    {
        internal Expression(Expression body)
            : base(body)
        {
        }

        internal override Type PublicType => typeof(Expression<TDelegate>);
        internal sealed override Type TypeCore => typeof(TDelegate);

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public new TDelegate Compile()
        {
            return Compile(preferInterpretation: false);
        }

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <param name="preferInterpretation">A <see cref="bool"/> that indicates if the expression should be compiled to an interpreted form, if available.</param>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public new TDelegate Compile(bool preferInterpretation)
        {
#if FEATURE_INTERPRET
            if (preferInterpretation)
            {
                return (TDelegate)(object)new Interpreter.LightCompiler().CompileTop(this).CreateDelegate();
            }
#endif
            Theraot.No.Op(preferInterpretation);
            return (TDelegate)(object)LambdaCompiler.Compile(this);
        }

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <param name="debugInfoGenerator">Debugging information generator used by the compiler to mark sequence points and annotate local variables.</param>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public new TDelegate Compile(DebugInfoGenerator debugInfoGenerator)
        {
            Theraot.No.Op(debugInfoGenerator);
            return Compile();
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="body">The <see cref="LambdaExpression.Body" /> property of the result.</param>
        /// <param name="parameters">The <see cref="LambdaExpression.Parameters" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public Expression<TDelegate> Update(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == Body)
            {
                // Ensure parameters is safe to enumerate twice.
                // (If this means a second call to ToReadOnlyCollection it will return quickly).
                ICollection<ParameterExpression> pars;
                if (parameters == null)
                {
                    pars = null;
                }
                else
                {
                    pars = parameters as ICollection<ParameterExpression>;
                    if (pars == null)
                    {
                        parameters = pars = parameters.ToReadOnlyCollection();
                    }
                }

                if (SameParameters(pars))
                {
                    return this;
                }
            }

            return Lambda<TDelegate>(body, Name, TailCall, parameters);
        }

        internal static Expression<TDelegate> Create(Expression body, string name, bool tailCall, ParameterExpression[] parameters)
        {
            if (name == null && !tailCall)
            {
                switch (parameters.Length)
                {
                    case 0: return new Expression0<TDelegate>(body);
                    case 1: return new Expression1<TDelegate>(body, parameters[0]);
                    case 2: return new Expression2<TDelegate>(body, parameters[0], parameters[1]);
                    case 3: return new Expression3<TDelegate>(body, parameters[0], parameters[1], parameters[2]);
                    default: return new ExpressionN<TDelegate>(body, parameters);
                }
            }

            return new FullExpression<TDelegate>(body, name, tailCall, parameters);
        }

        internal override LambdaExpression AcceptStackSpiller(StackSpiller spiller)
        {
            return spiller.Rewrite(this);
        }

        internal virtual Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
        {
            throw ContractUtils.Unreachable;
        }

        internal virtual bool SameParameters(ICollection<ParameterExpression> parameters)
        {
            throw ContractUtils.Unreachable;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLambda(this);
        }
    }

    public partial class Expression
    {
        private enum TryGetFuncActionArgsResult
        {
            Valid,
            ArgumentNull,
            ByRef,
            PointerOrVoid
        }

        /// <summary>
        /// Creates a <see cref="System.Type"/> object that represents a generic System.Action delegate type that has specific type arguments.
        /// </summary>
        /// <param name="typeArgs">An array of <see cref="System.Type"/> objects that specify the type arguments for the System.Action delegate type.</param>
        /// <returns>The type of a System.Action delegate that has the specified type arguments.</returns>
        public static Type GetActionType(params Type[] typeArgs)
        {
            switch (ValidateTryGetFuncActionArgs(typeArgs))
            {
                case TryGetFuncActionArgsResult.ArgumentNull:
                    throw new ArgumentNullException(nameof(typeArgs));
                case TryGetFuncActionArgsResult.ByRef:
                    throw new ArgumentException("type must not be ByRef", nameof(typeArgs));
                default:

                    // This includes pointers or void. We allow the exception that comes
                    // from trying to use them as generic arguments to pass through.
                    var result = DelegateHelpers.GetActionType(typeArgs);
                    if (result == null)
                    {
                        throw new ArgumentException("An incorrect number of type args were specified for the declaration of an Action type.", nameof(typeArgs));
                    }

                    return result;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Type"/> object that represents a generic System.Func or System.Action delegate type that has specific type arguments.
        /// The last type argument determines the return type of the delegate. If no Func or Action is large enough, it will generate a custom
        /// delegate type.
        /// </summary>
        /// <param name="typeArgs">An array of <see cref="System.Type"/> objects that specify the type arguments of the delegate type.</param>
        /// <returns>The delegate type.</returns>
        /// <remarks>
        /// As with Func, the last argument is the return type. It can be set
        /// to <see cref="void"/> to produce an Action.</remarks>
        public static Type GetDelegateType(params Type[] typeArgs)
        {
            ContractUtils.RequiresNotEmpty(typeArgs, nameof(typeArgs));
            ContractUtils.RequiresNotNullItems(typeArgs, nameof(typeArgs));
            return DelegateHelpers.MakeDelegateType(typeArgs);
        }

        /// <summary>
        /// Creates a <see cref="System.Type"/> object that represents a generic System.Func delegate type that has specific type arguments.
        /// The last type argument specifies the return type of the created delegate.
        /// </summary>
        /// <param name="typeArgs">An array of <see cref="System.Type"/> objects that specify the type arguments for the System.Func delegate type.</param>
        /// <returns>The type of a System.Func delegate that has the specified type arguments.</returns>
        public static Type GetFuncType(params Type[] typeArgs)
        {
            switch (ValidateTryGetFuncActionArgs(typeArgs))
            {
                case TryGetFuncActionArgsResult.ArgumentNull:
                    throw new ArgumentNullException(nameof(typeArgs));
                case TryGetFuncActionArgsResult.ByRef:
                    throw new ArgumentException("type must not be ByRef", nameof(typeArgs));
                default:

                    // This includes pointers or void. We allow the exception that comes
                    // from trying to use them as generic arguments to pass through.
                    var result = DelegateHelpers.GetFuncType(typeArgs);
                    if (result == null)
                    {
                        throw new ArgumentException("An incorrect number of type args were specified for the declaration of a Func type.", nameof(typeArgs));
                    }

                    return result;
            }
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type.</typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
        {
            return Lambda<TDelegate>(body, false, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type.</typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, params ParameterExpression[] parameters)
        {
            return Lambda<TDelegate>(body, tailCall, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type.</typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda<TDelegate>(body, null, false, parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type.</typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda<TDelegate>(body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type.</typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="name">The name of the lambda. Used for generating debugging info.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda<TDelegate>(body, name, false, parameters);
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> where the delegate type is known at compile time.
        /// </summary>
        /// <typeparam name="TDelegate">The delegate type.</typeparam>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="name">The name of the lambda. Used for generating debugging info.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            var parameterList = Theraot.Collections.Extensions.AsArray(parameters);
            ValidateLambdaArgs(typeof(TDelegate), ref body, parameterList, nameof(TDelegate));
            return (Expression<TDelegate>)CreateLambda(typeof(TDelegate), body, name, tailCall, parameterList);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
        {
            return Lambda(body, false, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, bool tailCall, params ParameterExpression[] parameters)
        {
            return Lambda(body, tailCall, (IEnumerable<ParameterExpression>)parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(body, null, false, parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
        {
            return Lambda(delegateType, body, null, false, parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An array that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, params ParameterExpression[] parameters)
        {
            return Lambda(delegateType, body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(delegateType, body, null, false, parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(delegateType, body, null, tailCall, parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, string name, IEnumerable<ParameterExpression> parameters)
        {
            return Lambda(body, name, false, parameters);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            ContractUtils.RequiresNotNull(body, nameof(body));

            var parameterList = Theraot.Collections.Extensions.AsArray(parameters);

            var paramCount = parameterList.Length;
            var typeArgs = new Type[paramCount + 1];
            if (paramCount > 0)
            {
                var set = new HashSet<ParameterExpression>();
                for (var i = 0; i < paramCount; i++)
                {
                    var param = parameterList[i];
                    ContractUtils.RequiresNotNull(param, "parameter");
                    typeArgs[i] = param.IsByRef ? param.Type.MakeByRefType() : param.Type;
                    if (!set.Add(param))
                    {
                        throw new ArgumentException($"Found duplicate parameter '{param}'. Each ParameterExpression in the list must be a unique object.", i >= 0 ? $"{nameof(parameters)}[{i}]" : nameof(parameters));
                    }
                }
            }
            typeArgs[paramCount] = body.Type;

            var delegateType = DelegateHelpers.MakeDelegateType(typeArgs);

            return CreateLambda(delegateType, body, name, tailCall, parameterList);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, string name, IEnumerable<ParameterExpression> parameters)
        {
            var paramList = Theraot.Collections.Extensions.AsArray(parameters);
            ValidateLambdaArgs(delegateType, ref body, paramList, nameof(delegateType));

            return CreateLambda(delegateType, body, name, false, paramList);
        }

        /// <summary>
        /// Creates a <see cref="LambdaExpression"/> by first constructing a delegate type.
        /// </summary>
        /// <param name="delegateType">A <see cref="Type"/> representing the delegate signature for the lambda.</param>
        /// <param name="body">An <see cref="Expression"/> to set the <see cref="LambdaExpression.Body"/> property equal to.</param>
        /// <param name="name">The name for the lambda. Used for emitting debug information.</param>
        /// <param name="tailCall">A <see cref="bool"/> that indicates if tail call optimization will be applied when compiling the created expression.</param>
        /// <param name="parameters">An <see cref="IEnumerable{T}"/> that contains <see cref="ParameterExpression"/> objects to use to populate the <see cref="LambdaExpression.Parameters"/> collection.</param>
        /// <returns>A <see cref="LambdaExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.Lambda"/> and the <see cref="LambdaExpression.Body"/> and <see cref="LambdaExpression.Parameters"/> properties set to the specified values.</returns>
        public static LambdaExpression Lambda(Type delegateType, Expression body, string name, bool tailCall, IEnumerable<ParameterExpression> parameters)
        {
            var paramList = Theraot.Collections.Extensions.AsArray(parameters);
            ValidateLambdaArgs(delegateType, ref body, paramList, nameof(delegateType));

            return CreateLambda(delegateType, body, name, tailCall, paramList);
        }

        /// <summary>
        /// Creates a <see cref="System.Type"/> object that represents a generic System.Action delegate type that has specific type arguments.
        /// </summary>
        /// <param name="typeArgs">An array of <see cref="System.Type"/> objects that specify the type arguments for the System.Action delegate type.</param>
        /// <param name="actionType">When this method returns, contains the generic System.Action delegate type that has specific type arguments. Contains null if there is no generic System.Action delegate that matches the <paramref name="typeArgs"/>.This parameter is passed uninitialized.</param>
        /// <returns>true if generic System.Action delegate type was created for specific <paramref name="typeArgs"/>; false otherwise.</returns>
        public static bool TryGetActionType(Type[] typeArgs, out Type actionType)
        {
            if (ValidateTryGetFuncActionArgs(typeArgs) == TryGetFuncActionArgsResult.Valid)
            {
                return (actionType = DelegateHelpers.GetActionType(typeArgs)) != null;
            }

            actionType = null;
            return false;
        }

        /// <summary>
        /// Creates a <see cref="System.Type"/> object that represents a generic System.Func delegate type that has specific type arguments.
        /// The last type argument specifies the return type of the created delegate.
        /// </summary>
        /// <param name="typeArgs">An array of <see cref="System.Type"/> objects that specify the type arguments for the System.Func delegate type.</param>
        /// <param name="funcType">When this method returns, contains the generic System.Func delegate type that has specific type arguments. Contains null if there is no generic System.Func delegate that matches the <paramref name="typeArgs"/>.This parameter is passed uninitialized.</param>
        /// <returns>true if generic System.Func delegate type was created for specific <paramref name="typeArgs"/>; false otherwise.</returns>
        public static bool TryGetFuncType(Type[] typeArgs, out Type funcType)
        {
            if (ValidateTryGetFuncActionArgs(typeArgs) == TryGetFuncActionArgsResult.Valid)
            {
                return (funcType = DelegateHelpers.GetFuncType(typeArgs)) != null;
            }

            funcType = null;
            return false;
        }

        internal static LambdaExpression CreateLambda(Type delegateType, Expression body, string name, bool tailCall, ParameterExpression[] parameters)
        {
            // Get or create a delegate to the public Expression.Lambda<T>
            // method and call that will be used for creating instances of this
            // delegate type
            var factories = _lambdaFactories;
            if (factories == null)
            {
                _lambdaFactories = factories = new CacheDict<Type, Func<Expression, string, bool, ParameterExpression[], LambdaExpression>>(50);
            }

            if (!factories.TryGetValue(delegateType, out var fastPath))
            {
                var create = typeof(Expression<>).MakeGenericType(delegateType).GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic);
                /*if (delegateType.IsCollectible)
                {
                    return (LambdaExpression)create.Invoke(null, new object[] { body, name, tailCall, parameters });
                }*/
                factories[delegateType] = fastPath =
                    (Func<Expression, string, bool, ParameterExpression[], LambdaExpression>)create.CreateDelegate
                    (
                        typeof(Func<Expression, string, bool, ParameterExpression[], LambdaExpression>)
                    );
            }

            return fastPath(body, name, tailCall, parameters);
        }

        private static void ValidateLambdaArgs(Type delegateType, ref Expression body, ParameterExpression[] parameters, string paramName)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ExpressionUtils.RequiresCanRead(body, nameof(body));

            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType) || delegateType == typeof(MulticastDelegate))
            {
                throw new ArgumentException("Lambda type parameter must be derived from System.Delegate", paramName);
            }

            TypeUtils.ValidateType(delegateType, nameof(delegateType), allowByRef: true, allowPointer: true);

            var ldc = _lambdaDelegateCache;
            if (!ldc.TryGetValue(delegateType, out var mi))
            {
                mi = delegateType.GetInvokeMethod();
                /*if (!delegateType.IsCollectible)
                {
                    ldc[delegateType] = mi;
                }*/
            }

            var pis = mi.GetParameters();

            if (pis.Length > 0)
            {
                if (pis.Length != parameters.Length)
                {
                    throw new ArgumentException("Incorrect number of parameters supplied for lambda declaration");
                }
                var set = new HashSet<ParameterExpression>();
                for (int i = 0, n = pis.Length; i < n; i++)
                {
                    var pex = parameters[i];
                    var pi = pis[i];
                    ExpressionUtils.RequiresCanRead(pex, nameof(parameters), i);
                    var pType = pi.ParameterType;
                    if (pex.IsByRef)
                    {
                        if (!pType.IsByRef)
                        {
                            //We cannot pass a parameter of T& to a delegate that takes T or any non-ByRef type.
                            throw new ArgumentException($"ParameterExpression of type '{pex.Type.MakeByRefType()}' cannot be used for delegate parameter of type '{pType}'");
                        }
                        pType = pType.GetElementType();
                    }
                    if (!pex.Type.IsReferenceAssignableFromInternal(pType))
                    {
                        throw new ArgumentException($"ParameterExpression of type '{pex.Type}' cannot be used for delegate parameter of type '{pType}'");
                    }
                    if (!set.Add(pex))
                    {
                        throw new ArgumentException($"Found duplicate parameter '{pex}'. Each ParameterExpression in the list must be a unique object.", i >= 0 ? $"{nameof(parameters)}[{i}]" : nameof(parameters));
                    }
                }
            }
            else if (parameters.Length > 0)
            {
                throw new ArgumentException("Incorrect number of parameters supplied for lambda declaration");
            }
            if (mi.ReturnType != typeof(void) && !mi.ReturnType.IsReferenceAssignableFromInternal(body.Type) && !TryQuote(mi.ReturnType, ref body))
            {
                throw new ArgumentException($"Expression of type '{body.Type}' cannot be used for return type '{mi.ReturnType}'");
            }
        }

        private static TryGetFuncActionArgsResult ValidateTryGetFuncActionArgs(Type[] typeArgs)
        {
            if (typeArgs == null)
            {
                return TryGetFuncActionArgsResult.ArgumentNull;
            }

            foreach (var a in typeArgs)
            {
                if (a == null)
                {
                    return TryGetFuncActionArgsResult.ArgumentNull;
                }

                if (a.IsByRef)
                {
                    return TryGetFuncActionArgsResult.ByRef;
                }

                if (a == typeof(void) || a.IsPointer)
                {
                    return TryGetFuncActionArgsResult.PointerOrVoid;
                }
            }

            return TryGetFuncActionArgsResult.Valid;
        }
    }

    /// <summary>
    /// Creates a <see cref="LambdaExpression"/> node.
    /// This captures a block of code that is similar to a .NET method body.
    /// </summary>
    /// <remarks>
    /// Lambda expressions take input through parameters and are expected to be fully bound.
    /// </remarks>
    [DebuggerTypeProxy(typeof(LambdaExpressionProxy))]
    public abstract class LambdaExpression : Expression, IParameterProvider
    {
        internal LambdaExpression(Expression body)
        {
            Body = body;
        }

        /// <summary>
        /// Gets the body of the lambda expression.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// Gets the name of the lambda expression.
        /// </summary>
        /// <remarks>Used for debugging purposes.</remarks>
        public string Name => NameCore;

        /// <summary>
        /// Returns the node type of this <see cref="Expression"/>. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> that represents this expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.Lambda;

        /// <summary>
        /// Gets the parameters of the lambda expression.
        /// </summary>
        public ReadOnlyCollection<ParameterExpression> Parameters => GetOrMakeParameters();

        /// <summary>
        /// Gets the return type of the lambda expression.
        /// </summary>
        public Type ReturnType => Type.GetInvokeMethod().ReturnType;

        /// <summary>
        /// Gets the value that indicates if the lambda expression will be compiled with
        /// tail call optimization.
        /// </summary>
        public bool TailCall => TailCallCore;

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression"/> represents. (Inherited from <see cref="Expression"/>.)
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public sealed override Type Type => TypeCore;

        internal virtual string NameCore => null;

        internal virtual int ParameterCount => throw ContractUtils.Unreachable;

        internal abstract Type PublicType { get; }
        internal virtual bool TailCallCore => false;
        internal abstract Type TypeCore { get; }

        int IParameterProvider.ParameterCount => ParameterCount;

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public Delegate Compile()
        {
            return Compile(preferInterpretation: false);
        }

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <param name="preferInterpretation">A <see cref="bool"/> that indicates if the expression should be compiled to an interpreted form, if available.</param>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public Delegate Compile(bool preferInterpretation)
        {
#if FEATURE_INTERPRET
            if (preferInterpretation)
            {
                return new Interpreter.LightCompiler().CompileTop(this).CreateDelegate();
            }
#endif
            Theraot.No.Op(preferInterpretation);
            return LambdaCompiler.Compile(this);
        }

        /// <summary>
        /// Produces a delegate that represents the lambda expression.
        /// </summary>
        /// <param name="debugInfoGenerator">Debugging information generator used by the compiler to mark sequence points and annotate local variables.</param>
        /// <returns>A delegate containing the compiled version of the lambda.</returns>
        public Delegate Compile(DebugInfoGenerator debugInfoGenerator)
        {
            Theraot.No.Op(debugInfoGenerator);
            return Compile();
        }

        /// <summary>
        /// Compiles the lambda into a method definition.
        /// </summary>
        /// <param name="method">A <see cref="Reflection.Emit.MethodBuilder"/> which will be used to hold the lambda's IL.</param>
        public void CompileToMethod(Reflection.Emit.MethodBuilder method)
        {
            ContractUtils.RequiresNotNull(method, nameof(method));
            ContractUtils.Requires(method.IsStatic, nameof(method));
            if (!(method.DeclaringType is Reflection.Emit.TypeBuilder))
            {
                throw new ArgumentException("MethodBuilder does not have a valid TypeBuilder");
            }

            LambdaCompiler.Compile(this, method);
        }

        internal abstract LambdaExpression AcceptStackSpiller(StackSpiller spiller);

        internal virtual ReadOnlyCollection<ParameterExpression> GetOrMakeParameters()
        {
            throw ContractUtils.Unreachable;
        }

        internal virtual ParameterExpression GetParameter(int index)
        {
            throw ContractUtils.Unreachable;
        }

        ParameterExpression IParameterProvider.GetParameter(int index) => GetParameter(index);
    }

    //#if !FEATURE_COMPILE

    //    // Separate expression creation class to hide the CreateExpressionFunc function from users reflecting on Expression<T>
    //    public class ExpressionCreator<TDelegate>
    //    {
    //        public static LambdaExpression CreateExpressionFunc(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
    //        {
    //            if (name == null && !tailCall)
    //            {
    //                switch (parameters.Count)
    //                {
    //                    case 0: return new Expression0<TDelegate>(body);
    //                    case 1: return new Expression1<TDelegate>(body, parameters[0]);
    //                    case 2: return new Expression2<TDelegate>(body, parameters[0], parameters[1]);
    //                    case 3: return new Expression3<TDelegate>(body, parameters[0], parameters[1], parameters[2]);
    //                    default: return new ExpressionN<TDelegate>(body, parameters.ToArray());
    //                }
    //            }

    //            return new FullExpression<TDelegate>(body, name, tailCall, parameters.ToArray());
    //        }
    //    }

    //#endif

    internal sealed class Expression0<TDelegate> : Expression<TDelegate>
    {
        public Expression0(Expression body)
            : base(body)
        {
        }

        internal override int ParameterCount => 0;

        internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters() => EmptyCollection<ParameterExpression>.Instance;

        internal override ParameterExpression GetParameter(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
        {
            Debug.Assert(body != null);
            Debug.Assert(parameters == null || parameters.Length == 0);

            return Lambda<TDelegate>(body, parameters);
        }

        internal override bool SameParameters(ICollection<ParameterExpression> parameters) =>
                                    parameters == null || parameters.Count == 0;
    }

    internal sealed class Expression1<TDelegate> : Expression<TDelegate>
    {
        private object _par0;

        public Expression1(Expression body, ParameterExpression par0)
            : base(body)
        {
            _par0 = par0;
        }

        internal override int ParameterCount => 1;

        internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters() => ExpressionUtils.ReturnReadOnly(this, ref _par0);

        internal override ParameterExpression GetParameter(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<ParameterExpression>(_par0);
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
        {
            Debug.Assert(body != null);
            Debug.Assert(parameters == null || parameters.Length == 1);

            if (parameters != null)
            {
                return Lambda<TDelegate>(body, parameters);
            }

            return Lambda<TDelegate>(body, ExpressionUtils.ReturnObject<ParameterExpression>(_par0));
        }

        internal override bool SameParameters(ICollection<ParameterExpression> parameters)
        {
            if (parameters?.Count == 1)
            {
                using (var en = parameters.GetEnumerator())
                {
                    en.MoveNext();
                    return en.Current == ExpressionUtils.ReturnObject<ParameterExpression>(_par0);
                }
            }

            return false;
        }
    }

    internal sealed class Expression2<TDelegate> : Expression<TDelegate>
    {
        private object _par0;
        private readonly ParameterExpression _par1;

        public Expression2(Expression body, ParameterExpression par0, ParameterExpression par1)
            : base(body)
        {
            _par0 = par0;
            _par1 = par1;
        }

        internal override int ParameterCount => 2;

        internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters() => ExpressionUtils.ReturnReadOnly(this, ref _par0);

        internal override ParameterExpression GetParameter(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<ParameterExpression>(_par0);
                case 1: return _par1;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
        {
            Debug.Assert(body != null);
            Debug.Assert(parameters == null || parameters.Length == 2);

            if (parameters != null)
            {
                return Lambda<TDelegate>(body, parameters);
            }

            return Lambda<TDelegate>(body, ExpressionUtils.ReturnObject<ParameterExpression>(_par0), _par1);
        }

        internal override bool SameParameters(ICollection<ParameterExpression> parameters)
        {
            if (parameters?.Count == 2)
            {
                if (_par0 is ParameterExpression[] alreadyArray)
                {
                    return ExpressionUtils.SameElements(parameters, alreadyArray);
                }

                using (var en = parameters.GetEnumerator())
                {
                    en.MoveNext();
                    if (en.Current == _par0)
                    {
                        en.MoveNext();
                        return en.Current == _par1;
                    }
                }
            }

            return false;
        }
    }

    internal sealed class Expression3<TDelegate> : Expression<TDelegate>
    {
        private object _par0;
        private readonly ParameterExpression _par1;
        private readonly ParameterExpression _par2;

        public Expression3(Expression body, ParameterExpression par0, ParameterExpression par1, ParameterExpression par2)
            : base(body)
        {
            _par0 = par0;
            _par1 = par1;
            _par2 = par2;
        }

        internal override int ParameterCount => 3;

        internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters() => ExpressionUtils.ReturnReadOnly(this, ref _par0);

        internal override ParameterExpression GetParameter(int index)
        {
            switch (index)
            {
                case 0: return ExpressionUtils.ReturnObject<ParameterExpression>(_par0);
                case 1: return _par1;
                case 2: return _par2;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
        {
            Debug.Assert(body != null);
            Debug.Assert(parameters == null || parameters.Length == 3);

            if (parameters != null)
            {
                return Lambda<TDelegate>(body, parameters);
            }

            return Lambda<TDelegate>(body, ExpressionUtils.ReturnObject<ParameterExpression>(_par0), _par1, _par2);
        }

        internal override bool SameParameters(ICollection<ParameterExpression> parameters)
        {
            if (parameters?.Count == 3)
            {
                if (_par0 is ParameterExpression[] alreadyArray)
                {
                    return ExpressionUtils.SameElements(parameters, alreadyArray);
                }

                using (var en = parameters.GetEnumerator())
                {
                    en.MoveNext();
                    if (en.Current == _par0)
                    {
                        en.MoveNext();
                        if (en.Current == _par1)
                        {
                            en.MoveNext();
                            return en.Current == _par2;
                        }
                    }
                }
            }

            return false;
        }
    }

    internal class ExpressionN<TDelegate> : Expression<TDelegate>
    {
        private readonly ParameterExpression[] _parameters;
        private readonly ArrayReadOnlyCollection<ParameterExpression> _parametersAsReadOnlyCollection;

        public ExpressionN(Expression body, ParameterExpression[] parameters)
            : base(body)
        {
            _parameters = parameters;
            _parametersAsReadOnlyCollection = ArrayReadOnlyCollection.Create(_parameters);
        }

        internal override int ParameterCount => _parameters.Length;

        internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters() => _parametersAsReadOnlyCollection;

        internal override ParameterExpression GetParameter(int index) => _parameters[index];

        internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
        {
            Debug.Assert(body != null);
            Debug.Assert(parameters == null || parameters.Length == _parameters.Length);

            return Lambda<TDelegate>(body, Name, TailCall, parameters ?? _parameters);
        }

        internal override bool SameParameters(ICollection<ParameterExpression> parameters) =>
                    ExpressionUtils.SameElements(parameters, _parameters);
    }

    internal sealed class FullExpression<TDelegate> : ExpressionN<TDelegate>
    {
        public FullExpression(Expression body, string name, bool tailCall, ParameterExpression[] parameters)
            : base(body, parameters)
        {
            NameCore = name;
            TailCallCore = tailCall;
        }

        internal override string NameCore { get; }
        internal override bool TailCallCore { get; }
    }
}

#endif