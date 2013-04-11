using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static class NumericHelper
    {
        public static IEnumerable<int> Bits(this int value)
        {
            int check = 1;
            unchecked
            {
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return check;
                    }
                    check *= 2;
                }
                while (check <= value);
            }
        }

        public static IEnumerable<short> Bits(this short value)
        {
            short check = 1;
            unchecked
            {
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return check;
                    }
                    check *= 2;
                }
                while (check <= value);
            }
        }

        public static IEnumerable<long> Bits(this long value)
        {
            long check = 1;
            unchecked
            {
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return check;
                    }
                    check *= 2;
                }
                while (check <= value);
            }
        }

        public static IEnumerable<int> BitsLog2(this int value)
        {
            int check = 1;
            int log2 = 0;
            unchecked
            {
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2;
                    }
                    check *= 2;
                    log2++;
                }
                while (check <= value);
            }
        }

        public static IEnumerable<int> BitsLog2(this short value)
        {
            short check = 1;
            int log2 = 0;
            unchecked
            {
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2;
                    }
                    check *= 2;
                    log2++;
                }
                while (check <= value);
            }
        }

        public static IEnumerable<int> BitsLog2(this long value)
        {
            long check = 1;
            int log2 = 0;
            unchecked
            {
                do
                {
                    if ((value & check) != 0)
                    {
                        yield return log2;
                    }
                    check *= 2;
                    log2++;
                }
                while (check <= value);
            }
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