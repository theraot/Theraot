// Needed for NET30

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Theraot.Collections
{
    [System.Diagnostics.DebuggerNonUserCode]
    [System.ComponentModel.ImmutableObject(true)]
    public sealed class EmptyCollection<T> : ReadOnlyCollection<T>, IReadOnlyCollection<T>
    {
        private static T[] _internal = new T[0];
        private static readonly EmptyCollection<T> _instance = new EmptyCollection<T>();

        private EmptyCollection()
            : base(_internal)
        {
        }

        public static EmptyCollection<T> Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}