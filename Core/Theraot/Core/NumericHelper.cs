// Needed for NET40

using System;

namespace Theraot.Core
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.DebuggerStepThrough]
    public static partial class NumericHelper
    {
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(int number)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException("The logarithm of a negative number is imaginary.");
            }
            else
            {
                return Log2(unchecked((uint)number));
            }
        }

        [CLSCompliantAttribute(false)]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(uint number)
        {
            if (number == 0)
            {
                throw new ArgumentOutOfRangeException("The logarithm of zero is not defined.");
            }
            else
            {
                number |= number >> 1;
                number |= number >> 2;
                number |= number >> 4;
                number |= number >> 8;
                number |= number >> 16;
                return PopulationCount(number >> 1);
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
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            return number + 1;
        }

        public static float Round(this float number, int decimals)
        {
            return (float)Math.Round(number, decimals);
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
            return (float)Math.Round(number, decimals, mode);
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
            return (float)Math.Round(number);
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
            return (float)Math.Round(number, mode);
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
        public static int Sqrt(int number)
        {
            // Newton's  method  aproximation  for  positive  integers
            // if  (number  ==  0)  return  0;
            int _x = number >> 1;
            while (true)
            {
                int x = (_x + (number / _x)) >> 1;
                if (x >= _x)
                {
                    return _x;
                }
                _x = x;
            }
        }
    }
}