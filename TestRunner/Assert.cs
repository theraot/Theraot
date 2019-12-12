using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theraot.Collections;

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

        public static void CollectionEquals<T>(IEnumerable<T> expected, IEnumerable<T> found, string message = null)
        {
            var expectedCollection = expected.AsICollection();
            var foundCollection = found.AsICollection();
            if (!Equals(expectedCollection.Count, foundCollection.Count))
            {
                throw new AssertionFailedException($"Expected Count: {expectedCollection.Count} - Found: {foundCollection.Count}{(message == null ? string.Empty : $" - Message: {message}")}");
            }

            var zip = expectedCollection.Zip(foundCollection, Tuple.Create);
            var index = 0;
            foreach (var (expectedItem, foundItem) in zip)
            {
                if (!Equals(expectedItem, foundItem))
                {
                    throw new AssertionFailedException($"Expected Item#{index}: {typeof(T).Name}({expectedItem}) - Found: {typeof(T).Name}({foundItem}){(message == null ? string.Empty : $" - Message: {message}")}");
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

            throw new AssertionFailedException(BuildMessage(false, true, message));
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

            throw new AssertionFailedException(BuildMessage(true, false, message));
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