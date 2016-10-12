#if NETCF

namespace System.Security
{
#if !NET20
    [Obsolete("SecurityCriticalScope is only used for .NET 2.0 transparency compatibility.")]
#endif
    public enum SecurityCriticalScope
    {
        Everything,
        Explicit
    }
}

#endif