#pragma warning disable CS8714 // Nullability of type argument doesn't match 'notnull' constraint

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Theraot;
using Theraot.Collections.Specialized;

namespace System.Collections.Generic
{
    [Serializable]
    [ComVisible(false)]
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IHasComparer<TKey>
    {
        public DictionaryEx()
        {
            // Empty
        }

        public DictionaryEx(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
            // Empty
        }

        public DictionaryEx(KeyValuePair<TKey, TValue>[] dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public DictionaryEx(KeyValuePair<TKey, TValue>[] dictionary, IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public DictionaryEx(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
            // Empty
        }

        public DictionaryEx(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            // Empty
        }

        public DictionaryEx(int capacity)
            : base(capacity)
        {
            // Empty
        }

        public DictionaryEx(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
            // Empty
        }

#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET

        protected DictionaryEx(SerializationInfo info, StreamingContext context)
            : base(info, context)

#else
        [Obsolete("This target platform does not support binary serialization.")]
        protected DictionaryEx(SerializationInfo info, StreamingContext context)
#endif
        {
            No.Op(info);
            No.Op(context);
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;
    }
}