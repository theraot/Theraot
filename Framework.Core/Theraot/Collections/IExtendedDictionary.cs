#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    public interface IExtendedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IExtendedReadOnlyDictionary<TKey, TValue>
    {
        // Collides IDictionary<TKey, TValue> with IExtendedReadOnlyDictionary<TKey, TValue>
        new IReadOnlyDictionary<TKey, TValue> AsReadOnly { get; }
    }
}

#endif