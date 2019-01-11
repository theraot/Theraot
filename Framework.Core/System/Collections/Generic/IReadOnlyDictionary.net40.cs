#if LESSTHAN_NET45

namespace System.Collections.Generic
{
    public partial interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        IEnumerable<TKey> Keys { get; }

        IEnumerable<TValue> Values { get; }
    }

    public partial interface IReadOnlyDictionary<TKey, TValue>
    {
        TValue this[TKey key] { get; }
    }

    public partial interface IReadOnlyDictionary<TKey, TValue>
    {
        bool ContainsKey(TKey key);

        bool TryGetValue(TKey key, out TValue value);
    }
}

#endif