using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
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