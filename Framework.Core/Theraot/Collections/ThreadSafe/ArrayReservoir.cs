// Needed for NET40

#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CA1825 // Avoid zero-length array allocations.
#pragma warning disable RECS0108 // Warns about static fields in generic types

using System;
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
        private static readonly Pool<T[]>[] _pools = CreatePools();

        internal static void DonateArray(T[] donation)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (donation == null)
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
            pool?.Donate(donation);
        }

        internal static T[] GetArray(int capacity)
        {
            if (capacity == 0)
            {
                return ArrayEx.Empty<T>();
            }

            if (capacity < _minCapacity)
            {
                capacity = _minCapacity;
            }

            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            if (capacity > _maxCapacity)
            {
                return new T[capacity];
            }

            var index = NumericHelper.Log2(capacity) - _minCapacityLog2;
            var currentPool = _pools?[index];
            if (currentPool != null && currentPool.TryGet(out var result))
            {
                return result;
            }
            return new T[capacity];
        }

        private static Pool<T[]>[] CreatePools()
        {
            var pools = new Pool<T[]>[_capacityCount];
            for (var index = 0; index < _capacityCount; index++)
            {
                var currentIndex = index;
                pools[index] = new Pool<T[]>
                (
                    _poolSize,
                    item =>
                    {
                        var currentCapacity = _minCapacity << currentIndex;
                        Array.Clear(item, 0, currentCapacity);
                    }
                );
            }

            return pools;
        }
    }
}