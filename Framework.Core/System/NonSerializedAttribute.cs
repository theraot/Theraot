#if LESSTHAN_NETSTANDARD13
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