// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Linq.Expressions.Compiler
{
    /// <summary>
    /// A simple dictionary of queues, keyed off a particular type
    /// This is useful for storing free lists of variables
    /// </summary>
    internal sealed class KeyedQueue<TK, TV>
    {
        private readonly Dictionary<TK, Queue<TV>> _data;

        internal KeyedQueue()
        {
            _data = new Dictionary<TK, Queue<TV>>();
        }

        internal void Enqueue(TK key, TV value)
        {
            if (!_data.TryGetValue(key, out Queue<TV> queue))
            {
                _data.Add(key, queue = new Queue<TV>());
            }
            queue.Enqueue(value);
        }

        internal TV Dequeue(TK key)
        {
            if (!_data.TryGetValue(key, out Queue<TV> queue))
            {
                throw Error.QueueEmpty();
            }
            var result = queue.Dequeue();
            if (queue.Count == 0)
            {
                _data.Remove(key);
            }
            return result;
        }

        internal bool TryDequeue(TK key, out TV value)
        {
            if (_data.TryGetValue(key, out Queue<TV> queue) && queue.Count > 0)
            {
                value = queue.Dequeue();
                if (queue.Count == 0)
                {
                    _data.Remove(key);
                }
                return true;
            }
            value = default;
            return false;
        }

        internal TV Peek(TK key)
        {
            if (!_data.TryGetValue(key, out Queue<TV> queue))
            {
                throw Error.QueueEmpty();
            }
            return queue.Peek();
        }

        internal int GetCount(TK key)
        {
            if (!_data.TryGetValue(key, out Queue<TV> queue))
            {
                return 0;
            }
            return queue.Count;
        }

        internal void Clear()
        {
            _data.Clear();
        }
    }
}