#if LESSTHAN_NET35

#pragma warning disable CC0031 // Check for null before calling a delegate

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return Select(source, (item, _) => selector(item));
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return SelectExtracted(source, selector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                foreach (var key in source)
                {
                    foreach (var item in selector(key))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                var count = 0;
                foreach (var key in source)
                {
                    foreach (var item in selector(key, count))
                    {
                        yield return item;
                    }

                    count++;
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (collectionSelector == null)
            {
                throw new ArgumentNullException(nameof(collectionSelector));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                foreach (var element in source)
                {
                    foreach (var collection in collectionSelector(element))
                    {
                        yield return resultSelector(element, collection);
                    }
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (collectionSelector == null)
            {
                throw new ArgumentNullException(nameof(collectionSelector));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            return SelectManyExtracted();

            IEnumerable<TResult> SelectManyExtracted()
            {
                var count = 0;
                foreach (var element in source)
                {
                    foreach (var collection in collectionSelector(element, count))
                    {
                        yield return resultSelector(element, collection);
                    }

                    count++;
                }
            }
        }

        private static IEnumerable<TResult> SelectExtracted<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            // NOTICE this method has no null check
            var count = 0;
            foreach (var item in source)
            {
                yield return selector(item, count);

                count++;
            }
        }
    }
}

#endif