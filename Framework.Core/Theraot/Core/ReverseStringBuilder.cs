using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace Theraot.Core
{
    public class ReverseStringBuilder : IEnumerable<char>
    {
        private readonly int _capacity;
        private char[] _buffer;
        private int _start;

        public ReverseStringBuilder(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _buffer = ArrayReservoir<char>.GetArray(capacity);
            _capacity = capacity;
            _start = capacity;
        }

        public int Length => _capacity - _start;

        public IEnumerator<char> GetEnumerator()
        {
            for (var position = _capacity - 1; position >= 0; position--)
            {
                yield return _buffer[position];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ~ReverseStringBuilder()
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (GCMonitor.FinalizingForUnload)
            {
                return;
            }

            var buffer = _buffer;
            if (buffer == null)
            {
                return;
            }

            ArrayReservoir<char>.DonateArray(buffer);
            _buffer = null;
        }

        public void Prepend(char character)
        {
            _buffer[--_start] = character;
        }

        public void Prepend(string str)
        {
            var newStart = _start - str.Length;
            str.CopyTo(0, _buffer, newStart, str.Length);
            _start = newStart;
        }

        public override string ToString()
        {
            return new string(_buffer, _start, _capacity - _start);
        }

        public string ToString(int maxLength)
        {
            var length = _capacity - _start;
            if (length > maxLength)
            {
                length = maxLength;
            }

            return new string(_buffer, _capacity - length, length);
        }

        public string ToString(int backIndex, int maxLength)
        {
            if (backIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(backIndex));
            }

            var length = _capacity - _start;
            if (length > maxLength)
            {
                length = maxLength;
            }

            return new string(_buffer, _capacity - backIndex, length);
        }
    }
}