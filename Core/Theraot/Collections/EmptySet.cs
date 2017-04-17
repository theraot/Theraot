#if FAT

using System.Collections.Generic;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class EmptySet<T> : ProgressiveSet<T>, IEnumerable<T>
    {
        private static readonly EmptySet<T> _instance = new EmptySet<T>();

        private EmptySet()
            : base(BuildEmptyEnumerable())
        {
            Progressor.AsEnumerable().Consume();
        }

        public static EmptySet<T> Instance
        {
            get { return _instance; }
        }

        private static IEnumerable<T> BuildEmptyEnumerable()
        {
            yield break;
        }
    }
}

#endif