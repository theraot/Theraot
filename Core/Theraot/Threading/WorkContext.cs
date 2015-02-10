#if FAT

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    public sealed class WorkContext : IDisposable
    {
        private static readonly WorkContext _defaultContext = new WorkContext(Environment.ProcessorCount, false);

        private static int _lastId;

        private readonly int _dedidatedThreadMax;
        private readonly bool _disposable;
        private readonly int _id;
        private readonly SafeQueue<Work> _works;

        private int _dedidatedThreadCount;
        private Pool<WorkThread> _threads;
        private int _waitRequest;
        private volatile bool _work;
        private int _workingTotalThreadCount;

        public WorkContext()
            : this(Environment.ProcessorCount, true)
        {
            //Empty
        }

        public WorkContext(int dedicatedThreads)
            : this(dedicatedThreads, true)
        {
            //Empty
        }

        private WorkContext(int dedicatedThreads, bool disposable)
        {
            if (dedicatedThreads < 0)
            {
                throw new ArgumentOutOfRangeException("dedicatedThreads", "dedicatedThreads < 0");
            }
            _dedidatedThreadMax = dedicatedThreads;
            _works = new SafeQueue<Work>();
            _threads = new Pool<WorkThread>(dedicatedThreads);
            _id = Interlocked.Increment(ref _lastId) - 1;
            _work = true;
            _disposable = disposable;
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
                DisposeExtracted();
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
                if (Thread.VolatileRead(ref _waitRequest) == 1)
                {
                    ThreadingHelper.SpinWaitUntil(ref _waitRequest, 0);
                }
                try
                {
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
                WorkThread thread;
                if (_threads.TryGet(out thread))
                {
                    thread.Go();
                }
                else
                {
                    if (Interlocked.Increment(ref _dedidatedThreadCount) < _dedidatedThreadMax)
                    {
                        (new WorkThread(this)).Go();
                    }
                    else
                    {
                        Interlocked.Decrement(ref _dedidatedThreadCount);
                    }
                }
            }
        }

        private void DisposeExtracted()
        {
            _work = false;
            var threads = Interlocked.Exchange(ref _threads, null);
            if (threads != null)
            {
                WorkThread thread;
                while (_threads.TryGet(out thread))
                {
                    thread.Go();
                }
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

        private sealed class WorkThread
        {
            private readonly WorkContext _context;
            private readonly object _locker;
            private readonly Thread _thread;
            private int _working;

            public WorkThread(WorkContext context)
            {
                _context = context;
                _thread = new Thread(Run)
                {
                    IsBackground = true,
                    Name = "Dedicated Thread on Context " + context.Id
                };
                _locker = new object();
            }

            public void Go()
            {
                if (Interlocked.CompareExchange(ref _working, 1, 0) == 0)
                {
                    if (!_thread.IsAlive)
                    {
                        _thread.Start();
                    }
                }
                else
                {
                    lock (_locker)
                    {
                        Monitor.Pulse(_locker);
                    }
                }
            }

            private void Run()
            {
            loopback:
                try
                {
                    Interlocked.Increment(ref _context._workingTotalThreadCount);
                    while (_context._work && !GCMonitor.FinalizingForUnload)
                    {
                        Work item;
                        if (_context._works.TryTake(out item))
                        {
                            _context.Execute(item);
                        }
                        else if (_context._works.Count == 0 || Thread.VolatileRead(ref _context._waitRequest) == 1)
                        {
                            Interlocked.Decrement(ref _context._workingTotalThreadCount);
                            var threads = _context._threads;
                            if (_context._work && !GCMonitor.FinalizingForUnload && threads != null)
                            {
                                lock (_locker)
                                {
                                    threads.Donate(this);
                                    Monitor.Wait(_locker);
                                }
                                Interlocked.Increment(ref _context._workingTotalThreadCount);
                            }
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
                    if (_context._work && !GCMonitor.FinalizingForUnload)
                    {
                        goto loopback;
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _context._workingTotalThreadCount);
                }
            }
        }
    }
}

#endif