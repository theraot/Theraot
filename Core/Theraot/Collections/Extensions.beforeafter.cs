#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static IEnumerable<T> After<T>(this IEnumerable<T> target, Action action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterCounted<T>(this IEnumerable<T> target, Action<int> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterCountedExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterEach<T>(this IEnumerable<T> target, Action action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterEachExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterEach<T>(this IEnumerable<T> target, Action<T> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterEachExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterEachCounted<T>(this IEnumerable<T> target, Action<int, T> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterEachCountedExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterEachCounted<T>(this IEnumerable<T> target, Action<int> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterEachCountedExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterAny<T>(this IEnumerable<T> target, Action action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterLastExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterAny<T>(this IEnumerable<T> target, Action<T> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterLastExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterLastCounted<T>(this IEnumerable<T> target, Action<int, T> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterLastCountedExtracted(target, action);
            }
        }

        public static IEnumerable<T> AfterLastCounted<T>(this IEnumerable<T> target, Action<int> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return AfterLastCountedExtracted(target, action);
            }
        }

        public static IEnumerable<T> Before<T>(this IEnumerable<T> target, Action action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return BeforeExtracted(target, action);
            }
        }

        public static IEnumerable<T> BeforeEach<T>(this IEnumerable<T> target, Action action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return BeforeEachExtracted(target, action);
            }
        }

        public static IEnumerable<T> BeforeEach<T>(this IEnumerable<T> target, Action<T> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return BeforeEachExtracted(target, action);
            }
        }

        public static IEnumerable<T> BeforeEachCounted<T>(this IEnumerable<T> target, Action<int, T> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return BeforeEachCountedExtracted(target, action);
            }
        }

        public static IEnumerable<T> BeforeEachCounted<T>(this IEnumerable<T> target, Action<int> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return BeforeEachCountedExtracted(target, action);
            }
        }

        public static IEnumerable<T> BeforeAny<T>(this IEnumerable<T> target, Action action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return BeforeFirstExtracted(target, action);
            }
        }

        public static IEnumerable<T> BeforeAny<T>(this IEnumerable<T> target, Action<T> action)
        {
            Check.NotNullArgument(target, "target");
            if (action == null)
            {
                return target;
            }
            else
            {
                return BeforeFirstExtracted(target, action);
            }
        }

        private static IEnumerable<T> AfterCountedExtracted<T>(IEnumerable<T> target, Action<int> action)
        {
            int count = 0;
            foreach (var item in target)
            {
                yield return item;
                count++;
            }
            action.Invoke(count);
        }

        private static IEnumerable<T> AfterEachCountedExtracted<T>(IEnumerable<T> target, Action<int> action)
        {
            int count = 0;
            foreach (var item in target)
            {
                yield return item;
                action.Invoke(count);
                count++;
            }
        }

        private static IEnumerable<T> AfterEachCountedExtracted<T>(IEnumerable<T> target, Action<int, T> action)
        {
            int count = 0;
            foreach (var item in target)
            {
                yield return item;
                action.Invoke(count, item);
                count++;
            }
        }

        private static IEnumerable<T> AfterEachExtracted<T>(IEnumerable<T> target, Action action)
        {
            foreach (var item in target)
            {
                yield return item;
                action.Invoke();
            }
        }

        private static IEnumerable<T> AfterEachExtracted<T>(IEnumerable<T> target, Action<T> action)
        {
            foreach (var item in target)
            {
                yield return item;
                action.Invoke(item);
            }
        }

        private static IEnumerable<T> AfterExtracted<T>(IEnumerable<T> target, Action action)
        {
            foreach (var item in target)
            {
                yield return item;
            }
            action.Invoke();
        }

        private static IEnumerable<T> AfterLastCountedExtracted<T>(IEnumerable<T> target, Action<int, T> action)
        {
            int count = 1;
            var enumerator = target.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    var found = enumerator.Current;
                    yield return found;
                    while (enumerator.MoveNext())
                    {
                        found = enumerator.Current;
                        yield return enumerator.Current;
                        count++;
                    }
                    action.Invoke(count, found);
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static IEnumerable<T> AfterLastCountedExtracted<T>(IEnumerable<T> target, Action<int> action)
        {
            int count = 1;
            var enumerator = target.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                        count++;
                    }
                    action.Invoke(count);
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static IEnumerable<T> AfterLastExtracted<T>(IEnumerable<T> target, Action<T> action)
        {
            var enumerator = target.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    var found = enumerator.Current;
                    yield return found;
                    while (enumerator.MoveNext())
                    {
                        found = enumerator.Current;
                        yield return enumerator.Current;
                    }
                    action.Invoke(found);
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static IEnumerable<T> AfterLastExtracted<T>(IEnumerable<T> target, Action action)
        {
            var enumerator = target.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                    action.Invoke();
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static IEnumerable<T> BeforeEachCountedExtracted<T>(IEnumerable<T> target, Action<int> action)
        {
            int count = 0;
            foreach (var item in target)
            {
                action.Invoke(count);
                yield return item;
                count++;
            }
        }

        private static IEnumerable<T> BeforeEachCountedExtracted<T>(IEnumerable<T> target, Action<int, T> action)
        {
            int count = 0;
            foreach (var item in target)
            {
                action.Invoke(count, item);
                yield return item;
                count++;
            }
        }

        private static IEnumerable<T> BeforeEachExtracted<T>(IEnumerable<T> target, Action action)
        {
            foreach (var item in target)
            {
                action.Invoke();
                yield return item;
            }
        }

        private static IEnumerable<T> BeforeEachExtracted<T>(IEnumerable<T> target, Action<T> action)
        {
            foreach (var item in target)
            {
                action.Invoke(item);
                yield return item;
            }
        }

        private static IEnumerable<T> BeforeExtracted<T>(IEnumerable<T> target, Action action)
        {
            action.Invoke();
            foreach (var item in target)
            {
                yield return item;
            }
        }

        private static IEnumerable<T> BeforeFirstExtracted<T>(IEnumerable<T> target, Action<T> action)
        {
            var enumerator = target.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    action.Invoke(current);
                    yield return current;
                }
                else
                {
                    yield break;
                }
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static IEnumerable<T> BeforeFirstExtracted<T>(IEnumerable<T> target, Action action)
        {
            var enumerator = target.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    action.Invoke();
                    yield return enumerator.Current;
                }
                else
                {
                    yield break;
                }
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }
}

#endif