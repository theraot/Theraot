// Needed for Workaround

using System;

namespace Theraot.Core
{
    public static class IntPtrHelper
    {
        public static IntPtr Add(IntPtr pointer, int offset)
        {
#if NET35
            switch (IntPtr.Size)
            {
                case 4:
                    return new IntPtr(unchecked((int)pointer + offset));
                case 8:
                    return new IntPtr(unchecked((long)pointer + offset));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
#else
            return IntPtr.Add(pointer, offset);
#endif
        }

        public static IntPtr Subtract(IntPtr pointer, int offset)
        {
#if NET35
            switch (IntPtr.Size)
            {
                case 4:
                    return new IntPtr(unchecked((int)pointer - offset));
                case 8:
                    return new IntPtr(unchecked((long)pointer - offset));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
#else
            return IntPtr.Subtract(pointer, offset);
#endif
        }
    }

    public static class UIntPtrHelper
    {
        [CLSCompliant(false)]
        public static UIntPtr Add(UIntPtr pointer, int offset)
        {
#if NET35
            switch (UIntPtr.Size)
            {
                case 4:
                    return new UIntPtr(unchecked((uint)((int)pointer + offset)));
                case 8:
                    return new UIntPtr(unchecked((ulong)((long)pointer + offset)));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
#else
            return UIntPtr.Add(pointer, offset);
#endif
        }

        [CLSCompliant(false)]
        public static UIntPtr Subtract(UIntPtr pointer, int offset)
        {
#if NET35
            switch (UIntPtr.Size)
            {
                case 4:
                    return new UIntPtr(unchecked((uint)((int)pointer - offset)));
                case 8:
                    return new UIntPtr(unchecked((ulong)((long)pointer - offset)));
                default:
                    throw new NotSupportedException("Not supported platform");
            }
#else
            return UIntPtr.Subtract(pointer, offset);
#endif
        }
    }
}
