using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal class ArrayPool<T>
    {
        private const int INT_MaxCapacity = 1024;
        private const int INT_MinCapacity = 16;
        private const int INT_WorkCapacityHint = 16;
        private const int INT_PoolSize = 16;

        private static LazyBucket<FixedSizeQueueBucket<T[]>> _data;
        private static Work.Context _recycle;
        private static int _done;
        private static Guard _guard;

        static ArrayPool()
        {
            _recycle = new Work.Context("Recycler", INT_WorkCapacityHint, 1);
            _data = new LazyBucket<FixedSizeQueueBucket<T[]>>
            (
                input =>
                {
                    return new FixedSizeQueueBucket<T[]>(INT_PoolSize);
                },
                NumericHelper.Log2(INT_MaxCapacity) - NumericHelper.Log2(INT_MinCapacity)
            );
            _guard = new Guard();
            Thread.MemoryBarrier();
            Thread.VolatileWrite(ref _done, 1);
        }

        public static bool DonateArray(T[] array)
        {
            if (ReferenceEquals(array, null))
            {
                //Ignore
                return false;
            }
            else
            {
                int capacity = array.Length;
                if (NumericHelper.PopulationCount(capacity) == 1)
                {
                    if (capacity < INT_MinCapacity || capacity > INT_MaxCapacity)
                    {
                        //Rejected
                        return false;
                    }
                    else
                    {
                        _recycle.AddWork
                        (
                            () =>
                            {
                                Array.Clear(array, 0, capacity);
                                var bucket = GetBucket(capacity);
                                bucket.Enqueue(array);
                            }
                        ).Start();
                        return true;
                    }
                }
                else
                {
                    throw new ArgumentException("The size of the array must be a power of two.", "array");
                }
            }
        }

        public static T[] GetArray(int capacity)
        {
            if (capacity < INT_MinCapacity)
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
            if (Thread.VolatileRead(ref _done) == 1)
            {
                IDisposable engagement;
                if (_guard.Enter(out engagement))
                {
                    using (engagement)
                    {
                        var bucket = GetBucket(capacity);
                        T[] result;
                        if (!bucket.TryDequeue(out result))
                        {
                            result = new T[capacity];
                        }
                        return result;
                    }
                }
                else
                {
                    return new T[capacity];
                }
            }
            else
            {
                return new T[capacity];
            }
        }

        private static FixedSizeQueueBucket<T[]> GetBucket(int capacity)
        {
            return _data.Get (NumericHelper.Log2(capacity) - NumericHelper.Log2(INT_MinCapacity));
        }
    }
}
