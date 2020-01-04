// Needed for NET40

#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable IDE0067 // Disposable object is never disposed
// ReSharper disable ImplicitlyCapturedClosure

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections
{
    [DebuggerTypeProxy(typeof(ProgressorProxy))]
    public sealed class Progressor<T> : IObservable<T>, IEnumerable<T>, IClosable
    {
        private ProxyObservable<T>? _proxy;

        private TryTake<T>? _tryTake;

        private Progressor(ProxyObservable<T> proxy, TryTake<T> tryTake)
        {
            _proxy = proxy;
            _tryTake = tryTake;
        }

        public bool IsClosed => Volatile.Read(ref _tryTake) == null;

        public static Progressor<T> CreateFromArray(T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            var index = -1;
            var proxy = new ProxyObservable<T>();

            return new Progressor<T>(proxy, (out T value) => Take(out value));

            bool Take(out T value)
            {
                value = default!;
                var currentIndex = Interlocked.Increment(ref index);
                if (currentIndex >= array.Length)
                {
                    return false;
                }

                value = array[currentIndex];
                return true;
            }
        }

        public static Progressor<T> CreateFromIEnumerable(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            switch (enumerable)
            {
                case T[] array:
                    return CreateFromArray(array);

                case IList<T> list:
                    return CreateFromIList(list);

                case IReadOnlyList<T> readOnlyList:
                    return CreateFromIReadOnlyList(readOnlyList);

                default:
                    break;
            }

            var enumerator = enumerable.GetEnumerator();
            return CreateFromIEnumerableExtracted(enumerator);
        }

        public static Progressor<T> CreateFromIList(IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var index = -1;
            var proxy = new ProxyObservable<T>();

            return new Progressor<T>(proxy, (out T value) => Take(out value));

            bool Take(out T value)
            {
                value = default!;
                var currentIndex = Interlocked.Increment(ref index);
                if (currentIndex >= list.Count)
                {
                    return false;
                }

                value = list[currentIndex];
                return true;
            }
        }

        public static Progressor<T> CreateFromIObservable(IObservable<T> observable, Action? exhaustedCallback = null, CancellationToken token = default)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (exhaustedCallback == null)
            {
                exhaustedCallback = ActionHelper.GetNoopAction();
            }

            var buffer = new ThreadSafeQueue<T>();
            var semaphore = new SemaphoreSlim(0);
            var source = new CancellationTokenSource();

            // ReSharper disable once RedundantExplicitArrayCreation
            var subscription = new IDisposable?[]
            {
                observable.Subscribe
                (
                    new CustomObserver<T>
                    (
                        source.Cancel,
                        OnError,
                        OnNext
                    )
                )
            };
            var proxy = new ProxyObservable<T>();
            var tryTake = new TryTake<T>[]
            {
                (out T val) =>
                {
                    val = default!;
                    return false;
                }
            };
            tryTake[0] = TakeInitial;

            return new Progressor<T>(proxy, Take);

            void OnError(Exception _)
            {
                source.Cancel();
            }

            void OnNext(T item)
            {
                buffer.Add(item);
                semaphore.Release();
            }

            bool TakeInitial(out T value)
            {
                if (source.IsCancellationRequested || token.IsCancellationRequested)
                {
                    if (Interlocked.CompareExchange(ref tryTake[0], (out T value1) => TakeReplacement(out value1), tryTake[0]) == tryTake[0])
                    {
                        Interlocked.Exchange(ref subscription[0], null)?.Dispose();
                        semaphore.Dispose();
                        source.Dispose();
                    }
                }
                else
                {
                    if (exhaustedCallback != null)
                    {
                        var spinWait = new SpinWait();
                        while
                        (
                            semaphore.CurrentCount == 0
                            && !source.IsCancellationRequested
                            && !token.IsCancellationRequested
                        )
                        {
                            exhaustedCallback();
                            spinWait.SpinOnce();
                        }
                    }
                }

                if (source.IsCancellationRequested || token.IsCancellationRequested)
                {
                    return TakeReplacement(out value);
                }

                try
                {
                    semaphore.Wait(source.Token);
                }
                catch (OperationCanceledException exception)
                {
                    _ = exception;
                }

                return TakeReplacement(out value);
            }

            bool Take(out T value)
            {
                return tryTake[0](out value);
            }

            bool TakeReplacement(out T value)
            {
                if (buffer.TryTake(out value))
                {
                    return true;
                }

                value = default!;
                return false;
            }
        }

        public static Progressor<T> CreateFromIReadOnlyList(IReadOnlyList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var index = -1;
            var proxy = new ProxyObservable<T>();

            return new Progressor<T>(proxy, (out T value) => Take(out value));

            bool Take(out T value)
            {
                value = default!;
                var currentIndex = Interlocked.Increment(ref index);
                if (currentIndex >= list.Count)
                {
                    return false;
                }

                value = list[currentIndex];
                return true;
            }
        }

        public void Close()
        {
            Volatile.Write(ref _tryTake, null);
            var proxy = Interlocked.Exchange(ref _proxy, null);
            proxy?.OnCompleted();
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (TryTake(out var item))
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

            observer?.OnCompleted();
            return NoOpDisposable.Instance;
        }

        public bool TryTake(out T item)
        {
            var tryTake = Volatile.Read(ref _tryTake);
            var proxy = Volatile.Read(ref _proxy);
            if (tryTake != null && proxy != null)
            {
                if (tryTake.Invoke(out item))
                {
                    proxy.OnNext(item);
                    return true;
                }

                Close();
            }

            item = default!;
            return false;
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
                    if (tryTake != null && tryTake(out var item) && condition())
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

        private static Progressor<T> CreateFromIEnumerableExtracted(IEnumerator<T> enumerator)
        {
            var proxy = new ProxyObservable<T>();
            // ReSharper disable once RedundantExplicitArrayCreation
            var enumeratorBox = new IEnumerator<T>?[] { enumerator };
            return new Progressor<T>(proxy, (out T value) => Take(out value));

            bool Take(out T value)
            {
                // We need a lock, there is no way around it. IEnumerator is just awful. Use another overload if possible.
                var enumeratorCopy = Volatile.Read(ref enumeratorBox[0]);
                if (enumeratorCopy != null)
                {
                    lock (enumeratorCopy)
                    {
                        if (enumeratorCopy == Volatile.Read(ref enumeratorBox[0]))
                        {
                            if (enumeratorCopy.MoveNext())
                            {
                                value = enumeratorCopy.Current;
                                return true;
                            }

                            Interlocked.Exchange(ref enumeratorBox[0], null)?.Dispose();
                        }
                    }
                }

                value = default!;
                return false;
            }
        }
    }

    internal sealed class ProgressorProxy
    {
        private readonly IClosable _node;

        public ProgressorProxy(IClosable node)
        {
            _node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public bool IsClosed => _node.IsClosed;
    }
}