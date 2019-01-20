#if LESSTHAN_NETSTANDARD13

// Note: Providing this attribute does not mean that serialization will work. However, it means that you could add it without the need of conditional compilation.

namespace System
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    [Runtime.InteropServices.ComVisible(true)]
    public sealed class SerializableAttribute : Attribute
    {
        // Empty
    }
}

#endif