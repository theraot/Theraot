#if NETCF

namespace System.Security
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    public sealed class SecurityCriticalAttribute : Attribute
    {
        private SecurityCriticalScope _scope;

        public SecurityCriticalAttribute()
            : this(SecurityCriticalScope.Explicit)
        {
            // Empty
        }

        public SecurityCriticalAttribute(SecurityCriticalScope scope)
        {
            _scope = scope;
        }

#if !NET20
        [Obsolete("SecurityCriticalScope is only used for .NET 2.0 transparency compatibility.")]
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