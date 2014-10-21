using System;

#if FAT

using Theraot.Threading;

#else

using Theraot.Core;

#endif

namespace Theraot.Collections.ThreadSafe
{
    public static class ArrayReservoir<T>
    {
#if !FAT
        private const int INT_MaxCapacity = 1024;
        private const int INT_MinCapacity = 16;
#endif

        private static readonly T[] _emptyArray;

        static ArrayReservoir()
        {
            if (typeof(T) == typeof(Type))
            {
                _emptyArray = (T[])(object)Type.EmptyTypes;
            }
            else
            {
                _emptyArray = new T[0];
            }
        }

        public static T[] EmptyArray
        {
            get
            {
                return _emptyArray;
            }
        }

        internal static void DonateArray(T[] donation)
        {
#if FAT
            ArrayPool<T>.DonateArray(donation);
#endif
        }

        internal static T[] GetArray(int capacity)
        {
#if FAT
            return ArrayPool<T>.GetArray(capacity);
#else
            if (capacity == 0)
            {
                return _emptyArray;
            }
            else if (capacity < INT_MinCapacity)
            {
                capacity = INT_MinCapacity;
            }
            else
            {
                if (capacity > INT_MaxCapacity)
                {
                    //Too big to leak it
                    return new T[capacity];
                }
                else
                {
                    capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
                }
            }
            return new T[capacity];
#endif
        }
    }
}