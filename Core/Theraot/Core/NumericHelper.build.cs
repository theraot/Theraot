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
                var tmpMantissa = (ulong)mantissa;
                return System.Numerics.NumericsHelpers.GetDoubleFromParts(sign, exponent, tmpMantissa);
            }
        }

        [CLSCompliant(false)]
        public static double BuildDouble(int sign, ulong mantissa, int exponent)
        {
            return System.Numerics.NumericsHelpers.GetDoubleFromParts(sign, exponent, mantissa);
        }

        public static long BuildInt64(int hi, int lo)
        {
            return unchecked((long)((ulong)(uint)hi << 32 | (uint)lo));
        }

        [CLSCompliant(false)]
        public static long BuildInt64(uint hi, uint lo)
        {
            return unchecked((long)((ulong)hi << 32 | lo));
        }

        public static float BuildSingle(int sign, int mantissa, int exponent)
        {
            return (float)BuildDouble(sign, mantissa, exponent);
        }

        [CLSCompliant(false)]
        public static float BuildSingle(int sign, uint mantissa, int exponent)
        {
            return (float)BuildDouble(sign, mantissa, exponent);
        }

        [CLSCompliant(false)]
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

        public static void GetParts(int value, out short lo, out short hi)
        {
            unchecked
            {
                lo = (short)value;
                hi = (short)(value >> 16);
            }
        }

        [CLSCompliant(false)]
        public static void GetParts(ulong value, out uint lo, out uint hi)
        {
            unchecked
            {
                lo = (uint)value;
                hi = (uint)(value >> 32);
            }
        }

        [CLSCompliant(false)]
        public static void GetParts(uint value, out ushort lo, out ushort hi)
        {
            unchecked
            {
                lo = (ushort)value;
                hi = (ushort)(value >> 16);
            }
        }

        public static void GetParts(float value, out int sign, out int mantissa, out int exponent, out bool finite)
        {
            if (value.CompareTo(0.0f) == 0)
            {
                sign = 0;
                mantissa = 0;
                exponent = 1;
                finite = true;
            }
            else
            {
                var bits = SingleAsInt32(value);
                sign = (bits < 0) ? -1 : 1;
                exponent = (bits >> 23) & 0xff;
                if (exponent == 2047)
                {
                    finite = false;
                    mantissa = 0;
                }
                else
                {
                    finite = true;
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
                        mantissa |= 1 << 23;
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
            ulong tmpMantissa;
            System.Numerics.NumericsHelpers.GetDoubleParts(value, out sign, out exponent, out tmpMantissa, out finite);
            mantissa = (long)tmpMantissa;
        }

        internal static int CbitLowZero(uint u)
        {
            if (u == 0)
            {
                return 32;
            }
            var cbit = 0;
            if ((u & 0x0000FFFF) == 0)
            {
                cbit += 16;
                u >>= 16;
            }
            if ((u & 0x000000FF) == 0)
            {
                cbit += 8;
                u >>= 8;
            }
            if ((u & 0x0000000F) == 0)
            {
                cbit += 4;
                u >>= 4;
            }
            if ((u & 0x00000003) == 0)
            {
                cbit += 2;
                u >>= 2;
            }
            if ((u & 0x00000001) == 0)
            {
                cbit += 1;
            }
            return cbit;
        }
    }
}