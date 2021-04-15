using System.Collections.Generic;

namespace Theraot.Collections
{
    internal interface IProgressive<T>
    {
        ICollection<T> Cache { get; }

        IEnumerable<T> Progress();
    }
}