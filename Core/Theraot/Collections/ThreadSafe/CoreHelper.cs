using System;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal static class CoreHelper
    {
        private static readonly int[] _capacities =
        {
            64,         // 64               // 1
            64,         // 128              // 2
            128,        // 256              // 3
            256,        // 512              // 4
            512,        // 1024             // 5
            1024,       // 2048             // 6
            2048,       // 4096             // 7
            4096,       // 8192             // 8
            8192,       // 16384            // 9
            16384,      // 32768            // 10
            32768,      // 65536            // 11
            65536,      // 131072           // 12
            131072,     // 262144           // 13
            262144,     // 524288           // 14
            524288,     // 1048576          // 15
            1048576,    // 2097152          // 16
            2097152,    // 4194304 x 1      // 17
            4194304     // 4194304 x 2      // 18
        };

        internal static T EnterMayDecrement<T>(ref int use, ref T second)
            where T : class
        {
            // May remove - if it doesn't exist, there is nothing to do
            // Read second
            var foundSecond = Interlocked.CompareExchange(ref second, null, null);
            if (foundSecond == null)
            {
                return null;
            }
            Interlocked.Increment(ref use);
            return foundSecond;
        }

        internal static T EnterMayIncrement<T>(ref int use, ref T first, ref T second, Func<T> factory, out bool isNew)
            where T : class
        {
            isNew = false;
            // May add - make sure second exists
            // Read first
            var foundFirst = Interlocked.CompareExchange(ref first, null, null);
            // Try to restore second
            var foundSecond = Interlocked.CompareExchange(ref second, foundFirst, null);
            if (foundSecond == null)
            {
                // second was set to first
                if (foundFirst == null)
                {
                    // We need to recreate the first
                    var result = factory();
                    // Try to set to first
                    foundFirst = Interlocked.CompareExchange(ref first, result, null);
                    if (foundFirst == null)
                    {
                        isNew = true;
                        // _firstt was set to result
                        // Try to set to second
                        Interlocked.CompareExchange(ref second, result, null);
                        // Use result - will return first
                        foundFirst = result;
                    }
                    // Another thread has set first, use first
                }
                // Use first
            }
            Interlocked.Increment(ref use);
            return foundFirst;
        }

        internal static T EnterRead<T>(ref int use, ref T first)
            where T : class
        {
            // Only to read
            // Read first
            var foundFirst = Interlocked.CompareExchange(ref first, null, null);
            if (foundFirst == null)
            {
                return null;
            }
            Interlocked.Increment(ref use);
            return foundFirst;
        }

        internal static int GetCapacity(int index)
        {
            if (index < _capacities.Length)
            {
                return _capacities[index];
            }
            if (index < 527)
            {
                return _capacities[_capacities.Length - 1];
            }
            if (index == 527)
            {
                return _capacities[_capacities.Length - 1] - 1;
            }
            return 0;
        }

        internal static T GetOrInsert<T>(this ICore<T> core, int index, Func<T> valueFactory)
        {
            var value = default(T);
            core.Do
                (
                    index,
                    true,
                    (int capacity, int position, ref int use, ref FixedSizeBucket<T> first, ref FixedSizeBucket<T> second) =>
                    {
                        try
                        {
                            T stored;
                            bool isNew;
                            var bucket = EnterMayIncrement(ref use, ref first, ref second,
                                () => new FixedSizeBucket<T>(capacity), out isNew);
                            // Do must have called Grow, ignore isNew
                            if (bucket.TryGetOrInsert(index - position, valueFactory, out stored))
                            {
                                Interlocked.Increment(ref use);
                            }
                            value = stored;
                        }
                        finally
                        {
                            Leave(ref use, ref first, ref second);
                        }
                    }
                );
            return value;
        }

        internal static bool Leave<T>(ref int use, ref T first, ref T second)
            where T : class
        {
            if (Interlocked.Decrement(ref use) != 0)
            {
                return false;
            }
            // Erase second
            var foundSecond = Interlocked.Exchange(ref second, null);
            // Erase first - second may have been restored by another thread
            Interlocked.CompareExchange(ref first, null, foundSecond);
            // Read second
            foundSecond = Interlocked.CompareExchange(ref second, null, null);
            // Set first to second - either erased or restored
            Interlocked.CompareExchange(ref first, foundSecond, null);
            return true;
        }

        internal static bool Remove<T>(this ICore<T> core, int index)
        {
            var result = false;
            core.Do
                (
                    index,
                    false,
                    (int capacity, int position, ref int use, ref FixedSizeBucket<T> first, ref FixedSizeBucket<T> second) =>
                    {
                        try
                        {
                            T previous;
                            var bucket = EnterMayDecrement(ref use, ref second);
                            if (bucket.RemoveAtInternal(index - position, out previous))
                            {
                                Interlocked.Decrement(ref use);
                                result = true;
                            }
                            result = false;
                        }
                        finally
                        {
                            Leave(ref use, ref first, ref second);
                        }
                    }
                );
            return result;
        }

        internal static void Set<T>(this ICore<T> core, int index, T value)
        {
            core.Do
                (
                    index,
                    true,
                    (int capacity, int position, ref int use, ref FixedSizeBucket<T> first, ref FixedSizeBucket<T> second) =>
                    {
                        try
                        {
                            bool isNew;
                            var bucket = EnterMayIncrement(ref use, ref first, ref second, () => new FixedSizeBucket<T>(capacity), out isNew);
                            // Do must have called Grow, ignore isNew
                            bucket.SetInternal(index - position, value, out isNew);
                            if (isNew)
                            {
                                Interlocked.Increment(ref use);
                            }
                        }
                        finally
                        {
                            Leave(ref use, ref first, ref second);
                        }
                    }
                );
        }

        internal static bool TryGetValue<T>(this ICore<T> core, int index, out T value)
        {
            var result = false;
            var found = default(T);
            core.Do
                (
                    index,
                    false,
                    (int capacity, int position, ref int use, ref FixedSizeBucket<T> first, ref FixedSizeBucket<T> second) =>
                    {
                        try
                        {
                            var bucket = EnterRead(ref use, ref first);
                            if (bucket == null)
                            {
                                result = false;
                            }
                            else
                            {
                                T tmp;
                                result = bucket.TryGet(index - position, out tmp);
                                found = tmp;
                            }
                        }
                        finally
                        {
                            Leave(ref use, ref first, ref second);
                        }
                    }
                );
            value = found;
            return result;
        }
    }
}