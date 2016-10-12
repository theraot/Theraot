#if NETCF

namespace System.Security
{
    [AttributeUsage(AttributeTargets.All, Inherited=false)]
    public sealed class SecuritySafeCriticalAttribute : Attribute
    {
        // Empty
    }
}

#endif