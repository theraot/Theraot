#if NET20 || NET30 || NET35

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    internal sealed class ContinuationTaskFromTask : Task, IContinuationTask
    {
        private Task _antecedent;

        public ContinuationTaskFromTask(Task antecedent, Delegate action, object state, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions)
            : base(action, state, InternalCurrentIfAttached(creationOptions), default(CancellationToken), creationOptions, internalOptions, antecedent.ExecutingTaskScheduler)
        {
            Contract.Requires(action is Action<Task> || action is Action<Task, object>, "Invalid delegate type in ContinuationTaskFromTask");
            _antecedent = antecedent;
            CapturedContext = ExecutionContext.Capture();
        }

        Task IContinuationTask.Antecedent
        {
            get { return _antecedent; }
        }

        /// <summary>
        /// Evaluates the value selector of the Task which is passed in as an object and stores the result.
        /// </summary>
        internal override void InnerInvoke()
        {
            // Get and null out the antecedent.  This is crucial to avoid a memory
            // leak with long chains of continuations.
            var antecedent = _antecedent;
            Contract.Assert(antecedent != null, "No antecedent was set for the ContinuationTaskFromTask.");
            _antecedent = null;
            // Invoke the delegate
            Contract.Assert(Action != null);
            var action = Action as Action<Task>;
            if (action != null)
            {
                action(antecedent);
                return;
            }
            var actionWithState = Action as Action<Task, object>;
            if (actionWithState != null)
            {
                actionWithState(antecedent, State);
                return;
            }
            Contract.Assert(false, "Invalid Action in ContinuationTaskFromTask");
        }
    }
}

#endif