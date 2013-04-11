using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public abstract class ExtendedEnumerableBase<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _extension;
        private readonly IEnumerable<T> _target;

        protected ExtendedEnumerableBase(IEnumerable<T> target, IEnumerable<T> extension)
        {
            _target = Check.NotNullArgument(target, "target");
            _extension = Check.NotNullArgument(extension, "extension");
        }

        protected IEnumerable<T> Extension
        {
            get
            {
                return _extension;
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