// Needed for NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections
{
    public sealed class Progressor<T> : IObservable<T>
    {
        private bool _done;
        private ProxyObservable<T> _proxy;
        private TryTake<T> _tryTake;

        public Progressor(Progressor<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }

            var control = 0;

            Predicate<T> newFilter = item => Volatile.Read(ref control) == 0;
            var buffer = new SafeQueue<T>();
            wrapped.SubscribeAction
            (
                item =>
                {
                    if (newFilter(item))
                    {
                        buffer.Add(item);
                    }
                }
            );
            _proxy = new ProxyObservable<T>();

            _tryTake = (out T value) =>
            {
                Interlocked.Increment(ref control);
                try
                {
                    if (buffer.TryTake(out value) || wrapped.TryTake(out value))
                    {
                        _proxy.OnNext(value);
                        return true;
                    }
                    else
                    {
                        _done = wrapped._done;
                        return false;
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref control);
                }
            };
        }

        public Progressor(IEnumerable<T> preface, Progressor<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            if (preface == null)
            {
                throw new ArgumentNullException("preface");
            }
            var enumerator = preface.GetEnumerator();
            if (enumerator == null)
            {
                throw new ArgumentException("preface.GetEnumerator()");
            }

            var control = 0;
            var guard = 0;

            Predicate<T> newFilter = item => Volatile.Read(ref control) == 0;
            var buffer = new SafeQueue<T>();
            wrapped.SubscribeAction
            (
                item =>
                {
                    if (newFilter(item))
                    {
                        buffer.Add(item);
                    }
                }
            );
            _proxy = new ProxyObservable<T>();

            TryTake<T> tryTakeReplacement = (out T value) =>
            {
                Interlocked.Increment(ref control);
                try
                {
                    if (buffer.TryTake(out value) || wrapped.TryTake(out value))
                    {
                        _proxy.OnNext(value);
                        return true;
                    }
                    else
                    {
                        _done = wrapped._done;
                        return false;
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref control);
                }
            };

            _tryTake = (out T value) =>
            {
                value = default(T);
                if (Volatile.Read(ref guard) == 0)
                {
                    bool result;
                    // We need a lock, there is no way around it. IEnumerator is just awful. Use another overload if possible.
                    lock (enumerator)
                    {
                        result = enumerator.MoveNext();
                        if (result)
                        {
                            value = enumerator.Current;
                        }
                    }
                    if (result)
                    {
                        _proxy.OnNext(value);
                        return true;
                    }
                    enumerator.Dispose();
                    Interlocked.CompareExchange(ref guard, 1, 0);
                }
                if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
                {
                    _tryTake = tryTakeReplacement;
                    Volatile.Write(ref guard, 3);
                }
                else
                {
                    ThreadingHelper.SpinWaitUntil(ref guard, 3);
                }
                var tryTake = _tryTake;
                return tryTake(out value);
            };
        }

        public Progressor(T[] wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }

            var guard = 0;
            var index = -1;

            _proxy = new ProxyObservable<T>();

            TryTake<T> tryTakeReplacement = (out T value) =>
            {
                value = default(T);
                return false;
            };

            _tryTake = (out T value) =>
            {
                value = default(T);
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
                    _tryTake = tryTakeReplacement;
                }
                return false;
            };
        }

        public Progressor(T[] preface, Progressor<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            if (preface == null)
            {
                throw new ArgumentNullException("preface");
            }

            var control = 0;
            var guard = 0;
            var index = -1;

            Predicate<T> newFilter = item => Volatile.Read(ref control) == 0;
            var buffer = new SafeQueue<T>();
            wrapped.SubscribeAction
            (
                item =>
                {
                    if (newFilter(item))
                    {
                        buffer.Add(item);
                    }
                }
            );
            _proxy = new ProxyObservable<T>();

            TryTake<T> tryTakeReplacement = (out T value) =>
            {
                Interlocked.Increment(ref control);
                try
                {
                    if (buffer.TryTake(out value) || wrapped.TryTake(out value))
                    {
                        _proxy.OnNext(value);
                        return true;
                    }
                    else
                    {
                        _done = wrapped._done;
                        return false;
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref control);
                }
            };

            _tryTake = (out T value) =>
            {
                if (Volatile.Read(ref guard) == 0)
                {
                    var currentIndex = Interlocked.Increment(ref index);
                    if (currentIndex < preface.Length)
                    {
                        value = preface[currentIndex];
                        _proxy.OnNext(value);
                        return true;
                    }
                    Interlocked.CompareExchange(ref guard, 1, 0);
                }
                if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
                {
                    _tryTake = tryTakeReplacement;
                    Volatile.Write(ref guard, 3);
                }
                else
                {
                    ThreadingHelper.SpinWaitUntil(ref guard, 3);
                }
                var tryTake = _tryTake;
                return tryTake(out value);
            };
        }

        public Progressor(IEnumerable<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            var enumerator = wrapped.GetEnumerator();
            if (enumerator == null)
            {
                throw new ArgumentException("wrapped.GetEnumerator()");
            }

            var guard = 0;

            _proxy = new ProxyObservable<T>();

            TryTake<T> tryTakeReplacement = (out T value) =>
            {
                value = default(T);
                return false;
            };

            _tryTake = (out T value) =>
            {
                value = default(T);
                if (Volatile.Read(ref guard) == 0)
                {
                    bool result;
                    // We need a lock, there is no way around it. IEnumerator is just awful. Use another overload if possible.
                    lock (enumerator)
                    {
                        result = enumerator.MoveNext();
                        if (result)
                        {
                            value = enumerator.Current;
                        }
                    }
                    if (result)
                    {
                        _proxy.OnNext(value);
                        return true;
                    }
                    enumerator.Dispose();
                    Interlocked.CompareExchange(ref guard, 1, 0);
                }
                if (Interlocked.CompareExchange(ref guard, 2, 1) == 1)
                {
                    _tryTake = tryTakeReplacement;
                }
                return false;
            };
        }

        public Progressor(TryTake<T> tryTake, bool doneOnFalse)
        {
            if (tryTake == null)
            {
                throw new ArgumentNullException("tryTake");
            }
            _proxy = new ProxyObservable<T>();
            _tryTake = GetTryTake(tryTake, doneOnFalse, this);
        }

        public Progressor(TryTake<T> tryTake, Func<bool> isDone)
        {
            if (tryTake == null)
            {
                throw new ArgumentNullException("tryTake");
            }
            if (isDone == null)
            {
                throw new ArgumentNullException("isDone");
            }
            _proxy = new ProxyObservable<T>();
            _tryTake = GetTryTake(tryTake, isDone, this);
        }

        public Progressor(IObservable<T> wrapped)
        {
            var buffer = new SafeQueue<T>();
            wrapped.Subscribe
                (
                    new CustomObserver<T>
                    (
                        () => _done = true,
                        exception => _done = true,
                        buffer.Add
                    )
                );
            _proxy = new ProxyObservable<T>();

            _tryTake = (out T value) =>
            {
                if (buffer.TryTake(out value))
                {
                    _proxy.OnNext(value);
                    return true;
                }
                value = default(T);
                return false;
            };
        }

        private Progressor(TryTake<T> tryTake, ProxyObservable<T> proxy)
        {
            _proxy = proxy;
            _tryTake = tryTake;
        }

        public bool IsClosed
        {
            get { return _tryTake == null; }
        }

        public static Progressor<T> CreateConverted<TInput>(Progressor<TInput> wrapped, Func<TInput, T> converter)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            var control = 0;

            Predicate<TInput> newFilter = item => Volatile.Read(ref control) == 0;
            var buffer = new SafeQueue<T>();
            var proxy = new ProxyObservable<T>();

            var result = new Progressor<T>(
                (out T value) =>
                {
                    Interlocked.Increment(ref control);
                    try
                    {
                        TInput item;
                        if (buffer.TryTake(out value))
                        {
                            proxy.OnNext(value);
                            return true;
                        }
                        else if (wrapped.TryTake(out item))
                        {
                            value = converter(item);
                            proxy.OnNext(value);
                            return true;
                        }
                        value = default(T);
                        return false;
                    }
                    finally
                    {
                        Interlocked.Decrement(ref control);
                    }
                },
                proxy
            );
            wrapped.Subscribe
            (
                new CustomObserver<TInput>
                (
                    () => result._done = true,
                    exception => result._done = true,
                    item =>
                    {
                        if (newFilter(item))
                        {
                            buffer.Add(converter(item));
                        }
                    }
                )
            );
            return result;
        }

        public static Progressor<T> CreatedFiltered(Progressor<T> wrapped, Predicate<T> filter)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            var control = 0;

            Predicate<T> newFilter = item => Volatile.Read(ref control) == 0 && filter(item);
            var buffer = new SafeQueue<T>();
            var proxy = new ProxyObservable<T>();

            var result = new Progressor<T>(
                (out T value) =>
                {
                    Volatile.Write(ref control, 1);
                    try
                    {
                        again:
                        if (buffer.TryTake(out value))
                        {
                            proxy.OnNext(value);
                            return true;
                        }
                        else if (wrapped.TryTake(out value))
                        {
                            if (filter(value))
                            {
                                proxy.OnNext(value);
                                return true;
                            }
                            else
                            {
                                goto again;
                            }
                        }
                        value = default(T);
                        return false;
                    }
                    finally
                    {
                        Interlocked.Decrement(ref control);
                    }
                },
                proxy
            );
            wrapped.Subscribe
            (
                new CustomObserver<T>
                (
                    () => result._done = true,
                    exception => result._done = true,
                    item =>
                    {
                        if (newFilter(item))
                        {
                            buffer.Add(item);
                        }
                    }
                )
            );
            return result;
        }

        public static Progressor<T> CreatedFilteredConverted<TInput>(Progressor<TInput> wrapped, Predicate<TInput> filter, Func<TInput, T> converter)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            var control = 0;

            Predicate<TInput> newFilter = item => Volatile.Read(ref control) == 0 && filter(item);
            var buffer = new SafeQueue<T>();
            var proxy = new ProxyObservable<T>();

            var result = new Progressor<T>(
                (out T value) =>
                {
                    Interlocked.Increment(ref control);
                    try
                    {
                        TInput item;
                        again:
                        if (buffer.TryTake(out value))
                        {
                            proxy.OnNext(value);
                            return true;
                        }
                        else if (wrapped.TryTake(out item))
                        {
                            if (filter(item))
                            {
                                value = converter(item);
                                proxy.OnNext(value);
                                return true;
                            }
                            else
                            {
                                goto again;
                            }
                        }
                        value = default(T);
                        return false;
                    }
                    finally
                    {
                        Interlocked.Decrement(ref control);
                    }
                },
                proxy
            );
            wrapped.Subscribe
            (
                new CustomObserver<TInput>
                (
                    () => result._done = true,
                    exception => result._done = true,
                    item =>
                    {
                        if (newFilter(item))
                        {
                            buffer.Add(converter(item));
                        }
                    }
                )
            );
            return result;
        }

        public static Progressor<T> CreateDistinct(Progressor<T> wrapped)
        {
            if (wrapped == null)
            {
                throw new ArgumentNullException("wrapped");
            }

            var control = 0;

            var buffer = new SafeDictionary<T, bool>();
            Predicate<T> newFilter = item => Volatile.Read(ref control) == 0;
            var proxy = new ProxyObservable<T>();

            var result = new Progressor<T>(
                (out T value) =>
                {
                    Interlocked.Increment(ref control);
                    try
                    {
                        again:
                        foreach (var item in buffer.Where(item => !item.Value))
                        {
                            value = item.Key;
                            buffer.Set(value, true);
                            proxy.OnNext(value);
                            return true;
                        }
                        if (wrapped.TryTake(out value))
                        {
                            bool seen;
                            if (!buffer.TryGetValue(value, out seen) || !seen)
                            {
                                buffer.Set(value, true);
                                proxy.OnNext(value);
                                return true;
                            }
                            else
                            {
                                goto again;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref control);
                    }
                },
                proxy
            );
            wrapped.Subscribe
            (
                new CustomObserver<T>
                (
                    () => result._done = true,
                    exception => result._done = true,
                    item =>
                    {
                        if (newFilter(item))
                        {
                            buffer.TryAdd(item, false);
                        }
                    }
                )
            );
            return result;
        }

        public IEnumerable<T> AsEnumerable()
        {
            // After enumerating - the consumer of this method must check if the Progressor is closed.
            while (true)
            {
                T item;
                var tryTake = _tryTake;
                if (tryTake(out item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        public void Close()
        {
            _tryTake = null;
            _proxy.OnCompleted();
            _proxy = null;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_proxy != null)
            {
                return _proxy.Subscribe(observer);
            }
            return Disposable.Create(ActionHelper.GetNoopAction());
        }

        public bool TryTake(out T item)
        {
            if (_tryTake != null)
            {
                if (_tryTake.Invoke(out item))
                {
                    return true;
                }
                if (_done)
                {
                    Close();
                }
                return false;
            }
            item = default(T);
            return false;
        }

        public IEnumerable<T> While(Predicate<T> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            return WhileExtracted(condition);
        }

        public IEnumerable<T> While(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            return WhileExtracted(condition);
        }

        private static TryTake<T> GetTryTake(TryTake<T> tryTake, bool doneOnFalse, Progressor<T> that)
        {
            var tryTakeCopy = tryTake;
            return (out T value) =>
            {
                if (tryTakeCopy(out value))
                {
                    that._proxy.OnNext(value);
                    return true;
                }
                that._done = doneOnFalse;
                return false;
            };
        }

        private static TryTake<T> GetTryTake(TryTake<T> tryTake, Func<bool> isDone, Progressor<T> that)
        {
            var tryTakeCopy = tryTake;
            return (out T value) =>
            {
                if (tryTakeCopy(out value))
                {
                    that._proxy.OnNext(value);
                    return true;
                }
                that._done = new ValueFuncClosure<bool>(isDone).InvokeReturn();
                return false;
            };
        }

        private IEnumerable<T> WhileExtracted(Predicate<T> condition)
        {
            while (true)
            {
                T item;
                var tryTake = _tryTake;
                if (tryTake(out item) && condition(item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        private IEnumerable<T> WhileExtracted(Func<bool> condition)
        {
            while (true)
            {
                T item;
                var tryTake = _tryTake;
                if (tryTake(out item) && condition())
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