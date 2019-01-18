using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Core
{
    public class ReverseStringBuilder : IEnumerable<char>
    {
        private readonly char[] _buffer;
        private int _start;

        public ReverseStringBuilder(int capacity)
        {
            try
            {
                _buffer = new char[capacity];
                _start = _buffer.Length;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
        }

        public int Length => _buffer.Length - _start;

        public IEnumerator<char> GetEnumerator()
        {
            for (var position = _buffer.Length - 1; position >= 0; position--)
            {
                yield return _buffer[position];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            return new string(_buffer, _start, _buffer.Length - _start);
        }

        public string ToString(int maxLength)
        {
            var length = _buffer.Length - _start;
            if (length > maxLength)
            {
                length = maxLength;
            }
            return new string(_buffer, _buffer.Length - length, length);
        }

        public string ToString(int backIndex, int maxLength)
        {
            var length = _buffer.Length - _start;
            if (length > maxLength)
            {
                length = maxLength;
            }
            return new string(_buffer, _buffer.Length - backIndex, length);
        }
    }
}