using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    /// <summary>
    /// Represent a thread-safe wait-free bucket.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public sealed class Bucket<T> : IBucket<T>
    {
        private readonly BucketCore _bucketCore;
        private int _count;

        public Bucket()
        {
            _bucketCore = new BucketCore(7);
        }

        public Bucket(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            _bucketCore = new BucketCore(7);
            var index = 0;
            foreach (var item in source)
            {
                var copy = item;
                _bucketCore.DoMayIncrement
                    (
                        index,
                        (ref object target) => Interlocked.Exchange(ref target, (object)copy ?? BucketHelper.Null) == null
                    );
                index++;
                _count++;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public IEnumerable<T> EnumerateRange(int indexFrom, int indexTo)
        {
            foreach (var value in _bucketCore.EnumerateRange(indexFrom, indexTo))
            {
                yield return value == BucketHelper.Null ? default(T) : (T)value;
            }
        }

        public bool Exchange(int index, T item, out T previous)
        {
            var found = BucketHelper.Null;
            previous = default(T);
            var result = _bucketCore.DoMayIncrement
                (
                    index,
                    (ref object target) =>
                    {
                        found = Interlocked.Exchange(ref target, (object)item ?? BucketHelper.Null);
                        return found == null;
                    }
                );
            if (result)
            {
                Interlocked.Increment(ref _count);
                return true;
            }
            if (found != BucketHelper.Null)
            {
                previous = (T)found;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var value in _bucketCore)
            {
                yield return value == BucketHelper.Null ? default(T) : (T)value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Insert(int index, T item)
        {
            var result = _bucketCore.DoMayIncrement
                (
                    index,
                    (ref object target) =>
                    {
                        var found = Interlocked.CompareExchange(ref target, (object)item ?? BucketHelper.Null, null);
                        return found == null;
                    }
                );
            if (result)
            {
                Interlocked.Increment(ref _count);
            }
            return result;
        }

        public bool Insert(int index, T item, out T previous)
        {
            var found = BucketHelper.Null;
            previous = default(T);
            var result = _bucketCore.DoMayIncrement
                (
                    index,
                    (ref object target) =>
                    {
                        found = Interlocked.CompareExchange(ref target, (object)item ?? BucketHelper.Null, null);
                        return found == null;
                    }
                );
            if (result)
            {
                Interlocked.Increment(ref _count);
                return true;
            }
            if (found != BucketHelper.Null)
            {
                previous = (T)found;
            }
            return false;
        }

        public bool RemoveAt(int index)
        {
            var result = _bucketCore.DoMayDecrement
                (
                    index,
                    (ref object target) => Interlocked.Exchange(ref target, null) != null
                );
            if (result)
            {
                Interlocked.Decrement(ref _count);
            }
            return result;
        }

        public bool RemoveAt(int index, out T previous)
        {
            var found = BucketHelper.Null;
            previous = default(T);
            var result = _bucketCore.DoMayDecrement
                (
                    index,
                    (ref object target) =>
                    {
                        found = Interlocked.Exchange(ref target, null);
                        return found != null;
                    }
                );
            if (!result)
            {
                return false;
            }
            Interlocked.Decrement(ref _count);
            if (found != BucketHelper.Null)
            {
                previous = (T)found;
            }
            return true;
        }

        public bool RemoveAt(int index, Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            return _bucketCore.DoMayDecrement
                (
                    index,
                    (ref object target) =>
                    {
                        var found = Interlocked.CompareExchange(ref target, null, null);
                        if (found != null)
                        {
                            var comparisonItem = found == BucketHelper.Null ? default(T) : (T)found;
                            if (check(comparisonItem))
                            {
                                var compare = Interlocked.CompareExchange(ref target, null, found);
                                if (found == compare)
                                {
                                    Interlocked.Decrement(ref _count);
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                );
        }

        public void Set(int index, T item, out bool isNew)
        {
            isNew = _bucketCore.DoMayIncrement
                (
                    index,
                    (ref object target) => Interlocked.Exchange(ref target, (object)item ?? BucketHelper.Null) == null
                );
            if (isNew)
            {
                Interlocked.Increment(ref _count);
            }
        }

        public bool TryGet(int index, out T value)
        {
            var found = BucketHelper.Null;
            value = default(T);
            var done = _bucketCore.Do
                (
                    index,
                    (ref object target) =>
                    {
                        found = Interlocked.CompareExchange(ref target, null, null);
                        return true;
                    }
                );
            if (!done || found == null)
            {
                return false;
            }
            if (found != BucketHelper.Null)
            {
                value = (T)found;
            }
            return true;
        }

        public bool Update(int index, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isEmpty)
        {
            if (itemUpdateFactory == null)
            {
                throw new ArgumentNullException("itemUpdateFactory");
            }
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            var found = BucketHelper.Null;
            var compare = BucketHelper.Null;
            var result = false;
            var done = _bucketCore.Do
                (
                    index,
                    (ref object target) =>
                    {
                        found = Interlocked.CompareExchange(ref target, null, null);
                        if (found != null)
                        {
                            var comparisonItem = found == BucketHelper.Null ? default(T) : (T)found;
                            if (check(comparisonItem))
                            {
                                var item = itemUpdateFactory(comparisonItem);
                                compare = Interlocked.CompareExchange(ref target, (object)item ?? BucketHelper.Null, found);
                                result = found == compare;
                            }
                        }
                        return true;
                    }
                );
            if (!done)
            {
                isEmpty = true;
                return false;
            }
            isEmpty = found == null || compare == null;
            return result;
        }

        public IEnumerable<T> Where(Predicate<T> check)
        {
            if (check == null)
            {
                throw new ArgumentNullException("check");
            }
            return WhereExtracted();

            IEnumerable<T> WhereExtracted()
            {
                foreach (var value in _bucketCore)
                {
                    var castedValue = value == BucketHelper.Null ? default(T) : (T)value;
                    if (check(castedValue))
                    {
                        yield return castedValue;
                    }
                }
            }
        }
    }
}