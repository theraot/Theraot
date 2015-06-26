// Needed for NET30

using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public sealed class ConditionalExtendedEnumerable<T> : ExtendedEnumerableBase<T>, IEnumerable<T>
    {
        private readonly Func<bool> _enumerateAppend;
        private readonly Func<bool> _enumerateTarget;

        public ConditionalExtendedEnumerable(IEnumerable<T> target, IEnumerable<T> append, Func<bool> enumerateTarget, Func<bool> enumerateAppend)
            : base(target, append)
        {
            _enumerateTarget = Check.NotNullArgument(enumerateTarget, "enumerateTarget");
            _enumerateAppend = enumerateAppend ?? (null == append ? FuncHelper.GetFallacyFunc() : FuncHelper.GetTautologyFunc());
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
            if (_enumerateAppend.Invoke())
            {
                foreach (T item in Append)
                {
                    yield return item;
                }
            }
        }
    }
}