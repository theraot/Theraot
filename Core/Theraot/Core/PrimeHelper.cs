using System;

namespace Theraot.Core
{
    public static partial class PrimeHelper
    {
        [System.Diagnostics.DebuggerNonUserCode]
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
                    var max = NumericHelper.Sqrt(number) + 1;
                    var index = 2;
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
                    var test = index - (index % 6) + 5;
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

        [System.Diagnostics.DebuggerNonUserCode]
        public static int NextPrime(int fromNumber)
        {
            if (fromNumber < 2)
            {
                return 2;
            }
            if (fromNumber >= 2147483629)
            {
                throw new OverflowException("2147483629 is the last prime below int.MaxValue");
            }
            else
            {
                fromNumber++;
                return ToPrimeInternal(fromNumber);
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public static int ToPrime(int fromNumber)
        {
            if (fromNumber <= 2)
            {
                return 2;
            }
            if (fromNumber >= 2147483629)
            {
                return 2147483629;
            }
            else
            {
                return ToPrimeInternal(fromNumber);
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        internal static int ToPrimeInternal(int fromNumber)
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
                for (var index = fromNumber; index < int.MaxValue; index += 2)
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