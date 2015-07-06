#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
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
        private object _action;
        private readonly TaskCreationOptions _creationOptions;
        private readonly int _id;
        private readonly Task _parent;
        private readonly TaskScheduler _scheduler;
        private CancellationToken _cancellationToken;
        private ExecutionContext _capturedContext;
        private AggregateException _exception;
        private int _isDisposed = 0;
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
                if (_parent != null)
                {
                    _parent.AddChild(this);
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
                if (_current != null)
                {
                    return _current.Id;
                }
                else
                {
                    return null;
                }
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

        public TaskStatus Status
        {
            get
            {
                return (TaskStatus)Thread.VolatileRead(ref _status);
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
            if (milliseconds == -1)
            {
                Wait();
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            while(true)
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
                    return false;
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
                    return false;
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
                    return false;
                }
                if (!IsCompleted)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    GC.KeepAlive(cancellationToken.WaitHandle);
                }
            }
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
                if (scheduler == null)
                {
                    popSucceeded = false;
                }
                else
                {
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
                            // Pokémon
                        }
                        else
                        {
                            taskSchedulerException = new TaskSchedulerException(exception);
                        }
                    }
                    if (!popSucceeded && requiresAtomicStartTransition)
                    {
                        cancelSucceeded = cancelSucceeded || Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.Created) == (int)TaskStatus.WaitingToRun;
                        cancelSucceeded = cancelSucceeded || Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingForActivation) == (int)TaskStatus.WaitingToRun;
                        cancelSucceeded = cancelSucceeded || Interlocked.CompareExchange(ref _status, (int)TaskStatus.Canceled, (int)TaskStatus.WaitingToRun) == (int)TaskStatus.WaitingToRun;
                    }
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
                    SetCompleted();
                    FinishStageThree();
                }
                else
                {
                    ExecuteWithThreadLocal(ref _current);
                }
            }
            return true;
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

        private void AddChild(Task task)
        {
            // TODO
            throw new NotImplementedException();
        }

        private void AddException(Exception exception)
        {
            AggregateExceptionHelper.AddException(ref _exception, exception);
        }

        /// <summary>
        /// Adds an exception to the list of exceptions this task has thrown.
        /// </summary>
        /// <param name="exceptionObject">An object representing either an Exception or a collection of Exceptions.</param>
        /// <param name="representsCancellation">Whether the exceptionObject is an OperationCanceledException representing cancellation.</param>
        internal void AddException(object exceptionObject, bool representsCancellation)
        {
            Contract.Requires(exceptionObject != null, "Task.AddException: Expected a non-null exception object");

#if DEBUG
            var eoAsException = exceptionObject as Exception;
            var eoAsEnumerableException = exceptionObject as IEnumerable<Exception>;
            var eoAsEdi = exceptionObject as ExceptionDispatchInfo;
            var eoAsEnumerableEdi = exceptionObject as IEnumerable<ExceptionDispatchInfo>;

            Contract.Assert(
                eoAsException != null || eoAsEnumerableException != null || eoAsEdi != null || eoAsEnumerableEdi != null,
                "Task.AddException: Expected an Exception, ExceptionDispatchInfo, or an IEnumerable<> of one of those");

            var eoAsOce = exceptionObject as OperationCanceledException;

            Contract.Assert(
                !representsCancellation ||
                eoAsOce != null ||
                (eoAsEdi != null && eoAsEdi.SourceException is OperationCanceledException),
                "representsCancellation should be true only if an OCE was provided.");
#endif

            //
            // WARNING: A great deal of care went into ensuring that
            // AddException() and GetExceptions() are never called
            // simultaneously.  See comment at start of GetExceptions().
            //

            // Lazily initialize the holder, ensuring only one thread wins.
            if (_exceptionsHolder == null)
            {
                TaskExceptionHolder holder = new TaskExceptionHolder(this);
                if (Interlocked.CompareExchange(ref _exceptionsHolder, holder, null) != null)
                {
                    // If we lost the ----, suppress finalization.
                    holder.MarkAsHandled(false);
                }
            }
            lock (_exceptionsHolder)
            {
                _exceptionsHolder.Add(exceptionObject, representsCancellation);
            }
        }

        private void Schedule(TaskScheduler scheduler)
        {
            // Only called from Start where status is set to TaskStatus.WaitingForActivation
            _exception = null;
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