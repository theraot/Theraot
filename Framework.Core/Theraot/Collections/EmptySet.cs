using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public sealed class EmptySet<T> : ProgressiveSet<T>
    {
        internal EmptySet(IEqualityComparer<T>? comparer)
            : base(ArrayEx.Empty<T>(), comparer)
        {
            ConsumeAll();
        }

        private EmptySet()
            : base(ArrayEx.Empty<T>())
        {
            ConsumeAll();
        }

        public static EmptySet<T> Instance { get; } = new EmptySet<T>();
    }
}