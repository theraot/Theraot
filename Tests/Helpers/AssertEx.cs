using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using Theraot.Collections;
using Theraot.Reflection;

namespace Tests.Helpers
{
    public static class AssertEx
    {
        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void AreEqual<T>(T expected, T actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Throws<TException>(Func<object> code)
            where TException : Exception
        {
            Assert.Throws<TException>(() => code());
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Throws<TException, T>(Func<T> code)
            where TException : Exception
        {
            Assert.Throws<TException>(() => code());
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CollectionEquals<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var expectedCollection = expected.AsICollection();
            var foundCollection = actual.AsICollection();
            if (!Equals(expectedCollection.Count, foundCollection.Count))
            {
                Assert.Fail($"Expected Count: {expectedCollection.Count} - Found: {foundCollection.Count}");
            }

            var zip = expectedCollection.Zip(foundCollection, Tuple.Create);
            var index = 0;
            foreach (var (expectedItem, foundItem) in zip)
            {
                if (!Equals(expectedItem, foundItem))
                {
                    Assert.Fail($"Expected Item#{index}: {typeof(T).Name}({expectedItem}) - Found: {typeof(T).Name}({foundItem})");
                }

                index++;
            }
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void AsyncThrows<TException>(Func<Task> func)
            where TException : Exception
        {
            try
            {
                func.Invoke().Wait();
            }
            catch (AggregateException exception)
            {
                Assert.IsTrue(exception.InnerException?.GetType().IsSameOrSubclassOf(typeof(TException)));
                return;
            }
            catch (Exception exception)
            {
                Assert.IsTrue(exception.GetType().IsSameOrSubclassOf(typeof(TException)));
                return;
            }
            Assert.Fail();
        }
    }
}