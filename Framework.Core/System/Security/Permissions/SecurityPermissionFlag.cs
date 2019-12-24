#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1714 // Flags enums should have plural names
#pragma warning disable RCS1191 // Declare enum value as combination of names.
#pragma warning disable S2346 // Flags enumerations zero-value members should be named "None"

namespace System.Security.Permissions
{
    [Flags]
    public enum SecurityPermissionFlag
    {
        NoFlags = 0,
        Assertion = 1,
        UnmanagedCode = 2,
        SkipVerification = 4,
        Execution = 8,
        ControlThread = 16,
        ControlEvidence = 32,
        ControlPolicy = 64,
        SerializationFormatter = 128,
        ControlDomainPolicy = 256,
        ControlPrincipal = 512,
        ControlAppDomain = 1024,
        RemotingConfiguration = 2048,
        Infrastructure = 4096,
        BindingRedirects = 8192,
        AllFlags = 16383
    }
}

#endif