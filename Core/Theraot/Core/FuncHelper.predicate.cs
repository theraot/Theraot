using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static partial class FuncHelper
    {
        public static Predicate<T> CreateEquals<T>(T comparand, IEqualityComparer<T> comparer)
        {
            Check.NotNullArgument(comparer, "comparer");
            return delegate(T y)
            {
                return comparer.Equals(comparand, y);
            };
        }

        public static Predicate<T> CreateNotEquals<T>(T comparand, IEqualityComparer<T> comparer)
        {
            Check.NotNullArgument(comparer, "comparer");
            return delegate(T y)
            {
                return !comparer.Equals(comparand, y);
            };
        }

        public static Predicate<T> GetFallacyPredicate<T>(T item)
        {
            return HelperFallacyPredicate<T>.Instance;
        }

        public static Predicate<T> GetTautologyPredicate<T>(T item)
        {
            return HelperTautologyPredicate<T>.Instance;
        }

        public static Func<T, bool> ToFunc<T>(this Predicate<T> predicate)
        {
            var target = predicate.Target;
            if (target.GetType() == typeof(FuncWrapper<T>))
            {
                return (target as FuncWrapper<T>).Func;
            }
            else
            {
                var wrapper = new PredicateWrapper<T>(predicate);
                return new Func<T, bool>(wrapper.Invoke);
            }
        }

        public static Predicate<T> ToPredicate<T>(this Func<T, bool> func)
        {
            var target = func.Target;
            if (target.GetType() == typeof(PredicateWrapper<T>))
            {
                return (target as PredicateWrapper<T>).Predicate;
            }
            else
            {
                var wrapper = new FuncWrapper<T>(func);
                return new Predicate<T>(wrapper.Invoke);
            }
        }

        private static class HelperFallacyPredicate<T>
        {
            private static readonly Predicate<T> _instance;

            static HelperFallacyPredicate()
            {
                _instance = FallacyFunc;
            }

            public static Predicate<T> Instance
            {
                get
                {
                    return _instance;
                }
            }

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "By Design")]
            private static bool FallacyFunc(T obj)
            {
                return false;
            }
        }

        private static class HelperTautologyPredicate<T>
        {
            private static readonly Predicate<T> _instance;

            static HelperTautologyPredicate()
            {
                _instance = TautologyFunc;
            }

            public static Predicate<T> Instance
            {
                get
                {
                    return _instance;
                }
            }

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "By Design")]
            private static bool TautologyFunc(T obj)
            {
                return true;
            }
        }

        private class FuncWrapper<T>
        {
            private Func<T, bool> _func;

            public FuncWrapper(Func<T, bool> func)
            {
                _func = Check.NotNullArgument(func, "func");
            }

            public Func<T, bool> Func
            {
                get
                {
                    return _func;
                }
            }

            public bool Invoke(T input)
            {
                return _func.Invoke(input);
            }
        }

        private class PredicateWrapper<T>
        {
            private Predicate<T> _predicate;

            public PredicateWrapper(Predicate<T> predicate)
            {
                _predicate = Check.NotNullArgument(predicate, "predicate");
            }

            public Predicate<T> Predicate
            {
                get
                {
                    return _predicate;
                }
            }

            public bool Invoke(T input)
            {
                return _predicate.Invoke(input);
            }
        }
    }
}