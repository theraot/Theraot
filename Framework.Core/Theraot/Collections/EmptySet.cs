using System.Collections.Generic;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class EmptySet<T> : ProgressiveSet<T>
    {
        private EmptySet()
            : base(BuildEmptyEnumerable())
        {
            ConsumeAll();
        }

        public static EmptySet<T> Instance { get; } = new EmptySet<T>();

        private static IEnumerable<T> BuildEmptyEnumerable()
        {
            yield break;
        }
    }
}