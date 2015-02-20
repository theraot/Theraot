#if FAT

using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    public sealed class TaskScheduler : IDisposable
    {
        private static readonly TaskScheduler _default = new TaskScheduler(Environment.ProcessorCount, false);

        private static int _lastId;

        private readonly int _dedidatedThreadMax;
        private readonly bool _disposable;
        private readonly int _id;
        private readonly SafeQueue<Task> _tasks;

        private int _dedidatedThreadCount;
        private Pool<WorkThread> _threads;
        private int _waitRequest;
        private volatile bool _work;
        private int _workingTotalThreadCount;

        public TaskScheduler()
            : this(Environment.ProcessorCount, true)
        {
            //Empty
        }

        public TaskScheduler(int dedicatedThreads)
            : this(dedicatedThreads, true)
        {
            //Empty
        }

        private TaskScheduler(int dedicatedThreads, bool disposable)
        {
            if (dedicatedThreads < 0)
            {
                throw new ArgumentOutOfRangeException("dedicatedThreads", "dedicatedThreads < 0");
            }
            _dedidatedThreadMax = dedicatedThreads;
            _tasks = new SafeQueue<Task>();
            _threads = new Pool<WorkThread>(dedicatedThreads);
            _id = Interlocked.Increment(ref _lastId) - 1;
            _work = true;
            _disposable = disposable;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~TaskScheduler()
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

        public static TaskScheduler Default
        {
            get
            {
                return _default;
            }
        }

        public TaskScheduler Current
        {
            get
            {
                var currentTask = Task.Current;
                if (currentTask != null)
                {
                    return currentTask.Context;
                }
                return Default;
            }
        }
        public int Id
        {
            get
            {
                return _id;
            }
        }

        public Task AddWork(Action action)
        {
            return GCMonitor.FinalizingForUnload ? null : new Task(action, false, this);
        }

        public Task AddWork(Action action, bool exclusive)
        {
            return GCMonitor.FinalizingForUnload ? null : new Task(action, exclusive, this);
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
                    Task item;
                    if (_tasks.TryTake(out item))
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

        internal void ScheduleWork(Task task)
        {
            if (_work)
            {
                _tasks.Add(task);
                while (_tasks.Count > 0)
                {
                    WorkThread thread;
                    // Sometimes a thread goes to wait just after we try to awake a thread
                    // When there that happens and there is no room to create a new thread...
                    // It may happen that no thread takes the Task.
                    // That's why we loop until we can wake up or create a thread, or all Task is done.
                    if (_threads.TryGet(out thread) || NewDedicatedThread(out thread))
                    {
                        thread.Go();
                        break;
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
                while (threads.TryGet(out thread))
                {
                    thread.Go();
                }
            }
        }

        private void Execute(Task item)
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

        private bool NewDedicatedThread(out WorkThread thread)
        {
            var threadNumber = Interlocked.Increment(ref _dedidatedThreadCount);
            if (threadNumber <= _dedidatedThreadMax)
            {
                thread = new WorkThread(this, "Dedicated Thread #" + threadNumber + " on Context " + _id);
                return true;
            }
            Interlocked.Decrement(ref _dedidatedThreadCount);
            thread = null;
            return false;
        }

        private sealed class WorkThread
        {
            private readonly TaskScheduler _context;
            private readonly object _locker;
            private readonly Thread _thread;
            private int _working;

            public WorkThread(TaskScheduler context, string name)
            {
                _context = context;
                _thread = new Thread(Run)
                {
                    IsBackground = true,
                    Name = name
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
                        Task item;
                        if (_context._tasks.TryTake(out item))
                        {
                            _context.Execute(item);
                        }
                        else if (_context._tasks.Count == 0 || Thread.VolatileRead(ref _context._waitRequest) == 1)
                        {
                            // Sometimes a Task is added just after we check there is no Task
                            // If that happens and the wake up call will come before this threads goes to wait...
                            // Then this thread will go to wait anyway, regardless of the new Task.
                            // That's why it is necesary to make sure a thread is started for this to Task correctly.
                            // This could be solve by spinning instead, but it is convenient to be able to put the thread to wait.
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