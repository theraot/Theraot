using System;
using System.Collections.Generic;

using Theraot.Collections.Specialized;
using Theraot.Core;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, IEnumerable<T> append)
        {
            return new ExtendedEnumerable<T>(target, append);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, T append)
        {
            return new ExtendedEnumerable<T>(target, AsUnaryEnumerable(append));
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, IEnumerable<T> append, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(target, append, match);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> target, T append, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(target, AsUnaryEnumerable(append), match);
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
            var _result = collection as ISet<TItem>;
            if (_result == null)
            {
                return new ProgressiveSet<TItem>(collection);
            }
            else
            {
                return _result;
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
            return Check.NotNullArgument(source, "source").Where(item => !_predicate.Invoke(item));
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> target, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(target, null, match);
        }

        public static bool HasAtLeast<TSource>(this IEnumerable<TSource> source, int count)
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

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, IEnumerable<T> prepend)
        {
            return new ExtendedEnumerable<T>(prepend, target);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, T prepend)
        {
            return new ExtendedEnumerable<T>(AsUnaryEnumerable(prepend), target);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, IEnumerable<T> prepend, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(prepend, target, match);
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> target, T prepend, Predicate<T> match)
        {
            return new ExtendedFilteredEnumerable<T>(AsUnaryEnumerable(prepend), target, match);
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

        private static ICollection<TItem> AsCollection<TItem>(IEnumerable<TItem> collection)
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

        private static IEnumerable<T> SkipItemsExtracted<T>(IEnumerable<T> target, int skipCount)
        {
            int count = 0;
            foreach (var item in target)
            {
                if (count < skipCount)
                {
                    count++;
                    continue;
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
                    continue;
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
                    continue;
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
                    continue;
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