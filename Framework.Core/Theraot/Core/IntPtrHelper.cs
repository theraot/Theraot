// Needed for Workaround

using System;

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System.Runtime.CompilerServices;
#endif

namespace Theraot.Core
{
    public static class IntPtrHelper
    {
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif

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

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif
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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#endif
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