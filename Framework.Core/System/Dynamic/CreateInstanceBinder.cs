﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;

namespace System.Dynamic
{
    /// <summary>
    ///     Represents the create dynamic operation at the call site, providing the binding semantic and the details about the
    ///     operation.
    /// </summary>
    public abstract class CreateInstanceBinder : DynamicMetaObjectBinder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateInstanceBinder" />.
        /// </summary>
        /// <param name="callInfo">The signature of the arguments at the call site.</param>
        protected CreateInstanceBinder(CallInfo callInfo)
        {
            ContractUtils.RequiresNotNull(callInfo, nameof(callInfo));
            CallInfo = callInfo;
        }

        /// <summary>
        ///     Gets the signature of the arguments at the call site.
        /// </summary>
        public CallInfo CallInfo { get; }

        /// <summary>
        ///     The result type of the operation.
        /// </summary>
        public sealed override Type ReturnType => typeof(object);

        /// <summary>
        ///     Always returns <c>true</c> because this is a standard <see cref="DynamicMetaObjectBinder" />.
        /// </summary>
        internal sealed override bool IsStandardBinder => true;

        /// <summary>
        ///     Performs the binding of the dynamic create operation.
        /// </summary>
        /// <param name="target">The target of the dynamic create operation.</param>
        /// <param name="args">An array of arguments of the dynamic create operation.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, nameof(target));
            ContractUtils.RequiresNotNullItems(args, nameof(args));

            return target.BindCreateInstance(this, args);
        }

        /// <summary>
        ///     Performs the binding of the dynamic create operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic create operation.</param>
        /// <param name="args">The arguments of the dynamic create operation.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public DynamicMetaObject FallbackCreateInstance(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            return FallbackCreateInstance(target, args, errorSuggestion: null);
        }

        /// <summary>
        ///     When overridden in the derived class, performs the binding of the dynamic create operation if the target dynamic
        ///     object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic create operation.</param>
        /// <param name="args">The arguments of the dynamic create operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public abstract DynamicMetaObject FallbackCreateInstance(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject? errorSuggestion);
    }
}

#endif