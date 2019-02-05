#if FAT
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static T TakeAndReturn<T>(this IProducerConsumerCollection<T> producerConsumerCollection)
        {
            if (producerConsumerCollection == null)
            {
                throw new ArgumentNullException(nameof(producerConsumerCollection));
            }
            if (producerConsumerCollection.TryTake(out var item))
            {
                return item;
            }
            throw new InvalidOperationException();
        }

        public static bool TryTakeAndIgnore<T>(this IProducerConsumerCollection<T> producerConsumerCollection)
        {
            if (producerConsumerCollection == null)
            {
                throw new ArgumentNullException(nameof(producerConsumerCollection));
            }
            return producerConsumerCollection.TryTake(out _);
        }

        public static bool TryTakeUntil<T>(this IProducerConsumerCollection<T> producerConsumerCollection, Predicate<T> check, out T item)
        {
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }
            if (producerConsumerCollection == null)
            {
                throw new ArgumentNullException(nameof(producerConsumerCollection));
            }

            while (true)
            {
                if (!producerConsumerCollection.TryTake(out item))
                {
                    return false;
                }
                if (check(item))
                {
                    return true;
                }
            }

        }

        public static bool TryTakeUntil<T>(this IProducerConsumerCollection<T> producerConsumerCollection, Predicate<T> check, ICollection<T> trail)
        {
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }
            if (producerConsumerCollection == null)
            {
                throw new ArgumentNullException(nameof(producerConsumerCollection));
            }
            if (trail == null)
            {
                throw new ArgumentNullException(nameof(trail));
            }

            while (true)
            {
                if (!producerConsumerCollection.TryTake(out var item))
                {
                    return false;
                }

                if (check(item))
                {
                    return true;
                }

                trail.Add(item);
            }
        }
    }
}

#endif