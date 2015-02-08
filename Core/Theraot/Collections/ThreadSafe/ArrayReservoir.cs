using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    public static class ArrayReservoir<T>
    {
        // sizes:
        // 0  1   2   3   4    5    6    7
        // 8, 16, 32, 64, 128, 256, 512, 1024

        private const int INT_CapacityCount = INT_MaxCapacityLog2 - INT_MinCapacityLog2;
        private const int INT_MaxCapacity = 1 << INT_MaxCapacityLog2;
        private const int INT_MaxCapacityLog2 = 10;
        private const int INT_MinCapacity = 1 << INT_MinCapacityLog2;
        private const int INT_MinCapacityLog2 = 3;
        private const int INT_PoolSize = 16;

        private static readonly T[] _emptyArray;
        private static readonly Pool<T[]>[] _pools;
        private static int _done;

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
            _pools = new Pool<T[]>[INT_CapacityCount];
            for (int index = 0; index < INT_CapacityCount; index++)
            {
                var currentIndex = index;
                _pools[index] = new Pool<T[]>
                    (
                        INT_PoolSize,
                        item =>
                        {
                            var currentCapacity = INT_MinCapacity << currentIndex;
                            Array.Clear(item, 0, currentCapacity);
                        }
                    );
            }
            Thread.VolatileWrite(ref _done, 1);
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
            if (donation == null || Thread.VolatileRead(ref _done) == 0)
            {
                return;
            }
            var capacity = donation.Length;
            if (capacity == 0 || capacity < INT_MinCapacity || capacity > INT_MaxCapacity)
            {
                return;
            }
            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            var index = NumericHelper.Log2(capacity) - INT_MinCapacityLog2;
            _pools[index].Donate(donation);
        }

        internal static T[] GetArray(int capacity)
        {
            if (capacity == 0)
            {
                return _emptyArray;
            }
            if (capacity < INT_MinCapacity)
            {
                capacity = INT_MinCapacity;
            }
            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            if (capacity <= INT_MaxCapacity && Thread.VolatileRead(ref _done) == 1)
            {
                var index = NumericHelper.Log2(capacity) - INT_MinCapacityLog2;
                T[] result;
                var currentPool = _pools[index];
                if (currentPool.TryGet(out result))
                {
                    return result;
                }
            }
            return new T[capacity];
        }
    }
}