#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2

namespace System
{
    [Runtime.InteropServices.ComVisible(true)]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NonSerializedAttribute : Attribute
    {
        // Empty
    }
}

#endif