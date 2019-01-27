#if FAT
using System;
using System.Diagnostics;

namespace Theraot.Core
{
    public static partial class PrimeHelper
    {
        [DebuggerNonUserCode]
        public static bool IsPrime(int number)
        {
            if (number < 0)
            {
                return false;
            }
            if (number < _smallPrimes[_smallPrimes.Length - 1])
            {
                return Array.BinarySearch(_smallPrimes, number) >= 0;
            }
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
                ref var current = ref _smallPrimes[index];
                if (number % current == 0)
                {
                    return false;
                }
                if (current > max)
                {
                    return true;
                }
            }
            var test = index - index % 6 + 5;
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

        [DebuggerNonUserCode]
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
            fromNumber++;
            return ToPrimeInternal(fromNumber);
        }

        [DebuggerNonUserCode]
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
            return ToPrimeInternal(fromNumber);
        }

        [DebuggerNonUserCode]
        internal static int ToPrimeInternal(int fromNumber)
        {
            if (fromNumber < _smallPrimes[_smallPrimes.Length - 1])
            {
                var index = Array.BinarySearch(_smallPrimes, fromNumber);
                if (index < 0)
                {
                    return _smallPrimes[-index - 1];
                }
                return fromNumber;
            }
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

#endif