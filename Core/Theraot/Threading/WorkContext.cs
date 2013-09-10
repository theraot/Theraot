using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading
{
    public sealed class WorkContext : IDisposable
    {
        private const int INT_InitialWorkCapacityHint = 128;
        private const int INT_LoopCountHint = 4;
        private const int INT_SpinWaitHint = 16;
        private static WorkContext _defaultContext = new WorkContext(INT_InitialWorkCapacityHint, Environment.ProcessorCount, "Default Context", false);

        private static int _lastId;
        private int _dedidatedThreadCount;
        private bool _disposable;
        private AutoResetEvent _event;
        private int _id;
        private LazyBucket<Thread> _threads;
        private int _waitRequest;
        private volatile bool _work;
        private int _workingDedicatedThreadCount;
        private int _workingTotalThreadCount;
        private QueueBucket<Work> _works;

        public WorkContext()
            : this(INT_InitialWorkCapacityHint, Environment.ProcessorCount, null, true)
        {
            //Empty
        }

        public WorkContext(int initialWorkCapacity)
            : this(initialWorkCapacity, Environment.ProcessorCount, null, true)
        {
            //Empty
        }

        public WorkContext(int initialWorkCapacity, int dedicatedThreads)
            : this(initialWorkCapacity, dedicatedThreads, null, true)
        {
            //Empty
        }

        public WorkContext(string name)
            : this(INT_InitialWorkCapacityHint, Environment.ProcessorCount, Check.NotNullArgument(name, "name"), true)
        {
            //Empty
        }

        public WorkContext(string name, int initialWorkCapacity)
            : this(initialWorkCapacity, Environment.ProcessorCount, Check.NotNullArgument(name, "name"), true)
        {
            //Empty
        }

        public WorkContext(string name, int initialWorkCapacity, int dedicatedThreads)
            : this(initialWorkCapacity, dedicatedThreads, Check.NotNullArgument(name, "name"), true)
        {
            //Empty
        }

        private WorkContext(int initialWorkCapacity, int dedicatedThreads, string name, bool disposable)
        {
            if (dedicatedThreads < 0)
            {
                throw new ArgumentOutOfRangeException("dedicatedThreads", "dedicatedThreads < 0");
            }
            else
            {
                _works = new QueueBucket<Work>(initialWorkCapacity);
                Converter<int, Thread> valueFactory = null;
                if (StringHelper.IsNullOrWhiteSpace(name))
                {
                    if (dedicatedThreads == 1)
                    {
                        valueFactory = input => new Thread(DoWorks)
                        {
                            Name = string.Format("Dedicated Thread on Work.Context {0}", _id),
                            IsBackground = true
                        };
                    }
                    else if (dedicatedThreads > 1)
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
                    if (dedicatedThreads == 1)
                    {
                        valueFactory = input => new Thread(DoWorks)
                        {
                            Name = string.Format("Dedicated Thread on {0}", name),
                            IsBackground = true
                        };
                    }
                    else if (dedicatedThreads > 1)
                    {
                        valueFactory = input => new Thread(DoWorks)
                        {
                            Name = string.Format("Dedicated Thread {0} on {1}", input, name),
                            IsBackground = true
                        };
                    }
                }
                _threads = new LazyBucket<Thread>
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
                catch
                {
                    //Pokemon
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
            return new Work(action, false, this);
        }

        public Work AddWork(Action action, bool exclusive)
        {
            return new Work(action, exclusive, this);
        }

        public void Dispose()
        {
            if (_disposable)
            {
                DisposeExtracted();
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
                        ThreadingHelper.SpinWait(ref _waitRequest, 0);
                    }
                    Interlocked.Increment(ref _workingTotalThreadCount);
                    Work item;
                    if (_works.TryDequeue(out item))
                    {
                        Execute(item);
                    }
                }
                catch
                {
                    // Pokemon
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
                if (_works.Count == _works.Capacity)
                {
                    ActivateDedicatedThreads();
                }
                _works.Enqueue(work);
                if (_threads.Capacity == 0)
                {
                    ThreadPool.QueueUserWorkItem
                    (
                        _ =>
                        {
                            DoOneWork();
                        }
                    );
                }
                else
                {
                    ActivateDedicatedThreads();
                }
            }
        }

        private void ActivateDedicatedThreads()
        {
            var threadIndex = Interlocked.Increment(ref _dedidatedThreadCount) - 1;
            if (threadIndex < _threads.Capacity)
            {
                Thread thread = _threads.Get(threadIndex);
                thread.Start();
            }
            else
            {
                Thread.VolatileWrite(ref _dedidatedThreadCount, _threads.Capacity);
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
                while (Thread.VolatileRead(ref _dedidatedThreadCount) > 0)
                {
                    _event.Set();
                }
                _event.Close();
            }
            finally
            {
                try
                {
                    //Empty
                }
                finally
                {
                    _work = false;
                    _threads = null;
                    GC.SuppressFinalize(this);
                }
            }
        }

        private void DoWorks()
        {
            int count = INT_LoopCountHint;
            try
            {
                Interlocked.Increment(ref _workingTotalThreadCount);
                Interlocked.Increment(ref _workingDedicatedThreadCount);
                while (_work)
                {
                    Work item;
                    if (_works.TryDequeue(out item))
                    {
                        Execute(item);
                    }
                    else if (_works.Count == 0)
                    {
                        if (count > 0)
                        {
                            count--;
                            Thread.SpinWait(INT_SpinWaitHint);
                        }
                        else
                        {
                            try
                            {
                                Interlocked.Decrement(ref _workingDedicatedThreadCount);
                                Interlocked.Decrement(ref _workingTotalThreadCount);
                                _event.WaitOne();
                                count = INT_LoopCountHint;
                            }
                            finally
                            {
                                Interlocked.Increment(ref _workingTotalThreadCount);
                                Interlocked.Increment(ref _workingDedicatedThreadCount);
                            }
                        }
                    }
                    if (Thread.VolatileRead(ref _waitRequest) == 1)
                    {
                        Interlocked.Decrement(ref _workingTotalThreadCount);
                        ThreadingHelper.SpinWait(ref _waitRequest, 0);
                        Interlocked.Increment(ref _workingTotalThreadCount);
                    }
                }
            }
            catch
            {
                // Pokemon
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
                ThreadingHelper.SpinWait(ref _workingTotalThreadCount, 1);
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
