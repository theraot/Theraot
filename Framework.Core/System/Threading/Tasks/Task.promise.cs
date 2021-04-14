#if LESSTHAN_NET40

#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Theraot.Core;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        private Action _promiseCheck = ActionHelper.GetNoopAction();

        internal Task(TaskStatus taskStatus, InternalTaskOptions internalTaskOptions)
        {
            _status = (int)taskStatus;
            _internalOptions = internalTaskOptions;
            ExecutingTaskScheduler = TaskScheduler.Default;
            _waitHandle = new ManualResetEventSlim(initialState: false);
        }

        internal Task()
        {
            _status = (int)TaskStatus.WaitingForActivation;
            _internalOptions = InternalTaskOptions.PromiseTask;
            ExecutingTaskScheduler = TaskScheduler.Default;
            _waitHandle = new ManualResetEventSlim(initialState: false);
        }

        internal Task(TaskCreationOptions creationOptions, object? state)
        {
            if ((creationOptions & ~(TaskCreationOptions.AttachedToParent | TaskCreationOptions.RunContinuationsAsynchronously)) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(creationOptions));
            }

            // Only set a parent if AttachedToParent is specified.
            if ((creationOptions & TaskCreationOptions.AttachedToParent) != 0)
            {
                _parent = InternalCurrent;
            }

            State = state;
            CreationOptions = creationOptions;
            _status = (int)TaskStatus.WaitingForActivation;
            _internalOptions = InternalTaskOptions.PromiseTask;
            ExecutingTaskScheduler = TaskScheduler.Default;
            _waitHandle = new ManualResetEventSlim(initialState: false);
        }

        public static Task FromCanceled(CancellationToken cancellationToken)
        {
            return TaskExEx.FromCanceled(cancellationToken);
        }

        public static Task<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
        {
            return TaskExEx.FromCanceled<TResult>(cancellationToken);
        }

        public static Task FromException(Exception exception)
        {
            return TaskExEx.FromException(exception);
        }

        public static Task<TResult> FromException<TResult>(Exception exception)
        {
            return TaskExEx.FromException<TResult>(exception);
        }

        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            return new Task<TResult>(result);
        }

        internal bool SetCompleted(bool preventDoubleExecution)
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
            var spinWait = new SpinWait();
            while (true)
            {
                var lastValue = Volatile.Read(ref _status);
                if ((preventDoubleExecution && lastValue >= 3) || lastValue == 6)
                {
                    return false;
                }

                var tmp = Interlocked.CompareExchange(ref _status, 5, lastValue);
                if (tmp == lastValue)
                {
                    return true;
                }

                spinWait.SpinOnce();
            }
        }

        internal void SetPromiseCheck(Action value)
        {
            _promiseCheck = value ?? ActionHelper.GetNoopAction();
        }

        internal bool TrySetCanceled(CancellationToken cancellationToken)
        {
            if (IsCompleted)
            {
                return false;
            }

            CancellationToken = cancellationToken;
            try
            {
                // If an unstarted task has a valid CancellationToken that gets signalled while the task is still not queued
                // we need to proactively cancel it, because it may never execute to transition itself.
                // The only way to accomplish this is to register a callback on the CT.

                if (cancellationToken.IsCancellationRequested)
                {
                    // Fast path for an already-canceled cancellationToken
                    InternalCancel(cancelNonExecutingOnly: false);
                }
                else
                {
                    RecordInternalCancellationRequest();
                    Volatile.Write(ref _status, (int)TaskStatus.Canceled);

                    // Regular path for an uncanceled cancellationToken
                    var registration = cancellationToken.Register(_taskCancelCallback, this);
                    _cancellationRegistration = new StrongBox<CancellationTokenRegistration>(registration);
                }

                return true;
            }
            catch (Exception)
            {
                // If we have an exception related to our CancellationToken, then we need to subtract ourselves
                // from our parent before throwing it.
                if
                (
                    _parent != null
                    && (CreationOptions & TaskCreationOptions.AttachedToParent) != 0
                    && (_parent.CreationOptions & TaskCreationOptions.DenyChildAttach) == 0
                )
                {
                    _parent.DisregardChild();
                }

                throw;
            }
        }

        internal bool TrySetCanceledPromise(CancellationToken tokenToRecord)
        {
            Contract.Assert(Action == null);
            var returnValue = false;
            Contract.Assert(IsPromiseTask, "Task.RecordInternalCancellationRequest(CancellationToken) only valid for promise-style task");
            Contract.Assert(CancellationToken == default);
            if (tokenToRecord != default)
            {
                CancellationToken = tokenToRecord;
            }

            returnValue |= InternalCancel(cancelNonExecutingOnly: false);
            return returnValue;
        }

        internal bool TrySetException(Exception exception)
        {
            AddException(exception);
            var status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Faulted, (int)TaskStatus.WaitingForActivation);
            var succeeded = status == (int)TaskStatus.WaitingForActivation;
            if (!succeeded)
            {
                return false;
            }

            MarkCompleted();
            FinishStageThree();
            return true;
        }

        internal bool TrySetException(IEnumerable<Exception> exceptions)
        {
            foreach (var exception in exceptions)
            {
                AddException(exception);
            }

            var status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Faulted, (int)TaskStatus.WaitingForActivation);
            var succeeded = status == (int)TaskStatus.WaitingForActivation;
            if (!succeeded)
            {
                return false;
            }

            MarkCompleted();
            FinishStageThree();
            return true;
        }

        internal bool TrySetException(IEnumerable<ExceptionDispatchInfo> exceptions)
        {
            foreach (var exception in exceptions)
            {
                AddException(exception);
            }

            var status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Faulted, (int)TaskStatus.WaitingForActivation);
            var succeeded = status == (int)TaskStatus.WaitingForActivation;
            if (!succeeded)
            {
                return false;
            }

            MarkCompleted();
            FinishStageThree();
            return true;
        }

        internal bool TrySetResult()
        {
            if (IsFaulted)
            {
                return false;
            }

            if (IsCanceled)
            {
                return false;
            }

            if (!SetCompleted(preventDoubleExecution: true))
            {
                return false;
            }

            MarkCompleted();
            FinishStageThree();
            return true;
        }

        private void PromiseCheck()
        {
            _promiseCheck.Invoke();
        }
    }

    public partial class Task<TResult>
    {
        internal Task(TResult result)
            : base(TaskStatus.RanToCompletion, InternalTaskOptions.DoNotDispose)
        {
            InternalResult = result;
        }

        internal Task(TaskCreationOptions creationOptions, object? state)
            : base(creationOptions, state)
        {
            // Empty
        }

        internal bool TrySetResult(TResult result)
        {
            if (IsFaulted)
            {
                return false;
            }

            if (IsCanceled)
            {
                return false;
            }

            if (!SetCompleted(preventDoubleExecution: true))
            {
                return false;
            }

            InternalResult = result;
            MarkCompleted();
            FinishStageThree();
            return true;
        }
    }
}

#endif