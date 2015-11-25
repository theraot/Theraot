#if NET20 || NET30 || NET35

using System.Diagnostics.Contracts;
using System.Security;

namespace System.Threading.Tasks
{
    /// <summary>Task continuation for awaiting with a task scheduler.</summary>
    internal sealed class TaskSchedulerAwaitTaskContinuation : AwaitTaskContinuation
    {
        /// <summary>The scheduler on which to run the action.</summary>
        private readonly TaskScheduler _scheduler;

        /// <summary>Initializes the TaskSchedulerAwaitTaskContinuation.</summary>
        /// <param name="scheduler">The task scheduler with which to invoke the action.  Must not be null.</param>
        /// <param name="action">The action to invoke. Must not be null.</param>
        /// <param name="flowExecutionContext">Whether to capture and restore ExecutionContext.</param>
        /// <param name="stackMark">The captured stack mark.</param>
        [SecurityCritical]
        internal TaskSchedulerAwaitTaskContinuation(TaskScheduler scheduler, Action action, bool flowExecutionContext)
            : base(action, flowExecutionContext)
        {
            Contract.Assert(scheduler != null);
            _scheduler = scheduler;
        }

        /// <summary>Inlines or schedules the continuation.</summary>
        /// <param name="ignored">The antecedent task, which is ignored.</param>
        /// <param name="canInlineContinuationTask">true if inlining is permitted; otherwise, false.</param>
        internal override void Run(Task ignored, bool canInlineContinuationTask)
        {
            // If we're targeting the default scheduler, we can use the faster path provided by the base class.
            if (_scheduler == TaskScheduler.Default)
            {
                base.Run(ignored, canInlineContinuationTask);
            }
            else
            {
                // We permit inlining if the caller allows us to, and 
                // either we're on a thread pool thread (in which case we're fine running arbitrary code)
                // or we're already on the target scheduler (in which case we'll just ask the scheduler
                // whether it's ok to run here).  We include the IsThreadPoolThread check here, whereas
                // we don't in AwaitTaskContinuation.Run, since here it expands what's allowed as opposed
                // to in AwaitTaskContinuation.Run where it restricts what's allowed.
                var inlineIfPossible = canInlineContinuationTask && (TaskScheduler.Current == _scheduler || Thread.CurrentThread.IsThreadPoolThread);

                // Create the continuation task task. If we're allowed to inline, try to do so.  
                // The target scheduler may still deny us from executing on this thread, in which case this'll be queued.
                var task = CreateTask(state => {
                                                   try { ((Action)state)(); }
                                                   catch (Exception exc) { ThrowAsyncIfNecessary(exc); }
                }, Action, _scheduler);

                if (inlineIfPossible)
                {
                    InlineIfPossibleOrElseQueue(task);
                }
                else
                {
                    // We need to run asynchronously, so just schedule the task.
                    try
                    {
                        task.TryStart(task.Scheduler, false);
                    }
                    catch (TaskSchedulerException exception)
                    {
                        // No further action is necessary, as ScheduleAndStart already transitioned task to faulted
                        GC.KeepAlive(exception);
                    }
                }
            }
        }
    }
}

#endif