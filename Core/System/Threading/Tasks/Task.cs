#if FAT

using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public sealed class Task : IPromise
    {
        private const int INT_StatusCompleted = 2;
        private const int INT_StatusNew = 0;
        private const int INT_StatusRunning = 1;

        [ThreadStatic]
        private static Task _current;

        private static int _lastId;
        private readonly Action _action;
        private readonly bool _exclusive;
        private readonly int _id = Interlocked.Increment(ref _lastId) - 1;
        private readonly TaskScheduler _scheduler;
        private Exception _error;
        private int _status = INT_StatusNew;

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
                return false;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Thread.VolatileRead(ref _status) == INT_StatusCompleted;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return !ReferenceEquals(_error, null);
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

        public void Start()
        {
            if (GCMonitor.FinalizingForUnload)
            {
                // Silent fail
                return;
            }
            if (Interlocked.CompareExchange(ref _status, INT_StatusRunning, INT_StatusNew) != INT_StatusNew)
            {
                throw new InvalidOperationException();
            }
            _error = null;
            _scheduler.QueueTask(this);
        }

        public void Wait()
        {
            _scheduler.RunAndWait(this, Thread.VolatileRead(ref _status) == INT_StatusRunning);
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
            while (Thread.VolatileRead(ref _status) != INT_StatusCompleted)
            {
                _scheduler.RunAndWait(this, Thread.VolatileRead(ref _status) == INT_StatusRunning);
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
                Thread.VolatileWrite(ref _status, INT_StatusCompleted);
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
                    (Interlocked.CompareExchange(ref _status, INT_StatusRunning, INT_StatusNew) != INT_StatusNew)
                    && (Interlocked.CompareExchange(ref _status, INT_StatusRunning, INT_StatusCompleted) != INT_StatusCompleted)
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