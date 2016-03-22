#if NET20 || NET30

namespace System.Security
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class SecuritySafeCriticalAttribute : Attribute
    {
        // Empty
    }
}

#endif