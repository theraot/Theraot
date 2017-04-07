#if NET20 || NET30

namespace System.Runtime.CompilerServices
{
    public interface IStrongBox
    {
        object Value { get; set; }
    }
}

#endif