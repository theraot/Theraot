using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        [CLSCompliantAttribute(false)]
        public static IEnumerable<sbyte> Bits(this sbyte value)
        {
            unchecked
            {
                byte check = (byte)1 << 7;
                int log2 = 8;
                byte _value = (byte)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return (sbyte)check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<short> Bits(this short value)
        {
            unchecked
            {
                ushort check = (ushort)1 << 15;
                int log2 = 16;
                ushort _value = (ushort)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return (short)check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> Bits(this int value)
        {
            unchecked
            {
                uint check = (uint)1 << 31;
                int log2 = 32;
                uint _value = (uint)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return (int)check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<long> Bits(this long value)
        {
            unchecked
            {
                ulong check = (ulong)1 << 63;
                int log2 = 64;
                ulong _value = (ulong)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return (long)check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<byte> Bits(this byte value)
        {
            unchecked
            {
                byte check = (byte)1 << 7;
                int log2 = 8;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return (byte)check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<ushort> Bits(this ushort value)
        {
            unchecked
            {
                ushort check = (ushort)1 << 15;
                int log2 = 16;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<uint> Bits(this uint value)
        {
            unchecked
            {
                uint check = (uint)1 << 31;
                int log2 = 32;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<ulong> Bits(this ulong value)
        {
            unchecked
            {
                ulong check = (ulong)1 << 63;
                int log2 = 64;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return check;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsBinary(this sbyte value)
        {
            unchecked
            {
                byte check = (byte)1 << 7;
                int log2 = 8;
                byte _value = (byte)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsBinary(this short value)
        {
            unchecked
            {
                ushort check = (ushort)1 << 15;
                int log2 = 16;
                ushort _value = (ushort)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsBinary(this int value)
        {
            unchecked
            {
                uint check = (uint)1 << 31;
                int log2 = 32;
                uint _value = (uint)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsBinary(this long value)
        {
            unchecked
            {
                ulong check = (ulong)1 << 63;
                int log2 = 64;
                ulong _value = (ulong)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsBinary(this byte value)
        {
            unchecked
            {
                byte check = (byte)1 << 7;
                int log2 = 8;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsBinary(this ushort value)
        {
            unchecked
            {
                ushort check = (ushort)1 << 15;
                int log2 = 16;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsBinary(this uint value)
        {
            unchecked
            {
                uint check = (uint)1 << 31;
                int log2 = 32;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsBinary(this ulong value)
        {
            unchecked
            {
                ulong check = (ulong)1 << 63;
                int log2 = 64;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return 1;
                    }
                    else
                    {
                        yield return 0;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsLog2(this sbyte value)
        {
            unchecked
            {
                byte check = (byte)1 << 7;
                int log2 = 8;
                byte _value = (byte)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsLog2(this short value)
        {
            unchecked
            {
                ushort check = (ushort)1 << 15;
                int log2 = 16;
                ushort _value = (ushort)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsLog2(this int value)
        {
            unchecked
            {
                uint check = (uint)1 << 31;
                int log2 = 32;
                uint _value = (uint)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsLog2(this long value)
        {
            unchecked
            {
                ulong check = (ulong)1 << 63;
                int log2 = 64;
                ulong _value = (ulong)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static IEnumerable<int> BitsLog2(this byte value)
        {
            unchecked
            {
                byte check = (byte)1 << 7;
                int log2 = 8;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsLog2(this ushort value)
        {
            unchecked
            {
                ushort check = (ushort)1 << 15;
                int log2 = 16;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsLog2(this uint value)
        {
            unchecked
            {
                uint check = (uint)1 << 31;
                int log2 = 32;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<int> BitsLog2(this ulong value)
        {
            unchecked
            {
                ulong check = (ulong)1 << 63;
                int log2 = 64;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        [CLSCompliantAttribute(false)]
        public static ulong BuildUlong(uint hi, uint lo)
        {
            return (ulong)hi << 32 | (ulong)lo;
        }

        public static int CheckedDecrement(this int value)
        {
            checked
            {
                value--;
                return value;
            }
        }

        public static short CheckedDecrement(this short value)
        {
            checked
            {
                value--;
                return value;
            }
        }

        public static long CheckedDecrement(this long value)
        {
            checked
            {
                value--;
                return value;
            }
        }

        public static int CheckedIncrement(this int value)
        {
            checked
            {
                value++;
                return value;
            }
        }

        public static short CheckedIncrement(this short value)
        {
            checked
            {
                value++;
                return value;
            }
        }

        public static long CheckedIncrement(this long value)
        {
            checked
            {
                value++;
                return value;
            }
        }

        [CLSCompliantAttribute(false)]
        public static void GetParts(ulong value, out uint lo, out uint hi)
        {
            unchecked
            {
                lo = (uint)value;
                hi = (uint)(value >> 32);
            }
        }

        //Gem from Hacker's Delight
        //Returns the number of bits set in @value
        [CLSCompliantAttribute(false)]
        public static int PopulationCount(uint value)
        {
            value = value - ((value >> 1) & 0x55555555);
            value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
            value = (value + (value >> 4)) & 0x0F0F0F0F;
            value = value + (value >> 8);
            value = value + (value >> 16);
            return (int)(value & 0x0000003F);
        }

        //Gem from BitTwiddler1011 at StackOverflow
        //Returns the number of bits set in @value
        [CLSCompliantAttribute(false)]
        public static int PopulationCount(ulong value)
        {
            value -= (value >> 1) & 0x5555555555555555UL;
            value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);
            value = (value + (value >> 4)) & 0x0f0f0f0f0f0f0f0fUL;
            return (int)((value * 0x0101010101010101UL) >> 56);
        }

        public static int PopulationCount(int value)
        {
            unchecked
            {
                return PopulationCount((uint)value);
            }
        }

        public static int PopulationCount(long value)
        {
            unchecked
            {
                return PopulationCount((ulong)value);
            }
        }

        public static float Round(this float number, int decimals)
        {
            return (float)Math.Round((double)number, decimals);
        }

        public static double Round(this double number, int decimals)
        {
            return Math.Round(number, decimals);
        }

        public static decimal Round(this decimal number, int decimals)
        {
            return Math.Round(number, decimals);
        }

        public static float Round(this float number, int decimals, MidpointRounding mode)
        {
            return (float)Math.Round((double)number, decimals, mode);
        }

        public static double Round(this double number, int decimals, MidpointRounding mode)
        {
            return Math.Round(number, decimals, mode);
        }

        public static decimal Round(this decimal number, int decimals, MidpointRounding mode)
        {
            return Math.Round(number, decimals, mode);
        }

        public static float Round(this float number)
        {
            return (float)Math.Round((double)number);
        }

        public static double Round(this double number)
        {
            return Math.Round(number);
        }

        public static decimal Round(this decimal number)
        {
            return Math.Round(number);
        }

        public static float Round(this float number, MidpointRounding mode)
        {
            return (float)Math.Round((double)number, mode);
        }

        public static double Round(this double number, MidpointRounding mode)
        {
            return Math.Round(number, mode);
        }

        public static decimal Round(this decimal number, MidpointRounding mode)
        {
            return Math.Round(number, mode);
        }
    }
}