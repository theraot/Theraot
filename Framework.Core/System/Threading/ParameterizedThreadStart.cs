#if LESSTHAN_NETSTANDARD13
namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(false)]
    public delegate void ParameterizedThreadStart(object obj);
}

#endif