using System.Collections.Generic;

namespace Theraot.Collections.ThreadSafe
{
    internal interface ICore<T> : IEnumerable<T>
    {
        int Length
        {
            get;
        }

        void Do(int index, bool grow, DoAction<T> callback);
    }
}