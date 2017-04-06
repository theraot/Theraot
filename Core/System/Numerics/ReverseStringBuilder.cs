#if NET20 || NET30 || NET35

using System.Collections;
using System.Collections.Generic;

namespace System.Numerics
{
    internal class ReverseStringBuilder : IEnumerable<char>
    {
        private readonly char[] _buffer;
        private int _start;

        public ReverseStringBuilder(int capacity)
        {
            _buffer = new char[capacity];
            _start = _buffer.Length;
        }

        public char[] Buffer
        {
            get
            {
                return _buffer;
            }
        }

        public int Length
        {
            get
            {
                return _buffer.Length - _start;
            }
        }

        public void Prepend(char character)
        {
            _buffer[--_start] = character;
        }

        public void Prepend(string str)
        {
            for (var index = str.Length - 1; index > -1; index--)
            {
                _buffer[--_start] = str[index];
            }
        }

        public IEnumerator<char> GetEnumerator()
        {
            for (var position = _buffer.Length - 1; position >= 0; position--)
            {
                yield return _buffer[position];
            }
        }

        public override
            string ToString()
        {
            return new string(_buffer, _start, _buffer.Length - _start);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            /*if (backIndex < length)
            {
                throw new ArgumentOutOfRangeException("maxLength", "Too small");
            }
            if (backIndex > _buffer.Length)
            {
                throw new ArgumentOutOfRangeException("maxLength", "Too large");
            }*/
            return new string(_buffer, _buffer.Length - backIndex, length);
        }
    }
}

#endif