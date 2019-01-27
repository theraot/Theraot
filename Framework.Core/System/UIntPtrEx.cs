// Needed for Workaround

using System.Runtime.CompilerServices;

namespace System
{
    public static class UIntPtrEx
    {
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static UIntPtr Add(UIntPtr pointer, int offset)
        {
#if LESSTHAN_NET40
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
#if LESSTHAN_NET40
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