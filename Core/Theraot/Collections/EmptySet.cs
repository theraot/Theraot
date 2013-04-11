using System.Collections.Generic;

namespace Theraot.Collections
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.ComponentModel.ImmutableObject(true)]
    public sealed class EmptySet<T> : ProgressiveSet<T>, IEnumerable<T>
    {
        private static readonly EmptySet<T> _instance = new EmptySet<T>();

        private EmptySet()
            : base(BuildEmptyEnumerable())
        {
            Progressor.TakeAll();
        }

        public static EmptySet<T> Instance
        {
            get
            {
                return _instance;
            }
        }

        private static IEnumerable<T> BuildEmptyEnumerable()
        {
            yield break;
        }
    }
}