using System.Diagnostics;
using System.Runtime.Serialization;

#if NET35

using System.Runtime.CompilerServices;

#endif

#if (GREATERTHAN_NET30 && LESSTHAN_NET472) || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

#pragma warning disable CA1001 // Types that own disposable fields should be disposable

using System.Diagnostics.CodeAnalysis;

#endif

namespace System.Collections.Generic
{
    // ReSharper disable once BadPreprocessorIndent
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public class HashSetEx<T> : HashSet<T>
#if NET35
        , ISet<T>
#endif
#if GREATERTHAN_NET30 || LESSTHAN_NET50 || LESSTHAN_NETSTANDARD22
        , IReadOnlySet<T>
#endif
    {
        public HashSetEx()
            : this(EqualityComparer<T>.Default)
        {
            // Empty
        }

        public HashSetEx(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
            // Empty
        }

        public HashSetEx(IEnumerable<T> collection, IEqualityComparer<T>? comparer)
#if (GREATERTHAN_NET30 && LESSTHAN_NET472) || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21
            : base(collection, SpyEqualityComparer.GetFrom(comparer))
#else
            : base(collection, comparer ?? EqualityComparer<T>.Default)
#endif
        {
            // Empty
        }

        public HashSetEx(IEqualityComparer<T>? comparer)
#if (GREATERTHAN_NET30 && LESSTHAN_NET472) || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21
            : base(SpyEqualityComparer.GetFrom(comparer))
#else
            : base(comparer ?? EqualityComparer<T>.Default)
#endif
        {
            // Empty
        }

#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET

        protected HashSetEx(SerializationInfo info, StreamingContext context)
            : base(info, context)

#else

        [Obsolete("This target platform does not support binary serialization.")]
        protected HashSetEx(SerializationInfo info, StreamingContext context)

#endif

        {
            _ = info;
            _ = context;
        }

#if NET35
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.Add(T item)
        {
            return Add(item);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            ExceptWith(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            IntersectWith(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other)
        {
            return IsProperSubsetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsProperSupersetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.IsSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.Overlaps(IEnumerable<T> other)
        {
            return Overlaps(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        bool ISet<T>.SetEquals(IEnumerable<T> other)
        {
            return SetEquals(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            SymmetricExceptWith(other);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            UnionWith(other);
        }

#endif

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

        public static IEqualityComparer<HashSet<T>> CreateSetComparer()
        {
            return HashSetEqualityComparer<T>.Instance;
        }

#endif

#if (GREATERTHAN_NET30 && LESSTHAN_NET472) || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

        public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
        {
            var spy = (Comparer as SpyEqualityComparer)!;
            var found = equalValue!;
            spy.SetCallback
            (
                (stored, check) =>
                {
                    _ = check;
                    found = stored;
                }
            );
            var result = Contains(equalValue);
            spy.SetCallback(callback: null);
            if (result)
            {
                actualValue = found;
                return true;
            }

            actualValue = default;
            return false;
        }

        private sealed class SpyEqualityComparer : IEqualityComparer<T>
        {
            private readonly Threading.ThreadLocal<Action<T, T>?> _callback = new();
            private readonly IEqualityComparer<T> _wrapped;

            private SpyEqualityComparer(IEqualityComparer<T> wrapped)
            {
                _wrapped = wrapped;
            }

            public static IEqualityComparer<T> GetFrom(IEqualityComparer<T>? comparer)
            {
                if (comparer == null)
                {
                    return new SpyEqualityComparer(EqualityComparer<T>.Default);
                }

                if (comparer is SpyEqualityComparer)
                {
                    return comparer;
                }

                return new SpyEqualityComparer(comparer);
            }

            public bool Equals(T x, T y)
            {
                GetCallback()?.Invoke(x, y);
                return _wrapped.Equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _wrapped.GetHashCode(obj);
            }

            public void SetCallback(Action<T, T>? callback)
            {
                _callback.Value = callback;
            }

            private Action<T, T>? GetCallback()
            {
                if (!_callback.IsValueCreated || _callback.Value == null)
                {
                    return null;
                }

                return _callback.Value;
            }
        }

#endif
    }
}