#if LESSTHAN_NET40

namespace System
{
    public enum LazyThreadSafetyMode
    {
        None,
        PublicationOnly,
        ExecutionAndPublication
    }
}

#endif