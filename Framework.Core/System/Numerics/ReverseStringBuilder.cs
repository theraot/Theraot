#if LESSTHAN_NET40

using System.Collections;
using System.Collections.Generic;

namespace System.Numerics
{
    internal class ReverseStringBuilder : IEnumerable<char>
    {
        private int _start;

        public ReverseStringBuilder(int capacity)
        {
            Buffer = new char[capacity];
            _start = Buffer.Length;
        }

        public char[] Buffer { get; }

        public int Length => Buffer.Length - _start;

        public IEnumerator<char> GetEnumerator()
        {
            for (var position = Buffer.Length - 1; position >= 0; position--)
            {
                yield return Buffer[position];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Prepend(char character)
        {
            Buffer[--_start] = character;
        }

        public void Prepend(string str)
        {
            for (var index = str.Length - 1; index > -1; index--)
            {
                Buffer[--_start] = str[index];
            }
        }

        public override
            string ToString()
        {
            return new string(Buffer, _start, Buffer.Length - _start);
        }

        public string ToString(int maxLength)
        {
            var length = Buffer.Length - _start;
            if (length > maxLength)
            {
                length = maxLength;
            }
            return new string(Buffer, Buffer.Length - length, length);
        }

        public string ToString(int backIndex, int maxLength)
        {
            var length = Buffer.Length - _start;
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
            return new string(Buffer, Buffer.Length - backIndex, length);
        }
    }
}

#endif