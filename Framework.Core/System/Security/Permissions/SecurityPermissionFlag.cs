#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

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
        AllFlags = 16383,
    }
}

#endif