#if LESSTHAN_NET45

#pragma warning disable CA1034 // Do not nest type Enumerator
#pragma warning disable CA1066 // Implement IEquatable when overriding Object.Equals
#pragma warning disable CA1305 // string.Format could vary
#pragma warning disable CA1815 // override equality and inequality
#pragma warning disable CA2225 // Provide a method as alternative to operator implicit
#pragma warning disable IDE0011 // Add braces

using System.Text;

namespace System
{
    public ref struct Span<T>
    {
        internal T[] _array;
        internal int _start;
        internal int _length;

        public Span(T[]? array)
        {
            if (array == null)
            {
                _array = ArrayEx.Empty<T>();
                _start = 0;
                _length = 0;
            }
            else
            {
                if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
                {
                    throw new ArrayTypeMismatchException(nameof(array));
                }

                _array = array;
                _start = 0;
                _length = array.Length;
            }
        }

        public Span(T[]? array, int start, int length)
        {
            if (array == null)
            {
                _array = ArrayEx.Empty<T>();
                _start = 0;
                _length = 0;
            }
            else
            {
                if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
                {
                    throw new ArrayTypeMismatchException(nameof(array));
                }

                _array = array;
                _start = start;
                _length = length;
            }
        }

        public static Span<T> Empty
        {
            get
            {
                return new Span<T>(ArrayEx.Empty<T>());
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _length == 0;
            }
        }

        public ref T this[int index]
        {
            get
            {
                return ref _array[_start + index];
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _array.Length; i++)
            {
                _array[i] = default!;
            }
        }

        public void CopyTo(Span<T> destination)
        {
            for (int i = 0; i < _array.Length; i++)
            {
                destination[i] = _array[_start + i];
            }
        }

        public override bool Equals(object obj)
        {
            throw new NotSupportedException();
        }

        public override int GetHashCode()
        {
            throw new NotSupportedException();
        }

        public void Fill(T value)
        {
            for (int i = 0; i < _array.Length; i++)
            {
                _array[_start + i] = value;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref T GetPinnableReference()
        {
            throw new NotSupportedException();
        }

        public Span<T> Slice(int start)
        {
            return new Span<T>(_array, _start + start, _length - start);
        }

        public Span<T> Slice(int start, int length)
        {
            return new Span<T>(_array, _start + start, _length - start - length);
        }

        public T[] ToArray()
        {
            if (_length == 0)
            {
                return ArrayEx.Empty<T>();
            }

            var array = new T[_length];
            Array.Copy(_array, _start, array, 0, _length);
            return array;
        }

        public override string ToString()
        {
            if (typeof(T) == typeof(char))
            {
                StringBuilder builder = new();

                foreach (T? character in this)
                {
                    // This condition only exists to cast T as char as this should always be true.
                    // The original uses Unsafe.As<T>, but until it is polyfilled, it has to be done like this.
                    if (character is char c)
                    {
                        builder.Append(c);
                    }
                }

                return builder.ToString();
            }

            return string.Format("System.Span<{0}>[{1}]", typeof(T).Name, _length);
        }

        public bool TryCopyTo(Span<T> destination)
        {
            if (destination.Length < _length)
            {
                return false;
            }

            CopyTo(destination);
            return true;
        }

        public static bool operator ==(Span<T> left, Span<T> right)
        {
            if (left.Length != right.Length) return false;

            for (int i = 0; i < left.Length; i++)
            {
                if (!Equals(left[i], right[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Span<T> left, Span<T> right) => !(left == right);

        public static implicit operator Span<T>(T[]? array) => new Span<T>(array);

        public static implicit operator Span<T>(ArraySegment<T> segment) =>
            new Span<T>(segment.Array, segment.Offset, segment.Count);

        public static implicit operator ReadOnlySpan<T>(Span<T> span) =>
            new ReadOnlySpan<T>(span._array, span._start, span._length);

        /// <summary>
        ///  From .NET source
        /// </summary>
        public ref struct Enumerator
        {
            private Span<T> _span;
            private int _index;

            internal Enumerator(Span<T> span)
            {
                _span = span;
                _index = -1;
            }

            public bool MoveNext()
            {
                int index = _index + 1;
                if (index < _span._length)
                {
                    _index = index;
                    return true;
                }

                return false;
            }

            public ref T Current
            {
                get => ref _span[_index];
            }
        }
    }
}

#endif