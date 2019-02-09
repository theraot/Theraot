#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

namespace System
{
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
}

#endif