#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

namespace System
{
    [Runtime.InteropServices.ComVisible(true)]
    public interface ICloneable
    {
        object Clone();
    }
}

#endif