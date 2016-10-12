#if NETCF

using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
    [AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Delegate, AllowMultiple = true)]
    [ComVisibleAttribute(true)]
    public sealed class DebuggerDisplayAttribute : Attribute
    {
        private string _name;
        private Type _target;
        private string _targetTypeName;
        private string _type;
        private string _value;

        public DebuggerDisplayAttribute(string value)
        {
            _value = value;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public Type Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _target = value;
            }
        }

        public string TargetTypeName
        {
            get
            {
                return _targetTypeName;
            }
            set
            {
                _targetTypeName = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
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