#if NET35 || NET40

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
        private readonly string _category;
        private readonly string _setting;
        private readonly bool _enabled;
        private readonly string _value;

        public ContractOptionAttribute(string category, string setting, bool enabled)
        {
            _category = category;
            _setting = setting;
            _enabled = enabled;
        }

        public ContractOptionAttribute(string category, string setting, string value)
        {
            _category = category;
            _setting = setting;
            _value = value;
        }

        public string Category
        {
            get
            {
                return _category;
            }
        }

        public string Setting
        {
            get
            {
                return _setting;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }
    }
}

#endif