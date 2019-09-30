using System.Diagnostics.CodeAnalysis;

#if LESSTHAN_NET35

#pragma warning disable CA1051 // Do not declare visible instance fields

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

        public StrongBox([AllowNull][MaybeNull]T value)
        {
            Value = value;
        }

        object? IStrongBox.Value
        {
            get => Value;

            set => Value = value is T valueAsT ? valueAsT : default;
        }
    }
}

#endif