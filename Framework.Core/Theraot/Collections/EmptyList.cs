#if FAT
using System.Collections.Generic;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class EmptyList<T> : ProgressiveList<T>
    {
        private EmptyList()
            : base(Enumerable.Empty<T>())
        {
            ConsumeAll();
        }

        public static EmptyList<T> Instance { get; } = new EmptyList<T>();
    }
}

#endif