﻿#if LESSTHAN_NET40

#pragma warning disable AsyncifyVariable // Use Task Async

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Theraot;

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Provides a set of static (Shared in Visual Basic) methods for working with specific kinds of
    ///     <see cref="Task" /> instances.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        ///     Creates a proxy <see cref="Task">Task</see> that represents the
        ///     asynchronous operation of a Task{Task}.
        /// </summary>
        /// <remarks>
        ///     It is often useful to be able to return a Task from a
        ///     <see cref="Task{TResult}">
        ///         Task{TResult}
        ///     </see>
        ///     , where the inner Task represents work done as part of the outer Task{TResult}.  However,
        ///     doing so results in a Task{Task}, which, if not dealt with carefully, could produce unexpected behavior.  Unwrap
        ///     solves this problem by creating a proxy Task that represents the entire asynchronous operation of such a
        ///     Task{Task}.
        /// </remarks>
        /// <param name="task">The Task{Task} to unwrap.</param>
        /// <exception cref="ArgumentNullException">
        ///     The exception that is thrown if the
        ///     <paramref name="task" /> argument is null.
        /// </exception>
        /// <returns>A Task that represents the asynchronous operation of the provided Task{Task}.</returns>
        public static Task Unwrap(this Task<Task> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            // Fast path for an already successfully completed outer task: just return the inner one.
            // As in the subsequent slower path, a null inner task is special-cased to mean cancellation.
            if (task.Status == TaskStatus.RanToCompletion && (task.CreationOptions & TaskCreationOptions.AttachedToParent) == 0)
            {
                return task.Result ?? Task.FromCanceled(new CancellationToken(canceled: true));
            }

            // Create a new Task to serve as a proxy for the actual inner task.  Attach it
            // to the parent if the original was attached to the parent.
            var tcs = new TaskCompletionSource<VoidStruct>(task.CreationOptions & TaskCreationOptions.AttachedToParent);
            TransferAsynchronously(tcs, task);
            return tcs.Task;
        }

        /// <summary>
        ///     Creates a proxy <see cref="Task{TResult}">Task{TResult}</see> that represents the
        ///     asynchronous operation of a Task{Task{TResult}}.
        /// </summary>
        /// <remarks>
        ///     It is often useful to be able to return a Task{TResult} from a Task{TResult}, where the inner Task{TResult}
        ///     represents work done as part of the outer Task{TResult}.  However, doing so results in a Task{Task{TResult}},
        ///     which, if not dealt with carefully, could produce unexpected behavior.  Unwrap solves this problem by
        ///     creating a proxy Task{TResult} that represents the entire asynchronous operation of such a Task{Task{TResult}}.
        /// </remarks>
        /// <param name="task">The Task{Task{TResult}} to unwrap.</param>
        /// <exception cref="ArgumentNullException">
        ///     The exception that is thrown if the
        ///     <paramref name="task" /> argument is null.
        /// </exception>
        /// <returns>A Task{TResult} that represents the asynchronous operation of the provided Task{Task{TResult}}.</returns>
        public static Task<TResult> Unwrap<TResult>(this Task<Task<TResult>> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            // Fast path for an already successfully completed outer task: just return the inner one.
            // As in the subsequent slower path, a null inner task is special-cased to mean cancellation.
            if (task.Status == TaskStatus.RanToCompletion && (task.CreationOptions & TaskCreationOptions.AttachedToParent) == 0)
            {
                return task.Result ?? Task.FromCanceled<TResult>(new CancellationToken(canceled: true));
            }

            // Create a new Task to serve as a proxy for the actual inner task.  Attach it
            // to the parent if the original was attached to the parent.
            var tcs = new TaskCompletionSource<TResult>(task.CreationOptions & TaskCreationOptions.AttachedToParent);
            TransferAsynchronously(tcs, task);
            return tcs.Task;
        }

        /// <summary>
        ///     Transfer the results of the <paramref name="outer" /> task's inner task to the <paramref name="completionSource" />
        ///     .
        /// </summary>
        /// <param name="completionSource">The completion source to which results should be transferred</param>
        /// <param name="outer">
        ///     The outer task that when completed will yield an inner task whose results we want marshaled to the
        ///     <paramref name="completionSource" />.
        /// </param>
        private static void TransferAsynchronously<TResult, TInner>(TaskCompletionSource<TResult> completionSource, Task<TInner> outer)
            where TInner : Task
        {
            Action?[] callback = { null };

            // Create a continuation delegate.  For performance reasons, we reuse the same delegate/closure across multiple
            // continuations; by using .ConfigureAwait(false).GetAwaiter().UnsafeOnComplete(action), in most cases
            // this delegate can be stored directly into the Task's continuation field, eliminating the need for additional
            // allocations.  Thus, this whole Unwrap operation generally results in four allocations: one for the TaskCompletionSource,
            // one for the returned task, one for the delegate, and one for the closure.  Since the delegate is used
            // across multiple continuations, we use the callback variable as well to indicate which continuation we're in:
            // if the callback is non-null, then we're processing the continuation for the outer task and use the callback
            // object as the continuation off of the inner task; if the callback is null, then we're processing the
            // inner task.
            callback[0] = delegate
            {
                Debug.Assert(outer.IsCompleted);
                if (callback[0] != null)
                {
                    // Process the outer task's completion

                    // Clear out the callback field to indicate that any future invocations should
                    // be for processing the inner task, but store away a local copy in case we need
                    // to use it as the continuation off of the outer task.
                    var innerCallback = callback[0];
                    callback[0] = null;

                    var result = false;
                    switch (outer.Status)
                    {
                        case TaskStatus.Canceled:
                        case TaskStatus.Faulted:
                            // The outer task has completed as canceled or faulted; transfer that
                            // status to the completion source, and we're done.
                            result = completionSource.TrySetFromTask(outer);
                            break;

                        case TaskStatus.RanToCompletion:
                            Task? inner = outer.Result;
                            if (inner == null)
                            {
                                // The outer task completed successfully, but with a null inner task.
                                // cancel the completionSource, and we're done.
                                result = completionSource.TrySetCanceled(outer.CancellationToken);
                            }
                            else
                            {
                                if (inner.IsCompleted)
                                {
                                    // The inner task also completed!  Transfer the results, and we're done.
                                    result = completionSource.TrySetFromTask(inner);
                                }

                                if (!result)
                                {
                                    // Run this delegate again once the inner task has completed.
                                    inner.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(innerCallback!);
                                    return;
                                }
                            }

                            break;

                        default:
                            break;
                    }

                    if (!result)
                    {
                        outer.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(innerCallback!);
                    }
                }
                else
                {
                    // Process the inner task's completion.  All we need to do is transfer its results
                    // to the completion source.
                    Debug.Assert(outer.Status == TaskStatus.RanToCompletion);
                    Debug.Assert(outer.Result?.IsCompleted == true);
                    completionSource.TrySetFromTask(outer.Result!);
                }
            };

            // Kick things off by hooking up the callback as the task's continuation
            outer.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(callback[0]!);
        }

        private static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> completionSource, Task task)
        {
            Debug.Assert(task.IsCompleted);

            // Transfer the results from the supplied Task to the TaskCompletionSource.
            var result = false;
            switch (task.Status)
            {
                case TaskStatus.Canceled:
                    result = completionSource.TrySetCanceled(task.CancellationToken);
                    break;

                case TaskStatus.Faulted:
                    result = completionSource.TrySetException(task.Exception!.InnerExceptions);
                    break;

                case TaskStatus.RanToCompletion:
                    result = task is Task<TResult> resultTask ? completionSource.TrySetResult(resultTask.Result) : completionSource.TrySetResult(default!);
                    break;

                default:
                    break;
            }

            return result;
        }
    }
}

#endif