// Needed for NET40

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace Theraot.Collections
{
    public sealed class ProxyObservable<T> : IProxyObservable<T>
    {
        private readonly Bucket<IObserver<T>> _observers;
        private int _index;

        public ProxyObservable()
        {
            _observers = new Bucket<IObserver<T>>();
            _index = -1;
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
            var index = Interlocked.Increment(ref _index);
            _observers.Insert(index, observer);
            return Disposable.Create(() => _observers.RemoveAt(index));
        }
    }
}