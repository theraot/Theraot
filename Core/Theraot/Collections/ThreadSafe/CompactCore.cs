using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    internal class CompactCore<T> : ICore<T>
    {
        private readonly FixedSizeBucket<T>[] _bucketsFirst;

        private readonly FixedSizeBucket<T>[] _bucketsSecond;

        private readonly int[] _bucketsUse;

        private int _bucketsCount;

        private int _length;

        public CompactCore(int capacity)
        {
            _bucketsFirst = new FixedSizeBucket<T>[527];
            _bucketsSecond = new FixedSizeBucket<T>[527];
            _bucketsUse = new int[527];
            while (_length < capacity)
            {
                GrowPrivate(_bucketsCount);
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public void Do(int index, bool grow, DoAction<T> callback)
        {
            var position = 0;
            var bucketIndex = 0;
            var bucketCount = Interlocked.CompareExchange(ref _bucketsCount, 0, 0);
            while (true)
            {
                if (bucketIndex == bucketCount)
                {
                    return;
                }
                var capacity = CoreHelper.GetCapacity(bucketIndex);
                var nextPosition = capacity + position;
                if (nextPosition > index)
                {
                    callback(capacity, position, ref _bucketsUse[bucketIndex], ref _bucketsFirst[bucketIndex], ref _bucketsSecond[bucketIndex]);
                    return;
                }
                position = nextPosition;
                bucketIndex++;
                if (bucketIndex == bucketCount)
                {
                    if (grow)
                    {
                        bucketCount = GrowPrivate(bucketCount);
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var bucketIndex = 0;
            while (true)
            {
                if (bucketIndex == _bucketsCount)
                {
                    break;
                }
                try
                {
                    var bucket = CoreHelper.EnterRead(ref _bucketsUse[bucketIndex], ref _bucketsFirst[bucketIndex]);
                    if (bucket != null)
                    {
                        foreach (var value in bucket)
                        {
                            yield return value;
                        }
                    }
                }
                finally
                {
                    CoreHelper.Leave(ref _bucketsUse[bucketIndex], ref _bucketsFirst[bucketIndex], ref _bucketsSecond[bucketIndex]);
                }
                bucketIndex++;
            }
        }

        public bool Grow(int capacity)
        {
            if (capacity < 0)
            {
                return false;
            }
            while (_length < capacity)
            {
                GrowPrivate(Thread.VolatileRead(ref _bucketsCount));
            }
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GrowPrivate(int expectedContainerCount)
        {
            // Try to increase _bucketsCount if it is equal to expectedContainerCount
            var currentContainerCount = Interlocked.CompareExchange(ref _bucketsCount, expectedContainerCount + 1, expectedContainerCount);
            if (currentContainerCount != expectedContainerCount)
            {
                while (true)
                {
                    // Did not increase _bucketsCount
                    if (currentContainerCount > expectedContainerCount)
                    {
                        // The currentContainerCount is greater than expectedContainerCount
                        // Just return the currentContainerCount
                        return currentContainerCount;
                    }
                    // The currentContainerCount is lesser than expectedContainerCount
                    // Wait _bucketsCount to change
                    ThreadingHelper.SpinWaitWhile(ref _bucketsCount, currentContainerCount);
                    // Get the currentContainerCount
                    currentContainerCount = Interlocked.CompareExchange(ref _bucketsCount, 0, 0);
                    if (currentContainerCount == expectedContainerCount)
                    {
                        // currentContainerCount is equal to expectedContainerCount
                        break;
                    }
                }
            }
            // currentContainerCount is equal to expectedContainerCount
            var nextCapacity = CoreHelper.GetCapacity(expectedContainerCount);
            try
            {
                bool isNew;
                CoreHelper.EnterMayIncrement(ref _bucketsUse[expectedContainerCount], ref _bucketsFirst[expectedContainerCount], ref _bucketsSecond[expectedContainerCount], () => new FixedSizeBucket<T>(nextCapacity), out isNew);
                if (isNew)
                {
                    // Added a new bucket
                    Interlocked.Add(ref _length, nextCapacity);
                }
            }
            finally
            {
                CoreHelper.Leave(ref _bucketsUse[expectedContainerCount], ref _bucketsFirst[expectedContainerCount], ref _bucketsSecond[expectedContainerCount]);
            }
            // Either added a new bucket or another thread did it first
            return expectedContainerCount + 1;
        }
    }
}