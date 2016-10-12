#if NETCF

using System;
namespace System.Security
{
    [AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public sealed class SecurityCriticalAttribute : Attribute
    {
        private SecurityCriticalScope _scope;

        public SecurityCriticalAttribute()
        {
            this(SecurityCriticalScope.Explicit);
        }

        public SecurityCriticalAttribute(SecurityCriticalScope scope)
        {

        }

#if !NET20
        [ObsoleteAttribute("SecurityCriticalScope is only used for .NET 2.0 transparency compatibility.")]
#endif
        public SecurityCriticalScope Scope
        {
            get
            {
                return _scope;
            }
        }
    }
}

#endif