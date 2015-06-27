// Needed for NET40

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        public static int BinaryReverse(this int value)
        {
            unchecked
            {
                return (int)BinaryReverse((uint)value);
            }
        }

        public static long BinaryReverse(this long value)
        {
            unchecked
            {
                return (long)BinaryReverse((ulong)value);
            }
        }

        [CLSCompliantAttribute(false)]
        public static uint BinaryReverse(this uint value)
        {
            value = ((value & 0xaaaaaaaa) >> 1) | ((value & 0x55555555) << 1);
            value = ((value & 0xcccccccc) >> 2) | ((value & 0x33333333) << 2);
            value = ((value & 0xf0f0f0f0) >> 4) | ((value & 0x0f0f0f0f) << 4);
            value = ((value & 0xff00ff00) >> 8) | ((value & 0x00ff00ff) << 8);
            return (value >> 16) | (value << 16);
        }

        [CLSCompliantAttribute(false)]
        public static ulong BinaryReverse(this ulong value)
        {
            uint lo;
            uint hi;
            GetParts(value, out lo, out hi);
            return BuildUInt64(BinaryReverse(lo), BinaryReverse(hi));
        }

        [CLSCompliantAttribute(false)]
        public static IEnumerable<sbyte> Bits(this sbyte value)
        {
            unchecked
            {
                byte check = 1 << 7;
                int log2 = 8;
                var _value = (byte)value;
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
                ushort check = 1 << 15;
                int log2 = 16;
                var _value = (ushort)value;
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
                var _value = (uint)value;
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
                var _value = (ulong)value;
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
                byte check = 1 << 7;
                int log2 = 8;
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
        public static IEnumerable<ushort> Bits(this ushort value)
        {
            unchecked
            {
                ushort check = 1 << 15;
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
                byte check = 1 << 7;
                int log2 = 8;
                var _value = (byte)value;
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
                ushort check = 1 << 15;
                int log2 = 16;
                var _value = (ushort)value;
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
                var _value = (uint)value;
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
                var _value = (ulong)value;
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
                byte check = 1 << 7;
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
                ushort check = 1 << 15;
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
                byte check = 1 << 7;
                int log2 = 8;
                var _value = (byte)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2 - 1;
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
                ushort check = 1 << 15;
                int log2 = 16;
                var _value = (ushort)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2 - 1;
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
                var _value = (uint)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2 - 1;
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
                var _value = (ulong)value;
                do
                {
                    if ((_value & check) != 0)
                    {
                        yield return log2 - 1;
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
                byte check = 1 << 7;
                int log2 = 8;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2 - 1;
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
                ushort check = 1 << 15;
                int log2 = 16;
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2 - 1;
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
                        yield return log2 - 1;
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
                        yield return log2 - 1;
                    }
                    check >>= 1;
                    log2--;
                }
                while (log2 > 0);
            }
        }

        public static long DoubleAsInt64(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        [CLSCompliantAttribute(false)]
        public static ulong DoubleAsUInt64(double value)
        {
            unchecked
            {
                return (ulong)BitConverter.DoubleToInt64Bits(value);
            }
        }

        public static float Int32AsSingle(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        public static double Int64AsDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }

        public static int LeadingZeroCount(this int value)
        {
            unchecked
            {
                return LeadingZeroCount((uint)value);
            }
        }

        public static int LeadingZeroCount(this long value)
        {
            unchecked
            {
                return LeadingZeroCount((ulong)value);
            }
        }

        [CLSCompliantAttribute(false)]
        public static int LeadingZeroCount(this uint value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return (sizeof(int) * 8) - PopulationCount(value);
        }

        [CLSCompliantAttribute(false)]
        public static int LeadingZeroCount(this ulong value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            return (sizeof(long) * 8) - PopulationCount(value);
        }

        // Gem from Hacker's Delight
        // Returns the number of bits set in @value
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

        // Based on code by Zilong Tan on Ulib released under MIT license
        // Returns the number of bits set in @x
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

        public static int SingleAsInt32(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        [CLSCompliantAttribute(false)]
        public static uint SingleAsUInt32(float value)
        {
            unchecked
            {
                return (uint)BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
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
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        public static string ToStringBinary(this short value)
        {
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        public static string ToStringBinary(this int value)
        {
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        public static string ToStringBinary(this long value)
        {
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        public static string ToStringBinary(this byte value)
        {
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this ushort value)
        {
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this uint value)
        {
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        [CLSCompliantAttribute(false)]
        public static string ToStringBinary(this ulong value)
        {
            return StringHelper.Concat(value.BitsBinary(), input => input.ToString(CultureInfo.InvariantCulture));
        }

        public static int TrailingZeroCount(this int value)
        {
            return LeadingZeroCount(BinaryReverse(value));
        }

        public static int TrailingZeroCount(this long value)
        {
            return LeadingZeroCount(BinaryReverse(value));
        }

        [CLSCompliantAttribute(false)]
        public static int TrailingZeroCount(this uint value)
        {
            return LeadingZeroCount(BinaryReverse(value));
        }

        [CLSCompliantAttribute(false)]
        public static int TrailingZeroCount(this ulong value)
        {
            return LeadingZeroCount(BinaryReverse(value));
        }

        [CLSCompliantAttribute(false)]
        public static float UInt32AsSingle(uint value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        [CLSCompliantAttribute(false)]
        public static double UInt64AsDouble(ulong value)
        {
            unchecked
            {
                return BitConverter.Int64BitsToDouble((long)value);
            }
        }
    }
}