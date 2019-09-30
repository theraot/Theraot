#if LESSTHAN_NET35

namespace System.Runtime.CompilerServices
{
    public interface IStrongBox
    {
        object? Value { get; set; }
    }
}

#endif