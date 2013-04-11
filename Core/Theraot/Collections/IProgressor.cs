using System;

namespace Theraot.Collections
{
    public interface IProgressor<T> : IObservable<T>
    {
        bool IsClosed
        {
            get;
        }

        void Close();

        bool TryTake(out T item);
    }
}