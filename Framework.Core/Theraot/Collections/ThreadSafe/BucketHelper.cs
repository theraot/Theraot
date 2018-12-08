// Needed for NET40

using System;
using System.Linq;
using System.Collections.Generic;

namespace Theraot.Collections.ThreadSafe
{
    public static class BucketHelper
    {
        static BucketHelper()
        {
            Null = new object();
        }

        internal static object Null { get; }

        public static T GetOrInsert<T>(this IBucket<T> bucket, int index, T item)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            if (bucket.Insert(index, item, out var previous))
            {
                return item;
            }
            return previous;
        }

        public static T GetOrInsert<T>(this IBucket<T> bucket, int index, Func<T> itemFactory)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            if (!bucket.TryGet(index, out var stored))
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
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory)
        {
            InsertOrUpdate(bucket, index, item, itemUpdateFactory, out _);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            isNew = true;
            while (true)
            {
                if (isNew)
                {
                    if (bucket.Insert(index, item, out _))
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
        /// <param name="itemFactory">The item factory to create the item to insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            InsertOrUpdate(bucket, index, itemFactory, itemUpdateFactory, out _);
        }

        /// <summary>
        /// Inserts or replaces the item at the specified index.
        /// </summary>
        /// <param name="bucket">The bucket on which to operate.</param>
        /// <param name="index">The index.</param>
        /// <param name="itemFactory">The item factory to create the item to insert.</param>
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="isNew">if set to <c>true</c> the index was not previously used.</param>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static void InsertOrUpdate<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            isNew = true;
            var factoryUsed = false;
            var created = default(T);
            while (true)
            {
                if (isNew)
                {
                    if (!factoryUsed)
                    {
                        created = itemFactory.Invoke();
                        factoryUsed = true;
                    }
                    if (bucket.Insert(index, created, out _))
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
        /// <param name="itemUpdateFactory">The item factory to create the item to replace with.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <returns>
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check)
        {
            return InsertOrUpdateChecked(bucket, index, item, itemUpdateFactory, check, out _);
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
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, T item, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            isNew = true;
            while (true)
            {
                if (isNew)
                {
                    if (bucket.Insert(index, item, out _))
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
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <returns>
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
        {
            return InsertOrUpdateChecked(bucket, index, item, check, out _);
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
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            isNew = true;
            while (true)
            {
                if (isNew)
                {
                    if (bucket.Insert(index, item, out _))
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
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            return InsertOrUpdateChecked(bucket, index, itemFactory, itemUpdateFactory, check, out _);
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
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Func<T, T> itemUpdateFactory, Predicate<T> check, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            isNew = true;
            var factoryUsed = false;
            var created = default(T);
            while (true)
            {
                if (isNew)
                {
                    if (!factoryUsed)
                    {
                        created = itemFactory.Invoke();
                        factoryUsed = true;
                    }
                    if (bucket.Insert(index, created, out _))
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
        /// <param name="itemFactory">The item factory to create the item to set.</param>
        /// <param name="check">A predicate to decide if a particular item should be replaced.</param>
        /// <returns>
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Predicate<T> check)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            return InsertOrUpdateChecked(bucket, index, itemFactory, check, out _);
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
        ///   <c>true</c> if the item or replaced inserted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">index;index must be greater or equal to 0 and less than capacity</exception>
        /// <remarks>
        /// The operation will be attempted as long as check returns true - this operation may starve.
        /// </remarks>
        public static bool InsertOrUpdateChecked<T>(this IBucket<T> bucket, int index, Func<T> itemFactory, Predicate<T> check, out bool isNew)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            isNew = true;
            var factoryUsed = false;
            var created = default(T);
            while (true)
            {
                if (isNew)
                {
                    if (!factoryUsed)
                    {
                        created = itemFactory.Invoke();
                        factoryUsed = true;
                    }
                    if (bucket.Insert(index, created, out _))
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

        public static int RemoveWhere<T>(this IBucket<T> bucket, Predicate<T> check)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }
            var matches = bucket.WhereIndexed(value => check(value));
            var count = 0;
            foreach (var pair in matches)
            {
                if (bucket.RemoveAt(pair.Key))
                {
                    count++;
                }
            }
            return count;
        }

        public static IEnumerable<T> RemoveWhereEnumerable<T>(this IBucket<T> bucket, Predicate<T> check)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }
            var matches = bucket.WhereIndexed(value => check(value));
            return from pair in matches where bucket.RemoveAt(pair.Key) select pair.Value;
        }

        public static void Set<T>(this IBucket<T> bucket, int index, T value)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            bucket.Set(index, value, out _);
        }

        public static bool TryGetOrInsert<T>(this IBucket<T> bucket, int index, T item, out T stored)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            if (bucket.Insert(index, item, out var previous))
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
                throw new ArgumentNullException(nameof(bucket));
            }
            if (bucket.TryGet(index, out stored))
            {
                return false;
            }
            var created = itemFactory.Invoke();
            if (bucket.Insert(index, created, out stored))
            {
                stored = created;
                return true;
            }
            return false;
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, Func<T, T> itemUpdateFactory)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            return bucket.Update(index, itemUpdateFactory, Tautology, out _);
        }

        public static bool Update<T>(this IBucket<T> bucket, int index, Func<T, T> itemUpdateFactory, out bool isEmpty)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            return bucket.Update(index, itemUpdateFactory, Tautology, out isEmpty);
        }

        public static bool UpdateChecked<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            return bucket.Update(index, _ => item, check, out _);
        }

        public static bool UpdateChecked<T>(this IBucket<T> bucket, int index, T item, Predicate<T> check, out bool isEmpty)
        {
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            return bucket.Update(index, _ => item, check, out isEmpty);
        }

        private static bool Tautology<T>(T item)
        {
            GC.KeepAlive(item);
            return true;
        }
    }
}