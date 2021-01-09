#if LESSTHAN_NET40 || NETSTANDARD1_0

namespace System
{
    static class Array_ReferenceSources
    {
        internal const int MaxArrayLength = 0X7FEFFFFF;
        internal const int MaxByteArrayLength = 0x7FFFFFC7;
    }
}

#endif