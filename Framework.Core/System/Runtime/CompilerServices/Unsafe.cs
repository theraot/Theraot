#if LESSTHAN_NET45

using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
    /// <summary>Contains generic, low-level functionality for manipulating pointers.</summary>
    public static class Unsafe
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ref T Add<T>(ref T source, int elementOffset)
        {
            return ref UnsafeHelper.AddByteOffset(ref source, elementOffset * (nint)SizeOf<T>());
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ref T Add<T>(ref T source, IntPtr elementOffset)
        {
            return ref UnsafeHelper.AddByteOffset(ref source, elementOffset * (nint)SizeOf<T>());
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe void* Add<T>(void* source, int elementOffset)
        {
            return (byte*)source + (elementOffset * (nint)SizeOf<T>());
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
        {
            return ref UnsafeHelper.AddByteOffset(ref source, byteOffset);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool AreSame<T>(ref T left, ref T right)
        {
            unsafe
            {
                return UnsafeHelper.AsPointer(ref left) == UnsafeHelper.AsPointer(ref right);
            }
        }

        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static unsafe void* AsPointer<T>(ref T value)
        {
            return UnsafeHelper.AsPointer(ref value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static int SizeOf<T>()
        {
            return Marshal.SizeOf(typeof(T));
        }
    }
}

#endif