#if FAT

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public sealed class ExtendedFilteredEnumerable<T> : ExtendedEnumerableBase<T>, IEnumerable<T>
    {
        private readonly Predicate<T> _match;

        public ExtendedFilteredEnumerable(IEnumerable<T> target, IEnumerable<T> append, Predicate<T> match)
            : base(target, append)
        {
            _match = Check.NotNullArgument(match, "filter");
        }

        public override IEnumerator<T> GetEnumerator()
        {
            foreach (T item in Target)
            {
                if (_match.Invoke(item))
                {
                    yield return item;
                }
            }
            foreach (T item in Append)
            {
                if (_match.Invoke(item))
                {
                    yield return item;
                }
            }
        }
    }
}

#endif