using System.Runtime.CompilerServices;
using Theraot.Threading;

namespace System.Threading
{
    public static class ThreadEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void MemoryBarrier()
        {
            ThreadingHelper.MemoryBarrier();
        }

#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#else

        [MethodImpl(MethodImplOptionsEx.NoInlining)]
#endif
        public static bool VolatileRead(ref bool location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            var flag = location;
            Thread.MemoryBarrier();
            return flag;
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static sbyte VolatileRead(ref sbyte location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static byte VolatileRead(ref byte location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileRead(ref short location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Read(ref location);
#else
            Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ushort VolatileRead(ref ushort location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static int VolatileRead(ref int location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint VolatileRead(ref uint location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static long VolatileRead(ref long location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ulong VolatileRead(ref ulong location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IntPtr VolatileRead(ref IntPtr location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static UIntPtr VolatileRead(ref UIntPtr location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float VolatileRead(ref float location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static double VolatileRead(ref double location)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Thread.VolatileRead(ref location);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T VolatileRead<T>(ref T location)
            where T : class?
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref location);
#else
            return Interlocked.CompareExchange(ref location, location, location);
#endif
        }

#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#else

        [MethodImpl(MethodImplOptionsEx.NoInlining)]
#endif
        public static void VolatileWrite(ref bool location, bool value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            GC.KeepAlive(location);
            Thread.MemoryBarrier();
            location = value;
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref sbyte location, sbyte value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref byte location, byte value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref short location, short value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref ushort location, ushort value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref int location, int value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref uint location, uint value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref long location, long value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref ulong location, ulong value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref IntPtr location, IntPtr value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref UIntPtr location, UIntPtr value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref float location, float value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref double location, double value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Thread.VolatileWrite(ref location, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite<T>(ref T location, T value)
            where T : class?
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref location, value);
#else
            Interlocked.Exchange(ref location, value);
#endif
        }
    }
}