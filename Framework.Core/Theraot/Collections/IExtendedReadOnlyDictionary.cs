#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    public interface IExtendedReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IExtendedCollection<KeyValuePair<TKey, TValue>>
    {
        // Intended to collide with IDictionary<TKey, TValue>
        new IReadOnlyCollection<TKey> Keys { get; }

        new IReadOnlyCollection<TValue> Values { get; }
    }
}

#endif