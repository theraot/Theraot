﻿#if LESSTHAN_NET40

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    /// <summary>Provides the standard implementation of a task continuation.</summary>
    internal class StandardTaskContinuation : TaskContinuation
    {
        /// <summary>The options to use with the continuation task.</summary>
        internal readonly TaskContinuationOptions Options;

        /// <summary>The unstarted continuation task.</summary>
        internal readonly Task Task;

        /// <summary>The task scheduler with which to run the continuation task.</summary>
        private readonly TaskScheduler _scheduler;

        /// <summary>Initializes a new continuation.</summary>
        /// <param name="task">The task to be activated.</param>
        /// <param name="options">The continuation options.</param>
        /// <param name="scheduler">The scheduler to use for the continuation.</param>
        internal StandardTaskContinuation(Task task, TaskContinuationOptions options, TaskScheduler scheduler)
        {
            Task = task;
            Options = options;
            _scheduler = scheduler;
        }

        /// <summary>Invokes the continuation for the target completion task.</summary>
        /// <param name="completedTask">The completed task.</param>
        /// <param name="canInlineContinuationTask">Whether the continuation can be inlined.</param>
        internal override void Run(Task completedTask, bool canInlineContinuationTask)
        {
            if (completedTask == null)
            {
                Contract.Assert(condition: false);
                throw new InvalidOperationException();
            }

            Contract.Assert(completedTask.IsCompleted, "ContinuationTask.Run(): completedTask not completed");

            // Check if the completion status of the task works with the desired
            // activation criteria of the TaskContinuationOptions.
            var options = Options;
            var isRightKind = CheckKind(completedTask, options);

            // If the completion status is allowed, run the continuation.
            var continuationTask = Task;
            if (isRightKind)
            {
                continuationTask.ExecutingTaskScheduler = _scheduler;
                // Either run directly or just queue it up for execution, depending
                // on whether synchronous or asynchronous execution is wanted.
                if (canInlineContinuationTask && (options & TaskContinuationOptions.ExecuteSynchronously) != 0)
                {
                    // synchronous execution was requested by the continuation's creator
                    InlineIfPossibleOrElseQueue(continuationTask);
                }
                else
                {
                    try
                    {
                        continuationTask.TryStart(continuationTask.ExecutingTaskScheduler, inline: false);
                    }
                    catch (TaskSchedulerException exception)
                    {
                        // No further action is necessary -- ScheduleAndStart() already transitioned the
                        // task to faulted.  But we want to make sure that no exception is thrown from here.
                        _ = exception;
                    }
                }
            }
            // Otherwise, the final state of this task does not match the desired
            // continuation activation criteria; cancel it to denote this.
            else
            {
                continuationTask.InternalCancel(cancelNonExecutingOnly: false);
            }
        }

        private static bool CheckKind(Task completedTask, TaskContinuationOptions options)
        {
            if (completedTask.Status == TaskStatus.RanToCompletion)
            {
                return (options & TaskContinuationOptions.NotOnRanToCompletion) == 0;
            }

            if (completedTask.IsCanceled)
            {
                return (options & TaskContinuationOptions.NotOnCanceled) == 0;
            }

            return (options & TaskContinuationOptions.NotOnFaulted) == 0;
        }
    }
}

#endif