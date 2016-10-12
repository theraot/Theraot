#if NETCF

using System.Runtime.InteropServices;

namespace System.Diagnostics
{
    [ComVisible(true)]
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}

#endif