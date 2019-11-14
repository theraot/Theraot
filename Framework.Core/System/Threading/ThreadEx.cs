using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Threading
{
    public static class ThreadEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void MemoryBarrier()
        {
#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
            Interlocked.MemoryBarrier();
#else
            Thread.MemoryBarrier();
#endif
        }

#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#else

        [MethodImpl(MethodImplOptionsEx.NoInlining)]
#endif
        public static bool VolatileRead(ref bool address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            var flag = address;
            Thread.MemoryBarrier();
            return flag;
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static sbyte VolatileRead(ref sbyte address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static byte VolatileRead(ref byte address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileRead(ref short address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Read(ref address);
#else
            Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ushort VolatileRead(ref ushort address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static int VolatileRead(ref int address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint VolatileRead(ref uint address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static long VolatileRead(ref long address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static ulong VolatileRead(ref ulong address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IntPtr VolatileRead(ref IntPtr address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static UIntPtr VolatileRead(ref UIntPtr address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static float VolatileRead(ref float address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static double VolatileRead(ref double address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T VolatileRead<T>(ref T address)
            where T : class?
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Interlocked.CompareExchange(ref address, address, address);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNullIfNotNull("address")]
        public static object? VolatileRead(ref object? address)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            return Volatile.Read(ref address);
#else
            return Thread.VolatileRead(ref address);
#endif
        }

#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
#else

        [MethodImpl(MethodImplOptionsEx.NoInlining)]
#endif
        public static void VolatileWrite(ref bool address, bool value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            GC.KeepAlive(address);
            Thread.MemoryBarrier();
            address = value;
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref sbyte address, sbyte value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref byte address, byte value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref short address, short value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref ushort address, ushort value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref int address, int value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref uint address, uint value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref long address, long value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref ulong address, ulong value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref IntPtr address, IntPtr value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [CLSCompliant(false)]
        public static void VolatileWrite(ref UIntPtr address, UIntPtr value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref float address, float value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref double address, double value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite<T>(ref T address, T value)
            where T : class?
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Interlocked.Exchange(ref address, value);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void VolatileWrite(ref object? address, object? value)
        {
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20
            Volatile.Write(ref address, value);
#else
            Thread.VolatileWrite(ref address, value);
#endif
        }
    }
}