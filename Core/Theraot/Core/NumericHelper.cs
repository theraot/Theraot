using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static partial class NumericHelper
    {
        public static double BuildDouble(int sign, long mantissa, int exponent)
        {
            if (sign == 0)
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
                ulong _mantissa = (ulong)mantissa;
                exponent += 1075;
                if (_mantissa != 0)
                {
                    while (true)
                    {
                        if ((_mantissa & (0xfff0000000000000L)) != 0 || exponent == 1)
                        {
                            break;
                        }
                        else
                        {
                            var tmp = _mantissa << 1;
                            _mantissa = tmp;
                            exponent--;
                        }
                    }
                }
                if (exponent == 1 && (_mantissa & (1L << 52)) == 0)
                {
                    exponent = 0;
                }
                uint divisor = 1;
                while (_mantissa > 0x1fffffffffffffL)
                {
                    divisor >>= 1;
                    exponent++;
                }
                _mantissa = _mantissa / divisor;
                _mantissa = _mantissa & 0xfffffffffffffL;
                if (exponent != 2047 && (exponent & 0x7ffL) == exponent && (_mantissa & 0xfffffffffffffL) == _mantissa)
                {
                    unchecked
                    {
                        ulong bits = (ulong)_mantissa | (ulong)((ulong)exponent << 52);
                        if (sign < 0)
                        {
                            bits |= 0x8000000000000000uL;
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

        public static long BuildLong(int hi, int lo)
        {
            return unchecked ((long)BuildUlong((uint)hi, (uint)lo));
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

        public static void GetParts(double value, out int sign, out long mantissa, out int exponent)
        {
            if (value == 0.0)
            {
                sign = 0;
                mantissa = 0;
                exponent = 1;
            }
            else
            {
                long bits = BitConverter.DoubleToInt64Bits(value);
                sign = (bits < 0) ? -1 : 1;
                exponent = (int) ((bits >> 52) & 0x7ffL);
                if (exponent == 2047)
                {
                    throw new ArgumentException("The value is NaN, PositiveInfinity or NegativeInfinity");
                }
                else
                {
                    mantissa = bits & 0xfffffffffffffL;
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
                        mantissa = mantissa | (1L << 52);
                    }

                    // Bias the exponent. It's actually biased by 1023, but we're
                    // treating the mantissa as m.0 rather than 0.m, so we need
                    // to subtract another 52 from it.
                    exponent -= 1075;

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

        [global::System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(int number)
        {
            if (number == 0)
            {
                throw new ArgumentException("The logarithm of zero is not defined.");
            }
            if (number <= 0)
            {
                throw new ArgumentException("The logarithm of a negative number is imaginary.");
            }
            else
            {
                number |= (number >> 1);
                number |= (number >> 2);
                number |= (number >> 4);
                number |= (number >> 8);
                number |= (number >> 16);
                return (PopulationCount(number >> 1));
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public static int NextPowerOf2(int number)
        {
            if (number < 0)
            {
                return 1;
            }
            else
            {
                uint _number;
                unchecked
                {
                    _number = (uint)number;
                }
                return (int)NextPowerOf2(_number);
            }
        }

        [CLSCompliantAttribute(false)]
        public static uint NextPowerOf2(uint number)
        {
            number |= (number >> 1);
            number |= (number >> 2);
            number |= (number >> 4);
            number |= (number >> 8);
            number |= (number >> 16);
            return (number + 1);
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

        [global::System.Diagnostics.DebuggerNonUserCode]
        private static int Sqrt(int number)
        {
            //  Newton's  method  aproximation  for  positive  integers
            //  if  (number  ==  0)  return  0;
            int x, _x = number >> 1;
            while (true)
            {
                x = (_x + (number / _x)) >> 1;
                if (x >= _x)
                {
                    return _x;
                }
                _x = x;
            }
        }
    }
}