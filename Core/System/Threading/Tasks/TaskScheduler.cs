#if FAT

using System.Collections.Generic;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    public sealed class DefaultTaskScheduler : TaskScheduler
    {
        private static int _lastId;

        private readonly int _dedidatedThreadMax;
        private readonly bool _disposable;
        private readonly int _id;
        private readonly SafeQueue<Task> _tasks;

        private int _dedidatedThreadCount;
        private Pool<TimeSlot> _servSlots;
        private int _waitRequest;
        private volatile bool _work;
        private int _workingTotalThreadCount;

        public DefaultTaskScheduler()
            : this(Environment.ProcessorCount, true)
        {
            //Empty
        }

        public DefaultTaskScheduler(int dedicatedThreads)
            : this(dedicatedThreads, true)
        {
            //Empty
        }

        internal DefaultTaskScheduler(int dedicatedThreads, bool disposable)
        {
            if (dedicatedThreads < 0)
            {
                throw new ArgumentOutOfRangeException("dedicatedThreads", "dedicatedThreads < 0");
            }
            _dedidatedThreadMax = dedicatedThreads;
            _tasks = new SafeQueue<Task>();
            _servSlots = new Pool<TimeSlot>(dedicatedThreads * 2);
            _id = Interlocked.Increment(ref _lastId) - 1;
            _work = true;
            _disposable = disposable;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~DefaultTaskScheduler()
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

        public void Dispose()
        {
            if (_disposable)
            {
                DisposeExtracted();
                GC.SuppressFinalize(this);
            }
        }

        protected internal override void QueueTask(Task task)
        {
            if (_work)
            {
                _tasks.Add(task);
                while (_tasks.Count > 0)
                {
                    TimeSlot slot;
                    if (_servSlots.TryGet(out slot) || NewDedicatedThread(out slot))
                    {
                        slot.Use();
                        break;
                    }
                }
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (_work)
            {
                if (!taskWasPreviouslyQueued)
                {
                    //--- ?
                }
                var slot = new TimeSlot(this);
                while (!task.IsCompleted)
                {
                    slot.ServeOnce();
                }
                return task.IsCompleted;
            }
            return false;
        }

        private void DisposeExtracted()
        {
            _work = false;
            var slots = Interlocked.Exchange(ref _servSlots, null);
            if (slots != null)
            {
                TimeSlot slot;
                while (slots.TryGet(out slot))
                {
                    slot.Reject();
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

        private bool NewDedicatedThread(out TimeSlot slot)
        {
            var threadNumber = Interlocked.Increment(ref _dedidatedThreadCount);
            if (threadNumber <= _dedidatedThreadMax)
            {
                GC.KeepAlive(new WorkThread(this, "Dedicated Thread #" + threadNumber + " on scheduler " + _id, out slot));
                return true;
            }
            Interlocked.Decrement(ref _dedidatedThreadCount);
            slot = null;
            return false;
        }

        private sealed class TimeSlot
        {
            private readonly object _locker;
            private readonly DefaultTaskScheduler _scheduler;
            private int _status;

            public TimeSlot(DefaultTaskScheduler scheduler)
            {
                _scheduler = scheduler;
                _locker = new object();
            }

            public void Reject()
            {
                if (Interlocked.CompareExchange(ref _status, 0, 1) == 1)
                {
                    lock (_locker)
                    {
                        Monitor.Pulse(_locker);
                    }
                }
            }

            public bool ServeAll()
            {
                if (Interlocked.CompareExchange(ref _status, 1, 0) == 0)
                {
                    lock (_locker)
                    {
                        _scheduler._servSlots.Donate(this);
                        Monitor.Wait(_locker);
                    }
                    if (Interlocked.CompareExchange(ref _status, 3, 2) == 2)
                    {
                        RunAll();
                        Thread.VolatileWrite(ref _status, 0);
                        return true;
                    }
                }
                return false;
            }

            public bool ServeOnce()
            {
                if (Interlocked.CompareExchange(ref _status, 1, 0) == 0)
                {
                    lock (_locker)
                    {
                        _scheduler._servSlots.Donate(this);
                        Monitor.Wait(_locker);
                    }
                    if (Interlocked.CompareExchange(ref _status, 3, 2) == 2)
                    {
                        RunOnce();
                        Thread.VolatileWrite(ref _status, 0);
                        return true;
                    }
                }
                return false;
            }

            public void Use()
            {
                if (Interlocked.CompareExchange(ref _status, 2, 1) == 1)
                {
                    lock (_locker)
                    {
                        Monitor.Pulse(_locker);
                    }
                }
            }

            private void RunAll()
            {
                Task item;
                while (_scheduler._tasks.TryTake(out item))
                {
                    _scheduler.Execute(item);
                }
            }

            private void RunOnce()
            {
                Task item;
                if (_scheduler._tasks.TryTake(out item))
                {
                    _scheduler.Execute(item);
                }
            }
        }

        private sealed class WorkThread
        {
            private readonly object _locker;
            private readonly DefaultTaskScheduler _scheduler;
            private readonly TimeSlot _slot;

            public WorkThread(DefaultTaskScheduler scheduler, string name, out TimeSlot slot)
            {
                _scheduler = scheduler;
                _locker = new object();
                _slot = slot = new TimeSlot(scheduler);
                (new Thread(Run)
                {
                    IsBackground = true,
                    Name = name
                }).Start();
            }

            private void Run()
            {
                loopback:
                try
                {
                    Interlocked.Increment(ref _scheduler._workingTotalThreadCount);
                    while (_scheduler._work && !GCMonitor.FinalizingForUnload)
                    {
                        bool serve = _slot.ServeAll();
                        if (!serve && (_scheduler._tasks.Count == 0 || Thread.VolatileRead(ref _scheduler._waitRequest) == 1))
                        {
                            // Sometimes a Task is added just after we check there is no Task
                            // If that happens and the wake up call will come before this threads goes to wait...
                            // Then this thread will go to wait anyway, regardless of the new Task.
                            // That's why it is necesary to make sure a thread is started for this to Task correctly.
                            // This could be solve by spinning instead, but it is convenient to be able to put the thread to wait.
                            Interlocked.Decrement(ref _scheduler._workingTotalThreadCount);
                            var slots = _scheduler._servSlots;
                            if (_scheduler._work && !GCMonitor.FinalizingForUnload && slots != null)
                            {
                                lock (_locker)
                                {
                                    slots.Donate(_slot);
                                    Monitor.Wait(_locker);
                                }
                                Interlocked.Increment(ref _scheduler._workingTotalThreadCount);
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
                    if (_scheduler._work && !GCMonitor.FinalizingForUnload)
                    {
                        goto loopback;
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref _scheduler._workingTotalThreadCount);
                }
            }
        }
    }

    public abstract class TaskScheduler
    {
        private static readonly TaskScheduler _default = new DefaultTaskScheduler(Environment.ProcessorCount, false);

        private static int _lastId;

        private readonly int _id;

        protected TaskScheduler()
        {
            _id = Interlocked.Increment(ref _lastId) - 1;
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
                    return currentTask.Scheduler;
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

        public int MaximunConcurrencyLevel
        {
            get
            {
                return int.MaxValue;
            }
        }

        public static TaskScheduler FromCurrentSynchronizationContext()
        {
            throw new NotImplementedException();
        }

        public Task AddWork(Action action)
        {
            return GCMonitor.FinalizingForUnload ? null : new Task(action, false, this);
        }

        public Task AddWork(Action action, bool exclusive)
        {
            return GCMonitor.FinalizingForUnload ? null : new Task(action, exclusive, this);
        }

        internal void RunAndWait(Task task, bool taskWasPreviouslyQueued)
        {
            TryExecuteTaskInline(task, taskWasPreviouslyQueued);
        }

        protected internal abstract void QueueTask(Task task);

        protected internal virtual bool TryDequeue(Task task)
        {
            throw new NotImplementedException();
        }

        protected abstract IEnumerable<Task> GetScheduledTasks();

        protected bool TryExecuteTask(Task task)
        {
            throw new NotImplementedException();
        }
        protected abstract bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued);
    }
}

#endif