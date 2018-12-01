#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static partial class FuncHelper
    {
        public static Predicate<T> CreateEquals<T>(T comparand, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return y => comparer.Equals(comparand, y);
        }

        public static Predicate<T> CreateNotEquals<T>(T comparand, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return y => !comparer.Equals(comparand, y);
        }

        public static Predicate<T> GetFallacyPredicate<T>()
        {
            return HelperFallacyPredicate<T>.Instance;
        }

        public static Predicate<T> GetTautologyPredicate<T>()
        {
            return HelperTautologyPredicate<T>.Instance;
        }

        public static Func<T, bool> ToFunc<T>(this Predicate<T> predicate)
        {
            var target = predicate.Target;
            if (target is FuncWrapper<T> funcWrapper)
            {
                return funcWrapper.Func;
            }
            var wrapper = new PredicateWrapper<T>(predicate);
            return wrapper.Invoke;
        }

        public static Predicate<T> ToPredicate<T>(this Func<T, bool> func)
        {
            var target = func.Target;
            if (target is PredicateWrapper<T> predicateWrapper)
            {
                return predicateWrapper.Predicate;
            }
            var wrapper = new FuncWrapper<T>(func);
            return wrapper.Invoke;
        }

        private static class HelperFallacyPredicate<T>
        {
            static HelperFallacyPredicate()
            {
                Instance = FallacyFunc;
            }

            public static Predicate<T> Instance { get; }

            private static bool FallacyFunc(T obj)
            {
                GC.KeepAlive(obj);
                return false;
            }
        }

        private static class HelperTautologyPredicate<T>
        {
            static HelperTautologyPredicate()
            {
                Instance = TautologyFunc;
            }

            public static Predicate<T> Instance { get; }

            private static bool TautologyFunc(T obj)
            {
                GC.KeepAlive(obj);
                return true;
            }
        }

        private class FuncWrapper<T>
        {
            public FuncWrapper(Func<T, bool> func)
            {
                Func = func ?? throw new ArgumentNullException(nameof(func));
            }

            public Func<T, bool> Func { get; }

            public bool Invoke(T input)
            {
                return Func.Invoke(input);
            }
        }

        private class PredicateWrapper<T>
        {
            public PredicateWrapper(Predicate<T> predicate)
            {
                Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            }

            public Predicate<T> Predicate { get; }

            public bool Invoke(T input)
            {
                return Predicate.Invoke(input);
            }
        }
    }
}

#endif