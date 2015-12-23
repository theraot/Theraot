#if NET20 || NET30 || NET35

using System.Diagnostics.Contracts;
using System.Security;

namespace System.Threading.Tasks
{
    /// <summary>Task continuation for awaiting with a current synchronization context.</summary>
    internal sealed class SynchronizationContextAwaitTaskContinuation : AwaitTaskContinuation
    {
        /// <summary>SendOrPostCallback delegate to invoke the action.</summary>
        private static readonly SendOrPostCallback _postCallback = state => ((Action)state)(); // can't use InvokeAction as it's SecurityCritical
        /// <summary>Cached delegate for PostAction</summary>
        [SecurityCritical]
        private static ContextCallback _postActionCallback;
        /// <summary>The context with which to run the action.</summary>
        private readonly SynchronizationContext _syncContext;

        /// <summary>Initializes the SynchronizationContextAwaitTaskContinuation.</summary>
        /// <param name="context">The synchronization context with which to invoke the action.  Must not be null.</param>
        /// <param name="action">The action to invoke. Must not be null.</param>
        /// <param name="flowExecutionContext">Whether to capture and restore ExecutionContext.</param>
        [SecurityCritical]
        internal SynchronizationContextAwaitTaskContinuation(SynchronizationContext context, Action action, bool flowExecutionContext)
            : base(action, flowExecutionContext)
        {
            Contract.Assert(context != null);
            _syncContext = context;
        }

        [SecuritySafeCritical]
        internal override void Run(Task task, bool canInlineContinuationTask)
        {
            // If we're allowed to inline, run the action on this thread.
            if (canInlineContinuationTask && _syncContext == SynchronizationContext.Current)
            {
                RunCallback(GetInvokeActionCallback(), Action, ref Task.Current);
            }
            else
            {
                // Otherwise, Post the action back to the SynchronizationContext.
                RunCallback(GetPostActionCallback(), this, ref Task.Current);
            }
            // Any exceptions will be handled by RunCallback.
        }

        /// <summary>Calls InvokeOrPostAction(false) on the supplied SynchronizationContextAwaitTaskContinuation.</summary>
        /// <param name="state">The SynchronizationContextAwaitTaskContinuation.</param>
        [SecurityCritical]
        private static void PostAction(object state)
        {
            var c = (SynchronizationContextAwaitTaskContinuation)state;
            c._syncContext.Post(_postCallback, c.Action); // s_postCallback is manually cached, as the compiler won't in a SecurityCritical method
        }

        /// <summary>Gets a cached delegate for the PostAction method.</summary>
        /// <returns>
        /// A delegate for PostAction, which expects a SynchronizationContextAwaitTaskContinuation 
        /// to be passed as state.
        /// </returns>
        [SecurityCritical]
        private static ContextCallback GetPostActionCallback()
        {
            var callback = _postActionCallback;
            if (callback == null)
            {
                // lazily initialize SecurityCritical delegate
                _postActionCallback = callback = PostAction;
            }
            return callback;
        }
    }
}

#endif