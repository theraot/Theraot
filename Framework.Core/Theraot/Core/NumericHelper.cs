// Needed for NET40

using System;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerStepThrough]
    public static partial class NumericHelper
    {
        [CLSCompliant(false)]
        public static uint Abs(int a)
        {
            var mask = (uint)(a >> 31);
            return ((uint)a ^ mask) - mask;
        }

        public static int GCD(int left, int right)
        {
            if (left < 0)
            {
                left = -left;
            }
            if (right < 0)
            {
                right = -right;
            }
            return (int)GCD((uint)left, (uint)right);
        }

        [CLSCompliant(false)]
        public static uint GCD(uint left, uint right)
        {
            const int CvMax = 32;
            if (left < right)
            {
                Swap(ref left, ref right);
            }
            while (true)
            {
                if (right == 0)
                {
                    return left;
                }
                for (var cv = CvMax; ;)
                {
                    left -= right;
                    if (left < right)
                    {
                        break;
                    }
                    if (--cv == 0)
                    {
                        left %= right;
                        break;
                    }
                }
                Swap(ref left, ref right);
            }
        }

        public static long GCD(long left, long right)
        {
            if (left < 0)
            {
                left = -left;
            }
            if (right < 0)
            {
                right = -right;
            }
            return (long)GCD((ulong)left, (ulong)right);
        }

        [CLSCompliant(false)]
        public static ulong GCD(ulong uu1, ulong uu2)
        {
            const int CvMax = 32;
            if (uu1 < uu2)
            {
                Swap(ref uu1, ref uu2);
            }
            while (true)
            {
                if (uu1 <= uint.MaxValue)
                {
                    break;
                }
                if (uu2 == 0)
                {
                    return uu1;
                }
                for (var cv = CvMax; ;)
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
                Swap(ref uu1, ref uu2);
            }
            return GCD((uint)uu1, (uint)uu2);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(int number)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "The logarithm of a negative number is imaginary.");
            }
            return Log2(unchecked((uint)number));
        }

        [CLSCompliant(false)]
        [System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(uint number)
        {
            if (number == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "The logarithm of zero is not defined.");
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
                throw new ArgumentOutOfRangeException(nameof(number), "The logarithm of a negative number is imaginary.");
            }
            return Log2(unchecked((ulong)number));
        }

        [CLSCompliant(false)]
        [System.Diagnostics.DebuggerNonUserCode]
        public static int Log2(ulong number)
        {
            if (number == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "The logarithm of zero is not defined.");
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
            // Newton's  method  approximation  for  positive  integers
            // if  (number  ==  0)  return  0;
            var x = number >> 1;
            while (true)
            {
                var xNext = (x + (number / x)) >> 1;
                if (xNext >= x)
                {
                    return x;
                }
                x = xNext;
            }
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
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