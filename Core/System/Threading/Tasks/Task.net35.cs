#if FAT
#if NET20 || NET30 || NET35

using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public sealed class Task
    {
        [ThreadStatic]
        private static Task _current;

        private static int _lastId;
        private readonly Action _action;
        private readonly int _id = Interlocked.Increment(ref _lastId) - 1;
        private readonly TaskScheduler _scheduler;
        private AggregateException _exception;
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
            _waitHandle = new ManualResetEventSlim(false);
        }

        internal Task(Action action, TaskScheduler scheduler)
        {
            if (ReferenceEquals(scheduler, null))
            {
                throw new ArgumentNullException("scheduler");
            }
            _scheduler = scheduler;
            _action = action ?? ActionHelper.GetNoopAction();
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

        public static int CurrentId
        {
            get
            {
                return _current.Id;
            }
        }

        public AggregateException Exception
        {
            get
            {
                return _exception;
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

        internal static Task Current
        {
            get
            {
                return _current;
            }
        }

        private bool IsScheduled
        {
            get
            {
                var status = Thread.VolatileRead(ref _status);
                return status == (int)TaskStatus.WaitingToRun || status == (int)TaskStatus.Running || status == (int)TaskStatus.WaitingForChildrenToComplete;
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
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingToRun, (int)TaskStatus.Created) != (int)TaskStatus.Created)
            {
                throw new InvalidOperationException();
            }
            Schedule();
        }

        public void Start(TaskScheduler scheduler)
        {
            if (GCMonitor.FinalizingForUnload)
            {
                // Silent fail
                return;
            }
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingToRun, (int)TaskStatus.Created) != (int)TaskStatus.Created)
            {
                throw new InvalidOperationException();
            }
            Schedule();
        }

        public void Wait()
        {
            var scheduled = IsScheduled;
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, scheduled);
            }
        }

        public void Wait(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var scheduled = IsScheduled;
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, scheduled);
                if (!IsCompleted)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    GC.KeepAlive(cancellationToken.WaitHandle);
                }
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
            var scheduled = IsScheduled;
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, scheduled);
                if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) >= milliseconds)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Wait(TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var start = ThreadingHelper.TicksNow();
            var scheduled = IsScheduled;
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, scheduled);
                if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) >= milliseconds)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Wait(int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds < -1)
            {
                throw new ArgumentOutOfRangeException("milliseconds");
            }
            if (milliseconds == -1)
            {
                Wait(cancellationToken);
                return true;
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var start = ThreadingHelper.TicksNow();
            var scheduled = IsScheduled;
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, scheduled);
                if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) >= milliseconds)
                {
                    return false;
                }
                if (!IsCompleted)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    GC.KeepAlive(cancellationToken.WaitHandle);
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
                _exception = new AggregateException(exception);
            }
            finally
            {
                Thread.VolatileWrite(ref _status, (int)TaskStatus.RanToCompletion);
                _waitHandle.Value.Set();
                Interlocked.Exchange(ref _current, oldCurrent);
            }
        }

        private void Schedule()
        {
            _exception = null;
            _scheduler.QueueTask(this);
        }
    }
}

#endif
#endif