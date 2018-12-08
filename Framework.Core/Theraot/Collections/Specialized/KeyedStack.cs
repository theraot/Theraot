using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    public sealed class KeyedStack<TKey, TValue>
    {
        private readonly NullAwareDictionary<TKey, Stack<TValue>> _data;

        public KeyedStack()
        {
            _data = new NullAwareDictionary<TKey, Stack<TValue>>();
        }

        public void Add(TKey key, TValue item)
        {
            if (!_data.TryGetValue(key, out var stack))
            {
                _data.Add(key, stack = new Stack<TValue>());
            }
            stack.Push(item);
        }

        public int GetCount(TKey key)
        {
            if (_data.TryGetValue(key, out var stack))
            {
                return stack.Count;
            }
            return 0;
        }

        public bool TryTake(TKey key, out TValue item)
        {
            if (_data.TryGetValue(key, out var stack))
            {
                return stack.TryTake(out item);
            }
            item = default;
            return false;
        }
    }
}