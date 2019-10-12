#if LESSTHAN_NET40

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    internal sealed class ContinuationResultTaskFromResultTask<TAntecedentResult, TResult> : Task<TResult>, IContinuationTask
    {
        private Task<TAntecedentResult>? _antecedent;

        public ContinuationResultTaskFromResultTask
        (
            Task<TAntecedentResult> antecedent,
            Delegate function,
            object? state,
            TaskCreationOptions creationOptions,
            InternalTaskOptions internalOptions
        )
            : base(function, state, InternalCurrentIfAttached(creationOptions), default, creationOptions, internalOptions, TaskScheduler.Default)
        {
            Contract.Requires(function is Func<Task<TAntecedentResult>, TResult> || function is Func<Task<TAntecedentResult>, object?, TResult>, "Invalid delegate type in ContinuationResultTaskFromResultTask");
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
            Contract.Assert(antecedent != null, "No antecedent was set for the ContinuationResultTaskFromResultTask.");
            _antecedent = null;

            // Invoke the delegate
            Contract.Assert(Action != null);
            switch (Action)
            {
                case Func<Task<TAntecedentResult>, TResult> func:
                    InternalResult = func(antecedent!);
                    return;

                case Func<Task<TAntecedentResult>, object?, TResult> funcWithState:
                    InternalResult = funcWithState(antecedent!, State);
                    return;

                default:
                    Contract.Assert(false, "Invalid Action in ContinuationResultTaskFromResultTask");
                    break;
            }
        }
    }
}

#endif