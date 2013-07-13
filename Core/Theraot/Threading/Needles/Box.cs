using System;

namespace Theraot.Threading.Needles
{
    public sealed class Box<T> : IReadOnlyNeedle<T>
    {
        private T _field;

        public Box(T field)
        {
            _field = field;
        }

        public T Value
        {
            get
            {
                return _field;
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return true;
            }
        }

        public static implicit operator Box<T>(T field)
        {
            return new Box<T>(field);
        }

        public static explicit operator T(Box<T> box)
        {
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }
            else
            {
                return box.Value;
            }
        }

        public override bool Equals(object obj)
        {
            var _objBox = obj as Box<T>;
            if (!ReferenceEquals(null, _objBox))
            {
                return _field.Equals(_objBox._field);
            }
            if (obj is T)
            {
                return _field.Equals((T)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _field.GetHashCode();
        }

        public override string ToString()
        {
            return _field.ToString();
        }
    }
}