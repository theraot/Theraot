#if LESSTHAN_NET40

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    internal sealed class ContinuationTaskFromResultTask<TAntecedentResult> : Task, IContinuationTask
    {
        private Task<TAntecedentResult> _antecedent;

        public ContinuationTaskFromResultTask(Task<TAntecedentResult> antecedent, Delegate action, object state, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions)
            : base(action, state, InternalCurrentIfAttached(creationOptions), default, creationOptions, internalOptions, TaskScheduler.Default)
        {
            Contract.Requires(action is Action<Task<TAntecedentResult>> || action is Action<Task<TAntecedentResult>, object>, "Invalid delegate type in ContinuationTaskFromResultTask");
            _antecedent = antecedent;
            CapturedContext = ExecutionContext.Capture();
        }

        public Task Antecedent => _antecedent;

        /// <inheritdoc />
        /// <summary>
        /// Evaluates the value selector of the Task which is passed in as an object and stores the result.
        /// </summary>
        internal override void InnerInvoke()
        {
            // Get and null out the antecedent.  This is crucial to avoid a memory
            // leak with long chains of continuations.
            var antecedent = _antecedent;
            Contract.Assert(antecedent != null, "No antecedent was set for the ContinuationTaskFromResultTask.");
            _antecedent = null;

            // Invoke the delegate
            Contract.Assert(Action != null);
            switch (Action)
            {
                case Action<Task<TAntecedentResult>> action:
                    action(antecedent);
                    return;

                case Action<Task<TAntecedentResult>, object> actionWithState:
                    actionWithState(antecedent, State);
                    return;

                default:
                    Contract.Assert(false, "Invalid Action in ContinuationTaskFromResultTask");
                    break;
            }
        }
    }
}

#endif