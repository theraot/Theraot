#if LESSTHAN_NET40

namespace System.Threading
{
    public enum LazyThreadSafetyMode
    {
        None,
        PublicationOnly,
        ExecutionAndPublication
    }
}

#endif