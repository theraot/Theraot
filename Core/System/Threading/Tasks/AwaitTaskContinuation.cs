#if NET35

using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using System.Security;

namespace System.Threading.Tasks
{
    /// <summary>Base task continuation class used for await continuations.</summary>
    internal class AwaitTaskContinuation : TaskContinuation, IThreadPoolWorkItem
    {
        /// <summary>The ExecutionContext with which to run the continuation.</summary>
        private ExecutionContext _capturedContext;
        /// <summary>The action to invoke.</summary>
        protected readonly Action Action;

        /// <summary>Initializes the continuation.</summary>
        /// <param name="action">The action to invoke. Must not be null.</param>
        /// <param name="flowExecutionContext">Whether to capture and restore ExecutionContext.</param>
        [SecurityCritical]
        internal AwaitTaskContinuation(Action action, bool flowExecutionContext)
        {
            Contract.Requires(action != null);
            Action = action;
            if (flowExecutionContext)
            {
                _capturedContext = ExecutionContext.Capture();
            }
        }

        /// <summary>Creates a task to run the action with the specified state on the specified scheduler.</summary>
        /// <param name="action">The action to run. Must not be null.</param>
        /// <param name="state">The state to pass to the action. Must not be null.</param>
        /// <param name="scheduler">The scheduler to target.</param>
        /// <returns>The created task.</returns>
        protected Task CreateTask(Action<object> action, object state, TaskScheduler scheduler)
        {
            Contract.Requires(action != null);
            Contract.Requires(scheduler != null);

            return new Task(
                action, state, null, default(CancellationToken),
                TaskCreationOptions.None, InternalTaskOptions.QueuedByRuntime, scheduler)
            {
                CapturedContext = _capturedContext
            };
        }

        [SecuritySafeCritical]
        internal override void Run(Task task, bool canInlineContinuationTask)
        {
            // For the base AwaitTaskContinuation, we allow inlining if our caller allows it
            // and if we're in a "valid location" for it.  See the comments on 
            // IsValidLocationForInlining for more about what's valid.  For performance
            // reasons we would like to always inline, but we don't in some cases to avoid
            // running arbitrary amounts of work in suspected "bad locations", like UI threads.
            if (canInlineContinuationTask && IsValidLocationForInlining)
            {
                RunCallback(GetInvokeActionCallback(), Action, ref Task.Current); // any exceptions from Action will be handled by s_callbackRunAction
            }
            else
            {
                // We couldn't inline, so now we need to schedule it
                ThreadPoolAdapter.QueueWorkItem(this);
            }
        }

        /// <summary>
        /// Gets whether the current thread is an appropriate location to inline a continuation's execution.
        /// </summary>
        /// <remarks>
        /// Returns whether SynchronizationContext is null and we're in the default scheduler.
        /// If the await had a SynchronizationContext/TaskScheduler where it began and the 
        /// default/ConfigureAwait(true) was used, then we won't be on this path.  If, however, 
        /// ConfigureAwait(false) was used, or the SynchronizationContext and TaskScheduler were 
        /// naturally null/Default, then we might end up here.  If we do, we need to make sure
        /// that we don't execute continuations in a place that isn't set up to handle them, e.g.
        /// running arbitrary amounts of code on the UI thread.  It would be "correct", but very
        /// expensive, to always run the continuations asynchronously, incurring lots of context
        /// switches and allocations and locks and the like.  As such, we employ the heuristic
        /// that if the current thread has a non-null SynchronizationContext or a non-default
        /// scheduler, then we better not run arbitrary continuations here.
        /// </remarks>
        internal static bool IsValidLocationForInlining
        {
            get
            {
                if (SynchronizationContext.Current == null)
                {
                    return false;
                }
                var scheduler = TaskScheduler.Current;
                if (scheduler != null && scheduler != TaskScheduler.Default)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>IThreadPoolWorkItem override, which is the entry function for this when the ThreadPool scheduler decides to run it.</summary>
        [SecurityCritical]
        void ExecuteWorkItemHelper()
        {
            // We're not inside of a task, so t_currentTask doesn't need to be specially maintained.
            // We're on a thread pool thread with no higher-level callers, so exceptions can just propagate.

            // If there's no execution context, just invoke the delegate.
            if (_capturedContext == null)
            {
                Action.Invoke();
            }
            // If there is an execution context, get the cached delegate and run the action under the context.
            else
            {
                try
                {
                    ExecutionContext.Run(_capturedContext, GetInvokeActionCallback(), Action);
                }
                finally
                {
                    _capturedContext = null;
                }
            }
        }

        [SecurityCritical]
        void IThreadPoolWorkItem.ExecuteWorkItem()
        {
            // inline the fast path
            if (_capturedContext == null)
            {
                Action.Invoke();
            }
            else
            {
                ExecuteWorkItemHelper();
            }
        }

        [SecurityCritical]
        void IThreadPoolWorkItem.MarkAborted(ThreadAbortException tae) { /* nop */ }

        /// <summary>Cached delegate that invokes an Action passed as an object parameter.</summary>
        [SecurityCritical]
        private static ContextCallback _invokeActionCallback;

        /// <summary>Runs an action provided as an object parameter.</summary>
        /// <param name="state">The Action to invoke.</param>
        [SecurityCritical]
        private static void InvokeAction(object state) { ((Action)state)(); }

        [SecurityCritical]
        protected static ContextCallback GetInvokeActionCallback()
        {
            var callback = _invokeActionCallback;
            if (callback == null) { _invokeActionCallback = callback = InvokeAction; } // lazily initialize SecurityCritical delegate
            return callback;
        }

        /// <summary>Runs the callback synchronously with the provided state.</summary>
        /// <param name="callback">The callback to run.</param>
        /// <param name="state">The state to pass to the callback.</param>
        /// <param name="currentTask">A reference to Task.t_currentTask.</param>
        [SecurityCritical]
        protected void RunCallback(ContextCallback callback, object state, ref Task currentTask)
        {
            Contract.Requires(callback != null);
            Contract.Assert(currentTask == Task.Current);

            // Pretend there's no current task, so that no task is seen as a parent
            // and TaskScheduler.Current does not reflect false information
            var prevCurrentTask = currentTask;
            try
            {
                if (prevCurrentTask != null)
                {
                    currentTask = null;
                }

                if (_capturedContext == null)
                {
                    // If there's no captured context, just run the callback directly.
                    callback(state);
                }
                else
                {
                    // Otherwise, use the captured context to do so.
                    ExecutionContext.Run(_capturedContext, callback, state);
                }
            }
            catch (Exception exc)
            {
                // we explicitly do not request handling of dangerous exceptions like AVs
                ThrowAsyncIfNecessary(exc);
            }
            finally
            {
                // Restore the current task information
                if (prevCurrentTask != null)
                {
                    currentTask = prevCurrentTask;
                }
                _capturedContext = null;
            }
        }

        /// <summary>Invokes or schedules the action to be executed.</summary>
        /// <param name="action">The action to invoke or queue.</param>
        /// <param name="allowInlining">
        /// true to allow inlining, or false to force the action to run asynchronously.
        /// </param>
        /// <param name="currentTask">
        /// A reference to the t_currentTask thread static value.
        /// This is passed by-ref rather than accessed in the method in order to avoid
        /// unnecessary thread-static writes.
        /// </param>
        /// <remarks>
        /// No ExecutionContext work is performed used.  This method is only used in the
        /// case where a raw Action continuation delegate was stored into the Task, which
        /// only happens in Task.SetContinuationForAwait if execution context flow was disabled
        /// via using TaskAwaiter.UnsafeOnCompleted or a similar path.
        /// </remarks>
        [SecurityCritical]
        internal static void RunOrScheduleAction(Action action, bool allowInlining, ref Task currentTask)
        {
            Contract.Assert(currentTask == Task.Current);

            // If we're not allowed to run here, schedule the action
            if (!allowInlining || !IsValidLocationForInlining)
            {
                UnsafeScheduleAction(action);
                return;
            }
            // Otherwise, run it, making sure that t_currentTask is null'd out appropriately during the execution
            var prevCurrentTask = currentTask;
            try
            {
                if (prevCurrentTask != null) currentTask = null;
                action();
            }
            catch (Exception exception)
            {
                ThrowAsyncIfNecessary(exception);
            }
            finally
            {
                if (prevCurrentTask != null) currentTask = prevCurrentTask;
            }
        }

        [SecurityCritical]
        internal static void UnsafeScheduleAction(Action action)
        {
            var atc = new AwaitTaskContinuation(action, flowExecutionContext: false);

            ThreadPoolAdapter.QueueWorkItem(atc);
        }

        /// <summary>Throws the exception asynchronously on the ThreadPool.</summary>
        /// <param name="exc">The exception to throw.</param>
        protected static void ThrowAsyncIfNecessary(Exception exc)
        {
            // Awaits should never experience an exception (other than an TAE or ADUE), 
            // unless a malicious user is explicitly passing a throwing action into the TaskAwaiter. 
            // We don't want to allow the exception to propagate on this stack, as it'll emerge in random places, 
            // and we can't fail fast, as that would allow for elevation of privilege.
            //
            // If unhandled error reporting APIs are available use those, otherwise since this 
            // would have executed on the thread pool otherwise, let it propagate there.
            if (exc is ThreadAbortException || exc is AppDomainUnloadedException)
            {
                return;
            }
            var edi = ExceptionDispatchInfo.Capture(exc);
            ThreadPool.QueueUserWorkItem(s => ((ExceptionDispatchInfo)s).Throw(), edi);
        }
    }
}

#endif