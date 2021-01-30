#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable S2346 // Flags enumerations zero-value members should be named "None".
#pragma warning disable S2344 // Rename this enumeration to remove the 'Flags' suffix.

using System.Runtime.InteropServices;

namespace System.Reflection
{
    [ComVisible(true)]
    [Flags]
    public enum BindingFlags
    {
        Default = 0,
        IgnoreCase = 1,
        DeclaredOnly = 2,
        Instance = 4,
        Static = 8,
        Public = 16,
        NonPublic = 32,
        FlattenHierarchy = 64,
        InvokeMethod = 256,
        CreateInstance = 512,
        GetField = 1024,
        SetField = 2048,
        GetProperty = 4096,
        SetProperty = 8192,
        PutDispProperty = 16384,
        PutRefDispProperty = 32768,
        ExactBinding = 65536,
        SuppressChangeType = 131072,
        OptionalParamBinding = 262144,
        IgnoreReturn = 16777216
    }
}

#endif