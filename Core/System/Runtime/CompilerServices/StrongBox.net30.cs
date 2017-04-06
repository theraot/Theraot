#if NET20 || NET30

namespace System.Runtime.CompilerServices
{
    public class StrongBox<T> : IStrongBox
    {
        public T Value;

        public StrongBox()
        {
            // Empty
        }

        public StrongBox(T value)
        {
            Value = value;
        }

        object IStrongBox.Value
        {
            get
            {
                return Value;
            }

            set
            {
                Value = (T)value;
            }
        }
    }
}

#endif