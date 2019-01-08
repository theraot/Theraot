#if LESSTHAN_NET45

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics.Contracts
{
    /// <summary>
    /// Allows setting contract and tool options at assembly, type, or method granularity.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [Conditional("CONTRACTS_FULL")]
    public sealed class ContractOptionAttribute : Attribute
    {
        public ContractOptionAttribute(string category, string setting, bool enabled)
        {
            Category = category;
            Setting = setting;
            Enabled = enabled;
        }

        public ContractOptionAttribute(string category, string setting, string value)
        {
            Category = category;
            Setting = setting;
            Value = value;
        }

        public string Category { get; }

        public bool Enabled { get; }
        public string Setting { get; }
        public string Value { get; }
    }
}

#endif