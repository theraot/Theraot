#if LESSTHAN_NET471 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This attribute should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class IsReadOnlyAttribute : Attribute
    {
        /// Empty
    }
}

#endif