#if LESSTHAN_NETSTANDARD20

#pragma warning disable CA1008 // Enums should have zero value
#pragma warning disable CA1027 // Mark enums with FlagsAttribute

using System.ComponentModel;

namespace System.Diagnostics
{

    public enum TraceEventType
    {
        Critical = 1,
        Error = 2,
        Warning = 4,
        Information = 8,
        Verbose = 16,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Start = 256,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Stop = 512,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Suspend = 1024,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Resume = 2048,
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Transfer = 4096
    }
}

#endif