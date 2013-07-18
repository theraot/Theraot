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
        private const int INT_MinCapacity = 16;
        private const int INT_MaxCapacity = 1024;
        private const int INT_WorkCapacityHint = 16;

        private static LazyNeedle<LazyBucket<QueueBucket<T[]>>> _data;
        private static Work.Context _recycle;

        static ArrayPool()
        {
            _recycle = new Work.Context("Recycler", INT_WorkCapacityHint, 1);
            _data = new LazyNeedle<LazyBucket<QueueBucket<T[]>>>
            (
                () =>
                {
                    return new LazyBucket<QueueBucket<T[]>>
                    (
                        input =>
                        {
                            return new QueueBucket<T[]>();
                        },
                        NumericHelper.Log2(INT_MaxCapacity) - NumericHelper.Log2(INT_MinCapacity)
                    );
                }
            );
            GC.KeepAlive(_data.Value);
        }

        private static QueueBucket<T[]> GetBucket(int capacity)
        {
            LazyBucket<QueueBucket<T[]>> data = _data.Value;
            return data.Get (NumericHelper.Log2(capacity) - NumericHelper.Log2(INT_MinCapacity));
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
                    capacity = NumericHelper.NextPowerOf2(capacity);
                }
            }
            if (_data.IsCached)
            {
                QueueBucket<T[]> bucket = GetBucket(capacity);
                T[] result;
                if (!bucket.TryDequeue(out result))
                {
                    result = new T[capacity];
                }
                return result;
            }
            else
            {
                return new T[capacity];
            }
        }

        public static bool DonateArray(T[] array)
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
                            for (int index = 0; index < capacity; index++)
                            {
                                array[index] = default(T);
                            }
                            QueueBucket<T[]> bucket = GetBucket(capacity);
                            bucket.Enqueue(array);
                        }
                    );
                    return true;
                }
            }
            else
            {
                throw new ArgumentException("The size of the array must be a power of two.", "array");
            }
        }
    }
}
