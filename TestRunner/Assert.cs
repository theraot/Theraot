using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theraot.Collections;

#if LESSTHAN_NET35 || GREATERTHAN_NET35 || TARGETS_NETSTANDARD || TARGETS_NETCORE

using System.Linq;

#endif

namespace TestRunner
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T found, string message = null)
        {
            if (Equals(expected, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {expected} - Found: {found} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {expected} - Found: {found}");
        }

        public static void AreNotEqual<T>(T expected, T found, string message = null)
        {
            if (!Equals(expected, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Unexpected: {found} - Message: {message}");
            }
            throw new AssertionFailedException($"Unexpected: {found}");
        }

        public static void CollectionEquals<T>(IEnumerable<T> expected, IEnumerable<T> found, string message = null)
        {
            var expectedCollection = Extensions.AsICollection(expected);
            var foundCollection = Extensions.AsICollection(found);
            if (!Equals(expectedCollection.Count, foundCollection.Count))
            {
                if (message != null)
                {
                    throw new AssertionFailedException($"Expected Count: {expectedCollection.Count} - Found: {foundCollection.Count} - Message: {message}");
                }
                throw new AssertionFailedException($"Expected Count: {expectedCollection.Count} - Found: {foundCollection.Count}");
            }
            var zip = expectedCollection.Zip(foundCollection, Tuple.Create);
            var index = 0;
            foreach (var tuple in zip)
            {
                if (!Equals(tuple.Item1, tuple.Item2))
                {
                    if (message != null)
                    {
                        throw new AssertionFailedException($"Expected Item#{index}: {tuple.Item1} - Found: {tuple.Item2} - Message: {message}");
                    }
                    throw new AssertionFailedException($"Expected Item#{index}: {tuple.Item1} - Found: {tuple.Item2}");
                }
                index++;
            }
        }

        public static void Fail(string message = null)
        {
            if (message != null)
            {
                throw new AssertionFailedException($"Failed - Message: {message}");
            }
            throw new AssertionFailedException("Failed");
        }

        public static void IsFalse(bool found, string message = null)
        {
            if (Equals(false, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {false} - Found: {found} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {false} - Found: {found}");
        }

        public static void IsNotNull<T>(T found, string message = null)
            where T : class
        {
            if (!Equals(null, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Unexpected: {null} - Message: {message}");
            }
            throw new AssertionFailedException($"Unexpected: {null}");
        }

        public static void IsNull<T>(T found, string message = null)
            where T : class
        {
            if (Equals(null, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {null} - Found: {found} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {null} - Found: {found}");
        }

        public static void IsTrue(bool found, string message = null)
        {
            if (Equals(true, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {true} - Found: {found} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {true} - Found: {found}");
        }

        public static TException Throws<TException>(Action action, string message = null)
            where TException : Exception
        {
            try
            {
                action?.Invoke();
            }
            catch (TException exception)
            {
                return exception;
            }
            catch (Exception exception)
            {
                if (message != null)
                {
                    throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception} - Message: {message}", exception);
                }
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception}", exception);
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {typeof(TException).Name}");
        }

        public static TException Throws<TException, T>(Func<T> func, string message = null)
            where TException : Exception
        {
            T foundValue;
            try
            {
                foundValue = func == null ? default : func.Invoke();
            }
            catch (TException exception)
            {
                return exception;
            }
            catch (Exception exception)
            {
                if (message != null)
                {
                    throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception} - Message: {message}", exception);
                }
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception}", exception);
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found value: {foundValue} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found value: {foundValue}");
        }

        public static TException AsyncThrows<TException>(Func<Task> func, string message = null)
            where TException : Exception
        {
            try
            {
                func?.Invoke().Wait();
            }
            catch (AggregateException aggregateException) when (aggregateException.InnerException is TException exception)
            {
                return exception;
            }
            catch (AggregateException aggregateException) when (aggregateException.InnerException is Exception exception)
            {
                if (message != null)
                {
                    throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception} - Message: {message}", exception);
                }
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception}", exception);
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {typeof(TException).Name}");
        }

        public static TException AsyncThrows<TException, T>(Func<Task<T>> func, string message = null)
            where TException : Exception
        {
            T foundValue;
            try
            {
                foundValue = func == null ? default : func.Invoke().Result;
            }
            catch (AggregateException aggregateException) when (aggregateException.InnerException is TException exception)
            {
                return exception;
            }
            catch (AggregateException aggregateException) when (aggregateException.InnerException is Exception exception)
            {
                if (message != null)
                {
                    throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception} - Message: {message}", exception);
                }
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception}", exception);
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found value: {foundValue} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found value: {foundValue}");
        }
    }
}