#if FAT

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public sealed class WorkContext : IDisposable
    {
        private static readonly WorkContext _defaultContext = new WorkContext(Environment.ProcessorCount, "Default Context", false);

        private static int _lastId;
        private readonly int _dedidatedThreadMax;
        private readonly bool _disposable;
        private readonly int _id;
        private readonly SafeQueue<Work> _works;
        private int _dedidatedThreadCount;
        private AutoResetEvent _event;
        private NeedleBucket<Thread, LazyNeedle<Thread>> _threads;
        private int _waitRequest;
        private volatile bool _work;
        private int _workingDedicatedThreadCount;
        private int _workingTotalThreadCount;

        public WorkContext()
            : this(Environment.ProcessorCount, null, true)
        {
            //Empty
        }

        public WorkContext(int dedicatedThreads)
            : this(dedicatedThreads, null, true)
        {
            //Empty
        }

        public WorkContext(string name)
            : this(Environment.ProcessorCount, Check.NotNullArgument(name, "name"), true)
        {
            //Empty
        }

        public WorkContext(string name, int dedicatedThreads)
            : this(dedicatedThreads, Check.NotNullArgument(name, "name"), true)
        {
            //Empty
        }

        private WorkContext(int dedicatedThreads, string name, bool disposable)
        {
            if (dedicatedThreads < 0)
            {
                throw new ArgumentOutOfRangeException("dedicatedThreads", "dedicatedThreads < 0");
            }
            else
            {
                _dedidatedThreadMax = dedicatedThreads;
                _works = new SafeQueue<Work>();
                Converter<int, Thread> valueFactory = null;
                if (StringHelper.IsNullOrWhiteSpace(name))
                {
                    if (_dedidatedThreadMax == 1)
                    {
                        valueFactory = input => new Thread(DoWorks)
                        {
                            Name = string.Format("Dedicated Thread on Work.Context {0}", _id),
                            IsBackground = true
                        };
                    }
                    else if (_dedidatedThreadMax > 1)
                    {
                        valueFactory = input => new Thread(DoWorks)
                        {
                            Name = string.Format("Dedicated Thread {0} on Work.Context {1}", input, _id),
                            IsBackground = true
                        };
                    }
                }
                else
                {
                    if (_dedidatedThreadMax == 1)
                    {
                        valueFactory = input => new Thread(DoWorks)
                        {
                            Name = string.Format("Dedicated Thread on {0}", name),
                            IsBackground = true
                        };
                    }
                    else if (_dedidatedThreadMax > 1)
                    {
                        valueFactory = input => new Thread(DoWorks)
                        {
                            Name = string.Format("Dedicated Thread {0} on {1}", input, name),
                            IsBackground = true
                        };
                    }
                }
                _threads = new NeedleBucket<Thread, LazyNeedle<Thread>>
                    (
                        valueFactory,
                        dedicatedThreads
                    );
                _id = Interlocked.Increment(ref _lastId) - 1;
                _event = new AutoResetEvent(false);
                _work = true;
                _disposable = disposable;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~WorkContext()
        {
            try
            {
                //Empty
            }
            finally
            {
                try
                {
                    DisposeExtracted();
                }
                catch (Exception exception)
                {
                    // Pokemon - fields may be partially collected.
                    GC.KeepAlive(exception);
                }
            }
        }

        public static WorkContext DefaultContext
        {
            get
            {
                return _defaultContext;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public Work AddWork(Action action)
        {
            return GCMonitor.FinalizingForUnload ? null : new Work(action, false, this);
        }

        public Work AddWork(Action action, bool exclusive)
        {
            return GCMonitor.FinalizingForUnload ? null : new Work(action, exclusive, this);
        }

        public void Dispose()
        {
            if (_disposable)
            {
                DisposeExtracted();
                GC.SuppressFinalize(this);
            }
        }

        public void DoOneWork()
        {
            if (_work)
            {
                try
                {
                    if (Thread.VolatileRead(ref _waitRequest) == 1)
                    {
                        ThreadingHelper.SpinWaitUntil(ref _waitRequest, 0);
                    }
                    Interlocked.Increment(ref _workingTotalThreadCount);
                    Work item;
                    if (_works.TryTake(out item))
                    {
                        Execute(item);
                    }
                }
                catch (Exception exception)
                {
                    // Pokemon
                    GC.KeepAlive(exception);
                }
                finally
                {
                    Interlocked.Decrement(ref _workingTotalThreadCount);
                }
            }
            else
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        internal void ScheduleWork(Work work)
        {
            if (_work)
            {
                _works.Add(work);
                if (_threads.Capacity == 0)
                {
                    ThreadPool.QueueUserWorkItem(_ => DoOneWork());
                }
                else
                {
                    ActivateDedicatedThreads();
                }
            }
        }

        private void ActivateDedicatedThreads()
        {
            if (GCMonitor.FinalizingForUnload)
            {
                return;
            }
            var threadIndex = Interlocked.Increment(ref _dedidatedThreadCount) - 1;
            if (threadIndex < _dedidatedThreadMax)
            {
                Thread thread = _threads.Get(threadIndex);
                thread.Start();
            }
            else
            {
                Thread.VolatileWrite(ref _dedidatedThreadCount, _dedidatedThreadMax);
                if (Thread.VolatileRead(ref _workingDedicatedThreadCount) < _threads.Count)
                {
                    _event.Set();
                }
            }
        }

        private void DisposeExtracted()
        {
            try
            {
                _work = false;
                _event = null;
            }
            finally
            {
                try
                {
                    // Empty
                }
                finally
                {
                    _threads = null;
                }
            }
        }

        private void DoWorks()
        {
            int count = 0;
        loopback:
            try
            {
                Interlocked.Increment(ref _workingTotalThreadCount);
                Interlocked.Increment(ref _workingDedicatedThreadCount);
                while (_work && !GCMonitor.FinalizingForUnload)
                {
                    Work item;
                    if (_works.TryTake(out item))
                    {
                        Execute(item);
                    }
                    else if (_works.Count == 0)
                    {
                        if (count == ThreadingHelper.INT_SleepCountHint)
                        {
                            try
                            {
                                Interlocked.Decrement(ref _workingDedicatedThreadCount);
                                Interlocked.Decrement(ref _workingTotalThreadCount);
                                count = 0;
                                _event.WaitOne();
                            }
                            finally
                            {
                                Interlocked.Increment(ref _workingTotalThreadCount);
                                Interlocked.Increment(ref _workingDedicatedThreadCount);
                            }
                        }
                        else
                        {
                            ThreadingHelper.SpinOnce(ref count);
                        }
                    }
                    if (Thread.VolatileRead(ref _waitRequest) == 1)
                    {
                        Interlocked.Decrement(ref _workingTotalThreadCount);
                        ThreadingHelper.SpinWaitUntil(ref _waitRequest, 0);
                        Interlocked.Increment(ref _workingTotalThreadCount);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // Nothing to do
            }
            catch (ObjectDisposedException)
            {
                // Nothing to do
            }
            catch (NullReferenceException)
            {
                // Nothing to do
            }
            catch
            {
                // Pokemon
                if (_work && !GCMonitor.FinalizingForUnload)
                {
                    goto loopback;
                }
            }
            finally
            {
                Interlocked.Decrement(ref _workingDedicatedThreadCount);
                Interlocked.Decrement(ref _workingTotalThreadCount);
            }
        }

        private void Execute(Work item)
        {
            if (item.Exclusive)
            {
                Thread.VolatileWrite(ref _waitRequest, 1);
                ThreadingHelper.SpinWaitUntil(ref _workingTotalThreadCount, 1);
                item.Execute();
                Thread.VolatileWrite(ref _waitRequest, 0);
            }
            else
            {
                item.Execute();
            }
        }
    }
}

#endif