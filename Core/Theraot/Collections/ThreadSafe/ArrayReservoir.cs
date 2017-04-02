// Needed for NET40

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

        private const int _capacityCount = 1 + _maxCapacityLog2 - _minCapacityLog2;
        private const int _maxCapacity = 1 << _maxCapacityLog2;
        private const int _maxCapacityLog2 = 10;
        private const int _minCapacity = 1 << _minCapacityLog2;
        private const int _minCapacityLog2 = 3;
        private const int _poolSize = 16;

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
            _pools = new Pool<T[]>[_capacityCount];
            for (int index = 0; index < _capacityCount; index++)
            {
                var currentIndex = index;
                _pools[index] = new Pool<T[]>
                    (
                        _poolSize,
                        item =>
                        {
                            var currentCapacity = _minCapacity << currentIndex;
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
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (donation == null || Volatile.Read(ref _done) == 0)
            {
                return;
            }
            var pools = _pools;
            if (pools == null)
            {
                return;
            }
            var capacity = donation.Length;
            if (capacity == 0 || capacity < _minCapacity || capacity > _maxCapacity)
            {
                return;
            }
            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            var index = NumericHelper.Log2(capacity) - _minCapacityLog2;
            var pool = pools[index];
            if (pool == null)
            {
                return;
            }
            pool.Donate(donation);
        }

        internal static T[] GetArray(int capacity)
        {
            if (capacity == 0)
            {
                return _emptyArray;
            }
            if (capacity < _minCapacity)
            {
                capacity = _minCapacity;
            }
            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            if (capacity <= _maxCapacity && Thread.VolatileRead(ref _done) == 1)
            {
                var index = NumericHelper.Log2(capacity) - _minCapacityLog2;
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