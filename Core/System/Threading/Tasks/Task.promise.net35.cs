#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        internal Task(TaskStatus taskStatus, InternalTaskOptions internalTaskOptions)
        {
            _status = (int)taskStatus;
            _internalOptions = internalTaskOptions;
        }

        internal Task()
        {
            _status = (int)TaskStatus.WaitingForActivation;
            _internalOptions = InternalTaskOptions.PromiseTask;
        }

        internal Task(object state, TaskCreationOptions creationOptions)
        {
            if ((creationOptions & ~(TaskCreationOptions.AttachedToParent | TaskCreationOptions.RunContinuationsAsynchronously)) != 0)
            {
                throw new ArgumentOutOfRangeException("creationOptions");
            }
            // Only set a parent if AttachedToParent is specified.
            if ((creationOptions & TaskCreationOptions.AttachedToParent) != 0)
            {
                _parent = Current;
            }
            State = state;
            _creationOptions = creationOptions;
            _status = (int)TaskStatus.WaitingForActivation;
            _internalOptions = InternalTaskOptions.PromiseTask;
            Scheduler = TaskScheduler.Default;
        }

        public static Task FromCancellation(CancellationToken token)
        {
            var result = new Task(TaskStatus.WaitingForActivation, InternalTaskOptions.PromiseTask)
            {
                CancellationToken = token,
                Scheduler = TaskScheduler.Default
            };
            if (token.IsCancellationRequested)
            {
                result.InternalCancel(false);
            }
            else if (token.CanBeCanceled)
            {
                token.Register(() => result.InternalCancel(false));
            }
            return result;
        }

        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            return new Task<TResult>(TaskStatus.RanToCompletion, InternalTaskOptions.DoNotDispose)
            {
                CancellationToken = default(CancellationToken),
                InternalResult = result
            };
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
            var count = 0;
            while (true)
            {
                var lastValue = Thread.VolatileRead(ref _status);
                if ((preventDoubleExecution && lastValue >= 3) || lastValue == 6)
                {
                    return false;
                }
                var tmp = Interlocked.CompareExchange(ref _status, 5, lastValue);
                if (tmp == lastValue)
                {
                    return true;
                }
                ThreadingHelper.SpinOnce(ref count);
            }
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
                    InternalCancel(false);
                }
                else
                {
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
                    (_parent != null)
                    && ((_creationOptions & TaskCreationOptions.AttachedToParent) != 0)
                    && ((_parent._creationOptions & TaskCreationOptions.DenyChildAttach) == 0)
                )
                {
                    _parent.DisregardChild();
                }
                throw;
            }
        }

        internal bool TrySetCanceled(CancellationToken tokenToRecord, object cancellationException)
        {
            Contract.Assert(Action == null, "Task<T>.TrySetCanceled(): non-null m_action");
            /*var ceAsEdi = cancellationException as ExceptionDispatchInfo;
            Contract.Assert(
                cancellationException == null ||
                cancellationException is OperationCanceledException ||
                (ceAsEdi != null && ceAsEdi.SourceException is OperationCanceledException),
                "Expected null or an OperationCanceledException");
            */
            var returnValue = false;
            // "Reserve" the completion for this task, while making sure that: (1) No prior reservation
            // has been made, (2) The result has not already been set, (3) An exception has not previously
            // been recorded, and (4) Cancellation has not been requested.
            //
            // If the reservation is successful, then record the cancellation and finish completion processing.
            //
            // Note: I had to access static Task variables through Task<object>
            // instead of Task, because I have a property named Task and that
            // was confusing the compiler.
            Contract.Assert(IsPromiseTask, "Task.RecordInternalCancellationRequest(CancellationToken) only valid for promise-style task");
            Contract.Assert(CancellationToken == default(CancellationToken));
            // Store the supplied cancellation token as this task's token.
            // Waiting on this task will then result in an OperationCanceledException containing this token.
            if (tokenToRecord != default(CancellationToken))
            {
                CancellationToken = tokenToRecord;
            }
            if (InternalCancel(false))
            {
                returnValue = true;
            }
            return returnValue;
        }

        internal bool TrySetException(Exception exception)
        {
            AddException(exception);
            var status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Faulted, (int)TaskStatus.WaitingForActivation);
            var succeeded = status == (int)TaskStatus.WaitingForActivation;
            if (succeeded)
            {
                MarkCompleted();
                FinishStageThree();
            }
            return succeeded;
        }

        internal bool TrySetException(IEnumerable<Exception> exceptions)
        {
            foreach (var exception in exceptions)
            {
                AddException(exception);
            }
            var status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Faulted, (int)TaskStatus.WaitingForActivation);
            var succeeded = status == (int)TaskStatus.WaitingForActivation;
            if (succeeded)
            {
                MarkCompleted();
                FinishStageThree();
            }
            return succeeded;
        }

        internal bool TrySetException(IEnumerable<ExceptionDispatchInfo> exceptions)
        {
            foreach (var exception in exceptions)
            {
                AddException(exception);
            }
            var status = Interlocked.CompareExchange(ref _status, (int)TaskStatus.Faulted, (int)TaskStatus.WaitingForActivation);
            var succeeded = status == (int)TaskStatus.WaitingForActivation;
            if (succeeded)
            {
                MarkCompleted();
                FinishStageThree();
            }
            return succeeded;
        }

        private static Task CreateCompletedTask()
        {
            return new Task(TaskStatus.RanToCompletion, InternalTaskOptions.DoNotDispose)
            {
                CancellationToken = default(CancellationToken)
            };
        }
    }

    public partial class Task<TResult>
    {
        internal Task(TaskStatus taskStatus, InternalTaskOptions internalTaskOptions)
            : base(taskStatus, internalTaskOptions)
        {
            // Empty
        }

        internal Task()
        {
            // Empty
        }

        internal Task(object state, TaskCreationOptions creationOptions)
            : base(state, creationOptions)
        {
            // Empty
        }

        public new static Task<TResult> FromCancellation(CancellationToken token)
        {
            var result = new Task<TResult>(TaskStatus.WaitingForActivation, InternalTaskOptions.PromiseTask)
            {
                CancellationToken = token,
                Scheduler = TaskScheduler.Default
            };
            if (token.IsCancellationRequested)
            {
                result.InternalCancel(false);
            }
            else if (token.CanBeCanceled)
            {
                token.Register(() => result.InternalCancel(false));
            }
            return result;
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
            if (SetCompleted(true))
            {
                InternalResult = result;
                MarkCompleted();
                FinishStageThree();
                return true;
            }
            return false;
        }
    }
}

#endif