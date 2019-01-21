#if TARGETS_NETSTANDARD

namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(false)]
    public delegate void ParameterizedThreadStart(object obj);
}

#endif