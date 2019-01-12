#if LESSTHAN_NET35

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
            get => Value;

            set => Value = (T)value;
        }
    }
}

#endif