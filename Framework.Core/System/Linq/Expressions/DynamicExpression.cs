﻿#if LESSTHAN_NET35

#pragma warning disable CA1062 // Validate arguments of public methods

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections;
using Theraot.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    ///     Represents a dynamic operation.
    /// </summary>
    public class DynamicExpression : Expression, IDynamicExpression
    {
        internal DynamicExpression(Type delegateType, CallSiteBinder binder)
        {
            Debug.Assert(delegateType.GetInvokeMethod().GetReturnType() == typeof(object) || GetType() != typeof(DynamicExpression));
            DelegateType = delegateType;
            Binder = binder;
        }

        int IArgumentProvider.ArgumentCount => throw ContractUtils.Unreachable;

        /// <summary>
        ///     Gets the arguments to the dynamic operation.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments => GetOrMakeArguments();

        /// <summary>
        ///     Gets the <see cref="CallSiteBinder" />, which determines the runtime behavior of the
        ///     dynamic site.
        /// </summary>
        public CallSiteBinder Binder { get; }

        /// <summary>
        ///     Gets a value that indicates whether the expression tree node can be reduced.
        /// </summary>
        public override bool CanReduce => true;

        /// <summary>
        ///     Gets the type of the delegate used by the <see cref="CallSite" />.
        /// </summary>
        public Type DelegateType { get; }

        /// <summary>
        ///     Returns the node type of this Expression. Extension nodes should return
        ///     ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> of the expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.Dynamic;

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents.
        /// </summary>
        /// <returns>
        ///     The <see cref="Type" /> that represents the static type of the
        ///     expression.
        /// </returns>
        public override Type Type => typeof(object);

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="Binder">Binder</see> and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DelegateType">DelegateType</see> property of the result will be inferred
        ///     from the types of the arguments and the specified return type.
        /// </remarks>
        public static new DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments)
        {
            return ExpressionExtension.Dynamic(binder, returnType, arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="Binder">Binder</see> and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DelegateType">DelegateType</see> property of the result will be inferred
        ///     from the types of the arguments and the specified return type.
        /// </remarks>
        public static new DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments)
        {
            return ExpressionExtension.Dynamic(binder, returnType, arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="Binder">Binder</see> and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DelegateType">DelegateType</see> property of the result will be inferred
        ///     from the types of the arguments and the specified return type.
        /// </remarks>
        public static new DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0)
        {
            return ExpressionExtension.Dynamic(binder, returnType, arg0);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="Binder">Binder</see> and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DelegateType">DelegateType</see> property of the result will be inferred
        ///     from the types of the arguments and the specified return type.
        /// </remarks>
        public static new DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1)
        {
            return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="Binder">Binder</see> and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DelegateType">DelegateType</see> property of the result will be inferred
        ///     from the types of the arguments and the specified return type.
        /// </remarks>
        public static new DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2)
        {
            return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1, arg2);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <param name="arg3">The fourth argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="Binder">Binder</see> and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DelegateType">DelegateType</see> property of the result will be inferred
        ///     from the types of the arguments and the specified return type.
        /// </remarks>
        public static new DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            return ExpressionExtension.Dynamic(binder, returnType, arg0, arg1, arg2, arg3);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DelegateType">DelegateType</see>,
        ///     <see cref="Binder">Binder</see>, and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static new DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression> arguments)
        {
            return ExpressionExtension.MakeDynamic(delegateType, binder, arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DelegateType">DelegateType</see>,
        ///     <see cref="Binder">Binder</see>, and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static new DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments)
        {
            return ExpressionExtension.MakeDynamic(delegateType, binder, arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and one argument.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DelegateType">DelegateType</see>,
        ///     <see cref="Binder">Binder</see>, and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static new DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0)
        {
            return ExpressionExtension.MakeDynamic(delegateType, binder, arg0);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and two arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DelegateType">DelegateType</see>,
        ///     <see cref="Binder">Binder</see>, and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static new DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
        {
            return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and three arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DelegateType">DelegateType</see>,
        ///     <see cref="Binder">Binder</see>, and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static new DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
        {
            return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1, arg2);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and four arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <param name="arg3">The fourth argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DelegateType">DelegateType</see>,
        ///     <see cref="Binder">Binder</see>, and
        ///     <see cref="Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static new DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            return ExpressionExtension.MakeDynamic(delegateType, binder, arg0, arg1, arg2, arg3);
        }

        object IDynamicExpression.CreateCallSite()
        {
            return CallSite.Create(DelegateType, Binder);
        }

        Expression IArgumentProvider.GetArgument(int index)
        {
            throw ContractUtils.Unreachable;
        }

        /// <summary>
        ///     Reduces the dynamic expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var site = Constant(CallSite.Create(DelegateType, Binder));
            return Invoke
            (
                Field
                (
                    site,
                    "Target"
                ),
                Arguments.AddFirst(site)
            );
        }

        Expression IDynamicExpression.Rewrite(Expression[] args)
        {
            return Rewrite(args);
        }

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public DynamicExpression Update(IEnumerable<Expression>? arguments)
        {
            ICollection<Expression>? args;
            if (arguments == null)
            {
                args = null;
            }
            else
            {
                args = arguments as ICollection<Expression>;
                if (args == null)
                {
                    arguments = args = arguments.ToReadOnlyCollection();
                }
            }

            return SameArguments(args) ? this : ExpressionExtension.MakeDynamic(DelegateType, Binder, arguments);
        }

        internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression[] arguments)
        {
            return returnType == typeof(object) ? new DynamicExpressionN(delegateType, binder, arguments) : new TypedDynamicExpressionN(returnType, delegateType, binder, arguments);
        }

        internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0)
        {
            return returnType == typeof(object) ? new DynamicExpression1(delegateType, binder, arg0) : new TypedDynamicExpression1(returnType, delegateType, binder, arg0);
        }

        internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
        {
            return returnType == typeof(object) ? new DynamicExpression2(delegateType, binder, arg0, arg1) : new TypedDynamicExpression2(returnType, delegateType, binder, arg0, arg1);
        }

        internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
        {
            return returnType == typeof(object) ? new DynamicExpression3(delegateType, binder, arg0, arg1, arg2) : new TypedDynamicExpression3(returnType, delegateType, binder, arg0, arg1, arg2);
        }

        internal static DynamicExpression Make(Type returnType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            return returnType == typeof(object) ? new DynamicExpression4(delegateType, binder, arg0, arg1, arg2, arg3) : new TypedDynamicExpression4(returnType, delegateType, binder, arg0, arg1, arg2, arg3);
        }

        internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            throw ContractUtils.Unreachable;
        }

        internal virtual DynamicExpression Rewrite(Expression[]? args)
        {
            throw ContractUtils.Unreachable;
        }

        internal virtual bool SameArguments(ICollection<Expression>? arguments)
        {
            throw ContractUtils.Unreachable;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is DynamicExpressionVisitor dynVisitor)
            {
                return dynVisitor.VisitDynamic(this);
            }

            return base.Accept(visitor);
        }
    }

    internal static class ExpressionExtension
    {
        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments)
        {
            return Dynamic(binder, returnType, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0)
        {
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            ValidateDynamicArgument(arg0, nameof(arg0));
            var delegateType = DelegateHelper.GetDelegateTypeInternal(typeof(CallSite), arg0.Type, returnType);
            return DynamicExpression.Make(returnType, delegateType, binder, arg0);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1)
        {
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            ValidateDynamicArgument(arg0, nameof(arg0));
            ValidateDynamicArgument(arg1, nameof(arg1));
            var delegateType = DelegateHelper.GetDelegateTypeInternal(typeof(CallSite), arg0.Type, arg1.Type, returnType);
            return DynamicExpression.Make(returnType, delegateType, binder, arg0, arg1);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2)
        {
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            ValidateDynamicArgument(arg0, nameof(arg0));
            ValidateDynamicArgument(arg1, nameof(arg1));
            ValidateDynamicArgument(arg2, nameof(arg2));
            var delegateType = DelegateHelper.GetDelegateTypeInternal(typeof(CallSite), arg0.Type, arg1.Type, arg2.Type, returnType);
            return DynamicExpression.Make(returnType, delegateType, binder, arg0, arg1, arg2);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <param name="arg3">The fourth argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            ValidateDynamicArgument(arg0, nameof(arg0));
            ValidateDynamicArgument(arg1, nameof(arg1));
            ValidateDynamicArgument(arg2, nameof(arg2));
            ValidateDynamicArgument(arg3, nameof(arg3));
            var delegateType = DelegateHelper.GetDelegateTypeInternal(typeof(CallSite), arg0.Type, arg1.Type, arg2.Type, arg3.Type, returnType);
            return DynamicExpression.Make(returnType, delegateType, binder, arg0, arg1, arg2, arg3);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="returnType">The result type of the dynamic expression.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.Binder">Binder</see> and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        /// <remarks>
        ///     The <see cref="DynamicExpression.DelegateType">DelegateType</see> property of the
        ///     result will be inferred from the types of the arguments and the specified return type.
        /// </remarks>
        public static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments)
        {
            ContractUtils.RequiresNotNull(arguments, nameof(arguments));
            ContractUtils.RequiresNotNull(returnType, nameof(returnType));

            var args = arguments.AsArrayInternal();
            ContractUtils.RequiresNotEmpty(args, nameof(arguments));
            return MakeDynamic(binder, returnType, args);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments)
        {
            return MakeDynamic(delegateType, binder, (IEnumerable<Expression>)arguments);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" />.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arguments">The arguments to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression>? arguments)
        {
            var argumentList = arguments.AsArrayInternal();
            switch (argumentList.Length)
            {
                case 1:
                    return MakeDynamic(delegateType, binder, argumentList[0]);

                case 2:
                    return MakeDynamic(delegateType, binder, argumentList[0], argumentList[1]);

                case 3:
                    return MakeDynamic(delegateType, binder, argumentList[0], argumentList[1], argumentList[2]);

                case 4:
                    return MakeDynamic(delegateType, binder, argumentList[0], argumentList[1], argumentList[2], argumentList[3]);

                default:
                    break;
            }

            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new ArgumentException("Type must be derived from System.Delegate", nameof(delegateType));
            }

            var method = GetValidMethodForDynamic(delegateType);

            ExpressionUtils.ValidateArgumentTypes(method, ExpressionType.Dynamic, ref argumentList, nameof(delegateType));

            return DynamicExpression.Make(method.GetReturnType(), delegateType, binder, argumentList);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and one argument.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new ArgumentException("Type must be derived from System.Delegate", nameof(delegateType));
            }

            var method = GetValidMethodForDynamic(delegateType);
            var parameters = method.GetParameters();

            ExpressionUtils.ValidateArgumentCount(method, ExpressionType.Dynamic, 2, parameters);
            ValidateDynamicArgument(arg0, nameof(arg0));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg0, parameters[1], nameof(delegateType), nameof(arg0));

            return DynamicExpression.Make(method.GetReturnType(), delegateType, binder, arg0);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and two arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new ArgumentException("Type must be derived from System.Delegate", nameof(delegateType));
            }

            var method = GetValidMethodForDynamic(delegateType);
            var parameters = method.GetParameters();

            ExpressionUtils.ValidateArgumentCount(method, ExpressionType.Dynamic, 3, parameters);
            ValidateDynamicArgument(arg0, nameof(arg0));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg0, parameters[1], nameof(delegateType), nameof(arg0));
            ValidateDynamicArgument(arg1, nameof(arg1));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg1, parameters[2], nameof(delegateType), nameof(arg1));

            return DynamicExpression.Make(method.GetReturnType(), delegateType, binder, arg0, arg1);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and three arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new ArgumentException("Type must be derived from System.Delegate", nameof(delegateType));
            }

            var method = GetValidMethodForDynamic(delegateType);
            var parameters = method.GetParameters();

            ExpressionUtils.ValidateArgumentCount(method, ExpressionType.Dynamic, 4, parameters);
            ValidateDynamicArgument(arg0, nameof(arg0));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg0, parameters[1], nameof(delegateType), nameof(arg0));
            ValidateDynamicArgument(arg1, nameof(arg1));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg1, parameters[2], nameof(delegateType), nameof(arg1));
            ValidateDynamicArgument(arg2, nameof(arg2));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg2, parameters[3], nameof(delegateType), nameof(arg2));

            return DynamicExpression.Make(method.GetReturnType(), delegateType, binder, arg0, arg1, arg2);
        }

        /// <summary>
        ///     Creates a <see cref="DynamicExpression" /> that represents a dynamic operation bound by the provided
        ///     <see cref="CallSiteBinder" /> and four arguments.
        /// </summary>
        /// <param name="delegateType">The type of the delegate used by the <see cref="CallSite" />.</param>
        /// <param name="binder">The runtime binder for the dynamic operation.</param>
        /// <param name="arg0">The first argument to the dynamic operation.</param>
        /// <param name="arg1">The second argument to the dynamic operation.</param>
        /// <param name="arg2">The third argument to the dynamic operation.</param>
        /// <param name="arg3">The fourth argument to the dynamic operation.</param>
        /// <returns>
        ///     A <see cref="DynamicExpression" /> that has <see cref="DynamicExpression.NodeType" /> equal to
        ///     <see cref="ExpressionType.Dynamic">Dynamic</see> and has the
        ///     <see cref="DynamicExpression.DelegateType">DelegateType</see>,
        ///     <see cref="DynamicExpression.Binder">Binder</see>, and
        ///     <see cref="DynamicExpression.Arguments">Arguments</see> set to the specified values.
        /// </returns>
        public static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
        {
            ContractUtils.RequiresNotNull(delegateType, nameof(delegateType));
            ContractUtils.RequiresNotNull(binder, nameof(binder));
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new ArgumentException("Type must be derived from System.Delegate", nameof(delegateType));
            }

            var method = GetValidMethodForDynamic(delegateType);
            var parameters = method.GetParameters();

            ExpressionUtils.ValidateArgumentCount(method, ExpressionType.Dynamic, 5, parameters);
            ValidateDynamicArgument(arg0, nameof(arg0));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg0, parameters[1], nameof(delegateType), nameof(arg0));
            ValidateDynamicArgument(arg1, nameof(arg1));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg1, parameters[2], nameof(delegateType), nameof(arg1));
            ValidateDynamicArgument(arg2, nameof(arg2));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg2, parameters[3], nameof(delegateType), nameof(arg2));
            ValidateDynamicArgument(arg3, nameof(arg3));
            ExpressionUtils.ValidateOneArgument(method, ExpressionType.Dynamic, arg3, parameters[4], nameof(delegateType), nameof(arg3));

            return DynamicExpression.Make(method.GetReturnType(), delegateType, binder, arg0, arg1, arg2, arg3);
        }

        private static MethodInfo GetValidMethodForDynamic(Type delegateType)
        {
            var method = delegateType.GetInvokeMethod();
            var pi = method.GetParameters();
            if (pi.Length == 0 || pi[0].ParameterType != typeof(CallSite))
            {
                throw new ArgumentException("First argument of delegate must be CallSite", nameof(delegateType));
            }

            return method;
        }

        private static DynamicExpression MakeDynamic(CallSiteBinder binder, Type returnType, Expression[] arguments)
        {
            ContractUtils.RequiresNotNull(binder, nameof(binder));

            var n = arguments.Length;

            for (var i = 0; i < n; i++)
            {
                var arg = arguments[i];

                ValidateDynamicArgument(arg, nameof(arguments), i);
            }

            var delegateType = DelegateHelper.GetDelegateTypeInternal(arguments.ConvertAll(exp => exp.Type).Prepend(typeof(CallSite)).Append(returnType).ToArray());

            // Since we made a delegate with argument types that exactly match,
            // we can skip delegate and argument validation

            switch (n)
            {
                case 1: return DynamicExpression.Make(returnType, delegateType, binder, arguments[0]);
                case 2: return DynamicExpression.Make(returnType, delegateType, binder, arguments[0], arguments[1]);
                case 3: return DynamicExpression.Make(returnType, delegateType, binder, arguments[0], arguments[1], arguments[2]);
                case 4: return DynamicExpression.Make(returnType, delegateType, binder, arguments[0], arguments[1], arguments[2], arguments[3]);
                default: return DynamicExpression.Make(returnType, delegateType, binder, arguments);
            }
        }

        private static void ValidateDynamicArgument(Expression arg, string paramName, int index = -1)
        {
            ContractUtils.RequiresNotNull(arg, paramName, index);
            ExpressionUtils.RequiresCanRead(arg, paramName, index);
            var type = arg.Type;
            ContractUtils.RequiresNotNull(type, nameof(type));
            TypeUtils.ValidateType(type, nameof(type), allowByRef: true, allowPointer: true);
            if (type == typeof(void))
            {
                throw new ArgumentException("Argument type cannot be void", nameof(arg));
            }
        }
    }

    internal class DynamicExpression1 : DynamicExpression, IArgumentProvider
    {
        private object _arg0; // storage for the 1st argument or a read-only collection.  See IArgumentProvider for more info.

        internal DynamicExpression1(Type delegateType, CallSiteBinder binder, Expression arg0)
            : base(delegateType, binder)
        {
            _arg0 = arg0;
        }

        int IArgumentProvider.ArgumentCount => 1;

        Expression IArgumentProvider.GetArgument(int index)
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

        internal override DynamicExpression Rewrite(Expression[]? args)
        {
            return ExpressionExtension.MakeDynamic(DelegateType, Binder, args![0]);
        }

        internal override bool SameArguments(ICollection<Expression>? arguments)
        {
            if (arguments?.Count != 1)
            {
                return false;
            }

            using (var en = arguments.GetEnumerator())
            {
                en.MoveNext();
                return en.Current == ExpressionUtils.ReturnObject<Expression>(_arg0);
            }
        }
    }

    internal class DynamicExpression2 : DynamicExpression, IArgumentProvider
    {
        private readonly Expression _arg1;

        private object _arg0; // storage for the 1st argument or a read-only collection.  See IArgumentProvider for more info.
        // storage for the 2nd argument

        internal DynamicExpression2(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
            : base(delegateType, binder)
        {
            _arg0 = arg0;
            _arg1 = arg1;
        }

        int IArgumentProvider.ArgumentCount => 2;

        Expression IArgumentProvider.GetArgument(int index)
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

        internal override DynamicExpression Rewrite(Expression[]? args)
        {
            return ExpressionExtension.MakeDynamic(DelegateType, Binder, args![0], args![1]);
        }

        internal override bool SameArguments(ICollection<Expression>? arguments)
        {
            if (arguments?.Count != 2)
            {
                return false;
            }

            if (_arg0 is Expression[] alreadyArray)
            {
                return ExpressionUtils.SameElements(arguments, alreadyArray);
            }

            using (var en = arguments.GetEnumerator())
            {
                en.MoveNext();
                if (en.Current != _arg0)
                {
                    return false;
                }

                en.MoveNext();
                return en.Current == _arg1;
            }
        }
    }

    internal class DynamicExpression3 : DynamicExpression, IArgumentProvider
    {
        private readonly Expression _arg1, _arg2;

        private object _arg0; // storage for the 1st argument or a read-only collection.  See IArgumentProvider for more info.
        // storage for the 2nd & 3rd arguments

        internal DynamicExpression3(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
            : base(delegateType, binder)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        int IArgumentProvider.ArgumentCount => 3;

        Expression IArgumentProvider.GetArgument(int index)
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

        internal override DynamicExpression Rewrite(Expression[]? args)
        {
            return ExpressionExtension.MakeDynamic(DelegateType, Binder, args![0], args![1], args![2]);
        }

        internal override bool SameArguments(ICollection<Expression>? arguments)
        {
            if (arguments?.Count != 3)
            {
                return false;
            }

            if (_arg0 is Expression[] alreadyArray)
            {
                return ExpressionUtils.SameElements(arguments, alreadyArray);
            }

            using (var en = arguments.GetEnumerator())
            {
                en.MoveNext();
                if (en.Current != _arg0)
                {
                    return false;
                }

                en.MoveNext();
                if (en.Current != _arg1)
                {
                    return false;
                }

                en.MoveNext();
                return en.Current == _arg2;
            }
        }
    }

    internal class DynamicExpression4 : DynamicExpression, IArgumentProvider
    {
        private readonly Expression _arg1, _arg2, _arg3;

        private object _arg0; // storage for the 1st argument or a read-only collection.  See IArgumentProvider for more info.
        // storage for the 2nd - 4th arguments

        internal DynamicExpression4(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
            : base(delegateType, binder)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
        }

        int IArgumentProvider.ArgumentCount => 4;

        Expression IArgumentProvider.GetArgument(int index)
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

        internal override DynamicExpression Rewrite(Expression[]? args)
        {
            return ExpressionExtension.MakeDynamic(DelegateType, Binder, args![0], args![1], args![2], args![3]);
        }

        internal override bool SameArguments(ICollection<Expression>? arguments)
        {
            if (arguments?.Count != 4)
            {
                return false;
            }

            if (_arg0 is Expression[] alreadyArray)
            {
                return ExpressionUtils.SameElements(arguments, alreadyArray);
            }

            using (var en = arguments.GetEnumerator())
            {
                en.MoveNext();
                if (en.Current != _arg0)
                {
                    return false;
                }

                en.MoveNext();
                if (en.Current != _arg1)
                {
                    return false;
                }

                en.MoveNext();
                if (en.Current != _arg2)
                {
                    return false;
                }

                en.MoveNext();
                return en.Current == _arg3;
            }
        }
    }

    internal class DynamicExpressionN : DynamicExpression, IArgumentProvider
    {
        private readonly Expression[] _arguments;
        private readonly ReadOnlyCollectionEx<Expression> _argumentsAsReadOnlyCollection;

        internal DynamicExpressionN(Type delegateType, CallSiteBinder binder, Expression[] arguments)
            : base(delegateType, binder)
        {
            _arguments = arguments;
            _argumentsAsReadOnlyCollection = ReadOnlyCollectionEx.Create(_arguments);
        }

        int IArgumentProvider.ArgumentCount => _arguments.Length;

        Expression IArgumentProvider.GetArgument(int index)
        {
            return _arguments[index];
        }

        internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            return _argumentsAsReadOnlyCollection;
        }

        internal override DynamicExpression Rewrite(Expression[]? args)
        {
            return ExpressionExtension.MakeDynamic(DelegateType, Binder, args!);
        }

        internal override bool SameArguments(ICollection<Expression>? arguments)
        {
            return ExpressionUtils.SameElements(arguments, _arguments);
        }
    }

    internal sealed class TypedDynamicExpression1 : DynamicExpression1
    {
        internal TypedDynamicExpression1(Type retType, Type delegateType, CallSiteBinder binder, Expression arg0)
            : base(delegateType, binder, arg0)
        {
            Type = retType;
        }

        public override Type Type { get; }
    }

    internal sealed class TypedDynamicExpression2 : DynamicExpression2
    {
        internal TypedDynamicExpression2(Type retType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1)
            : base(delegateType, binder, arg0, arg1)
        {
            Type = retType;
        }

        public override Type Type { get; }
    }

    internal sealed class TypedDynamicExpression3 : DynamicExpression3
    {
        internal TypedDynamicExpression3(Type retType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2)
            : base(delegateType, binder, arg0, arg1, arg2)
        {
            Type = retType;
        }

        public override Type Type { get; }
    }

    internal sealed class TypedDynamicExpression4 : DynamicExpression4
    {
        internal TypedDynamicExpression4(Type retType, Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3)
            : base(delegateType, binder, arg0, arg1, arg2, arg3)
        {
            Type = retType;
        }

        public override Type Type { get; }
    }

    internal class TypedDynamicExpressionN : DynamicExpressionN
    {
        internal TypedDynamicExpressionN(Type returnType, Type delegateType, CallSiteBinder binder, Expression[] arguments)
            : base(delegateType, binder, arguments)
        {
            Debug.Assert(delegateType.GetInvokeMethod().GetReturnType() == returnType);
            Type = returnType;
        }

        public sealed override Type Type { get; }
    }
}

#endif