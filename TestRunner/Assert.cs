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
            throw new AssertionFailedException(BuildMessage(expected, found, message));
        }

        public static void AreNotEqual<T>(T expected, T found, string message = null)
        {
            if (!Equals(expected, found))
            {
                return;
            }
            throw new AssertionFailedException($"Unexpected: {typeof(T).Name}({found}){(message == null ? string.Empty : $" - Message: {message}")}");
        }

        public static void CollectionEquals<T>(IEnumerable<T> expected, IEnumerable<T> found, string message = null)
        {
            var expectedCollection = Extensions.AsICollection(expected);
            var foundCollection = Extensions.AsICollection(found);
            if (!Equals(expectedCollection.Count, foundCollection.Count))
            {
                throw new AssertionFailedException($"Expected Count: {expectedCollection.Count} - Found: {foundCollection.Count}{(message == null ? string.Empty : $" - Message: {message}")}");
            }
            var zip = expectedCollection.Zip(foundCollection, Tuple.Create);
            var index = 0;
            foreach (var tuple in zip)
            {
                if (!Equals(tuple.Item1, tuple.Item2))
                {
                    throw new AssertionFailedException($"Expected Item#{index}: {typeof(T).Name}({tuple.Item1}) - Found: {typeof(T).Name}({tuple.Item2}){(message == null ? string.Empty : $" - Message: {message}")}");
                }
                index++;
            }
        }

        public static void Fail(string message = null)
        {
            throw new AssertionFailedException($"Failed{(message == null ? string.Empty : $" - Message: {message}")}");
        }

        public static void IsFalse(bool found, string message = null)
        {
            if (Equals(false, found))
            {
                return;
            }
            throw new AssertionFailedException(BuildMessage(false, found, message));
        }

        public static void IsNotNull<T>(T found, string message = null)
            where T : class
        {
            if (!Equals(null, found))
            {
                return;
            }
            throw new AssertionFailedException($"Unexpected: {null}{(message == null ? string.Empty : $" - Message: {message}")}");
        }

        public static void IsNull<T>(T found, string message = null)
            where T : class
        {
            if (Equals(null, found))
            {
                return;
            }
            throw new AssertionFailedException(BuildMessage<object, T>(null, found, message));
        }

        public static void IsTrue(bool found, string message = null)
        {
            if (Equals(true, found))
            {
                return;
            }
            throw new AssertionFailedException(BuildMessage(true, found, message));
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
                throw new AssertionFailedException(BuildMessage<TException>(exception, message), exception);
            }
            throw new AssertionFailedException(BuildMessage<TException>(message));
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
                throw new AssertionFailedException(BuildMessage<TException>(exception, message), exception);
            }
            throw new AssertionFailedException(BuildMessage<TException, T>(foundValue, message));
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
                throw new AssertionFailedException(BuildMessage<TException>(exception, message), exception);
            }
            throw new AssertionFailedException(BuildMessage<TException>(message));
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
                throw new AssertionFailedException(BuildMessage<TException>(exception, message), exception);
            }
            throw new AssertionFailedException(BuildMessage<TException, T>(foundValue, message));
        }

        private static string BuildMessage<TException>(Exception exception, string message)
            where TException : Exception
        {
            return $"Expected: {typeof(TException).Name} - Found: {exception.GetType().Name}{(message == null ? string.Empty : $" - Message: {message}")}";
        }

        private static string BuildMessage<TException>(string message)
            where TException : Exception
        {
            return $"Expected: {typeof(TException).Name}{(message == null ? string.Empty : $" - Message: {message}")}";
        }

        private static string BuildMessage<TException, TFound>(TFound found, string message)
            where TException : Exception
        {
            return $"Expected: {typeof(TException).Name} - Found value: {typeof(TFound).Name}({found}){(message == null ? string.Empty : $" - Message: {message}")}";
        }

        private static string BuildMessage<TExpected, TFound>(TExpected expected, TFound found, string message)
        {
            return $"Expected: {typeof(TExpected).Name}({expected}) - Found value: {typeof(TFound).Name}({found}){(message == null ? string.Empty : $" - Message: {message}")}";
        }
    }
}