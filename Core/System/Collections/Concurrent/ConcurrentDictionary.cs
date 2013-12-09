using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;

namespace System.Collections.Concurrent
{
    [SerializableAttribute]
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        //TODO
    }
}
