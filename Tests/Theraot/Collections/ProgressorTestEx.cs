using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    public class ProgressorTestEx
    {
        [Test]
        [Category("Performance")]
        public void ObservableProgressorWithPauseLoop()
        {
            for (int index = 0; index < 100000; index++)
            {
                ObservableProgressorWithPause();
            }
        }

        [Test]
        public void ObservableProgressorWithPause()
        {
            var source = new SlowObservable<int>(new[] { 0, 1, 2, 3, 4, 5 });
            var progressor = Progressor<int>.CreateFromIObservable(source);
            Assert.IsFalse(progressor.IsClosed);
            var data = new[] { 0, 0, 0 };
            using
            (
                progressor.SubscribeAction
                (
                    value =>
                    {
                        if (value != Volatile.Read(ref data[0]))
                        {
                            Volatile.Write(ref data[2], 1);
                        }

                        Interlocked.Increment(ref data[0]);
                    }
                )
            )
            {
#if TARGETS_NET
                Thread.MemoryBarrier();
                Thread.MemoryBarrier();
#else
                Interlocked.MemoryBarrier();
                Interlocked.MemoryBarrier();
#endif
                source.Show();
#if TARGETS_NET
                Thread.MemoryBarrier();
#else
                Interlocked.MemoryBarrier();
#endif
                while (progressor.TryTake(out var item))
                {
                    Assert.AreEqual(0, Volatile.Read(ref data[2]));
                    Assert.AreEqual(item, Volatile.Read(ref data[1]));
                    Interlocked.Increment(ref data[1]);
                }
                if (Volatile.Read(ref data[0]) != 6 || Volatile.Read(ref data[1]) != 6)
                {
                    Debugger.Break();
                }
            }
            Assert.AreEqual(6, Volatile.Read(ref data[0]));
            Assert.AreEqual(Volatile.Read(ref data[0]), Volatile.Read(ref data[1]));
            Assert.IsTrue(progressor.IsClosed);
        }

        private class SlowObservable<T> : IObservable<T>
        {
            private readonly IEnumerable<T> _source;
            private readonly Bucket<IObserver<T>> _observers;
            private int _index;
            private object _last;
            private bool _done;
            private Exception _exception;

            public SlowObservable(IEnumerable<T> source)
            {
                _source = source;
                _observers = new Bucket<IObserver<T>>();
                _index = -1;
            }

            public void Show()
            {
                ThreadPool.QueueUserWorkItem
                    (
                        _ =>
                        {
                            try
                            {
                                foreach (var item in _source)
                                {
                                    Volatile.Write(ref _last, item);
                                    OnNext(item);
                                }
                                OnCompleted();
                                Volatile.Write(ref _done, true);
                            }
                            catch (Exception exception)
                            {
                                OnError(exception);
                                Volatile.Write(ref _exception, exception);
                            }
                        }
                    );
            }

            private void OnCompleted()
            {
                foreach (var item in _observers)
                {
                    item.OnCompleted();
                }
            }

            private void OnError(Exception error)
            {
                foreach (var item in _observers)
                {
                    item.OnError(error);
                }
            }

            private void OnNext(T value)
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
}