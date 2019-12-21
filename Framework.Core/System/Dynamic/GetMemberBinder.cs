#if LESSTHAN_NET35

#pragma warning disable RCS1193 // Overriding member cannot change 'params' modifier.
#pragma warning disable S3600 // "params" should not be introduced on overrides
// ReSharper disable RedundantParams
// ReSharper disable UnusedParameter.Global

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;

namespace System.Dynamic
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents the dynamic get member operation at the call site, providing the binding semantic and the details about
    ///     the operation.
    /// </summary>
    public abstract class GetMemberBinder : DynamicMetaObjectBinder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GetMemberBinder" />.
        /// </summary>
        /// <param name="name">The name of the member to get.</param>
        /// <param name="ignoreCase">true if the name should be matched ignoring case; false otherwise.</param>
        protected GetMemberBinder(string name, bool ignoreCase)
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

        /// <inheritdoc />
        /// <summary>
        ///     The result type of the operation.
        /// </summary>
        public sealed override Type ReturnType => typeof(object);

        /// <inheritdoc />
        /// <summary>
        ///     Always returns <c>true</c> because this is a standard <see cref="DynamicMetaObjectBinder" />.
        /// </summary>
        internal sealed override bool IsStandardBinder => true;

        /// <inheritdoc />
        /// <summary>
        ///     Performs the binding of the dynamic get member operation.
        /// </summary>
        /// <param name="target">The target of the dynamic get member operation.</param>
        /// <param name="args">An array of arguments of the dynamic get member operation.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject Bind(DynamicMetaObject target, params DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, nameof(target));
            ContractUtils.Requires(args == null || args.Length == 0, nameof(args));

            return target.BindGetMember(this);
        }

        /// <summary>
        ///     Performs the binding of the dynamic get member operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic get member operation.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public DynamicMetaObject FallbackGetMember(DynamicMetaObject target)
        {
            return FallbackGetMember(target, null);
        }

        /// <summary>
        ///     When overridden in the derived class, performs the binding of the dynamic get member operation if the target
        ///     dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic get member operation.</param>
        /// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject" /> representing the result of the binding.</returns>
        public abstract DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion);
    }
}

#endif