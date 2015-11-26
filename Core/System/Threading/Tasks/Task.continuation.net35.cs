#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        private List<object> _continuations = null;
        private int initialized;

        /// <summary>
        /// Runs all of the continuations, as appropriate.
        /// </summary>
        internal void FinishContinuations()
        {
            var continuations = _continuations;
            if (Thread.VolatileRead(ref initialized) == 1 && continuations != null)
            {
                // Skip synchronous execution of continuations if this task's thread was aborted
                var canInlineContinuations =
                    (
                        Thread.VolatileRead(ref _threadAbortedmanaged) == 0
                        && (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
                        && ((_creationOptions & TaskCreationOptions.RunContinuationsAsynchronously) == 0)
                    );
                
                //
                // Begin processing of continuation list
                //

                // Wait for any concurrent adds or removes to be retired
                lock (continuations)
                {
                    _continuations = null;
                }
                var continuationCount = continuations.Count;

                // Fire the asynchronous continuations first ...
                for (var index = 0; index < continuationCount; index++)
                {
                    // Synchronous continuation tasks will have the ExecuteSynchronously option,
                    // and we're looking for asynchronous tasks...
                    var tc = continuations[index] as StandardTaskContinuation;
                    if (tc == null || (tc.Options & TaskContinuationOptions.ExecuteSynchronously) != 0)
                    {
                        continue;
                    }
                    continuations[index] = null; // so that we can skip this later
                    tc.Run(this, canInlineContinuations);
                }

                // ... and then fire the synchronous continuations (if there are any).
                // This includes ITaskCompletionAction, AwaitTaskContinuations, and
                // Action delegates, which are all by default implicitly synchronous.
                for (var index = 0; index < continuationCount; index++)
                {
                    var currentContinuation = continuations[index];
                    if (currentContinuation == null) continue;
                    continuations[index] = null; // to enable free'ing up memory earlier
                    // If the continuation is an Action delegate, it came from an await continuation,
                    // and we should use AwaitTaskContinuation to run it.
                    var ad = currentContinuation as Action;
                    if (ad != null)
                    {
                        AwaitTaskContinuation.RunOrScheduleAction(ad, canInlineContinuations, ref Current);
                    }
                    else
                    {
                        // If it's a TaskContinuation object of some kind, invoke it.
                        var tc = currentContinuation as TaskContinuation;
                        if (tc != null)
                        {
                            // We know that this is a synchronous continuation because the
                            // asynchronous ones have been weeded out
                            tc.Run(this, canInlineContinuations);
                        }
                        // Otherwise, it must be an ITaskCompletionAction, so invoke it.
                        else
                        {
                            // TODO TODO TODO
                            // Contract.Assert(currentContinuation is ITaskCompletionAction, "Expected continuation element to be Action, TaskContinuation, or ITaskContinuationAction");
                            // var action = (ITaskCompletionAction)currentContinuation;
                            // action.Invoke(this);
                            throw new NotImplementedException();
                        }
                    }
                }
            }
        }

        internal void RemoveContinuation(object continuationObject) // could be TaskContinuation or Action<Task>
        {
            var continuations = _continuations;
            if (Thread.VolatileRead(ref initialized) == 1 && continuations != null)
            {
                lock (continuations)
                {
                    if (_continuations != continuations)
                    {
                        return;
                    }
                    var index = continuations.IndexOf(continuationObject);
                    if (index != -1)
                    {
                        continuations[index] = null;
                    }
                }
            }
        }
    }
}

#endif