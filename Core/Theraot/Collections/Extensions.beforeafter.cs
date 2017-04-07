#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static IEnumerable<T> After<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterExtracted(source, action);
        }

        public static IEnumerable<T> AfterCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterCountedExtracted(source, action);
        }

        public static IEnumerable<T> AfterEach<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterEachExtracted(source, action);
        }

        public static IEnumerable<T> AfterEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterEachExtracted(source, action);
        }

        public static IEnumerable<T> AfterEachCounted<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterEachCountedExtracted(source, action);
        }

        public static IEnumerable<T> AfterEachCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterEachCountedExtracted(source, action);
        }

        public static IEnumerable<T> AfterAny<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterLastExtracted(source, action);
        }

        public static IEnumerable<T> AfterAny<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterLastExtracted(source, action);
        }

        public static IEnumerable<T> AfterLastCounted<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterLastCountedExtracted(source, action);
        }

        public static IEnumerable<T> AfterLastCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : AfterLastCountedExtracted(source, action);
        }

        public static IEnumerable<T> Before<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : BeforeExtracted(source, action);
        }

        public static IEnumerable<T> BeforeEach<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : BeforeEachExtracted(source, action);
        }

        public static IEnumerable<T> BeforeEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : BeforeEachExtracted(source, action);
        }

        public static IEnumerable<T> BeforeEachCounted<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : BeforeEachCountedExtracted(source, action);
        }

        public static IEnumerable<T> BeforeEachCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : BeforeEachCountedExtracted(source, action);
        }

        public static IEnumerable<T> BeforeAny<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : BeforeFirstExtracted(source, action);
        }

        public static IEnumerable<T> BeforeAny<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return action == null ? source : BeforeFirstExtracted(source, action);
        }

        private static IEnumerable<T> AfterCountedExtracted<T>(IEnumerable<T> source, Action<int> action)
        {
            var count = 0;
            foreach (var item in source)
            {
                yield return item;
                count++;
            }
            action.Invoke(count);
        }

        private static IEnumerable<T> AfterEachCountedExtracted<T>(IEnumerable<T> source, Action<int> action)
        {
            var count = 0;
            foreach (var item in source)
            {
                yield return item;
                action.Invoke(count);
                count++;
            }
        }

        private static IEnumerable<T> AfterEachCountedExtracted<T>(IEnumerable<T> source, Action<int, T> action)
        {
            var count = 0;
            foreach (var item in source)
            {
                yield return item;
                action.Invoke(count, item);
                count++;
            }
        }

        private static IEnumerable<T> AfterEachExtracted<T>(IEnumerable<T> source, Action action)
        {
            foreach (var item in source)
            {
                yield return item;
                action.Invoke();
            }
        }

        private static IEnumerable<T> AfterEachExtracted<T>(IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                yield return item;
                action.Invoke(item);
            }
        }

        private static IEnumerable<T> AfterExtracted<T>(IEnumerable<T> source, Action action)
        {
            foreach (var item in source)
            {
                yield return item;
            }
            action.Invoke();
        }

        private static IEnumerable<T> AfterLastCountedExtracted<T>(IEnumerable<T> source, Action<int, T> action)
        {
            var count = 1;
            var enumerator = source.GetEnumerator();
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

        private static IEnumerable<T> AfterLastCountedExtracted<T>(IEnumerable<T> source, Action<int> action)
        {
            var count = 1;
            var enumerator = source.GetEnumerator();
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

        private static IEnumerable<T> AfterLastExtracted<T>(IEnumerable<T> source, Action<T> action)
        {
            var enumerator = source.GetEnumerator();
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

        private static IEnumerable<T> AfterLastExtracted<T>(IEnumerable<T> source, Action action)
        {
            var enumerator = source.GetEnumerator();
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

        private static IEnumerable<T> BeforeEachCountedExtracted<T>(IEnumerable<T> source, Action<int> action)
        {
            var count = 0;
            foreach (var item in source)
            {
                action.Invoke(count);
                yield return item;
                count++;
            }
        }

        private static IEnumerable<T> BeforeEachCountedExtracted<T>(IEnumerable<T> source, Action<int, T> action)
        {
            var count = 0;
            foreach (var item in source)
            {
                action.Invoke(count, item);
                yield return item;
                count++;
            }
        }

        private static IEnumerable<T> BeforeEachExtracted<T>(IEnumerable<T> source, Action action)
        {
            foreach (var item in source)
            {
                action.Invoke();
                yield return item;
            }
        }

        private static IEnumerable<T> BeforeEachExtracted<T>(IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action.Invoke(item);
                yield return item;
            }
        }

        private static IEnumerable<T> BeforeExtracted<T>(IEnumerable<T> source, Action action)
        {
            action.Invoke();
            foreach (var item in source)
            {
                yield return item;
            }
        }

        private static IEnumerable<T> BeforeFirstExtracted<T>(IEnumerable<T> source, Action<T> action)
        {
            var enumerator = source.GetEnumerator();
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

        private static IEnumerable<T> BeforeFirstExtracted<T>(IEnumerable<T> source, Action action)
        {
            var enumerator = source.GetEnumerator();
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