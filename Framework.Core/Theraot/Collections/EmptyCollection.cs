// Needed for NET30

using System.Collections.ObjectModel;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public sealed class EmptyCollection<T> : ReadOnlyCollectionEx<T>
    {
        private EmptyCollection()
            : base(ArrayReservoir<T>.EmptyArray)
        {
            // Empty
        }

        public static EmptyCollection<T> Instance { get; } = new EmptyCollection<T>();
    }
}