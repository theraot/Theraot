using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
    [Serializable]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public class SortedSetEx<T> : SortedSet<T>
#if TARGETS_NET || LESSTHAN_NET50 || LESSTHAN_NETSTANDARD22
        , IReadOnlySet<T>
#endif
#if (LESSTHAN_NET46 && GREATERTHAN_NET30) || LESSTHAN_NETSTANDARD13
        , IReadOnlyCollection<T>
#endif
    {
        public SortedSetEx()
            : this(Comparer<T>.Default)
        {
            // Empty
        }

        public SortedSetEx(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
            // Empty
        }

        public SortedSetEx(IEnumerable<T> collection, IComparer<T>? comparer)
            : base(collection, comparer ?? Comparer<T>.Default)
        {
            // Empty
        }

        public SortedSetEx(IComparer<T>? comparer)
            : base(comparer ?? Comparer<T>.Default)
        {
            // Empty
        }

#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET

        protected SortedSetEx(SerializationInfo info, StreamingContext context)
            : base(info, context)

#else

        [Obsolete("This target platform does not support binary serialization.")]
        protected SortedSetEx(SerializationInfo info, StreamingContext context)

#endif

        {
            _ = info;
            _ = context;
        }

#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

        public static IEqualityComparer<SortedSet<T>> CreateSetComparer()
        {
            return SortedSetEqualityComparer<T>.Instance;
        }

        public static IEqualityComparer<SortedSet<T>> CreateSetComparer(IEqualityComparer<T>? memberEqualityComparer)
        {
            return new SortedSetEqualityComparer<T>(memberEqualityComparer);
        }
#endif
    }
}