#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.Contracts
{
    /// <summary>
    ///     Enables writing abbreviations for contracts that get copied to other methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [Conditional("CONTRACTS_FULL")]
    public sealed class ContractAbbreviatorAttribute : Attribute
    {
        // Empty
    }
}

#endif