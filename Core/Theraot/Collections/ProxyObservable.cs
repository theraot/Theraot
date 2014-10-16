using System;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class ProxyObservable<T> : IObservable<T>, IObserver<T>, IProxyObservable<T>
    {
        // Here we use SetBucket because it will allow iterating while it is being modified
        private readonly SetBucket<IObserver<T>> _observers;

        public ProxyObservable()
        {
            _observers = new SetBucket<IObserver<T>>();
        }

        public void OnCompleted()
        {
            foreach (var item in _observers)
            {
                item.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var item in _observers)
            {
                item.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            foreach (var item in _observers)
            {
                item.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return Disposable.Create(() => _observers.Remove(observer));
        }
    }
}