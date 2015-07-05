#if NET20 || NET30

using System;

[AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
public sealed class SecuritySafeCriticalAttribute : Attribute
{
}

#endif