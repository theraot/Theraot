#if NET20 || NET30 || NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.Contracts
{
    /// <summary>
    /// Types marked with this attribute specify that they are a contract for the type that is the argument of the constructor.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ContractClassForAttribute : Attribute
    {
        private readonly Type _typeIamAContractFor;

        public ContractClassForAttribute(Type typeContractsAreFor)
        {
            _typeIamAContractFor = typeContractsAreFor;
        }

        public Type TypeContractsAreFor
        {
            get
            {
                return _typeIamAContractFor;
            }
        }
    }
}

#endif