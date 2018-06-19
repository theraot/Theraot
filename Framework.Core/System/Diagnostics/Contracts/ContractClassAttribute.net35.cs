#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.Contracts
{
    /// <summary>
    /// Types marked with this attribute specify that a separate type contains the contracts for this type.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public sealed class ContractClassAttribute : Attribute
    {
        private readonly Type _typeWithContracts;

        public ContractClassAttribute(Type typeContainingContracts)
        {
            _typeWithContracts = typeContainingContracts;
        }

        public Type TypeContainingContracts
        {
            get { return _typeWithContracts; }
        }
    }
}

#endif