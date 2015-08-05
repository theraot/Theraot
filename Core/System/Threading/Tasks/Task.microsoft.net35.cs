#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using Theraot.Core;
using Theraot.Threading;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        internal StrongBox<CancellationTokenRegistration> _cancellationRegistration;
        private int _completionCountdown = 1;
        private List<Task> _exceptionalChildren;
        private TaskExceptionHolder _exceptionsHolder;
        private readonly static Predicate<Task> _IsExceptionObservedByParentPredicate = new Predicate<Task>((t) => { return t.IsExceptionObservedByParent; });
        private readonly static Action<object> _taskCancelCallback = new Action<object>(TaskCancelCallback);
        [SecurityCritical]
        private static ContextCallback _executionContextCallback;

        private int _cancellationAcknowledged;
        private int _cancellationRequested;
        private int _exceptionObservedByParent;
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
                    foreach (Task task in tmp)
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
                catch (ObjectDisposedException)
                {
                    // Empty
                }
                _cancellationRegistration = null;
            }
        }

        // This is called in the case where a new child is added, but then encounters a CancellationToken-related exception.
        // We need to subtract that child from m_completionCountdown, or the parent will never complete.
        internal void DisregardChild()
        {
            Contract.Assert(_current == this, "Task.DisregardChild(): Called from an external context");
            Contract.Assert(Thread.VolatileRead(ref _completionCountdown) >= 2, "Task.DisregardChild(): Expected parent count to be >= 2");
            Interlocked.Decrement(ref _completionCountdown);
        }

        /// <summary>
        /// Signals completion of this particular task.
        ///
        /// The bUserDelegateExecuted parameter indicates whether this Finish() call comes following the
        /// full execution of the user delegate. 
        /// 
        /// If bUserDelegateExecuted is false, it mean user delegate wasn't invoked at all (either due to
        /// a cancellation request, or because this task is a promise style Task). In this case, the steps
        /// involving child tasks (i.e. WaitForChildren) will be skipped.
        /// 
        /// </summary>
        internal void Finish(bool bUserDelegateExecuted)
        {
            if (!bUserDelegateExecuted)
            {
                // delegate didn't execute => no children. We can safely call the remaining finish stages
                FinishStageTwo();
            }
            else
            {
                // Reaching this sub clause means there may be remaining active children,
                // and we could be racing with one of them to call FinishStageTwo().
                // So whoever does the final Interlocked.Dec is responsible to finish.
                if (Interlocked.Decrement(ref _completionCountdown) == 0)
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
                        exceptionalChildren.RemoveAll(_IsExceptionObservedByParentPredicate); // RemoveAll has better performance than doing it ourselves
                    }
                }
            }
        }

        // ASSUMES THAT A SUCCESSFUL CANCELLATION HAS JUST OCCURRED ON THIS TASK!!!
        // And this method should be called at most once per task.
        internal void FinishStageThree()
        {
            _action = null;
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
        /// </summary>
        /// <param name="bTAEAddedToExceptionHolder">
        /// Indicates whether the ThreadAbortException was added to this task's exception holder. 
        /// This should always be true except for the case of non-root self replicating task copies.
        /// </param>
        /// <param name="delegateRan">Whether the delegate was executed.</param>
        internal void FinishThreadAbortedTask(bool bTAEAddedToExceptionHolder, bool delegateRan)
        {
            if (Interlocked.CompareExchange(ref _threadAbortedmanaged, 1, 0) == 0)
            {
                var exceptionsHolder = ThreadingHelper.VolatileRead(ref _exceptionsHolder);
                if (exceptionsHolder != null)
                {
                    // this will only be false for non-root self replicating task copies, because all of their exceptions go to the root task.
                    if (bTAEAddedToExceptionHolder)
                    {
                        exceptionsHolder.MarkAsHandled(false);
                    }
                    Finish(delegateRan);
                }
                else
                {
                    Contract.Assert(!bTAEAddedToExceptionHolder, "FinishThreadAbortedTask() called on a task whose exception holder wasn't initialized");
                }
            }
        }

        /// <summary>
        /// The actual code which invokes the body of the task. This can be overriden in derived types.
        /// </summary>
        internal virtual void InnerInvoke()
        {
            // Invoke the delegate
            Contract.Assert(_action != null, "Null action in InnerInvoke()");
            var action = _action as Action;
            if (action != null)
            {
                action.Invoke();
                return;
            }
            var actionWithState = _action as Action<object>;
            if (actionWithState != null)
            {
                actionWithState(_state);
                return;
            }
            Contract.Assert(false, "Invalid m_action in Task");
        }

        /// <summary>
        /// This is called by children of this task when they are completed.
        /// </summary>
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
                List<Task> tmp = ThreadingHelper.VolatileRead(ref _exceptionalChildren);
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
            if ((_parent != null) && ((_creationOptions & TaskCreationOptions.AttachedToParent) != 0) && ((_parent._creationOptions & TaskCreationOptions.DenyChildAttach) == 0) && Task._current == _parent)
            {
                Thread.VolatileWrite(ref _exceptionObservedByParent, 1);
            }
        }

        [SecurityCritical]
        private static void ExecutionContextCallback(object obj)
        {
            Task task = obj as Task;
            Contract.Assert(task != null, "expected a task object");
            task.Execute();
        }

        private static void TaskCancelCallback(object o)
        {
            var targetTask = o as Task;
            Contract.Assert(targetTask != null, "targetTask should have been non-null, with the supplied argument being a task or a tuple containing one");
            targetTask.CancelContinuations();
            targetTask.InternalCancel(false);
        }

        /// <summary>
        /// Handles everything needed for associating a CancellationToken with a task which is being constructed.
        /// This method is meant to be be called either from the TaskConstructorCore or from ContinueWithCore.
        /// </summary>
        private void AssignCancellationToken(CancellationToken cancellationToken)
        {
            Token = cancellationToken;
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
            catch
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
                FinishThreadAbortedTask(true, true);
            }
            catch (Exception exn)
            {
                // Record this exception in the task's exception list
                HandleException(exn);
            }
        }

        // A trick so we can refer to the TLS slot with a byref.
        [SecurityCritical]
        private void ExecuteWithThreadLocal(ref Task currentTaskSlot)
        {
            // Remember the current task so we can restore it after running, and then
            Task previousTask = currentTaskSlot;
            try
            {
                // place the current task into TLS.
                currentTaskSlot = this;

                ExecutionContext ec = _capturedContext;
                if (ec == null)
                {
                    // No context, just run the task directly.
                    InnerInvoke();
                }
                else
                {
                    // Run the task.  We need a simple shim that converts the
                    // object back into a Task object, so that we can Execute it.

                    // Lazily initialize the callback delegate; benign ----
                    var callback = _executionContextCallback;
                    if (callback == null) _executionContextCallback = callback = new ContextCallback(ExecutionContextCallback);
                    ExecutionContext.Run(ec, callback, this);
                }
                Finish(true);
            }
            finally
            {
                currentTaskSlot = previousTask;
            }
        }

        /// <summary>
        /// Performs whatever handling is necessary for an unhandled exception. Normally
        /// this just entails adding the exception to the holder object. 
        /// </summary>
        /// <param name="unhandledException">The exception that went unhandled.</param>
        private void HandleException(Exception unhandledException)
        {
            Contract.Requires(unhandledException != null);

            NewOperationCanceledException exceptionAsOce = unhandledException as NewOperationCanceledException;
            if (exceptionAsOce != null && IsCancellationRequested && Token == exceptionAsOce.CancellationToken)
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
            var exceptionsHolder = ThreadingHelper.VolatileRead(ref _exceptionsHolder);
            if (exceptionsHolder == null)
            {
                // This is the only time we write to _exceptionsHolder
                TaskExceptionHolder holder = new TaskExceptionHolder(this);
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
    }
}

#endif