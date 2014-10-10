using System.Collections.Generic;

namespace Theraot.Collections
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "By Design")]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public interface IExtendedReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IExtendedCollection<KeyValuePair<TKey, TValue>>
    {
        // Intended to collide with IDictionary<TKey, TValue>
        new IReadOnlyCollection<TKey> Keys
        {
            get;
        }

        new IReadOnlyCollection<TValue> Values
        {
            get;
        }
    }
}