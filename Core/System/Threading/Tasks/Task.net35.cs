#if NET20 || NET30 || NET35

using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public class Task : IDisposable, IAsyncResult
    {
        [ThreadStatic]
        private static Task _current;

        private static int _lastId;
        private readonly Action _action;
        private readonly int _id = Interlocked.Increment(ref _lastId) - 1;
        private readonly Task _parent;
        private readonly TaskScheduler _scheduler;
        private readonly TaskCreationOptions _options;
        private CancellationToken _cancellationToken;
        private ExecutionContext _capturedContext; // TODO
        private AggregateException _exception;
        private int _isDisposed = 0;
        private object _state;
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
            _state = null;
        }

        public Task(Action action, TaskCreationOptions creationOptions)
            : this(action)
        {
            _options = creationOptions;
        }

        internal Task(Action action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (ReferenceEquals(scheduler, null))
            {
                throw new ArgumentNullException("scheduler");
            }
            if ((creationOptions & TaskCreationOptions.AttachedToParent) != TaskCreationOptions.None)
            {
                _parent = Current;
                if (_parent != null)
                {
                    _parent.AddChild(this);
                }
            }
            _action = action ?? ActionHelper.GetNoopAction();
            _state = state;
            _scheduler = scheduler;
            _waitHandle = new ManualResetEventSlim(false);
            //TODO validate creationOptions
            _options = creationOptions;
            if (cancellationToken.CanBeCanceled)
            {
                // TODO
                // AssignCancellationToken(cancellationToken, null, null);
            }
        }

        private void AddChild(Task task)
        {
            // TODO
            throw new NotImplementedException();
        }

        ~Task()
        {
            Dispose(false);
        }

        public static int CurrentId
        {
            get
            {
                return _current.Id;
            }
        }

        public static TaskFactory Factory
        {
            get
            {
                return TaskFactory._defaultInstance;
            }
        }

        public object AsyncState
        {
            get
            {
                return _state;
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

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get
            {
                if (Thread.VolatileRead(ref _isDisposed) == 1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                return _waitHandle.Value.WaitHandle;
            }
        }

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool IAsyncResult.CompletedSynchronously
        {
            get
            {
                return false;
            }
        }

        internal static Task Current
        {
            get
            {
                return _current;
            }
        }

        internal TaskCreationOptions Options
        {
            get
            {
                return _options;
            }
        }

        internal TaskScheduler Scheduler
        {
            get
            {
                return _scheduler;
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

        public void Dispose()
        {
            Dispose(true);
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
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingForActivation, (int)TaskStatus.Created) != (int)TaskStatus.Created)
            {
                throw new InvalidOperationException();
            }
            Schedule(_scheduler);
        }

        public void Start(TaskScheduler scheduler)
        {
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingForActivation, (int)TaskStatus.Created) != (int)TaskStatus.Created)
            {
                throw new InvalidOperationException();
            }
            Schedule(scheduler);
        }

        public void Wait()
        {
            var isScheduled = IsScheduled;
            while (!IsCompleted)
            {
                _scheduler.RunAndWait(this, isScheduled);
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

        internal bool ExecuteEntry(bool preventDoubleExecution)
        {
            if (!SetRunning(preventDoubleExecution))
            {
                return false;
            }
            if (!IsCanceled)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    Thread.VolatileWrite(ref _status, (int)TaskStatus.Canceled);
                    // TODO: Notify? Clean up?
                }
                else
                {
                    var oldCurrent = Interlocked.Exchange(ref _current, this);
                    try
                    {
                        _action.Invoke();
                    }
                    catch (Exception exception)
                    {
                        AddException(exception);
                    }
                    finally
                    {
                        // TODO: Wait for children, what children?
                        Thread.VolatileWrite(ref _status, (int)TaskStatus.RanToCompletion);
                        _waitHandle.Value.Set();
                        Interlocked.Exchange(ref _current, oldCurrent);
                    }
                }
            }
            return true;
        }

        private void AddException(Exception exception)
        {
            AggregateExceptionHelper.AddException(ref _exception, exception);
        }

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Microsoft's Design")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!IsCompleted)
                {
                    throw new InvalidOperationException("A task may only be disposed if it is in a completion state.");
                }
                var waitHandle = _waitHandle.Value;
                if (!ReferenceEquals(waitHandle, null))
                {
                    if (!waitHandle.IsSet)
                    {
                        waitHandle.Set();
                    }
                    waitHandle.Dispose();
                    _waitHandle.Value = null;
                }
            }
            Thread.VolatileWrite(ref _isDisposed, 1);
        }

        private void Schedule(TaskScheduler scheduler)
        {
            // Only called from Start where status is set to TaskStatus.WaitingForActivation
            _exception = null;
            scheduler.QueueTask(this);
            // If _status is no longer TaskStatus.WaitingForActivation it means that it is already TaskStatus.Running or beyond
            Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingToRun, (int)TaskStatus.WaitingForActivation);
        }

        private bool SetRunning(bool preventDoubleExecution)
        {
            // For this method to be called the Task must have been scheduled,
            // this means that _status must be at least TaskStatus.WaitingForActivation (1),
            // if status is:
            // TaskStatus.WaitingForActivation (1) -> ok
            // WaitingToRun (2) -> ok
            // TaskStatus.Running (3) -> ok if preventDoubleExecution = false
            // TaskStatus.WaitingForChildrenToComplete (4) -> ok if preventDoubleExecution = false
            // TaskStatus.RanToCompletion (5) -> ok if preventDoubleExecution = false
            // TaskStatus.Canceled (6) -> not ok
            // TaskStatus.Faulted (7) -> -> ok if preventDoubleExecution = false
            int count = 0;
            retry:
            var lastValue = Thread.VolatileRead(ref _status);
            if ((preventDoubleExecution && lastValue >= 3) || lastValue == 6)
            {
                return false;
            }
            else
            {
                var tmpB = Interlocked.CompareExchange(ref _status, 3, lastValue);
                if (tmpB == lastValue)
                {
                    return true;
                }
            }
            ThreadingHelper.SpinOnce(ref count);
            goto retry;
        }
    }
}

#endif