// Needed for NET40

using System;

namespace Theraot.Collections.ThreadSafe
{
    public static class BucketHelper
    {
        private static readonly object _null;

        static BucketHelper()
        {
            _null = new object();
        }

        internal static object Null
        {
            get
            {
                return _null;
            }
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="itemUpdateFactory"></param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check)
        {
            T stored;
            bool isNew;
            return InsertOrUpdate(bucket, index, item, itemUpdateFactory, check, out stored, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="itemUpdateFactory"></param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="stored">the item that was left at the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check, out T stored)
        {
            bool isNew;
            return InsertOrUpdate(bucket, index, item, itemUpdateFactory, check, out stored, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="itemUpdateFactory"></param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
        {
            T stored;
            return InsertOrUpdate(bucket, index, item, itemUpdateFactory, check, out stored, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <param name="itemUpdateFactory"></param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="stored">the item that was left at the specified index.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check, out T stored, out bool isNew)
        {
            if (index < 0 || index >= bucket.Capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            stored = default(T);
            isNew = true;
            while (true)
            {
                if (isNew)
                {
                    if (bucket.Insert(index, item, out stored))
                    {
                        return true;
                    }
                    isNew = false;
                }
                else
                {
                    if (check(stored))
                    {
                        var result = itemUpdateFactory.Invoke(stored);
                        if (bucket.Update(index, result, stored, out stored, out isNew))
                        {
                            stored = result;
                            return true;
                        }
                    }
                    else
                    {
                        return false; // returns false only when check returns false
                    }
                }
            }
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check)
        {
            T stored;
            bool isNew;
            return InsertOrUpdate(bucket, index, itemFactory, itemUpdateFactory, check, out stored, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="stored">the item that was left at the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check, out T stored)
        {
            bool isNew;
            return InsertOrUpdate(bucket, index, itemFactory, itemUpdateFactory, check, out stored, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
        {
            T stored;
            return InsertOrUpdate(bucket, index, itemFactory, itemUpdateFactory, check, out stored, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="stored">the item that was left at the specified index.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check, out T stored, out bool isNew)
        {
            if (index < 0 || index >= bucket.Capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity");
            }
            stored = default(T);
            isNew = true;
            bool factoryUsed = false;
            var created = default(T);
            while (true)
            {
                if (isNew)
                {
                    if (factoryUsed || !bucket.TryGet(index, out stored))
                    {
                        if (!factoryUsed)
                        {
                            created = itemFactory.Invoke();
                            factoryUsed = true;
                        }
                        if (bucket.Insert(index, created, out stored))
                        {
                            return true;
                        }
                    }
                    isNew = false;
                }
                else
                {
                    if (check(stored))
                    {
                        var result = itemUpdateFactory.Invoke(stored);
                        if (bucket.Update(index, result, stored, out stored, out isNew))
                        {
                            stored = result;
                            return true;
                        }
                    }
                    else
                    {
                        return false; // returns false only when check returns false
                    }
                }
            }
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="check">verification for the old item.</param>
        /// <returns>
        ///   <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the item was overwritten or removed.
        /// </remarks>
        public static bool RemoveValueAt<T>(this IBucket<T> bucket, int index, Predicate<T> check)
        {
            T previous;
            return RemoveValueAt(bucket, index, check, out previous);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="check">verification for the old item.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <returns>
        ///   <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the item was overwritten or removed.
        /// </remarks>
        public static bool RemoveValueAt<T>(this IBucket<T> bucket, int index, Predicate<T> check, out T previous)
        {
            if (index < 0 || index >= bucket.Capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity.");
            }
            if (!bucket.TryGet(index, out previous))
            {
                // There was not an item
                return false;
            }
            // There was an item
            while (true)
            {
                if (!check(previous))
                {
                    // The item did not pass the check
                    return false;
                }
                // The item passes the check
                if (bucket.RemoveValueAt(index, previous, out previous))
                {
                    // The item was replaced
                    return true;
                }
            }
        }

        public static bool RemoveValueAt<T>(this IBucket<T> bucket, int index, T value)
        {
            T previous;
            return bucket.RemoveValueAt(index, value, out previous);
        }

        public static void Set<T>(this IBucket<T> bucket, int index, T value)
        {
            bool isNew;
            bucket.Set(index, value, out isNew);
        }

        public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, T item, out T stored)
        {
            T previous;
            if (bucket.Insert(index, item, out previous))
            {
                stored = item;
                return true;
            }
            stored = previous;
            return false;
        }

        public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, out T stored)
        {
            if (!bucket.TryGet(index, out stored))
            {
                var created = itemFactory.Invoke();
                if (bucket.Insert(index, created, out stored))
                {
                    stored = created;
                    return true;
                }
            }
            return false;
        }

        public static bool TryUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
        {
            T previous;
            bool isNew;
            return TryUpdate(bucket, index, item, check, out previous, out isNew);
        }

        public static bool TryUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out T previous)
        {
            bool isNew;
            return TryUpdate(bucket, index, item, check, out previous, out isNew);
        }

        public static bool TryUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isNew)
        {
            T previous;
            return TryUpdate(bucket, index, item, check, out previous, out isNew);
        }

        public static bool TryUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out T previous, out bool isNew)
        {
            if (index < 0 || index >= bucket.Capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity.");
            }
            isNew = false;
            if (!bucket.TryGet(index, out previous))
            {
                // There was not an item
                return false;
            }
            // There was an item
            if (!check(previous))
            {
                // The item did not pass the check
                return false;
            }
            // The item passes the check
            return bucket.Update(index, item, previous, out previous, out isNew);
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, T item, T comparisonItem)
        {
            T previous;
            bool isNew;
            return bucket.Update(index, item, comparisonItem, out previous, out isNew);
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, T item, T comparisonItem, out T previous)
        {
            bool isNew;
            return bucket.Update(index, item, comparisonItem, out previous, out isNew);
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, T item, T comparisonItem, out bool isNew)
        {
            T previous;
            return bucket.Update(index, item, comparisonItem, out previous, out isNew);
        }

        /// <summary>
        /// Replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The new item.</param>
        /// <param name="check">verification for the old item.</param>
        /// <param name="previous">The previous item in the specified index.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item was inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity.</exception>
        /// <remarks>
        /// The insertion can fail if the index is already used or is being written by another thread.
        /// If the index is being written it can be understood that the insert operation happened before but the item was overwritten or removed.
        /// </remarks>
        public static bool Update<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out T previous, out bool isNew)
        {
            if (index < 0 || index >= bucket.Capacity)
            {
                throw new ArgumentOutOfRangeException("index", "index must be greater or equal to 0 and less than capacity.");
            }
            isNew = false;
            if (!bucket.TryGet(index, out previous))
            {
                // There was not an item
                return false;
            }
            // There was an item
            while (true)
            {
                if (!check(previous))
                {
                    // The item did not pass the check
                    return false;
                }
                // The item passes the check
                if (bucket.Update(index, item, previous, out previous, out isNew))
                {
                    // The item was replaced
                    return true;
                }
                if (isNew)
                {
                    // There is no longer an item
                    return false;
                }
            }
        }
    }
}