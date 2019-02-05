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
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterExtracted();

            IEnumerable<T> AfterExtracted()
            {
                foreach (var item in source)
                {
                    yield return item;
                }
                action.Invoke();
            }
        }

        public static IEnumerable<T> AfterCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterCountedExtracted();

            IEnumerable<T> AfterCountedExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    yield return item;
                    count++;
                }
                action.Invoke(count);
            }
        }

        public static IEnumerable<T> AfterEach<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterEachExtracted();

            IEnumerable<T> AfterEachExtracted()
            {
                foreach (var item in source)
                {
                    yield return item;
                    action.Invoke();
                }
            }
        }

        public static IEnumerable<T> AfterEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterEachExtracted();

            IEnumerable<T> AfterEachExtracted()
            {
                foreach (var item in source)
                {
                    yield return item;
                    action.Invoke(item);
                }
            }
        }

        public static IEnumerable<T> AfterEachCounted<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterEachCountedExtracted();

            IEnumerable<T> AfterEachCountedExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    yield return item;
                    action.Invoke(count, item);
                    count++;
                }
            }
        }

        public static IEnumerable<T> AfterEachCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterEachCountedExtracted();

            IEnumerable<T> AfterEachCountedExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    yield return item;
                    action.Invoke(count);
                    count++;
                }
            }
        }

        public static IEnumerable<T> AfterAny<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterLastExtracted();

            IEnumerable<T> AfterLastExtracted()
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
        }

        public static IEnumerable<T> AfterAny<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterLastExtracted();

            IEnumerable<T> AfterLastExtracted()
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
        }

        public static IEnumerable<T> AfterLastCounted<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterLastCountedExtracted();

            IEnumerable<T> AfterLastCountedExtracted()
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
        }

        public static IEnumerable<T> AfterLastCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : AfterLastCountedExtracted();

            IEnumerable<T> AfterLastCountedExtracted()
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
        }

        public static IEnumerable<T> Before<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : BeforeExtracted();

            IEnumerable<T> BeforeExtracted()
            {
                action.Invoke();
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> BeforeEach<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : BeforeEachExtracted();

            IEnumerable<T> BeforeEachExtracted()
            {
                foreach (var item in source)
                {
                    action.Invoke();
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> BeforeEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : BeforeEachExtracted();

            IEnumerable<T> BeforeEachExtracted()
            {
                foreach (var item in source)
                {
                    action.Invoke(item);
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> BeforeEachCounted<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : BeforeEachCountedExtracted();

            IEnumerable<T> BeforeEachCountedExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    action.Invoke(count, item);
                    yield return item;
                    count++;
                }
            }
        }

        public static IEnumerable<T> BeforeEachCounted<T>(this IEnumerable<T> source, Action<int> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : BeforeEachCountedExtracted();

            IEnumerable<T> BeforeEachCountedExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    action.Invoke(count);
                    yield return item;
                    count++;
                }
            }
        }

        public static IEnumerable<T> BeforeAny<T>(this IEnumerable<T> source, Action action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : BeforeFirstExtracted();

            IEnumerable<T> BeforeFirstExtracted()
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

        public static IEnumerable<T> BeforeAny<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return action == null ? source : BeforeFirstExtracted();

            IEnumerable<T> BeforeFirstExtracted()
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
        }
    }
}

#endif