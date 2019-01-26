// Needed for NET35 (BigInteger)

using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Collections.Specialized
{
    [DebuggerNonUserCode]
    public sealed class ExtendedEnumerable<T> : ExtendedEnumerableBase<T>
    {
        public ExtendedEnumerable(IEnumerable<T> target, IEnumerable<T> append)
            : base(target, append)
        {
            // Empty
        }

        public override IEnumerator<T> GetEnumerator()
        {
            foreach (var item in Target)
            {
                yield return item;
            }

            foreach (var item in Append)
            {
                yield return item;
            }
        }
    }
}