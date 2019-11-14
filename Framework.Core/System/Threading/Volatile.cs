#if LESSTHAN_NET45

using System.Runtime.CompilerServices;

namespace System.Threading
{
    public static class Volatile
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Read(ref bool location)
        {
            return ThreadEx.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static sbyte Read(ref sbyte location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static byte Read(ref byte location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static short Read(ref short location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ushort Read(ref ushort location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static int Read(ref int location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint Read(ref uint location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static long Read(ref long location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ulong Read(ref ulong location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IntPtr Read(ref IntPtr location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static UIntPtr Read(ref UIntPtr location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float Read(ref float location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static double Read(ref double location)
        {
            return Thread.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T Read<T>(ref T location)
            where T : class?
        {
            return ThreadEx.VolatileRead(ref location);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref bool location, bool value)
        {
            ThreadEx.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void Write(ref sbyte location, sbyte value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref byte location, byte value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref short location, short value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void Write(ref ushort location, ushort value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref int location, int value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void Write(ref uint location, uint value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref long location, long value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void Write(ref ulong location, ulong value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref IntPtr location, IntPtr value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void Write(ref UIntPtr location, UIntPtr value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref float location, float value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write(ref double location, double value)
        {
            Thread.VolatileWrite(ref location, value);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Write<T>(ref T location, T value)
            where T : class?
        {
            ThreadEx.VolatileWrite(ref location, value);
        }
    }
}

#endif