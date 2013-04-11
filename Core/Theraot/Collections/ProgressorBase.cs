using System;
using System.Collections.Generic;

using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections
{
    [Serializable]
    public abstract class ProgressorBase<T> : IObservable<T>, IProgressor<T>
    {
        private IProxyObservable<T> _observable;
        private TryTake<T> _tryTake;

        internal ProgressorBase(IProgressor<T> wrapped, IProxyObservable<T> observable)
        {
            var _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            _tryTake = _wrapped.TryTake;
            _observable = Check.NotNullArgument(observable, "observable");
        }

        internal ProgressorBase(IEnumerable<T> wrapped, IProxyObservable<T> observable)
        {
            var enumerator = Check.CheckArgument(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), arg => arg != null, "wrapped.GetEnumerator()");
            _tryTake = (out T value) =>
            {
                if (enumerator.MoveNext())
                {
                    value = enumerator.Current;
                    return true;
                }
                else
                {
                    enumerator.Dispose();
                    value = default(T);
                    return false;
                }
            };
            _observable = Check.NotNullArgument(observable, "observable");
        }

        internal ProgressorBase(TryTake<T> trytake, IProxyObservable<T> observable)
        {
            _tryTake = Check.NotNullArgument(trytake, "tryTake");
            _observable = Check.NotNullArgument(observable, "observable");
        }

        protected ProgressorBase(IEnumerable<T> wrapped)
        {
            var enumerator = Check.CheckArgument(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), arg => arg != null, "wrapped.GetEnumerator()");
            _tryTake = (out T value) =>
            {
                if (enumerator.MoveNext())
                {
                    value = enumerator.Current;
                    return true;
                }
                else
                {
                    enumerator.Dispose();
                    value = default(T);
                    return false;
                }
            };
            _observable = new ProxyObservable<T>();
        }

        protected ProgressorBase(TryTake<T> trytake)
        {
            _tryTake = Check.NotNullArgument(trytake, "tryTake");
            _observable = new ProxyObservable<T>();
        }

        public bool EndOfEnumeration
        {
            get
            {
                return _tryTake != null;
            }
        }

        public bool IsClosed
        {
            get
            {
                return _tryTake != null;
            }
        }

        protected IProxyObservable<T> Observable
        {
            get
            {
                return _observable;
            }
        }

        public void Close()
        {
            if (System.Threading.Interlocked.Exchange(ref _tryTake, null) != null)
            {
                OnClose();
                _observable = null;
                GC.SuppressFinalize(this);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var observable = _observable;
            if (observable != null)
            {
                return observable.Subscribe(observer);
            }
            else
            {
                return Disposable.Create();
            }
        }

        public bool TryTake(out T item)
        {
            if (_tryTake != null)
            {
                if (_tryTake.Invoke(out item))
                {
                    OnTake(item);
                    return true;
                }
                else
                {
                    Close();
                    return false;
                }
            }
            else
            {
                item = default(T);
                return false;
            }
        }

        protected virtual void OnClose()
        {
            //Empty
        }

        protected virtual void OnTake(T item)
        {
            //Empty
        }
    }
}