#if LESSTHAN_NET35

#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable S1104 // Fields should not have public accessibility

using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
    public class StrongBox<T> : IStrongBox
    {
        [MaybeNull]
        public T Value;

        public StrongBox()
        {
            Value = default!;
        }

        public StrongBox(T value)
        {
            Value = value!;
        }

        object? IStrongBox.Value
        {
            get => Value!;

            set => Value = value is T valueAsT ? valueAsT : default!;
        }
    }
}

#endif