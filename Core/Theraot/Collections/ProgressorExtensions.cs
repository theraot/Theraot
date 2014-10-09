using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    public static class ProgressorExtensions
    {
        public static IEnumerable<T> All<T>(this IProgressor<T> progressor)
        {
            var _progressor = Check.NotNullArgument(progressor, "progressor");
            T item;
            while (_progressor.TryTake(out item))
            {
                yield return item;
            }
        }

        public static IEnumerable<T> While<T>(this IProgressor<T> progressor, Predicate<T> predicate)
        {
            var _progressor = Check.NotNullArgument(progressor, "progressor");
            var _condition = Check.NotNullArgument(predicate, "condition");
            T item;
            while (_progressor.TryTake(out item))
            {
                if (_condition(item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        public static IEnumerable<T> While<T>(this IProgressor<T> progressor, Func<bool> condition)
        {
            var _progressor = Check.NotNullArgument(progressor, "progressor");
            var _condition = Check.NotNullArgument(condition, "condition");
            T item;
            while (_progressor.TryTake(out item))
            {
                if (_condition())
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }
    }
}