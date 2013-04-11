#if FAT
using System.Collections.Generic;

namespace Theraot.Collections
{
    public class KeyValuePairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
    {
        private IComparer<TKey> _keyComparer;
        private IComparer<TValue> _valueComparer;

        public KeyValuePairComparer(IComparer<TKey> keyComparer, IComparer<TValue> valueComparer)
        {
            _keyComparer = keyComparer ?? Comparer<TKey>.Default;
            _valueComparer = valueComparer ?? Comparer<TValue>.Default;
        }

        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            var result = _keyComparer.Compare(x.Key, y.Key);
            if (result == 0)
            {
                return _valueComparer.Compare(x.Value, y.Value);
            }
            else
            {
                return result;
            }
        }
    }
}
#endif