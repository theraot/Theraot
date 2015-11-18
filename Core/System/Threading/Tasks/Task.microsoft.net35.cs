#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Security;
using Theraot.Core;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        private readonly static Predicate<Task> _isExceptionObservedByParentPredicate = (t => t.IsExceptionObservedByParent);
        private readonly static Action<object> _taskCancelCallback = (TaskCancelCallback);

        private int _cancellationAcknowledged;
        private StrongBox<CancellationTokenRegistration> _cancellationRegistration;
        private int _cancellationRequested;
        private int _completionCountdown = 1;
        private List<Task> _exceptionalChildren;
        private int _exceptionObservedByParent;
        private TaskExceptionHolder _exceptionsHolder;
        private int _threadAbortedmanaged;

        /// <summary>
        /// The property formerly known as IsFaulted.
        /// </summary>
        internal bool ExceptionRecorded
        {
            get
            {
                var exceptionsHolder = ThreadingHelper.VolatileRead(ref _exceptionsHolder);
                return exceptionsHolder != null && exceptionsHolder.ContainsFaultList;
            }
        }

        internal bool IsCancellationAcknowledged
        {
            get
            {
                return Thread.VolatileRead(ref _cancellationAcknowledged) == 1;
            }
        }

        internal bool IsCancellationRequested
        {
            get
            {
                return Thread.VolatileRead(ref _cancellationRequested) == 1;
            }
        }

        internal bool IsChildReplica
        {
            get
            {
                return (_internalOptions & InternalTaskOptions.ChildReplica) != 0;
            }
        }

        /// <summary>
        /// Checks whether the TASK_STATE_EXCEPTIONOBSERVEDBYPARENT status flag is set,
        /// This will only be used by the implicit wait to prevent double throws
        /// </summary>
        internal bool IsExceptionObservedByParent
        {
            get
            {
                return Thread.VolatileRead(ref _exceptionObservedByParent) == 1;
            }
        }

        internal bool IsSelfReplicatingRoot
        {
            get
            {
                // Return true if self-replicating bit is set and child replica bit is not set
                return (_internalOptions & (InternalTaskOptions.SelfReplicating | InternalTaskOptions.ChildReplica)) == InternalTaskOptions.SelfReplicating;
            }
        }

        /// <summary>
        /// This is to be called just before the task does its final state transition.
        /// It traverses the list of exceptional children, and appends their aggregate exceptions into this one's exception list
        /// </summary>
        internal void AddExceptionsFromChildren()
        {
            // In rare occurences during AppDomainUnload() processing, it is possible for this method to be called
            // simultaneously on the same task from two different contexts.  This can result in m_exceptionalChildren
            // being nulled out while it is being processed, which could lead to a NullReferenceException.  To
            // protect ourselves, we'll cache m_exceptionalChildren in a local variable.
            var tmp = ThreadingHelper.VolatileRead(ref _exceptionalChildren);

            if (tmp != null)
            {
                // This lock is necessary because even though AddExceptionsFromChildren is last to execute, it may still
                // be racing with the code segment at the bottom of Finish() that prunes the exceptional child array.
                lock (tmp)
                {
                    foreach (var task in tmp)
                    {
                        // Ensure any exceptions thrown by children are added to the parent.
                        // In doing this, we are implicitly marking children as being "handled".
                        Contract.Assert(task.IsCompleted, "Expected all tasks in list to be completed");
                        if (task.IsFaulted && !task.IsExceptionObservedByParent)
                        {
                            var exceptionsHolder = ThreadingHelper.VolatileRead(ref task._exceptionsHolder);
                            if (exceptionsHolder == null)
                            {
                                Contract.Assert(false);
                            }
                            else
                            {
                                // No locking necessary since child task is finished adding exceptions
                                // and concurrent CreateExceptionObject() calls do not constitute
                                // a concurrency hazard.
                                AddException(exceptionsHolder.CreateExceptionObject(false, null));
                            }
                        }
                    }
                }

                // Reduce memory pressure by getting rid of the array
                ThreadingHelper.VolatileWrite(ref _exceptionalChildren, null);
            }
        }

        /// <summary>
        /// Checks if we registered a CT callback during construction, and deregisters it.
        /// This should be called when we know the registration isn't useful anymore. Specifically from Finish() if the task has completed
        /// successfully or with an exception.
        /// </summary>
        internal void DeregisterCancellationCallback()
        {
            if (_cancellationRegistration != null)
            {
                // Harden against ODEs thrown from disposing of the CTR.
                // Since the task has already been put into a final state by the time this
                // is called, all we can do here is suppress the exception.
                try
                {
                    _cancellationRegistration.Value.Dispose();
                }
                catch (ObjectDisposedException exception)
                {
                    GC.KeepAlive(exception);
                    // Empty
                }
                _cancellationRegistration = null;
            }
        }

        // This is called in the case where a new child is added, but then encounters a CancellationToken-related exception.
        // We need to subtract that child from m_completionCountdown, or the parent will never complete.
        internal void DisregardChild()
        {
            Contract.Assert(Current == this, "Task.DisregardChild(): Called from an external context");
            Contract.Assert(Thread.VolatileRead(ref _completionCountdown) >= 2, "Task.DisregardChild(): Expected parent count to be >= 2");
            Interlocked.Decrement(ref _completionCountdown);
        }

        internal void Finish(bool userDelegateExecuted)
        {
            if (!userDelegateExecuted)
            {
                // delegate didn't execute => no children. We can safely call the remaining finish stages
                FinishStageTwo();
            }
            else
            {
                // Reaching this sub clause means there may be remaining active children,
                // and we could be racing with one of them to call FinishStageTwo().
                // So whoever does the final Interlocked.Dec is responsible to finish.
                if ((_completionCountdown == 1 && !IsSelfReplicatingRoot) || Interlocked.Decrement(ref _completionCountdown) == 0)
                {
                    FinishStageTwo();
                }
                else
                {
                    // Apparently some children still remain. It will be up to the last one to process the completion of this task on their own thread.
                    // We will now yield the thread back to ThreadPool. Mark our state appropriately before getting out.

                    // We have to use an atomic update for this and make sure not to overwrite a final state,
                    // because at this very moment the last child's thread may be concurrently completing us.
                    // Otherwise we risk overwriting the TaskStatus which may have been set by that child task.

                    Interlocked.CompareExchange(ref _status, (int)TaskStatus.WaitingForChildrenToComplete, (int)TaskStatus.Running);
                }

                // Now is the time to prune exceptional children. We'll walk the list and removes the ones whose exceptions we might have observed after they threw.
                // we use a local variable for exceptional children here because some other thread may be nulling out _exceptionalChildren
                var exceptionalChildren = ThreadingHelper.VolatileRead(ref _exceptionalChildren);

                if (exceptionalChildren != null)
                {
                    lock (exceptionalChildren)
                    {
                        exceptionalChildren.RemoveAll(_isExceptionObservedByParentPredicate); // RemoveAll has better performance than doing it ourselves
                    }
                }
            }
        }

        // ASSUMES THAT A SUCCESSFUL CANCELLATION HAS JUST OCCURRED ON THIS TASK!!!
        // And this method should be called at most once per task.
        internal void FinishStageThree()
        {
            Action = null;
            // Notify parent if this was an attached task
            if (_parent != null && ((_parent._creationOptions & TaskCreationOptions.DenyChildAttach) == 0) && (_creationOptions & TaskCreationOptions.AttachedToParent) != 0)
            {
                _parent.ProcessChildCompletion(this);
            }
            // Activate continuations (if any).
            FinishContinuations();
        }

        /// <summary>
        /// FinishStageTwo is to be executed as soon as we known there are no more children to complete.
        /// It can happen i) either on the thread that originally executed this task (if no children were spawned, or they all completed by the time this task's delegate quit)
        ///              ii) or on the thread that executed the last child.
        /// </summary>
        internal void FinishStageTwo()
        {
            AddExceptionsFromChildren();

            // At this point, the task is done executing and waiting for its children,
            // we can transition our task to a completion state.
            TaskStatus completionState;
            if (ExceptionRecorded)
            {
                completionState = TaskStatus.Faulted;
            }
            else if (IsCancellationRequested && IsCancellationAcknowledged)
            {
                // We transition into the TASK_STATE_CANCELED final state if the task's CT was signalled for cancellation,
                // and the user delegate acknowledged the cancellation request by throwing an OCE,
                // and the task hasn't otherwise transitioned into faulted state. (TASK_STATE_FAULTED trumps TASK_STATE_CANCELED)
                //
                // If the task threw an OCE without cancellation being requestsed (while the CT not being in signaled state),
                // then we regard it as a regular exception

                completionState = TaskStatus.Canceled;
            }
            else
            {
                completionState = TaskStatus.RanToCompletion;
            }

            // Use Interlocked.Exchange() to effect a memory fence, preventing
            // any SetCompleted() (or later) instructions from sneak back before it.
            Interlocked.Exchange(ref _status, (int)completionState);

            // Set the completion event if it's been lazy allocated.
            // And if we made a cancellation registration, it's now unnecessary.
            SetCompleted();
            DeregisterCancellationCallback();

            // ready to run continuations and notify parent.
            FinishStageThree();
        }

        /// <summary>
        /// Special purpose Finish() entry point to be used when the task delegate throws a ThreadAbortedException
        /// This makes a note in the state flags so that we avoid any costly synchronous operations in the finish codepath
        /// such as inlined continuations
        /// 
        /// Assumes ThreadAbortException was added to this task's exception holder.
        /// This should always be true except for the case of non-root self replicating task copies.
        /// </summary>
        /// <param name="delegateRan">Whether the delegate was executed.</param>
        internal void FinishThreadAbortedTaskWithException(bool delegateRan)
        {
            if (Interlocked.CompareExchange(ref _threadAbortedmanaged, 1, 0) == 0)
            {
                var exceptionsHolder = ThreadingHelper.VolatileRead(ref _exceptionsHolder);
                if (exceptionsHolder != null)
                {
                    exceptionsHolder.MarkAsHandled(false);
                    Finish(delegateRan);
                }
                else
                {
                    Contract.Assert(false, "FinishThreadAbortedTask() called on a task whose exception holder wasn't initialized");
                }
            }
        }

        /// <summary>
        /// Special purpose Finish() entry point to be used when the task delegate throws a ThreadAbortedException
        /// This makes a note in the state flags so that we avoid any costly synchronous operations in the finish codepath
        /// such as inlined continuations
        /// 
        /// Assumes ThreadAbortException was NOT added to this task's exception holder.
        /// This should always be true except for the case of non-root self replicating task copies.
        /// </summary>
        /// <param name="delegateRan">Whether the delegate was executed.</param>
        internal void FinishThreadAbortedTaskWithoutException(bool delegateRan)
        {
            if (Interlocked.CompareExchange(ref _threadAbortedmanaged, 1, 0) == 0)
            {
                var exceptionsHolder = ThreadingHelper.VolatileRead(ref _exceptionsHolder);
                if (exceptionsHolder != null)
                {
                    Finish(delegateRan);
                }
            }
        }

        /// <summary>
        /// The actual code which invokes the body of the task. This can be overriden in derived types.
        /// </summary>
        internal virtual void InnerInvoke()
        {
            // Invoke the delegate
            Contract.Assert(Action != null, "Null action in InnerInvoke()");
            var action = Action as Action;
            if (action != null)
            {
                action.Invoke();
                return;
            }
            var actionWithState = Action as Action<object>;
            if (actionWithState != null)
            {
                actionWithState(State);
                return;
            }
            Contract.Assert(false, "Invalid m_action in Task");
        }

        internal void ProcessChildCompletion(Task childTask)
        {
            Contract.Requires(childTask != null);
            Contract.Requires(childTask.IsCompleted, "ProcessChildCompletion was called for an uncompleted task");

            Contract.Assert(childTask._parent == this, "ProcessChildCompletion should only be called for a child of this task");

            // if the child threw and we haven't observed it we need to save it for future reference
            if (childTask.IsFaulted && !childTask.IsExceptionObservedByParent)
            {
                // Lazily initialize the child exception list
                if (ThreadingHelper.VolatileRead(ref _exceptionalChildren) == null)
                {
                    Interlocked.CompareExchange(ref _exceptionalChildren, new List<Task>(), null);
                }

                // In rare situations involving AppDomainUnload, it's possible (though unlikely) for FinishStageTwo() to be called
                // multiple times for the same task.  In that case, AddExceptionsFromChildren() could be nulling m_exceptionalChildren
                // out at the same time that we're processing it, resulting in a NullReferenceException here.  We'll protect
                // ourselves by caching m_exceptionChildren in a local variable.
                var tmp = ThreadingHelper.VolatileRead(ref _exceptionalChildren);
                if (tmp != null)
                {
                    lock (tmp)
                    {
                        tmp.Add(childTask);
                    }
                }
            }

            if (Interlocked.Decrement(ref _completionCountdown) == 0)
            {
                // This call came from the final child to complete, and apparently we have previously given up this task's right to complete itself.
                // So we need to invoke the final finish stage.

                FinishStageTwo();
            }
        }

        internal void RecordInternalCancellationRequest()
        {
            Thread.VolatileWrite(ref _cancellationRequested, 1);
        }

        internal void SetCancellationAcknowledged()
        {
            Thread.VolatileWrite(ref _cancellationAcknowledged, 1);
        }

        /// <summary>
        /// Checks whether this is an attached task, and whether we are being called by the parent task.
        /// And sets the TASK_STATE_EXCEPTIONOBSERVEDBYPARENT status flag based on that.
        ///
        /// This is meant to be used internally when throwing an exception, and when WaitAll is gathering
        /// exceptions for tasks it waited on. If this flag gets set, the implicit wait on children
        /// will skip exceptions to prevent duplication.
        ///
        /// This should only be called when this task has completed with an exception
        ///
        /// </summary>
        internal void UpdateExceptionObservedStatus()
        {
            if ((_parent != null) && ((_creationOptions & TaskCreationOptions.AttachedToParent) != 0) && ((_parent._creationOptions & TaskCreationOptions.DenyChildAttach) == 0) && Current == _parent)
            {
                Thread.VolatileWrite(ref _exceptionObservedByParent, 1);
            }
        }

        private static void ExecutionContextCallback(object obj)
        {
            var task = obj as Task;
            if (task == null)
            {
                Contract.Assert(false, "expected a task object");
            }
            else
            {
                task.Execute();
            }
        }

        private static void TaskCancelCallback(object obj)
        {
            var task = obj as Task;
            if (task == null)
            {
                Contract.Assert(false,
                    "targetTask should have been non-null, with the supplied argument being a task or a tuple containing one");
            }
            else
            {
                task.CancelContinuations();
                task.InternalCancel(false);
            }
        }

        /// <summary>
        /// Internal function that will be called by a new child task to add itself to 
        /// the children list of the parent (this).
        /// 
        /// Since a child task can only be created from the thread executing the action delegate
        /// of this task, reentrancy is neither required nor supported. This should not be called from
        /// anywhere other than the task construction/initialization codepaths.
        /// </summary>
        private void AddNewChild()
        {
            Contract.Assert(Current == this || IsSelfReplicatingRoot, "Task.AddNewChild(): Called from an external context");

            if (_completionCountdown == 1 && !IsSelfReplicatingRoot)
            {
                // A count of 1 indicates so far there was only the parent, and this is the first child task
                // Single kid => no fuss about who else is accessing the count. Let's save ourselves 100 cycles
                // We exclude self replicating root tasks from this optimization, because further child creation can take place on 
                // other cores and with bad enough timing this write may not be visible to them.
                _completionCountdown++;
            }
            else
            {
                // otherwise do it safely
                Interlocked.Increment(ref _completionCountdown);
            }
        }

        private void AssignCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            try
            {
                cancellationToken.ThrowIfSourceDisposed();

                // If an unstarted task has a valid CancellationToken that gets signalled while the task is still not queued
                // we need to proactively cancel it, because it may never execute to transition itself.
                // The only way to accomplish this is to register a callback on the CT.
                // We exclude Promise tasks from this, because TaskCompletionSource needs to fully control the inner tasks's lifetime (i.e. not allow external cancellations)

                // Translation notes:
                // unstarted task (... that) is still not queued means a task on TaskStatus.Created or TaskStatus.WaitingForActivation
                // TODO: No support for promise tasks yet

                var status = Thread.VolatileRead(ref _status);
                if (status == (int)TaskStatus.Created || status == (int)TaskStatus.WaitingForActivation)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Fast path for an already-canceled cancellationToken
                        InternalCancel(false);
                    }
                    else
                    {
                        // Regular path for an uncanceled cancellationToken
                        _cancellationRegistration = new StrongBox<CancellationTokenRegistration>(cancellationToken.Register(_taskCancelCallback, this, false));
                    }
                }
            }
            catch (Exception)
            {
                // If we have an exception related to our CancellationToken, then we need to subtract ourselves
                // from our parent before throwing it.
                if ((_parent != null)
                    && ((_creationOptions & TaskCreationOptions.AttachedToParent) != 0)
                    && ((_parent._creationOptions & TaskCreationOptions.DenyChildAttach) == 0)
                )
                {
                    _parent.DisregardChild();
                }
                throw;
            }
        }

        /// <summary>
        /// Executes the task. This method will only be called once, and handles bookeeping associated with
        /// self-replicating tasks, in addition to performing necessary exception marshaling.
        /// </summary>
        private void Execute()
        {
            try
            {
                InnerInvoke();
            }
            catch (ThreadAbortException tae)
            {
                // Record this exception in the task's exception list
                HandleException(tae);

                // This is a ThreadAbortException and it will be rethrown from this catch clause, causing us to
                // skip the regular Finish codepath. In order not to leave the task unfinished, we now call
                // FinishThreadAbortedTask here.
                FinishThreadAbortedTaskWithException(true);
            }
            catch (Exception exn)
            {
                // Record this exception in the task's exception list
                HandleException(exn);
            }
        }

        [SecurityCritical]
        private void ExecuteWithThreadLocal()
        {
            // Remember the current task so we can restore it after running, and then
            var previousTask = Current;
            try
            {
                // place the current task into TLS.
                Current = this;
                var executionContext = CapturedContext;
                if (executionContext == null)
                {
                    Execute();
                }
                else
                {
                    if (IsSelfReplicatingRoot || IsChildReplica)
                    {
                        CapturedContext = executionContext.CreateCopy();
                    }
                    ExecutionContext.Run(executionContext, ExecutionContextCallback, this);
                }

                Finish(true);
            }
            finally
            {
                Current = previousTask;
            }
        }

        /// <summary>
        /// Returns a list of exceptions by aggregating the holder's contents. Or null if
        /// no exceptions have been thrown.
        /// </summary>
        /// <param name="includeTaskCanceledExceptions">Whether to include a TCE if cancelled.</param>
        /// <returns>An aggregate exception, or null if no exceptions have been caught.</returns>
        private AggregateException GetExceptions(bool includeTaskCanceledExceptions)
        {
            //
            // WARNING: The Task/Task<TResult>/TaskCompletionSource classes
            // have all been carefully crafted to insure that GetExceptions()
            // is never called while AddException() is being called.  There
            // are locks taken on m_contingentProperties in several places:
            //
            // -- Task<TResult>.TrySetException(): The lock allows the
            //    task to be set to Faulted state, and all exceptions to
            //    be recorded, in one atomic action.  
            //
            // -- Task.Exception_get(): The lock ensures that Task<TResult>.TrySetException()
            //    is allowed to complete its operation before Task.Exception_get()
            //    can access GetExceptions().
            //
            // -- Task.ThrowIfExceptional(): The lock insures that Wait() will
            //    not attempt to call GetExceptions() while Task<TResult>.TrySetException()
            //    is in the process of calling AddException().
            //
            // For "regular" tasks, we effectively keep AddException() and GetException()
            // from being called concurrently by the way that the state flows.  Until
            // a Task is marked Faulted, Task.Exception_get() returns null.  And
            // a Task is not marked Faulted until it and all of its children have
            // completed, which means that all exceptions have been recorded.
            //
            // It might be a lot easier to follow all of this if we just required
            // that all calls to GetExceptions() and AddExceptions() were made
            // under a lock on m_contingentProperties.  But that would also
            // increase our lock occupancy time and the frequency with which we
            // would need to take the lock.
            //
            // If you add a call to GetExceptions() anywhere in the code,
            // please continue to maintain the invariant that it can't be
            // called when AddException() is being called.
            //

            // We'll lazily create a TCE if the task has been canceled.
            Exception canceledException = null;
            if (includeTaskCanceledExceptions && IsCanceled)
            {
                // Backcompat: 
                // Ideally we'd just use the cached OCE from this.GetCancellationExceptionDispatchInfo()
                // here.  However, that would result in a potentially breaking change from .NET 4, which
                // has the code here that throws a new exception instead of the original, and the EDI
                // may not contain a TCE, but an OCE or any OCE-derived type, which would mean we'd be
                // propagating an exception of a different type.
                canceledException = new TaskCanceledException(this);
            }

            var exceptionsHolder = ThreadingHelper.VolatileRead(ref _exceptionsHolder);
            if (exceptionsHolder != null && exceptionsHolder.ContainsFaultList)
            {
                // No need to lock around this, as other logic prevents the consumption of exceptions
                // before they have been completely processed.
                return _exceptionsHolder.CreateExceptionObject(false, canceledException);
            }
            if (canceledException != null)
            {
                // No exceptions, but there was a cancelation. Aggregate and return it.
                return new AggregateException(canceledException);
            }

            return null;
        }
        /// <summary>
        /// Performs whatever handling is necessary for an unhandled exception. Normally
        /// this just entails adding the exception to the holder object.
        /// </summary>
        /// <param name="unhandledException">The exception that went unhandled.</param>
        private void HandleException(Exception unhandledException)
        {
            Contract.Requires(unhandledException != null);

            var exceptionAsOce = unhandledException as NewOperationCanceledException;
            if (exceptionAsOce != null && IsCancellationRequested && CancellationToken == exceptionAsOce.CancellationToken)
            {
                // All conditions are satisfied for us to go into canceled state in Finish().
                // Mark the acknowledgement.  The exception is also stored to enable it to be
                // the exception propagated from an await.

                SetCancellationAcknowledged();
                AddException(exceptionAsOce, representsCancellation: true);
            }
            else
            {
                // Other exceptions, including any OCE from the task that doesn't match the tasks' own CT,
                // or that gets thrown without the CT being set will be treated as an ordinary exception
                // and added to the aggregate.

                AddException(unhandledException);
            }
        }
    }

    public partial class Task
    {
        /// <summary>
        /// Adds an exception to the list of exceptions this task has thrown.
        /// </summary>
        /// <param name="exceptionObject">An object representing either an Exception or a collection of Exceptions.</param>
        /// <param name="representsCancellation">Whether the exceptionObject is an OperationCanceledException representing cancellation.</param>
        internal void AddException(object exceptionObject, bool representsCancellation)
        {
            Contract.Requires(exceptionObject != null, "Task.AddException: Expected a non-null exception object");

            //
            // WARNING: A great deal of care went into ensuring that
            // AddException() and GetExceptions() are never called
            // simultaneously.  See comment at start of GetExceptions().
            //

            // Lazily initialize the holder, ensuring only one thread wins.
            var exceptionsHolder = ThreadingHelper.VolatileRead(ref _exceptionsHolder);
            if (exceptionsHolder == null)
            {
                // This is the only time we write to _exceptionsHolder
                var holder = new TaskExceptionHolder(this);
                exceptionsHolder = Interlocked.CompareExchange(ref _exceptionsHolder, holder, null);
                if (exceptionsHolder == null)
                {
                    // The current thread did initialize _exceptionsHolder.
                    exceptionsHolder = holder;
                }
                else
                {
                    // Another thread initialized _exceptionsHolder first.
                    // Suppress finalization.
                    holder.MarkAsHandled(false);
                }
            }
            lock (exceptionsHolder)
            {
                exceptionsHolder.Add(exceptionObject, representsCancellation);
            }
        }

        private void AddException(object exceptionObject)
        {
            Contract.Requires(exceptionObject != null, "Task.AddException: Expected a non-null exception object");
            AddException(exceptionObject, representsCancellation: false);
        }
    }
}

#endif