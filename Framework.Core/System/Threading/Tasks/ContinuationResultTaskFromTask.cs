#if LESSTHAN_NET40

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    internal sealed class ContinuationResultTaskFromTask<TResult> : Task<TResult>, IContinuationTask
    {
        private Task? _antecedent;

        public ContinuationResultTaskFromTask(Task antecedent, Delegate function, object? state, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions)
            : base(function, state, InternalCurrentIfAttached(creationOptions), default, creationOptions, internalOptions, antecedent.ExecutingTaskScheduler)
        {
            Contract.Requires(function is Func<Task, TResult> || function is Func<Task, object?, TResult>, "Invalid delegate type in ContinuationResultTaskFromTask");
            _antecedent = antecedent;
            CapturedContext = ExecutionContext.Capture();
        }

        Task? IContinuationTask.Antecedent => _antecedent;

        /// <summary>
        ///     Evaluates the value selector of the Task which is passed in as an object and stores the result.
        /// </summary>
        internal override void InnerInvoke()
        {
            // Get and null out the antecedent.  This is crucial to avoid a memory
            // leak with long chains of continuations.
            var antecedent = _antecedent;
            Contract.Assert(antecedent != null, "No antecedent was set for the ContinuationResultTaskFromTask.");
            _antecedent = null;
            // Invoke the delegate
            Contract.Assert(Action != null);
            switch (Action)
            {
                case Func<Task, TResult> func:
                    InternalResult = func(antecedent!);
                    return;

                case Func<Task, object?, TResult> funcWithState:
                    InternalResult = funcWithState(antecedent!, State);
                    return;

                default:
                    Contract.Assert(false, "Invalid Action in ContinuationResultTaskFromTask");
                    break;
            }
        }
    }
}

#endif