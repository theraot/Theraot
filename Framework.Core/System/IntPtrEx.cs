// Needed for Workaround

using System.Runtime.CompilerServices;

namespace System
{
    public static class IntPtrEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IntPtr Add(IntPtr pointer, int offset)
        {
#if LESSTHAN_NET40
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
#if LESSTHAN_NET40
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
}