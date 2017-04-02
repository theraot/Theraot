// Needed for NET40

using System;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerStepThrough]
    public static partial class NumericHelper
    {
        [System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(int number)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException("number", "The logarithm of a negative number is imaginary.");
            }
            return Log2(unchecked((uint)number));
        }

        [CLSCompliant(false)]
        [System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(uint number)
        {
            if (number == 0)
            {
                throw new ArgumentOutOfRangeException("number", "The logarithm of zero is not defined.");
            }
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            return PopulationCount(number >> 1);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(long number)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException("number", "The logarithm of a negative number is imaginary.");
            }
            return Log2(unchecked((ulong)number));
        }

        [CLSCompliant(false)]
        [System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(ulong number)
        {
            if (number == 0)
            {
                throw new ArgumentOutOfRangeException("number", "The logarithm of zero is not defined.");
            }
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            number |= number >> 32;
            return PopulationCount(number >> 1);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public static int NextPowerOf2(int number)
        {
            if (number < 0)
            {
                return 1;
            }
            uint unsignedNumber;
            unchecked
            {
                unsignedNumber = (uint)number;
            }
            return (int)NextPowerOf2(unsignedNumber);
        }

        [CLSCompliant(false)]
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

        [System.Diagnostics.DebuggerNonUserCode]
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

        public static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }

        internal static uint GCD(uint u1, uint u2)
        {
            const int cvMax = 32;
            if (u1 < u2)
            {
                goto LOther;
            }
            LTop:
            if (u2 == 0)
            {
                return u1;
            }
            for (int cv = cvMax; ;)
            {
                u1 -= u2;
                if (u1 < u2)
                {
                    break;
                }

                if (--cv == 0)
                {
                    u1 %= u2;
                    break;
                }
            }
            LOther:
            if (u1 == 0)
            {
                return u2;
            }
            for (int cv = cvMax; ;)
            {
                u2 -= u1;
                if (u2 < u1)
                {
                    break;
                }

                if (--cv == 0)
                {
                    u2 %= u1;
                    break;
                }
            }
            goto LTop;
        }

        internal static ulong GCD(ulong uu1, ulong uu2)
        {
            const int cvMax = 32;
            if (uu1 < uu2)
            {
                goto LOther;
            }
            LTop:
            if (uu1 <= uint.MaxValue)
            {
                goto LSmall;
            }
            if (uu2 == 0)
            {
                return uu1;
            }
            for (int cv = cvMax; ;)
            {
                uu1 -= uu2;
                if (uu1 < uu2)
                {
                    break;
                }

                if (--cv == 0)
                {
                    uu1 %= uu2;
                    break;
                }
            }
            LOther:
            if (uu2 <= uint.MaxValue)
            {
                goto LSmall;
            }
            if (uu1 == 0)
            {
                return uu2;
            }
            for (int cv = cvMax; ;)
            {
                uu2 -= uu1;
                if (uu2 < uu1)
                {
                    break;
                }

                if (--cv == 0)
                {
                    uu2 %= uu1;
                    break;
                }
            }
            goto LTop;
            LSmall:
            var u1 = (uint)uu1;
            var u2 = (uint)uu2;
            if (u1 < u2)
            {
                goto LOtherSmall;
            }
            LTopSmall:
            if (u2 == 0)
            {
                return u1;
            }
            for (int cv = cvMax; ;)
            {
                u1 -= u2;
                if (u1 < u2)
                {
                    break;
                }

                if (--cv == 0)
                {
                    u1 %= u2;
                    break;
                }
            }
            LOtherSmall:
            if (u1 == 0)
            {
                return u2;
            }
            for (int cv = cvMax; ;)
            {
                u2 -= u1;
                if (u2 < u1)
                {
                    break;
                }

                if (--cv == 0)
                {
                    u2 %= u1;
                    break;
                }
            }
            goto LTopSmall;
        }

        internal static uint GetHi(ulong uu)
        {
            return (uint)(uu >> 32);
        }

        internal static uint GetLo(ulong uu)
        {
            return (uint)uu;
        }
    }
}