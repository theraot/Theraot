using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public abstract class ExtendedEnumerableBase<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _append;
        private readonly IEnumerable<T> _target;

        protected ExtendedEnumerableBase(IEnumerable<T> target, IEnumerable<T> append)
        {
            _target = Check.NotNullArgument(target, "target");
            _append = append ?? EmptySet<T>.Instance;
        }

        protected IEnumerable<T> Append
        {
            get
            {
                return _append;
            }
        }

        protected IEnumerable<T> Target
        {
            get
            {
                return _target;
            }
        }

        public abstract IEnumerator<T> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}