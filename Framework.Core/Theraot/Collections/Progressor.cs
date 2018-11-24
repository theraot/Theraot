// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections
{
    public sealed class Progressor<T> : IObservable<T>, IEnumerable<T>
    {
        private ProxyObservable<T> _proxy;
        private TryTake<T> _tryTake;

        public Progressor(T[] wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped));
            }

            var guard = 0;
            var index = -1;

            _proxy = new ProxyObservable<T>();

            _tryTake = (out T value) =>
            {
                value = default;
                if (Volatile.Read(ref guard) == 0)
                {
                    var currentIndex = Interlocked.Increment(ref index);
                    if (currentIndex < wrapped.Length)
                    {
                        value = wrapped[currentIndex];
                        _proxy.OnNext(value);
                        return true;
                    }
                    Interlocked.CompareExchange(ref guard, 1, 0);
                }
                if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
                {
                    Close();
                }
                return false;
            };
        }

        public Progressor(IEnumerable<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException(nameof(wrapped));
            }
            var enumerator = wrapped.GetEnumerator();
            if (enumerator == null)
            {
                throw new ArgumentException("wrapped.GetEnumerator()");
            }

            var guard = 0;

            _proxy = new ProxyObservable<T>();

            _tryTake = (out T value) =>
            {
                value = default;
                if (Volatile.Read(ref guard) == 0)
                {
                    bool result;
                    // We need a lock, there is no way around it. IEnumerator is just awful. Use another overload if possible.
                    lock (enumerator)
                    {
                        if (Volatile.Read(ref guard) == 0)
                        {
                            result = enumerator.MoveNext();
                            if (result)
                            {
                                value = enumerator.Current;
                            }
                            else
                            {
                                Volatile.Write(ref guard, 1);
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    if (result)
                    {
                        _proxy.OnNext(value);
                        return true;
                    }
                }
                if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
                {
                    enumerator.Dispose();
                    Close();
                }
                return false;
            };
        }

        public Progressor(TryTake<T> tryTake, Func<bool> isDone)
        {
            if (tryTake == null)
            {
                throw new ArgumentNullException(nameof(tryTake));
            }
            if (isDone == null)
            {
                throw new ArgumentNullException(nameof(isDone));
            }
            _proxy = new ProxyObservable<T>();
            _tryTake = (out T value) =>
            {
                if (tryTake(out value))
                {
                    _proxy.OnNext(value);
                    return true;
                }
                if (isDone())
                {
                    Close();
                }
                return false;
            };
        }

        public Progressor(IObservable<T> wrapped)
        {
            var buffer = new SafeQueue<T>();
            var semaphore = new SemaphoreSlim(0);
            var source = new CancellationTokenSource();
            wrapped.Subscribe
                (
                    new CustomObserver<T>
                    (
                        onCompleted: Done,
                        onError: exception => Done(),
                        onNext: item =>
                        {
                            semaphore.Release();
                            buffer.Add(item);
                        }
                    )
                );
            _proxy = new ProxyObservable<T>();

            _tryTake = (out T value) =>
            {
                if (!source.IsCancellationRequested)
                {
                    try
                    {
                        semaphore.Wait(source.Token);
                        if (buffer.TryTake(out value))
                        {
                            _proxy.OnNext(value);
                            return true;
                        }
                    }
                    catch (OperationCanceledException exception)
                    {
                        GC.KeepAlive(exception);
                    }
                }
                value = default;
                return false;
            };

            void Done()
            {
                source.Cancel();
            }
        }

        private Progressor(TryTake<T> tryTake, ProxyObservable<T> proxy)
        {
            _proxy = proxy;
            _tryTake = tryTake;
        }

        public bool IsClosed
        {
            get { return Volatile.Read(ref _tryTake) == null; }
        }

        public void Close()
        {
            Volatile.Write(ref _tryTake, null);
            var proxy = Interlocked.Exchange(ref _proxy, null);
            if (proxy != null)
            {
                proxy.OnCompleted();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (Volatile.Read(ref _tryTake)(out T item))
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var proxy = Volatile.Read(ref _proxy);
            if (proxy != null)
            {
                return proxy.Subscribe(observer);
            }
            observer.OnCompleted();
            return Disposable.Create(ActionHelper.GetNoopAction());
        }

        public bool TryTake(out T item)
        {
            var tryTake = Volatile.Read(ref _tryTake);
            if (tryTake != null)
            {
                return tryTake.Invoke(out item);
            }
            item = default;
            return false;
        }

        public IEnumerable<T> While(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            return WhileExtracted();

            IEnumerable<T> WhileExtracted()
            {
                while (true)
                {
                    var tryTake = Volatile.Read(ref _tryTake);
                    if (tryTake != null && tryTake(out T item) && condition(item))
                    {
                        yield return item;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public IEnumerable<T> While(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            return WhileExtracted();

            IEnumerable<T> WhileExtracted()
            {
                while (true)
                {
                    var tryTake = Volatile.Read(ref _tryTake);
                    if (tryTake != null && tryTake(out T item) && condition())
                    {
                        yield return item;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}