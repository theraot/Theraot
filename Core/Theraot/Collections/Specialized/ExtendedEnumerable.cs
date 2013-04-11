using System.Collections.Generic;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public sealed class ExtendedEnumerable<T> : ExtendedEnumerableBase<T>, IEnumerable<T>
    {
        public ExtendedEnumerable(IEnumerable<T> target, IEnumerable<T> extension)
            : base(target, extension)
        {
            //Empty
        }

        public override IEnumerator<T> GetEnumerator()
        {
            foreach (T item in Target)
            {
                yield return item;
            }
            foreach (T item in Extension)
            {
                yield return item;
            }
        }
    }
}