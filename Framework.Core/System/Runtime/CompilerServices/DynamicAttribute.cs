#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    /// <inheritdoc />
    /// <summary>
    /// Indicates that the use of <see cref="T:System.Object" /> on a member is meant to be treated as a dynamically dispatched type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class DynamicAttribute : Attribute
    {
        private readonly bool[] _transformFlags;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.DynamicAttribute" /> class.
        /// </summary>
        /// <remarks>
        /// When used in an attribute specification, the default constructor is semantically
        /// equivalent to <c>DynamicAttribute({ true })</c>, and can be considered
        /// a shorthand for that expression. It should therefore only be used on an element
        /// of type <see cref="T:System.Object" />.
        /// </remarks>
        public DynamicAttribute()
        {
            _transformFlags = new[] { true };
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.DynamicAttribute" /> class.
        /// </summary>
        /// <param name="transformFlags">Specifies, in a prefix traversal of a type's
        /// construction, which <see cref="T:System.Object" /> occurrences are meant to
        /// be treated as a dynamically dispatched type.</param>
        /// <remarks>
        /// This constructor is meant to be used on types that are built on an underlying
        /// occurrence of <see cref="T:System.Object" /> that is meant to be treated dynamically.
        /// For instance, if <c>C</c> is a generic type with two type parameters, then a
        /// use of the constructed type<c>C&lt;<see cref="T:System.Object" />, <see cref="T:System.Object" />&gt;</c>
        /// might be intended to treat the first type argument dynamically and the second
        /// normally, in which case the appropriate attribute specification should
        /// use a <c>transformFlags</c> value of <c>{ false, true, false }</c>.
        /// </remarks>
        public DynamicAttribute(bool[] transformFlags)
        {
            _transformFlags = transformFlags ?? throw new ArgumentNullException(nameof(transformFlags));
        }

        /// <summary>
        /// Specifies, in a prefix traversal of a type's
        /// construction, which <see cref="object"/> occurrences are meant to
        /// be treated as a dynamically dispatched type.
        /// </summary>
        public IList<bool> TransformFlags => _transformFlags;
    }
}

#endif