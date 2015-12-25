#if NET20 || NET30 || NET35

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    internal sealed class ContinuationResultTaskFromResultTask<TAntecedentResult, TResult> : Task<TResult>
    {
        private Task<TAntecedentResult> _antecedent;

        public ContinuationResultTaskFromResultTask(Task<TAntecedentResult> antecedent, Delegate function, object state, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions)
            : base(function, state, InternalCurrentIfAttached(creationOptions), default(CancellationToken), creationOptions, internalOptions, null)
        {
            Contract.Requires(function is Func<Task<TAntecedentResult>, TResult> || function is Func<Task<TAntecedentResult>, object, TResult>, "Invalid delegate type in ContinuationResultTaskFromResultTask");
            _antecedent = antecedent;
            CapturedContext = ExecutionContext.Capture();
        }

        /// <summary>
        /// Evaluates the value selector of the Task which is passed in as an object and stores the result.
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
            var func = Action as Func<Task<TAntecedentResult>, TResult>;
            if (func != null)
            {
                InternalResult = func(antecedent);
                return;
            }
            var funcWithState = Action as Func<Task<TAntecedentResult>, object, TResult>;
            if (funcWithState != null)
            {
                InternalResult = funcWithState(antecedent, State);
                return;
            }
            Contract.Assert(false, "Invalid Action in ContinuationResultTaskFromResultTask");
        }
    }
}

#endif