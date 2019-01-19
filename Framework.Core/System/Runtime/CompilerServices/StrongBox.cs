#if LESSTHAN_NET35

#pragma warning disable CA1051 // Do not declare visible instance fields

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