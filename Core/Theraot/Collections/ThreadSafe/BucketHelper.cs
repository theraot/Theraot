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

        public static T GetOrInsert<T>(this IBucket<T> bucket, int index, T item)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            T previous;
            if (bucket.Insert(index, item, out previous))
            {
                return item;
            }
            return previous;
        }

        public static T GetOrInsert<T>(this IBucket<T> bucket, int index, Func<T> itemFactory)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            T stored;
            if (!bucket.TryGet(index, out stored))
            {
                var created = itemFactory.Invoke();
                if (bucket.Insert(index, created, out stored))
                {
                    return created;
                }
            }
            return stored;
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item set.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
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
            bool isNew;
            return InsertOrUpdate(bucket, index, item, itemUpdateFactory, check, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item insert.</param>
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
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            isNew = true;
            while (true)
            {
                if (isNew)
                {
                    T stored;
                    if (bucket.Insert(index, item, out stored))
                    {
                        return true;
                    }
                    isNew = false;
                }
                else
                {
                    if (bucket.Update(index, itemUpdateFactory, check, out isNew))
                    {
                        return true;
                    }
                    if (!isNew)
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
        /// <param name="item">The item set.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory)
        {
            bool isNew;
            InsertOrUpdate(bucket, index, item, itemUpdateFactory, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            isNew = true;
            while (true)
            {
                if (isNew)
                {
                    T stored;
                    if (bucket.Insert(index, item, out stored))
                    {
                        return;
                    }
                    isNew = false;
                }
                else
                {
                    if (bucket.Update(index, itemUpdateFactory, Tautology, out isNew))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item set.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
        {
            bool isNew;
            return InsertOrUpdate(bucket, index, item, check, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item set.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            isNew = true;
            while (true)
            {
                if (isNew)
                {
                    T stored;
                    if (bucket.Insert(index, item, out stored))
                    {
                        return true;
                    }
                    isNew = false;
                }
                else
                {
                    if (bucket.Update(index, _ => item, check, out isNew))
                    {
                        return true;
                    }
                    if (!isNew)
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
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            bool isNew;
            return InsertOrUpdate(bucket, index, itemFactory, itemUpdateFactory, check, out isNew);
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
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            isNew = true;
            bool factoryUsed = false;
            var created = default(T);
            while (true)
            {
                if (isNew)
                {
                    T stored;
                    if (!factoryUsed)
                    {
                        created = itemFactory.Invoke();
                        factoryUsed = true;
                    }
                    if (bucket.Insert(index, created, out stored))
                    {
                        return true;
                    }
                    isNew = false;
                }
                else
                {
                    if (bucket.Update(index, itemUpdateFactory, check, out isNew))
                    {
                        return true;
                    }
                    if (!isNew)
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
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            bool isNew;
            InsertOrUpdate(bucket, index, itemFactory, itemUpdateFactory, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            isNew = true;
            bool factoryUsed = false;
            var created = default(T);
            while (true)
            {
                if (isNew)
                {
                    T stored;
                    if (!factoryUsed)
                    {
                        created = itemFactory.Invoke();
                        factoryUsed = true;
                    }
                    if (bucket.Insert(index, created, out stored))
                    {
                        return;
                    }
                    isNew = false;
                }
                else
                {
                    if (bucket.Update(index, itemUpdateFactory, Tautology, out isNew))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to set.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Predicate<T> check)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            bool isNew;
            return InsertOrUpdate(bucket, index, itemFactory, check, out isNew);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to set.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <returns>
        ///   <c>true</c> if the item or repalced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Predicate<T> check, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            isNew = true;
            bool factoryUsed = false;
            var created = default(T);
            while (true)
            {
                if (isNew)
                {
                    T stored;
                    if (!factoryUsed)
                    {
                        created = itemFactory.Invoke();
                        factoryUsed = true;
                    }
                    if (bucket.Insert(index, created, out stored))
                    {
                        return true;
                    }
                    isNew = false;
                }
                else
                {
                    var result = itemFactory.Invoke();
                    if (bucket.Update(index, _ => result, check, out isNew))
                    {
                        return true;
                    }
                    if (!isNew)
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
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
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
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            T previous;
            return bucket.RemoveValueAt(index, value, out previous);
        }

        public static void Set<T>(this IBucket<T> bucket, int index, T value)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            bool isNew;
            bucket.Set(index, value, out isNew);
        }

        public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, T item, out T stored)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
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
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
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

        public static bool Update<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            bool isEmpty;
            return bucket.Update(index, _ => item, check, out isEmpty);
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isEmpty)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            return bucket.Update(index, _ => item, check, out isEmpty);
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, Func<T, T> itemUpdateFactory)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            bool isEmpty;
            return bucket.Update(index, itemUpdateFactory, Tautology, out isEmpty);
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, Func<T, T> itemUpdateFactory, out bool isEmpty)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException("bucket");
            }
            return bucket.Update(index, itemUpdateFactory, Tautology, out isEmpty);
        }

        private static bool Tautology<T>(T item)
        {
            return true;
        }
    }
}