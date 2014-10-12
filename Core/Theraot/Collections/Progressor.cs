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
            int control = 0;

            Predicate<T> newFilter = item => Thread.VolatileRead(ref control) == 0;
            var buffer = new QueueBucket<T>();
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
                try
                {
                    Thread.VolatileWrite(ref control, 1);
                    if (buffer.TryTake(out value) || wrapped.TryTake(out value))
                    {
                        _proxy.OnNext(value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    Thread.VolatileWrite(ref control, 0);
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

            _tryTake = (out T value) =>
            {
                if (buffer.TryTake(out value))
                {
                    _proxy.OnNext(value);
                    return true;
                }
                else
                {
                    value = default(T);
                    return false;
                }
            };
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

            int control = 0;

            Predicate<TInput> newFilter = item => Thread.VolatileRead(ref control) == 0;
            var buffer = new QueueBucket<T>();
            wrapped.SubscribeAction
            (
                item =>
                {
                    if (newFilter(item))
                    {
                        buffer.Add(converter(item));
                    }
                }
            );
            var proxy = new ProxyObservable<T>();

            return new Progressor<T>
            (
                (out T value) =>
                {
                    Thread.VolatileWrite(ref control, 1);
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
                        Thread.VolatileWrite(ref control, 0);
                    }
                },
                proxy
            );
        }

        public static Progressor<T> CreatedFiltered(Progressor<T> wrapped, Predicate<T> filter)
        {
            Check.NotNullArgument(wrapped, "wrapped");
            Check.NotNullArgument(filter, "filter");

            int control = 0;

            Predicate<T> newFilter = item => Thread.VolatileRead(ref control) == 0 && filter(item);
            var buffer = new QueueBucket<T>();
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
            var proxy = new ProxyObservable<T>();

            return new Progressor<T>
            (
                (out T value) =>
                {
                    Thread.VolatileWrite(ref control, 1);
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
                        Thread.VolatileWrite(ref control, 0);
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

            int control = 0;

            Predicate<TInput> newFilter = item => Thread.VolatileRead(ref control) == 0 && filter(item);
            var buffer = new QueueBucket<T>();
            wrapped.SubscribeAction
            (
                item =>
                {
                    if (newFilter(item))
                    {
                        buffer.Add(converter(item));
                    }
                }
            );
            var proxy = new ProxyObservable<T>();

            return new Progressor<T>
            (
                (out T value) =>
                {
                    Thread.VolatileWrite(ref control, 1);
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
                        Thread.VolatileWrite(ref control, 0);
                    }
                },
                proxy
            );
        }

        public static Progressor<T> CreateDistinct(Progressor<T> wrapped)
        {
            Check.NotNullArgument(wrapped, "wrapped");

            int control = 0;

            var buffer = new HashBucket<T, bool>();
            Predicate<T> newFilter = item => Thread.VolatileRead(ref control) == 0 && !buffer.ContainsKey(item);
            wrapped.SubscribeAction
            (
                item =>
                {
                    if (newFilter(item))
                    {
                        buffer.Add(item, false);
                    }
                }
            );
            var proxy = new ProxyObservable<T>();

            return new Progressor<T>
            (
                (out T value) =>
                {
                    try
                    {
                        Thread.VolatileWrite(ref control, 0);
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
                        Thread.VolatileWrite(ref control, 1);
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