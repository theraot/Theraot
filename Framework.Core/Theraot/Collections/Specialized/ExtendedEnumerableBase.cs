// Needed for NET30

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public abstract class ExtendedEnumerableBase<T> : IEnumerable<T>
    {
        protected ExtendedEnumerableBase(IEnumerable<T> target, IEnumerable<T> append)
        {
            Target = target ?? ArrayReservoir<T>.EmptyArray;
            Append = append ?? ArrayReservoir<T>.EmptyArray;
        }

        protected IEnumerable<T> Append { get; }

        protected IEnumerable<T> Target { get; }

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}