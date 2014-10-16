using System;
using System.Collections.Generic;
using Theraot.Core;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static ICollection<TItem> AsCollection<TItem>(IEnumerable<TItem> collection)
        {
            var _result = collection as ICollection<TItem>;
            if (_result == null)
            {
                return new ProgressiveCollection<TItem>(collection);
            }
            else
            {
                return _result;
            }
        }

        public static ICollection<TItem> AsDistinctCollection<TItem>(IEnumerable<TItem> collection)
        {
            // Workaround for .NET 3.5 when all you want is Contains and no duplicates
#if NET35
            var _resultHashSet = collection as HashSet<TItem>;
            if (_resultHashSet == null)
            {
                var _resultISet = collection as ISet<TItem>;
                if (_resultISet == null)
                {
                    return new ProgressiveSet<TItem>(collection);
                }
                else
                {
                    return _resultISet;
                }
            }
            else
            {
                return _resultHashSet;
            }
#else
            return AsSet(collection);
#endif
        }

        public static IList<TItem> AsList<TItem>(IEnumerable<TItem> collection)
        {
            var _result = collection as IList<TItem>;
            if (_result == null)
            {
                return new ProgressiveList<TItem>(collection);
            }
            else
            {
                return _result;
            }
        }

        public static ISet<TItem> AsSet<TItem>(IEnumerable<TItem> collection)
        {
            // Remember that On .NET 3.5 HashSet is not an ISet
            var _resultISet = collection as ISet<TItem>;
            if (_resultISet == null)
            {
                return new ProgressiveSet<TItem>(collection);
            }
            else
            {
                return _resultISet;
            }
        }

        public static IEnumerable<T> AsUnaryEnumerable<T>(this T target)
        {
            yield return target;
        }

        public static IList<T> AsUnaryList<T>(this T target)
        {
            return new ProgressiveList<T>
                   (
                       AsUnaryEnumerable(target)
                   );
        }

        public static ISet<T> AsUnarySet<T>(this T target)
        {
            return new ProgressiveSet<T>
                   (
                       AsUnaryEnumerable(target)
                   );
        }

        public static IEnumerable<T> ExceptWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            return Where(Check.NotNullArgument(source, "source"), item => !_predicate.Invoke(item));
        }

        public static bool HasAtLeast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            else
            {
                var collection = source as ICollection<TSource>;
                if (collection == null)
                {
                    int result = 0;
                    using (var item = source.GetEnumerator())
                    {
                        while (item.MoveNext())
                        {
                            checked
                            {
                                result++;
                            }
                            if (result == count)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    return collection.Count > count;
                }
            }
        }

        public static IEnumerable<T> SkipItems<T>(this IEnumerable<T> target, int skipCount)
        {
            return SkipItemsExtracted(Check.NotNullArgument(target, "target"), skipCount);
        }

        public static IEnumerable<T> SkipItems<T>(this IEnumerable<T> target, Predicate<T> predicateCount, int skipCount)
        {
            var _target = Check.NotNullArgument(target, "target");
            if (predicateCount == null)
            {
                return SkipItemsExtracted(_target, skipCount);
            }
            else
            {
                return SkipItemsExtracted(_target, predicateCount, skipCount);
            }
        }

        public static IEnumerable<T> StepItems<T>(this IEnumerable<T> target, int stepCount)
        {
            return StepItemsExtracted(Check.NotNullArgument(target, "target"), stepCount);
        }

        public static IEnumerable<T> StepItems<T>(this IEnumerable<T> target, Predicate<T> predicateCount, int stepCount)
        {
            var _target = Check.NotNullArgument(target, "target");
            if (predicateCount == null)
            {
                return StepItemsExtracted(_target, stepCount);
            }
            else
            {
                return StepItemsExtracted(_target, predicateCount, stepCount);
            }
        }

        public static IEnumerable<T> TakeItems<T>(this IEnumerable<T> target, int takeCount)
        {
            return TakeItemsExtracted(Check.NotNullArgument(target, "target"), takeCount);
        }

        public static IEnumerable<T> TakeItems<T>(this IEnumerable<T> target, Predicate<T> predicateCount, int takeCount)
        {
            var _target = Check.NotNullArgument(target, "target");
            if (predicateCount == null)
            {
                return TakeItemsExtracted(_target, takeCount);
            }
            else
            {
                return TakeItemsExtracted(_target, predicateCount, takeCount);
            }
        }

        private static IEnumerable<T> SkipItemsExtracted<T>(IEnumerable<T> target, int skipCount)
        {
            int count = 0;
            foreach (var item in target)
            {
                if (count < skipCount)
                {
                    count++;
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<T> SkipItemsExtracted<T>(IEnumerable<T> target, Predicate<T> predicateCount, int skipCount)
        {
            int count = 0;
            foreach (var item in target)
            {
                if (count < skipCount)
                {
                    if (predicateCount(item))
                    {
                        count++;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<T> StepItemsExtracted<T>(this IEnumerable<T> target, int stepCount)
        {
            int count = 0;
            foreach (var item in target)
            {
                if (count % stepCount == 0)
                {
                    count++;
                }
                else
                {
                    yield return item;
                    count++;
                }
            }
        }

        private static IEnumerable<T> StepItemsExtracted<T>(this IEnumerable<T> target, Predicate<T> predicateCount, int stepCount)
        {
            int count = 0;
            foreach (var item in target)
            {
                if (count % stepCount == 0)
                {
                    if (predicateCount(item))
                    {
                        count++;
                    }
                }
                else
                {
                    yield return item;
                    count++;
                }
            }
        }

        private static IEnumerable<T> TakeItemsExtracted<T>(IEnumerable<T> target, int takeCount)
        {
            int count = 0;
            foreach (var item in target)
            {
                if (count == takeCount)
                {
                    break;
                }
                else
                {
                    yield return item;
                    count++;
                }
            }
        }

        private static IEnumerable<T> TakeItemsExtracted<T>(IEnumerable<T> target, Predicate<T> predicateCount, int takeCount)
        {
            int count = 0;
            foreach (var item in target)
            {
                if (count == takeCount)
                {
                    break;
                }
                else
                {
                    yield return item;
                    if (predicateCount(item))
                    {
                        count++;
                    }
                }
            }
        }
    }
}