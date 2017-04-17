// Needed for NET30

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class EmptyCollection<T> : ReadOnlyCollection<T>, IReadOnlyCollection<T>
    {
        private static readonly EmptyCollection<T> _instance = new EmptyCollection<T>();

        private EmptyCollection()
            : base(ArrayReservoir<T>.EmptyArray)
        {
        }

        public static EmptyCollection<T> Instance
        {
            get { return _instance; }
        }
    }
}