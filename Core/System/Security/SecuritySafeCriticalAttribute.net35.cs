#if NET20 || NET30 || NET35

using System;

[AttributeUsage(AttributeTargets.All, Inherited = false)]
public sealed class SecuritySafeCriticalAttribute : Attribute
{
    // Empty
}

#endif