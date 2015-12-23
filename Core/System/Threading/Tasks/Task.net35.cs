using System.Diagnostics.Contracts;
using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public partial class Task : IDisposable, IAsyncResult, IThreadPoolWorkItem
    {
        [ThreadStatic]
        internal static Task Current;

        internal TaskScheduler Scheduler;
        protected readonly object State;
        protected object Action;
        private static int _lastId;
        private readonly TaskCreationOptions _creationOptions;
        private readonly int _id;
        private readonly InternalTaskOptions _internalOptions;
        private readonly Task _parent;
        private int _isDisposed;
        private int _status;
        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public Task(Action action)
            : this(action, null, null, default(CancellationToken), TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Action action, CancellationToken cancellationToken)
            : this(action, null, null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Action action, TaskCreationOptions creationOptions)
            : this(action, null, InternalCurrentIfAttached(creationOptions), default(CancellationToken), creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : this(action, null, InternalCurrentIfAttached(creationOptions), cancellationToken, creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        internal Task(Action<object> action, object state, Task parent, CancellationToken cancellationToken, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions, TaskScheduler scheduler)
            : this((Delegate)action, state, parent, cancellationToken, creationOptions, internalOptions, scheduler)
        {
            CapturedContext = ExecutionContext.Capture();
        }

        internal Task(Action action, Task parent, CancellationToken cancellationToken, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions, TaskScheduler scheduler)
            : this(action, null, parent, cancellationToken, creationOptions, internalOptions, scheduler)
        {
            CapturedContext = ExecutionContext.Capture();
        }

        /// <summary>
        /// An internal constructor used by the factory methods on task and its descendent(s).
        /// This variant does not capture the ExecutionContext; it is up to the caller to do that.
        /// </summary>
        /// <param name="action">An action to execute.</param>
        /// <param name="state">Optional state to pass to the action.</param>
        /// <param name="parent">Parent of Task.</param>
        /// <param name="cancellationToken">A CancellationToken for the task.</param>
        /// <param name="scheduler">A task scheduler under which the task will run.</param>
        /// <param name="creationOptions">Options to control its execution.</param>
        /// <param name="internalOptions">Internal options to control its execution</param>
        internal Task(Delegate action, object state, Task parent, CancellationToken cancellationToken, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions, TaskScheduler scheduler)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (ReferenceEquals(scheduler, null))
            {
                throw new ArgumentNullException("scheduler");
            }
            Contract.EndContractBlock();
            // This is readonly, and so must be set in the constructor
            // Keep a link to your parent if: (A) You are attached, or (B) you are self-replicating.
            if
                (
                ((creationOptions & TaskCreationOptions.AttachedToParent) != 0)
                || ((internalOptions & InternalTaskOptions.SelfReplicating) != 0)
                )
            {
                _parent = parent;
            }
            _id = Interlocked.Increment(ref _lastId) - 1;
            _status = (int)TaskStatus.Created;
            if
            (
                _parent != null
                && ((creationOptions & TaskCreationOptions.AttachedToParent) != 0)
                && ((_parent.CreationOptions & TaskCreationOptions.DenyChildAttach) == 0)
            )
            {
                _parent.AddNewChild();
            }
            Action = action;
            State = state;
            Scheduler = scheduler;
            _waitHandle = new ManualResetEventSlim(false);
            if ((creationOptions &
                    ~(TaskCreationOptions.AttachedToParent |
                      TaskCreationOptions.LongRunning |
                      TaskCreationOptions.DenyChildAttach |
                      TaskCreationOptions.HideScheduler |
                      TaskCreationOptions.PreferFairness |
                      TaskCreationOptions.RunContinuationsAsynchronously)) != 0)
            {
                throw new ArgumentOutOfRangeException("creationOptions");
            }
            // Throw exception if the user specifies both LongRunning and SelfReplicating
            if (((creationOptions & TaskCreationOptions.LongRunning) != 0) &&
                ((internalOptions & InternalTaskOptions.SelfReplicating) != 0))
            {
                throw new InvalidOperationException("An attempt was made to create a LongRunning SelfReplicating task.");
            }
            if ((internalOptions & InternalTaskOptions.ContinuationTask) != 0)
            {
                // For continuation tasks or TaskCompletionSource.Tasks, begin life in the
                // WaitingForActivation state rather than the Created state.
                _status = (int)TaskStatus.WaitingForActivation;
            }
            _creationOptions = creationOptions;
            _internalOptions = internalOptions;
            // if we have a non-null cancellationToken, allocate the contingent properties to save it
            // we need to do this as the very last thing in the construction path, because the CT registration could modify m_stateFlags
            if (cancellationToken.CanBeCanceled)
            {
                AssignCancellationToken(cancellationToken, null, null);
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
                var current = Current;
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
                return TaskFactory.DefaultInstance;
            }
        }

        public object AsyncState
        {
            get
            {
                return State;
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

        internal CancellationToken CancellationToken { get; set; }

        internal ExecutionContext CapturedContext { get; set; }

        private bool IsContinuationTask
        {
            get
            {
                return (_internalOptions & InternalTaskOptions.ContinuationTask) != 0;
            }
        }

        private bool IsPromiseTask
        {
            get
            {
                return (_internalOptions & InternalTaskOptions.PromiseTask) != 0;
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

        void IThreadPoolWorkItem.ExecuteWorkItem()
        {
            ExecuteEntry(false);
        }

        void IThreadPoolWorkItem.MarkAborted(ThreadAbortException exception)
        {
            if (!IsCompleted)
            {
                HandleException(exception);
                FinishThreadAbortedTask(true, false);
            }
        }

        public void RunSynchronously()
        {
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            PrivateRunSynchronously(Scheduler);
        }

        public void RunSynchronously(TaskScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            PrivateRunSynchronously(scheduler);
        }

        public void Start()
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("Start may not be called on a task that has completed.");
            }
            if ((_internalOptions & InternalTaskOptions.ContinuationTask) != 0)
            {
                throw new InvalidOperationException("Start may not be called on a continuation task.");
            }
            // TODO: Start may not be called on a promise-style task.
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (!PrivateStart(Scheduler, false, true))
            {
                throw new InvalidOperationException("Start may not be called on a task that was already started.");
            }
        }

        public void Start(TaskScheduler scheduler)
        {
            if (IsCompleted)
            {
                throw new InvalidOperationException("Start may not be called on a task that has completed.");
            }
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
            if ((_internalOptions & InternalTaskOptions.ContinuationTask) != 0)
            {
                throw new InvalidOperationException("Start may not be called on a continuation task.");
            }
            // TODO: Start may not be called on a promise-style task.
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (!PrivateStart(scheduler, false, true))
            {
                throw new InvalidOperationException("Start may not be called on a task that was already started.");
            }
        }

        public void Wait()
        {
            Wait(CancellationToken);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            PrivateWait(cancellationToken, true);
        }

        public bool Wait(int milliseconds)
        {
            return Wait(milliseconds, CancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            var milliseconds = (int)timeout.TotalMilliseconds;
            return Wait(milliseconds, CancellationToken);
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
            var start = ThreadingHelper.TicksNow();
            do
            {
                CancellationCheck(cancellationToken);
                switch (Status)
                {
                    case TaskStatus.WaitingToRun:
                        Scheduler.TryExecuteTaskInline(this, true);
                        break;

                    case TaskStatus.Created:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForChildrenToComplete:
                        var waitHandle = _waitHandle.Value;
                        if (_waitHandle.IsAlive)
                        {
                            waitHandle.Wait
                                (
                                    TimeSpan.FromMilliseconds
                                    (
                                        milliseconds - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start)
                                    ),
                                    cancellationToken
                                );
                        }
                        break;

                    case TaskStatus.RanToCompletion:
                    case TaskStatus.Canceled:
                        return true;

                    case TaskStatus.Faulted:
                        ThrowIfExceptional(true);
                        return true;
                }
            } while (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) < milliseconds);
            return false;
        }

        internal static Task InternalCurrentIfAttached(TaskCreationOptions creationOptions)
        {
            return (creationOptions & TaskCreationOptions.AttachedToParent) != 0 ? Current : null;
        }

        internal bool ExecuteEntry(bool preventDoubleExecution)
        {
            if (!SetRunning(preventDoubleExecution))
            {
                return false;
            }
            if (!IsCanceled)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    Thread.VolatileWrite(ref _status, (int)TaskStatus.Canceled);
                    MarkCompleted();
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
            Contract.Requires((_internalOptions & InternalTaskOptions.PromiseTask) == 0, "Task.InternalCancel() did not expect promise-style task");

            var popSucceeded = false;
            var cancelSucceeded = false;
            TaskSchedulerException taskSchedulerException = null;

            RecordInternalCancellationRequest();

            var status = Thread.VolatileRead(ref _status);
            if (status <= (int)TaskStatus.WaitingToRun)
            {
                // Note: status may advance to TaskStatus.Running or even TaskStatus.RanToCompletion during the execution of this method
                var scheduler = Scheduler;
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
                    status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.Created);
                    cancelSucceeded = status == (int)TaskStatus.Created;
                    status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingForActivation);
                    cancelSucceeded = cancelSucceeded || status == (int)TaskStatus.WaitingForActivation;
                    status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingToRun);
                    cancelSucceeded = cancelSucceeded || status == (int)TaskStatus.WaitingToRun;
                }
            }
            if (Thread.VolatileRead(ref _status) >= (int)TaskStatus.Running && !cancelNonExecutingOnly)
            {
                // We are going to pretend that the cancel call came after the task finished running, but we may still set to cancel on TaskStatus.WaitingForChildrenToComplete
                status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingForChildrenToComplete);
                cancelSucceeded = cancelSucceeded || status == (int)TaskStatus.WaitingForChildrenToComplete;
            }
            if (cancelSucceeded)
            {
                MarkCompleted();
                FinishStageThree();
            }
            if (taskSchedulerException != null)
            {
                throw taskSchedulerException;
            }
            return cancelSucceeded;
        }

        internal void MarkCompleted()
        {
            var waitHandle = _waitHandle.Value;
            if (_waitHandle.IsAlive)
            {
                waitHandle.Set();
            }
        }

        internal void Start(TaskScheduler scheduler, bool inline)
        {
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            PrivateStart(scheduler, inline, true);
        }

        internal void Start(TaskScheduler scheduler, bool inline, bool throwSchedulerExceptions)
        {
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            PrivateStart(scheduler, inline, throwSchedulerExceptions);
        }

        internal bool TryStart(TaskScheduler scheduler, bool inline)
        {
            if (Thread.VolatileRead(ref _isDisposed) == 1)
            {
                return false;
            }
            return PrivateStart(scheduler, inline, true);
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

        private void CancellationCheck(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                GC.KeepAlive(cancellationToken.WaitHandle);
            }
            catch (NewOperationCanceledException)
            {
                throw new AggregateException(new TaskCanceledException(this));
            }
        }

        private void PrivateRunSynchronously(TaskScheduler scheduler)
        {
            // Do not Run Synchronously Continuation Tasks
            if (IsContinuationTask)
            {
                throw new InvalidOperationException("RunSynchronously may not be called on a continuation task.");
            }
            // Do not Run Synchronously Promise Tasks
            if (IsPromiseTask)
            {
                throw new InvalidOperationException("RunSynchronously may not be called on a task not bound to a delegate, such as the task returned from an asynchronous method.");
            }
            // Can't call this method on a task that has already completed
            if (IsCompleted)
            {
                throw new InvalidOperationException("RunSynchronously may not be called on a task that has already completed.");
            }
            // Make sure that Task only gets started once.  Or else throw an exception.
            if (!PrivateStart(scheduler, true, true))
            {
                throw new InvalidOperationException("RunSynchronously may not be called on a task that was already started.");
            }
        }

        private bool PrivateStart(TaskScheduler scheduler, bool inline, bool throwSchedulerExceptions)
        {
            Scheduler = scheduler;
            var result = Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingForActivation, (int)TaskStatus.Created);
            if (result != (int)TaskStatus.Created && result != (int)TaskStatus.WaitingForActivation)
            {
                return false;
            }
            var didInline = false;
            try
            {
                if (inline)
                {
                    didInline = scheduler.TryExecuteTaskInline(this, IsScheduled);
                }
                if (!didInline)
                {
                    scheduler.QueueTask(this);
                    Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingToRun, (int)TaskStatus.WaitingForActivation);
                }
                else
                {
                    PrivateWait(CancellationToken, false);
                }
            }
            catch (ThreadAbortException exception)
            {
                if (!didInline)
                {
                    AddException(exception);
                    FinishThreadAbortedTask(true, false);
                }
            }
            catch (Exception exception)
            {
                var taskSchedulerException = new TaskSchedulerException(exception);
                RecordException(taskSchedulerException);
                if (throwSchedulerExceptions)
                {
                    throw taskSchedulerException;
                }
            }
            return true;
        }

        private void PrivateWait(CancellationToken cancellationToken, bool throwIfExceptional)
        {
            while (true)
            {
                CancellationCheck(cancellationToken);
                switch (Status)
                {
                    case TaskStatus.WaitingToRun:
                        Scheduler.TryExecuteTaskInline(this, true);
                        break;

                    case TaskStatus.Created:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForChildrenToComplete:
                        var waitHandle = _waitHandle.Value;
                        if (_waitHandle.IsAlive)
                        {
                            waitHandle.Wait(cancellationToken);
                        }
                        break;

                    case TaskStatus.RanToCompletion:
                    case TaskStatus.Canceled:
                        return;

                    case TaskStatus.Faulted:
                        if (throwIfExceptional)
                        {
                            ThrowIfExceptional(true);
                        }
                        return;
                }
            }
        }

        private void RecordException(TaskSchedulerException taskSchedulerException)
        {
            AddException(taskSchedulerException);
            Finish(false);
            if ((_internalOptions & InternalTaskOptions.ContinuationTask) != 0)
            {
                return;
            }
            if (_exceptionsHolder != null)
            {
                Contract.Assert(_exceptionsHolder.ContainsFaultList, "Expected _exceptionsHolder to have faults recorded.");
                _exceptionsHolder.MarkAsHandled(false);
            }
            else
            {
                Contract.Assert(false, "Expected _exceptionsHolder to exist.");
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
            while (true)
            {
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
            }
        }
    }
}