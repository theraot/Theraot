using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public sealed class ConditionalExtendedEnumerable<T> : ExtendedEnumerableBase<T>, IEnumerable<T>
    {
        private readonly Func<bool> _enumerateExtension;
        private readonly Func<bool> _enumerateTarget;

        public ConditionalExtendedEnumerable(IEnumerable<T> target, IEnumerable<T> extension, Func<bool> enumerateTarget, Func<bool> enumerateExtension)
            : base(target, extension)
        {
            _enumerateTarget = Check.NotNullArgument(enumerateTarget, "enumerateTarget");
            _enumerateExtension = Check.NotNullArgument(enumerateExtension, "enumerateExtension");
        }

        public override IEnumerator<T> GetEnumerator()
        {
            if (_enumerateTarget.Invoke())
            {
                foreach (T item in Target)
                {
                    yield return item;
                }
            }
            if (_enumerateExtension.Invoke())
            {
                foreach (T item in Extension)
                {
                    yield return item;
                }
            }
        }
    }
}