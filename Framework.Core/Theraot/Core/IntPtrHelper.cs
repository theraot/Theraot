// Needed for Workaround

using System;
using System.Runtime.CompilerServices;

namespace Theraot.Core
{
    public static class IntPtrHelper
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IntPtr Add(IntPtr pointer, int offset)
        {
#if NET20 || NET30 || NET35
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

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IntPtr Subtract(IntPtr pointer, int offset)
        {
#if NET20 || NET30 || NET35
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
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static UIntPtr Add(UIntPtr pointer, int offset)
        {
#if NET20 || NET30 || NET35
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
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static UIntPtr Subtract(UIntPtr pointer, int offset)
        {
#if NET20 || NET30 || NET35
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