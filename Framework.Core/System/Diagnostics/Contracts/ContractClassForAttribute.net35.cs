#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.Contracts
{
    /// <summary>
    /// Types marked with this attribute specify that they are a contract for the type that is the argument of the constructor.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ContractClassForAttribute : Attribute
    {
        public ContractClassForAttribute(Type typeContractsAreFor)
        {
            TypeContractsAreFor = typeContractsAreFor;
        }

        public Type TypeContractsAreFor { get; }
    }
}

#endif