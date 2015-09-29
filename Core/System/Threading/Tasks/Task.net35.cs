#if NET20 || NET30 || NET35

using System.Diagnostics.Contracts;
using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public partial class Task : IDisposable, IAsyncResult
    {
        [ThreadStatic]
        private static Task _current;

        private static int _lastId;
        private readonly TaskCreationOptions _creationOptions;
        private readonly int _id;
        private readonly Task _parent;
        private object _action;
        private int _isDisposed = 0;
        private TaskScheduler _scheduler;
        private object _state;
        private int _status;
        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public Task(Action action)
            : this(action, null, default(CancellationToken), TaskCreationOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Action action, CancellationToken cancellationToken)
            : this(action, null, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Action action, TaskCreationOptions creationOptions)
            : this(action, null, default(CancellationToken), creationOptions, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : this(action, null, cancellationToken, creationOptions, TaskScheduler.Default)
        {
            // Empty
        }

        internal Task(object action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (ReferenceEquals(action, null))
            {
                throw new ArgumentNullException("action");
            }
            if (ReferenceEquals(scheduler, null))
            {
                throw new ArgumentNullException("scheduler");
            }
            _id = Interlocked.Increment(ref _lastId) - 1;
            _status = (int)TaskStatus.Created;
            if ((creationOptions & TaskCreationOptions.AttachedToParent) != TaskCreationOptions.None)
            {
                _parent = Current;
                if (_parent != null && (_parent.CreationOptions & TaskCreationOptions.DenyChildAttach) == TaskCreationOptions.None)
                {
                    _parent.AddNewChild();
                }
            }
            _action = action;
            _state = state;
            _scheduler = scheduler;
            _waitHandle = new ManualResetEventSlim(false);
            // TODO validate creationOptions
            _creationOptions = creationOptions;
            if (cancellationToken.CanBeCanceled)
            {
                AssignCancellationToken(cancellationToken);
            }
        }

        ~Task()
        {
            Dispose(false);
        }

        public static int? CurrentId
        {
            get
            {
                var current = _current;
                if (current != null)
                {
                    return current.Id;
                }
                return null;
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

        public TaskCreationOptions CreationOptions
        {
            get
            {
                return _creationOptions;
            }
        }

        public AggregateException Exception
        {
            get
            {
                AggregateException e = null;

                // If you're faulted, retrieve the exception(s)
                if (IsFaulted)
                {
                    e = GetExceptions(false);
                }

                // Only return an exception in faulted state (skip manufactured exceptions)
                // A "benevolent" race condition makes it possible to return null when IsFaulted is
                // true (i.e., if IsFaulted is set just after the check to IsFaulted above).
                Contract.Assert((e == null) || IsFaulted, "Task.Exception_get(): returning non-null value when not Faulted");

                return e;
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

        public TaskStatus Status
        {
            get
            {
                return (TaskStatus)Thread.VolatileRead(ref _status);
            }
        }

        internal static Task Current
        {
            get
            {
                return _current;
            }
        }

        internal TaskScheduler Scheduler
        {
            get
            {
                return _scheduler;
            }
        }

        internal CancellationToken Token { get; set; }

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
            PrivateRunSynchronously();
        }

        public void RunSynchronously(TaskScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
            _scheduler = scheduler;
            PrivateRunSynchronously();
        }

        private void PrivateRunSynchronously()
        {
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            // TODO: Do not Run Synchronously Continuation Tasks: InvalidOperationException("RunSynchronously may not be called on a continuation task.");
            // TODO: Do not Run Synchronously Promise Tasks: InvalidOperationException("RunSynchronously may not be called on a task not bound to a delegate, such as the task returned from an asynchronous method.");

            // Can't call this method on a task that has already completed
            if (IsCompleted)
            {
                throw new InvalidOperationException("RunSynchronously may not be called on a task that has already completed.");
            }

            if (IsScheduled)
            {
                throw new InvalidOperationException("RunSynchronously may not be called on a task that was already started.");
            }

            // Make sure that Task only gets started once.  Or else throw an exception.
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingForActivation, (int)TaskStatus.Created) != (int)TaskStatus.Created)
            {
                throw new InvalidOperationException("RunSynchronously may not be called on a task that was already started.");
            }

            try
            {
                Schedule(_scheduler);
                while (!IsCompleted)
                {
                    _scheduler.RunAndWait(this, false);
                }
            }
            catch (Exception exception)
            {
                if (exception is ThreadAbortException)
                {
                    throw;
                }

                // Record the exception, marking ourselves as Completed/Faulted.
                var taskSchedulerException = new TaskSchedulerException(exception);
                AddException(taskSchedulerException);
                Finish(false);

                const string assertFailure = "Task.PrivateRunSynchronously(): Expected _exceptionsHolder to exist and to have faults recorded.";
                if (_exceptionsHolder != null)
                {
                    Contract.Assert(_exceptionsHolder.ContainsFaultList, assertFailure);
                    _exceptionsHolder.MarkAsHandled(false);
                }
                else
                {
                    Contract.Assert(false, assertFailure);
                }

                // And re-throw.
                throw taskSchedulerException;
                // We had a problem with waiting or this is a thread abort.  Just re-throw.
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
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
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
            while (true)
            {
                if (IsScheduled)
                {
                    _scheduler.RunAndWait(this, true);
                }
                if (IsCompleted)
                {
                    return;
                }
            }
        }

        public void Wait(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            while (true)
            {
                if (IsScheduled)
                {
                    _scheduler.RunAndWait(this, true);
                }
                if (IsCompleted)
                {
                    return;
                }
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
            var start = ThreadingHelper.TicksNow();
            while (true)
            {
                if (IsScheduled)
                {
                    _scheduler.RunAndWait(this, true);
                }
                if (IsCompleted)
                {
                    return true;
                }
                if (milliseconds != -1)
                {
                    if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) >= milliseconds)
                    {
                        return IsCompleted;
                    }
                }
            }
        }

        public bool Wait(TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var start = ThreadingHelper.TicksNow();
            while (true)
            {
                if (IsScheduled)
                {
                    _scheduler.RunAndWait(this, true);
                }
                if (IsCompleted)
                {
                    return true;
                }
                if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) >= milliseconds)
                {
                    return IsCompleted;
                }
            }
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
            while (true)
            {
                if (IsScheduled)
                {
                    _scheduler.RunAndWait(this, true);
                }
                if (IsCompleted)
                {
                    return true;
                }
                if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) >= milliseconds)
                {
                    return IsCompleted;
                }
                if (!IsCompleted)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    GC.KeepAlive(cancellationToken.WaitHandle);
                }
            }
        }

        internal bool ExecuteEntry(bool preventDoubleExecution)
        {
            if (!SetRunning(preventDoubleExecution))
            {
                return false;
            }
            if (!IsCanceled)
            {
                if (Token.IsCancellationRequested)
                {
                    Thread.VolatileWrite(ref _status, (int)TaskStatus.Canceled);
                    SetCompleted();
                    FinishStageThree();
                }
                else
                {
                    ExecuteWithThreadLocal();
                }
            }
            return true;
        }

        internal bool InternalCancel(bool cancelNonExecutingOnly)
        {
            // TODO: Promise tasks support?
            bool popSucceeded = false;
            bool cancelSucceeded = false;
            TaskSchedulerException taskSchedulerException = null;

            RecordInternalCancellationRequest();

            var status = Thread.VolatileRead(ref _status);
            if (status <= (int)TaskStatus.WaitingToRun)
            {
                // Note: status may advance to TaskStatus.Running or even TaskStatus.RanToCompletion during the execution of this method
                var scheduler = _scheduler;
                var requiresAtomicStartTransition = scheduler.RequiresAtomicStartTransition;
                try
                {
                    popSucceeded = scheduler.TryDequeue(this);
                }
                catch (Exception exception)
                {
                    if (exception is InternalSpecialCancelException)
                    {
                        // Special path for ThreadPool
                        requiresAtomicStartTransition = true;
                    }
                    else if (exception is ThreadAbortException)
                    {
                        // Ignore the exception
                    }
                    else
                    {
                        taskSchedulerException = new TaskSchedulerException(exception);
                    }
                }
                if (!popSucceeded && requiresAtomicStartTransition)
                {
                    cancelSucceeded = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.Created) == (int)TaskStatus.WaitingToRun;
                    cancelSucceeded = cancelSucceeded || Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingForActivation) == (int)TaskStatus.WaitingToRun;
                    cancelSucceeded = cancelSucceeded || Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingToRun) == (int)TaskStatus.WaitingToRun;
                }
            }
            if (Thread.VolatileRead(ref _status) >= (int)TaskStatus.Running && !cancelNonExecutingOnly)
            {
                // We are going to pretend that the cancel call came after the task finished running, but we may still set to cancel on TaskStatus.WaitingForChildrenToComplete
                cancelSucceeded = cancelSucceeded || Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingForChildrenToComplete) == (int)TaskStatus.WaitingForChildrenToComplete;
            }
            if (cancelSucceeded)
            {
                SetCompleted();
                FinishStageThree();
            }
            if (taskSchedulerException != null)
            {
                throw taskSchedulerException;
            }
            else
            {
                return cancelSucceeded;
            }
        }

        internal void InternalStart(TaskScheduler scheduler)
        {
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingForActivation, (int)TaskStatus.Created) == (int)TaskStatus.Created)
            {
                Schedule(scheduler);
            }
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
            scheduler.QueueTask(this);
            // If _status is no longer TaskStatus.WaitingForActivation it means that it is already TaskStatus.Running or beyond
            Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingToRun, (int)TaskStatus.WaitingForActivation);
        }

        private void SetCompleted()
        {
            var handle = _waitHandle.Value;
            if (_waitHandle.IsAlive)
            {
                handle.Set();
            }
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
            var count = 0;
        retry:
            var lastValue = Thread.VolatileRead(ref _status);
            if ((preventDoubleExecution && lastValue >= 3) || lastValue == 6)
            {
                return false;
            }
            var tmp = Interlocked.CompareExchange(ref _status, 3, lastValue);
            if (tmp == lastValue)
            {
                return true;
            }
            ThreadingHelper.SpinOnce(ref count);
            goto retry;
        }
    }
}

#endif