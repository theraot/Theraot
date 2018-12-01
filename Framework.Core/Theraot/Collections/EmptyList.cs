#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class EmptyList<T> : ProgressiveList<T>
    {
        private EmptyList()
            : base(BuildEmptyEnumerable())
        {
            Progressor.Consume();
        }

        public static EmptyList<T> Instance { get; } = new EmptyList<T>();

        private static IEnumerable<T> BuildEmptyEnumerable()
        {
            yield break;
        }
    }
}

#endif