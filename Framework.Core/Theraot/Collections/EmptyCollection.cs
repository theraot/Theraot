// Needed for NET30

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public sealed class EmptyCollection<T> : ReadOnlyCollectionEx<T>
    {
        private EmptyCollection()
            : base(ArrayEx.Empty<T>())
        {
            // Empty
        }

        public static EmptyCollection<T> Instance { get; } = new EmptyCollection<T>();
    }
}