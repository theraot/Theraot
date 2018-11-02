#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    public class KeyValuePairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    {
        private readonly IComparer<TKey> _keyComparer;
        private readonly IComparer<TValue> _valueComparer;

        public KeyValuePairComparer(IComparer<TKey> keyComparer, IComparer<TValue> valueComparer)
        {
            _keyComparer = keyComparer ?? Comparer<TKey>.Default;
            _valueComparer = valueComparer ?? Comparer<TValue>.Default;
        }

        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            var result = _keyComparer.Compare(x.Key, y.Key);
            return result == 0 ? _valueComparer.Compare(x.Value, y.Value) : result;
        }
    }
}

#endif