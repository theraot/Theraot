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

        //Gem from Hacker's Delight
        //Returns the number of bits set in @x
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
        //Returns the number of bits set in @x
        [CLSCompliantAttribute(false)]
        public static int PopulationCount(ulong value)
        {
            value -= (value >> 1) & 0x5555555555555555UL;
            value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);
            value = (value + (value >> 4)) & 0x0f0f0f0f0f0f0f0fUL;
            return (int)((value * 0x0101010101010101UL) >> 56);
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