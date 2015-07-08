using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    public class ProgressorTestEx
    {
        [Test]
        public void ObservableProgressorWithPause()
        {
            var source = new SlowObservable<int>(new[] { 0, 1, 2, 3, 4, 5 });
            var progresor = new Progressor<int>(source);
            Assert.IsFalse(progresor.IsClosed);
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(0, indexA);
            Assert.AreEqual(indexA, indexB);
            Assert.IsFalse(progresor.IsClosed);
            source.Show();
            while (indexA < 6)
            {
                while (progresor.TryTake(out item))
                {
                    Assert.AreEqual(item, indexA);
                    indexA++;
                }
            }
            Assert.AreEqual(indexA, indexB);
            Assert.IsFalse(progresor.IsClosed);
            source.Close();
            Assert.IsFalse(progresor.IsClosed);
            Assert.IsFalse(progresor.TryTake(out item));
            Assert.IsTrue(progresor.IsClosed);
        }

        private class SlowObservable<T> : IObservable<T>
        {
            private readonly IEnumerable<T> _source;
            private readonly SafeSet<Needle<IObserver<T>>> _observers;
            private int _done;

            public SlowObservable(IEnumerable<T> source)
            {
                _source = source;
                _observers = new SafeSet<Needle<IObserver<T>>>();
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
                                    if (Thread.VolatileRead(ref _done) == 1)
                                    {
                                        return;
                                    }
                                    OnNext(item);
                                }
                            }
                            catch (Exception exception)
                            {
                                if (Thread.VolatileRead(ref _done) == 0)
                                {
                                    OnError(exception);
                                }
                            }
                        }
                    );
            }

            public void Close()
            {
                if (Interlocked.CompareExchange(ref _done, 1, 0) == 0)
                {
                    OnCompleted();
                }
            }

            private void OnCompleted()
            {
                foreach (var item in _observers)
                {
                    item.Value.OnCompleted();
                }
            }

            private void OnError(Exception error)
            {
                foreach (var item in _observers)
                {
                    item.Value.OnError(error);
                }
            }

            private void OnNext(T value)
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
}