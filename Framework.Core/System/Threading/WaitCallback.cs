#if TARGETS_NETSTANDARD
namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(true)]
    public delegate void WaitCallback(object state);
}

#endif