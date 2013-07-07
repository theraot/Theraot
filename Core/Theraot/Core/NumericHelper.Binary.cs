using System;
using System.Collections.Generic;
using System.Text;

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

        public static string ToStringBinary(this double value)
        {
            return ToStringBinary(BitConverter.DoubleToInt64Bits(value));
        }

        public static string ToStringBinary(this float value)
        {
            return ToStringBinary(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this sbyte value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this short value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this int value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this long value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        public static string ToStringBinary(this byte value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this ushort value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this uint value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this ulong value)
        {
            return Theraot.Core.StringHelper.Concat(Theraot.Core.NumericHelper.BitsBinary(value), input => input.ToString());
        }

        //Gem from The Aggregate Magic Algorithms
        [CLSCompliantAttribute(false)]
        public static int TrailingZeroCount(this uint value)
        {
            return PopulationCount((value & -value) - 1);
        }

        public static int TrailingZeroCount(this int value)
        {
            uint _value;
            unchecked
            {
                _value = (uint)value;
            }
            return PopulationCount((_value & -_value) - 1);
        }

        //Gem from The Aggregate Magic Algorithms
        [CLSCompliantAttribute(false)]
        private static uint HighestBit(this uint value)
        {
            value = value | value >> 1;
            value = value | value >> 2;
            value = value | value >> 4;
            value = value | value >> 8;
            value = value | value >> 16;
            return value & ~(value >> 1);
        }
    }
}
