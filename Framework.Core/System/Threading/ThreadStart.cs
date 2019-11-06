#if LESSTHAN_NETSTANDARD13

namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(true)]
    public delegate void ThreadStart();
}

#endif