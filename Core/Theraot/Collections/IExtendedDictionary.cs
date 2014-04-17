using System.Collections.Generic;

namespace Theraot.Collections
{
    public interface IExtendedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IExtendedReadOnlyDictionary<TKey, TValue>
    {
        new IReadOnlyDictionary<TKey, TValue> AsReadOnly
        {
            get;
        }
    }
}