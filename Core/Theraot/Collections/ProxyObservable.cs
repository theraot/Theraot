using System;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class ProxyObservable<T> : IObservable<T>, IObserver<T>, IProxyObservable<T>
    {
        private readonly SafeSet<Needle<IObserver<T>>> _observers;

        public ProxyObservable()
        {
            _observers = new SafeSet<Needle<IObserver<T>>>();
        }

        public void OnCompleted()
        {
            foreach (var item in _observers)
            {
                item.Value.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var item in _observers)
            {
                item.Value.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            foreach (var item in _observers)
            {
                item.Value.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var needle = new Needle<IObserver<T>>(observer);
            _observers.AddNew(needle);
            return Disposable.Create(() => _observers.Remove(needle));
        }
    }
}