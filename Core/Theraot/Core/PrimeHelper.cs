#if FAT

using System;

namespace Theraot.Core
{
    public static partial class PrimeHelper
    {
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static bool IsPrime(int number)
        {
            if (number < 0)
            {
                return false;
            }
            else
            {
                if (number < _smallPrimes[_smallPrimes.Length - 1])
                {
                    return Array.BinarySearch(_smallPrimes, number) >= 0;
                }
                else
                {
                    if (number == 2 || number == 3)
                    {
                        return true;
                    }
                    if ((number & 1) == 0 || number % 3 == 0)
                    {
                        return false;
                    }
                    int max = NumericHelper.Sqrt(number) + 1;
                    int index = 2;
                    for (; index < _smallPrimes.Length; index++)
                    {
                        if (number % _smallPrimes[index] == 0)
                        {
                            return false;
                        }
                        if (_smallPrimes[index] > max)
                        {
                            return true;
                        }
                    }
                    int test = index - (index % 6) + 5;
                    while (test < max)
                    {
                        if (number % test == 0 || number % (test += 2) == 0)
                        {
                            return false;
                        }
                        test += 4;
                    }
                    return true;
                }
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public static int NextPrime(int fromNumber)
        {
            if (fromNumber < 2)
            {
                return 2;
            }
            else
            {
                if (fromNumber < _smallPrimes[_smallPrimes.Length - 1])
                {
                    var index = Array.BinarySearch(_smallPrimes, fromNumber);
                    if (index < 0)
                    {
                        return _smallPrimes[-index - 1];
                    }
                    else
                    {
                        return fromNumber;
                    }
                }
                else
                {
                    if (fromNumber % 2 == 0)
                    {
                        fromNumber++;
                    }
                    for (int index = fromNumber; index < int.MaxValue; index += 2)
                    {
                        if (IsPrime(index))
                        {
                            return index;
                        }
                    }
                    return fromNumber;
                }
            }
        }
    }
}

#endif