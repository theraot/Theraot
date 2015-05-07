#if FAT

using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public sealed class Task : IPromise
    {
        [ThreadStatic]
        private static Task _current;

        private static int _lastId;
        private readonly Action _action;
        private readonly bool _exclusive;
        private readonly int _id = Interlocked.Increment(ref _lastId) - 1;
        private readonly TaskScheduler _scheduler;
        private Exception _error;
        private int _status = (int)TaskStatus.Created;

        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public Task(Action action)
        {
            if (ReferenceEquals(action, null))
            {
                throw new ArgumentNullException("action");
            }
            _scheduler = TaskScheduler.Default;
            _action = action;
            _exclusive = false;
            _waitHandle = new ManualResetEventSlim(false);
        }

        internal Task(Action action, bool exclusive, TaskScheduler scheduler)
        {
            if (ReferenceEquals(scheduler, null))
            {
                throw new ArgumentNullException("scheduler");
            }
            _scheduler = scheduler;
            _action = action ?? ActionHelper.GetNoopAction();
            _exclusive = exclusive;
            _waitHandle = new ManualResetEventSlim(false);
        }

        ~Task()
        {
            var waitHandle = _waitHandle.Value;
            if (!ReferenceEquals(waitHandle, null))
            {
                waitHandle.Dispose();
            }
            _waitHandle.Value = null;
        }

        public static Task Current
        {
            get
            {
                return _current;
            }
        }

        public static int CurrentId
        {
            get
            {
                return _current.Id;
            }
        }

        public Exception Error
        {
            get
            {
                return _error;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public bool IsCanceled
        {
            get
            {
                var status = Thread.VolatileRead(ref _status);
                return status == (int)TaskStatus.Canceled;
            }
        }

        public bool IsCompleted
        {
            get
            {
                var status = Thread.VolatileRead(ref _status);
                return status == (int)TaskStatus.RanToCompletion || status == (int)TaskStatus.Faulted || status == (int)TaskStatus.Canceled;
            }
        }

        public bool IsFaulted
        {
            get
            {
                var status = Thread.VolatileRead(ref _status);
                return status == (int)TaskStatus.Faulted;
            }
        }

        public TaskScheduler Scheduler
        {
            get
            {
                return _scheduler;
            }
        }

        internal bool Exclusive
        {
            get
            {
                return _exclusive;
            }
        }

        public void RunSynchronously()
        {
            Start();
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, true);
            }
        }

        public void RunSynchronously(TaskScheduler scheduler)
        {
            Start(scheduler);
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, true);
            }
        }

        public void Start()
        {
            if (GCMonitor.FinalizingForUnload)
            {
                // Silent fail
                return;
            }
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.Running, (int)TaskStatus.Created) != (int)TaskStatus.Created)
            {
                throw new InvalidOperationException();
            }
            _error = null;
            _scheduler.QueueTask(this);
        }

        public void Start(TaskScheduler scheduler)
        {
            if (GCMonitor.FinalizingForUnload)
            {
                // Silent fail
                return;
            }
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.Running, (int)TaskStatus.Created) != (int)TaskStatus.Created)
            {
                throw new InvalidOperationException();
            }
            _error = null;
            scheduler.QueueTask(this);
        }

        public void Wait()
        {
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, Thread.VolatileRead(ref _status) == (int) TaskStatus.Running);
            }
        }

        public bool Wait(int milliseconds)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                Wait();
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, Thread.VolatileRead(ref _status) == (int)TaskStatus.Running);
                if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) >= milliseconds)
                {
                    return false;
                }
            }
            return true;
        }

        internal void Execute()
        {
            var oldCurrent = Interlocked.Exchange(ref _current, this);
            try
            {
                _action.Invoke();
            }
            catch (Exception exception)
            {
                _error = exception;
            }
            finally
            {
                Thread.VolatileWrite(ref _status, (int)TaskStatus.RanToCompletion);
                _waitHandle.Value.Set();
                Interlocked.Exchange(ref _current, oldCurrent);
            }
        }

        internal void Restart()
        {
            if (GCMonitor.FinalizingForUnload)
            {
                // Silent fail
                return;
            }
            if
                (
                    (Interlocked.CompareExchange(ref _status, (int)TaskStatus.Running, (int)TaskStatus.Created) != (int)TaskStatus.Created)
                    && (Interlocked.CompareExchange(ref _status, (int)TaskStatus.Running, (int)TaskStatus.Faulted) != (int)TaskStatus.Faulted)
                    && (Interlocked.CompareExchange(ref _status, (int)TaskStatus.Running, (int)TaskStatus.Canceled) != (int)TaskStatus.Canceled)
                    && (Interlocked.CompareExchange(ref _status, (int)TaskStatus.Running, (int)TaskStatus.RanToCompletion) != (int)TaskStatus.RanToCompletion)
                )
            {
                throw new InvalidOperationException();
            }
            _error = null;
            _scheduler.QueueTask(this);
        }
    }
}

#endif