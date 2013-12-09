#if NET20 || NET30 || NET35

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