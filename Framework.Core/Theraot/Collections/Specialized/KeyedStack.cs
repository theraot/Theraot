// Needed for NET30 (Expressions)

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Theraot.Collections.Specialized
{
    public sealed class KeyedStack<TKey, TValue>
    {
        private readonly Dictionary<TKey, Stack<TValue>> _data;

        public KeyedStack()
        {
            _data = new Dictionary<TKey, Stack<TValue>>();
        }

        public void Add(TKey key, TValue item)
        {
            if (!_data.TryGetValue(key, out var stack))
            {
                stack = new Stack<TValue>();
                _data.Add(key, stack);
            }

            stack.Push(item);
        }

        public bool TryTake(TKey key, [MaybeNullWhen(false)] out TValue item)
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