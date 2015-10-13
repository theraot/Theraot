using System;

namespace Theraot.Core
{
    public static class IntPtrHelper
    {
        public static IntPtr Add(IntPtr pointer, int offset)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return new IntPtr(unchecked((int)pointer + offset));
                case 8:
                    return new IntPtr(unchecked((long)pointer + offset));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
        }

        public static IntPtr Substract(IntPtr pointer, int offset)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return new IntPtr(unchecked((int)pointer - offset));
                case 8:
                    return new IntPtr(unchecked((long)pointer - offset));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
        }
    }

    public static class UIntPtrHelper
    {
        [CLSCompliantAttribute(false)]
        public static UIntPtr Add(UIntPtr pointer, int offset)
        {
            switch (UIntPtr.Size)
            {
                case 4:
                    return new UIntPtr(unchecked((uint)((int)pointer + offset)));
                case 8:
                    return new UIntPtr(unchecked((ulong)((long)pointer + offset)));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
        }

        [CLSCompliantAttribute(false)]
        public static UIntPtr Substract(UIntPtr pointer, int offset)
        {
            switch (UIntPtr.Size)
            {
                case 4:
                    return new UIntPtr(unchecked((uint)((int)pointer - offset)));
                case 8:
                    return new UIntPtr(unchecked((ulong)((long)pointer - offset)));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
        }
    }
}
