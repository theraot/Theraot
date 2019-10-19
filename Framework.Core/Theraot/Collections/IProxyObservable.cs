// Needed for NET40

using System;

namespace Theraot.Collections
{
    public interface IProxyObservable<T> : IObservable<T>, IObserver<T>
    {
    }
}