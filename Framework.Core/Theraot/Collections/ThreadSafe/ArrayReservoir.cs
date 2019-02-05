// Needed for NET40

using System;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    internal static class ArrayReservoir
    {
        internal const int CapacityCount = 1 + MaxCapacityLog2 - MinCapacityLog2;
        internal const int MaxCapacity = 1 << MaxCapacityLog2;
        internal const int MaxCapacityLog2 = 16;
        internal const int MinCapacity = 1 << MinCapacityLog2;
        internal const int MinCapacityLog2 = 3;
        internal const int PoolSize = 16;

        private static readonly CacheDict<Type, Pool<Array>[]> _pools = new CacheDict<Type, Pool<Array>[]>(256);

        public static Pool<Array> GetPool<T>(int index)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var pools = _pools;
            if (pools == null)
            {
                return null;
            }

            if (!pools.TryGetValue(typeof(T), out var poolArray))
            {
                poolArray = pools[typeof(T)] = new Pool<Array>[CapacityCount];
                PopulatePools(poolArray);
            }

            var pool = poolArray?[index];
            return pool;
        }

        private static void PopulatePools(Pool<Array>[] poolArray)
        {
            for (var index = 0; index < CapacityCount; index++)
            {
                var currentIndex = index;
                poolArray[currentIndex] = new Pool<Array>
                (
                    PoolSize,
                    item =>
                    {
                        var currentCapacity = MinCapacity << currentIndex;
                        Array.Clear(item, 0, currentCapacity);
                    }

                );
            }
        }
    }

    internal static class ArrayReservoir<T>
    {
        internal static void DonateArray(T[] donation)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (donation == null)
            {
                return;
            }

            var capacity = donation.Length;
            if (capacity == 0 || capacity < ArrayReservoir.MinCapacity || capacity > ArrayReservoir.MaxCapacity)
            {
                return;
            }

            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            var index = NumericHelper.Log2(capacity) - ArrayReservoir.MinCapacityLog2;
            var pool = ArrayReservoir.GetPool<T>(index);
            pool?.Donate(donation);
        }

        internal static T[] GetArray(int capacity)
        {
            if (capacity == 0)
            {
                return ArrayEx.Empty<T>();
            }

            if (capacity < ArrayReservoir.MinCapacity)
            {
                capacity = ArrayReservoir.MinCapacity;
            }

            capacity = NumericHelper.PopulationCount(capacity) == 1 ? capacity : NumericHelper.NextPowerOf2(capacity);
            if (capacity > ArrayReservoir.MaxCapacity)
            {
                return new T[capacity];
            }

            var index = NumericHelper.Log2(capacity) - ArrayReservoir.MinCapacityLog2;
            var pool = ArrayReservoir.GetPool<T>(index);
            if (pool == null)
            {
                return new T[capacity];
            }

            if (pool.TryGet(out var result))
            {
                return (T[])result;
            }

            return new T[capacity];
        }
    }
}