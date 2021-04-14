#if LESSTHAN_NET40

#pragma warning disable CA1062 // Validate arguments of public methods

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using Theraot.Collections;

namespace System.Dynamic
{
    /// <summary>
    ///     Describes arguments in the dynamic binding process.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ArgumentCount" /> - all inclusive number of arguments.
    ///         <see cref="ArgumentNames" /> - names for those arguments that are named.
    ///     </para>
    ///     <para>
    ///         Argument names match to the argument values in left to right order
    ///         and last name corresponds to the last argument.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///   Foo(arg1, arg2, arg3, name1 = arg4, name2 = arg5, name3 = arg6)
    /// </code>
    ///     will correspond to
    ///     <code>
    ///   new CallInfo(6, "name1", "name2", "name3")
    /// </code>
    /// </example>
    public sealed class CallInfo
    {
        private readonly ReadOnlyCollectionEx<string> _argumentNames;

        /// <summary>
        ///     Creates a new <see cref="CallInfo" /> that represents arguments in the dynamic binding process.
        /// </summary>
        /// <param name="argCount">The number of arguments.</param>
        /// <param name="argNames">The argument names.</param>
        /// <returns>The new <see cref="CallInfo" /> instance.</returns>
        public CallInfo(int argCount, params string[] argNames)
            : this(argCount, (IEnumerable<string>)argNames)
        {
            // Empty
        }

        /// <summary>
        ///     Creates a new <see cref="CallInfo" /> that represents arguments in the dynamic binding process.
        /// </summary>
        /// <param name="argCount">The number of arguments.</param>
        /// <param name="argNames">The argument names.</param>
        /// <returns>The new <see cref="CallInfo" /> instance.</returns>
        public CallInfo(int argCount, IEnumerable<string> argNames)
        {
            ContractUtils.RequiresNotNull(argNames, nameof(argNames));

            var argNameCol = argNames.ToReadOnlyCollection();

            if (argCount < argNameCol.Count)
            {
                throw new ArgumentException("Argument count must be greater than number of named arguments.", nameof(argCount));
            }

            ContractUtils.RequiresNotNullItems(argNameCol, nameof(argNames));

            ArgumentCount = argCount;
            _argumentNames = argNameCol;
        }

        /// <summary>
        ///     The number of arguments.
        /// </summary>
        public int ArgumentCount { get; }

        /// <summary>
        ///     The argument names.
        /// </summary>
        public ReadOnlyCollection<string> ArgumentNames => _argumentNames;

        /// <summary>
        ///     Determines whether the specified <see cref="CallInfo" /> instance is considered equal to the current instance.
        /// </summary>
        /// <param name="obj">The instance of <see cref="CallInfo" /> to compare with the current instance.</param>
        /// <returns>true if the specified instance is equal to the current one otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is CallInfo other && ArgumentCount == other.ArgumentCount && _argumentNames.ListEquals(other._argumentNames);
        }

        /// <summary>
        ///     Serves as a hash function for the current <see cref="CallInfo" />.
        /// </summary>
        /// <returns>A hash code for the current <see cref="CallInfo" />.</returns>
        public override int GetHashCode()
        {
            return ArgumentCount ^ _argumentNames.GetHashCode();
        }
    }
}

#endif