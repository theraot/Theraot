using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class Progressor<T> : IObservable<T>
    {
        private ProxyObservable<T> _proxy;
        private TryTake<T> _tryTake;

        public Progressor(Progressor<T> wrapped)
        {
            Thread control = null;

            Predicate<T> newFilter = item => ThreadingHelper.VolatileRead(ref control) != Thread.CurrentThread;
            var buffer = new QueueBucket<T>();
            wrapped.SubscribeAction(item => { if (newFilter(item)) buffer.Add(item); });
            _proxy = new ProxyObservable<T>();
            wrapped.Subscribe(_proxy);

            _tryTake = (out T value) =>
            {
                try
                {
                    ThreadingHelper.VolatileWrite(ref control, Thread.CurrentThread);
                    if (buffer.TryTake(out value) || wrapped.TryTake(out value))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    ThreadingHelper.VolatileWrite(ref control, null);
                }
            };
        }

        public Progressor(IEnumerable<T> wrapped)
        {
            var enumerator = Check.CheckArgument(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), arg => arg != null, "wrapped.GetEnumerator()");
            _proxy = new ProxyObservable<T>();
            _tryTake = (out T value) =>
            {
                if (enumerator.MoveNext())
                {
                    value = enumerator.Current;
                    _proxy.OnNext(value);
                    return true;
                }
                else
                {
                    enumerator.Dispose();
                    value = default(T);
                    return false;
                }
            };
        }

        public Progressor(TryTake<T> tryTake)
        {
            Check.NotNullArgument(tryTake, "tryTake");
            _proxy = new ProxyObservable<T>();
            _tryTake = (out T value) =>
            {
                if (tryTake(out value))
                {
                    _proxy.OnNext(value);
                    return true;
                }
                else
                {
                    return false;
                }
            };
        }

        public Progressor(IObservable<T> wrapped)
        {
            var buffer = new QueueBucket<T>();
            wrapped.SubscribeAction(buffer.Add);
            _proxy = new ProxyObservable<T>();
            wrapped.Subscribe(_proxy);
            _tryTake = buffer.TryTake;
        }

        private Progressor(TryTake<T> tryTake, ProxyObservable<T> proxy)
        {
            _proxy = proxy;
            _tryTake = tryTake;
        }

        public bool IsClosed
        {
            get
            {
                return _tryTake != null;
            }
        }

        public static Progressor<T> CreateConverted<TInput>(Progressor<TInput> wrapped, Converter<TInput, T> converter)
        {
            Check.NotNullArgument(wrapped, "wrapped");
            Check.NotNullArgument(converter, "converter");

            Thread control = null;

            Predicate<TInput> newFilter = item => ThreadingHelper.VolatileRead(ref control) != Thread.CurrentThread;
            var buffer = new QueueBucket<T>();
            wrapped.SubscribeAction(item => { if (newFilter(item)) buffer.Add(converter(item)); });
            var proxy = new ProxyObservable<T>();
            wrapped.SubscribeConverted(proxy, converter);

            return new Progressor<T>
            (
                (out T value) =>
                {
                    ThreadingHelper.VolatileWrite(ref control, Thread.CurrentThread);
                    try
                    {
                        TInput item;
                        if (buffer.TryTake(out value))
                        {
                            return true;
                        }
                        else if (wrapped.TryTake(out item))
                        {
                            value = converter(item);
                            return true;
                        }
                        value = default(T);
                        return false;
                    }
                    finally
                    {
                        ThreadingHelper.VolatileWrite(ref control, null);
                    }
                },
                proxy
            );
        }

        public static Progressor<T> CreatedFiltered(Progressor<T> wrapped, Predicate<T> filter)
        {
            Check.NotNullArgument(wrapped, "wrapped");
            Check.NotNullArgument(filter, "filter");

            Thread control = null;

            Predicate<T> newFilter = item => ThreadingHelper.VolatileRead(ref control) != Thread.CurrentThread && filter(item);
            var buffer = new QueueBucket<T>();
            wrapped.SubscribeAction(item => { if (newFilter(item)) buffer.Add(item); });
            var proxy = new ProxyObservable<T>();
            wrapped.SubscribeFiltered(proxy, filter);

            return new Progressor<T>
            (
                (out T value) =>
                {
                    ThreadingHelper.VolatileWrite(ref control, Thread.CurrentThread);
                    try
                    {
                    again:
                        if (buffer.TryTake(out value))
                        {
                            return true;
                        }
                        else if (wrapped.TryTake(out value))
                        {
                            if (filter(value))
                            {
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
                        ThreadingHelper.VolatileWrite(ref control, null);
                    }
                },
                proxy
            );
        }

        public static Progressor<T> CreatedFilteredConverted<TInput>(Progressor<TInput> wrapped, Predicate<TInput> filter, Converter<TInput, T> converter)
        {
            Check.NotNullArgument(wrapped, "wrapped");
            Check.NotNullArgument(filter, "filter");
            Check.NotNullArgument(converter, "converter");

            Thread control = null;

            Predicate<TInput> newFilter = item => ThreadingHelper.VolatileRead(ref control) != Thread.CurrentThread && filter(item);
            var buffer = new QueueBucket<T>();
            wrapped.SubscribeAction(item => { if (newFilter(item)) buffer.Add(converter(item)); });
            var proxy = new ProxyObservable<T>();
            wrapped.SubscribeFilteredConverted(proxy, filter, converter);

            return new Progressor<T>
            (
                (out T value) =>
                {
                    ThreadingHelper.VolatileWrite(ref control, Thread.CurrentThread);
                    try
                    {
                        TInput item;
                    again:
                        if (buffer.TryTake(out value))
                        {
                            return true;
                        }
                        else if (wrapped.TryTake(out item))
                        {
                            if (filter(item))
                            {
                                value = converter(item);
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
                        ThreadingHelper.VolatileWrite(ref control, null);
                    }
                },
                proxy
            );
        }

        public static Progressor<T> CreateDistinct(Progressor<T> wrapped)
        {
            Thread control = null;

            var buffer = new HashBucket<T, bool>();
            Predicate<T> newFilter = item => ThreadingHelper.VolatileRead(ref control) != Thread.CurrentThread && !buffer.ContainsKey(item);
            wrapped.SubscribeAction(item => { if (newFilter(item)) buffer.Add(item, false); });
            var proxy = new ProxyObservable<T>();
            wrapped.SubscribeFiltered
            (
                proxy,
                item =>
                {
                    bool seen;
                    return !buffer.TryGetValue(item, out seen) || !seen;
                }
            );
            return new Progressor<T>
            (
                (out T value) =>
                {
                    try
                    {
                        ThreadingHelper.VolatileWrite(ref control, Thread.CurrentThread);
                    again:
                        foreach (var item in buffer.Where(item => !item.Value))
                        {
                            value = item.Key;
                            buffer.Set(value, true);
                            return true;
                        }
                        if (wrapped.TryTake(out value))
                        {
                            bool seen;
                            if (!buffer.TryGetValue(value, out seen) || !seen)
                            {
                                buffer.Set(value, true);
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
                        ThreadingHelper.VolatileWrite(ref control, null);
                    }
                },
                proxy
            );
        }

        public IEnumerable<T> AsEnumerable()
        {
            T item;
            while (_tryTake(out item))
            {
                yield return item;
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
            else
            {
                return Disposable.Create(ActionHelper.GetNoopAction());
            }
        }

        public bool TryTake(out T item)
        {
            if (_tryTake != null)
            {
                if (_tryTake.Invoke(out item))
                {
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

        public IEnumerable<T> While(Predicate<T> predicate)
        {
            var _condition = Check.NotNullArgument(predicate, "condition");
            T item;
            while (_tryTake(out item))
            {
                if (_condition(item))
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        public IEnumerable<T> While(Func<bool> condition)
        {
            var _condition = Check.NotNullArgument(condition, "condition");
            T item;
            while (_tryTake(out item))
            {
                if (_condition())
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