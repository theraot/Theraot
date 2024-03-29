﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;

namespace System.Dynamic
{
    /// <summary>
    ///     Represents the convert dynamic operation at the call site, providing the binding semantic and the details about the
    ///     operation.
    /// </summary>
    public abstract class ConvertBinder : DynamicMetaObjectBinder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConvertBinder" />.
        /// </summary>
        /// <param name="type">The type to convert to.</param>
        /// <param name="explicit">true if the conversion should consider explicit conversions; otherwise, false.</param>
        protected ConvertBinder(Type type, bool @explicit)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));

            Type = type;
            Explicit = @explicit;
        }

        /// <summary>
        ///     Gets the value indicating if the conversion should consider explicit conversions.
        /// </summary>
        public bool Explicit { get; }

        /// <summary>
        ///     The result type of the operation.
        /// </summary>
        public sealed override Type ReturnType => Type;

        /// <summary>
        ///     The type to convert to.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Always returns <c>true</c> because this is a standard <see cref="DynamicMetaObjectBinder" />.
        /// </summary>
        internal sealed override bool IsStandardBinder => true;

        /// <summary>
        ///     Performs the binding of the dynamic convert operation.
        /// </summary>
        /// <param name="target">The target of the dynamic convert operation.</param>
        /// <param name="args">An array of arguments of the dynamic convert operation.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, nameof(target));
            ContractUtils.Requires(args == null || args.Length == 0, nameof(args));

            return target.BindConvert(this);
        }

        /// <summary>
        ///     Performs the binding of the dynamic convert operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic convert operation.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public DynamicMetaObject FallbackConvert(DynamicMetaObject target)
        {
            return FallbackConvert(target, errorSuggestion: null);
        }

        /// <summary>
        ///     When overridden in the derived class, performs the binding of the dynamic convert operation if the target dynamic
        ///     object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic convert operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public abstract DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject? errorSuggestion);
    }
}

#endif