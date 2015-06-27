// Needed for NET40

using System;

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        public static double BuildDouble(int sign, long mantissa, int exponent)
        {
            if (sign == 0 || mantissa == 0)
            {
                return 0.0;
            }
            else
            {
                if (mantissa < 0)
                {
                    mantissa = -mantissa;
                    sign = -sign;
                }
                var _mantissa = (ulong)mantissa;
                return BuildDouble(sign, _mantissa, exponent);
            }
        }

        [CLSCompliantAttribute(false)]
        public static double BuildDouble(int sign, ulong mantissa, int exponent)
        {
            const int ExponentBias = 1023;
            const int MantissaLength = 52;
            const int ExponentLength = 11;
            const int MaxExponent = 2046;
            const long MantissaMask = 0xfffffffffffffL;
            const long ExponentMask = 0x7ffL;
            const ulong NegativeMark = 0x8000000000000000uL;

            if (sign == 0 || mantissa == 0)
            {
                return 0.0;
            }
            else
            {
                exponent += ExponentBias + MantissaLength;
                int offset = LeadingZeroCount(mantissa) - ExponentLength;
                if (exponent - offset > MaxExponent)
                {
                    return sign > 0 ? double.PositiveInfinity : double.NegativeInfinity;
                }
                else
                {
                    if (offset < 0)
                    {
                        mantissa >>= -offset;
                        exponent += -offset;
                    }
                    else if (offset >= exponent)
                    {
                        mantissa <<= exponent - 1;
                        exponent = 0;
                    }
                    else
                    {
                        mantissa <<= offset;
                        exponent -= offset;
                    }
                    mantissa = mantissa & MantissaMask;
                    if ((exponent & ExponentMask) == exponent)
                    {
                        unchecked
                        {
                            ulong bits = mantissa | ((ulong)exponent << MantissaLength);
                            if (sign < 0)
                            {
                                bits |= NegativeMark;
                            }
                            return BitConverter.Int64BitsToDouble((long)bits);
                        }
                    }
                    else
                    {
                        return sign > 0 ? double.PositiveInfinity : double.NegativeInfinity;
                    }
                }
            }
        }

        public static long BuildInt64(int hi, int lo)
        {
            return unchecked((long)((ulong)(uint)hi << 32 | (uint)lo));
        }

        [CLSCompliantAttribute(false)]
        public static long BuildInt64(uint hi, uint lo)
        {
            return unchecked((long)((ulong)hi << 32 | lo));
        }

        public static float BuildSingle(int sign, int mantissa, int exponent)
        {
            return (float)BuildDouble(sign, mantissa, exponent);
        }

        [CLSCompliantAttribute(false)]
        public static float BuildSingle(int sign, uint mantissa, int exponent)
        {
            return (float)BuildDouble(sign, mantissa, exponent);
        }

        [CLSCompliantAttribute(false)]
        public static ulong BuildUInt64(uint hi, uint lo)
        {
            return (ulong)hi << 32 | lo;
        }

        public static void GetParts(long value, out int lo, out int hi)
        {
            unchecked
            {
                lo = (int)value;
                hi = (int)((ulong)value >> 32);
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

        public static void GetParts(float value, out int sign, out int mantissa, out int exponent)
        {
            if (value.CompareTo(0.0f) == 0)
            {
                sign = 0;
                mantissa = 0;
                exponent = 1;
            }
            else
            {
                int bits = SingleAsInt32(value);
                sign = (bits < 0) ? -1 : 1;
                exponent = (bits >> 23) & 0xff;
                if (exponent == 2047)
                {
                    throw new ArgumentException("The value is NaN, PositiveInfinity or NegativeInfinity");
                }
                else
                {
                    mantissa = bits & 0xffffff;
                    if (exponent == 0)
                    {
                        // Subnormal numbers; exponent is effectively one higher,
                        // but there's no extra normalisation bit in the mantissa
                        exponent = 1;
                    }
                    else
                    {
                        // Normal numbers; leave exponent as it is but add extra
                        // bit to the front of the mantissa
                        mantissa = mantissa | (1 << 23);
                    }
                    // Bias the exponent. It's actually biased by 127, but we're
                    // treating the mantissa as m.0 rather than 0.m, so we need
                    // to subtract another 23 from it.
                    exponent -= 150;
                    if (mantissa != 0)
                    {
                        while ((mantissa & 1) == 0)
                        {
                            mantissa >>= 1;
                            exponent++;
                        }
                    }
                }
            }
        }

        public static void GetParts(double value, out int sign, out long mantissa, out int exponent, out bool finite)
        {
            ulong _mantissa;
            System.Numerics.NumericsHelpers.GetDoubleParts(value, out sign, out exponent, out _mantissa, out finite);
            mantissa = (long)_mantissa;
        }
    }
}