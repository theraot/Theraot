﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;

namespace System.Dynamic
{
    /// <summary>
    ///     Represents the dynamic set member operation at the call site, providing the binding semantic and the details about
    ///     the operation.
    /// </summary>
    public abstract class SetMemberBinder : DynamicMetaObjectBinder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SetMemberBinder" />.
        /// </summary>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
        protected SetMemberBinder(string name, bool ignoreCase)
        {
            ContractUtils.RequiresNotNull(name, nameof(name));

            Name = name;
            IgnoreCase = ignoreCase;
        }

        /// <summary>
        ///     Gets the value indicating if the string comparison should ignore the case of the member name.
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        ///     Gets the name of the member to get.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The result type of the operation.
        /// </summary>
        public sealed override Type ReturnType => typeof(object);

        /// <summary>
        ///     Always returns <c>true</c> because this is a standard <see cref="DynamicMetaObjectBinder" />.
        /// </summary>
        internal sealed override bool IsStandardBinder => true;

        /// <summary>
        ///     Performs the binding of the dynamic set member operation.
        /// </summary>
        /// <param name="target">The target of the dynamic set member operation.</param>
        /// <param name="args">An array of arguments of the dynamic set member operation.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, nameof(target));
            ContractUtils.RequiresNotNull(args, nameof(args));
            ContractUtils.Requires(args.Length == 1, nameof(args));

            var arg0 = args[0];
            ContractUtils.RequiresNotNull(arg0, nameof(args));

            return target.BindSetMember(this, arg0);
        }

        /// <summary>
        ///     Performs the binding of the dynamic set member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic set member operation.</param>
        /// <param name="value">The value to set to the member.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value)
        {
            return FallbackSetMember(target, value, errorSuggestion: null);
        }

        /// <summary>
        ///     Performs the binding of the dynamic set member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic set member operation.</param>
        /// <param name="value">The value to set to the member.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public abstract DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject? errorSuggestion);
    }
}

#endif