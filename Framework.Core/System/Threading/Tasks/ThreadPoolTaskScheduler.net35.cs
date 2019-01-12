#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// TaskScheduler.cs
//
//
// This file contains the primary interface and management of tasks and queues.
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security;
using Theraot.Core;

namespace System.Threading.Tasks
{
    /// <summary>
    /// An implementation of TaskScheduler that uses the ThreadPool scheduler
    /// </summary>
    internal sealed class ThreadPoolTaskScheduler : TaskScheduler
    {
        private static readonly WaitCallback _executeCallback = TaskExecuteCallback;

        // static delegate for threads allocated to handle LongRunning tasks.
        private static readonly ParameterizedThreadStart _longRunningThreadWork = LongRunningThreadWork;

        /// <summary>
        /// This is the only scheduler that returns false for this property, indicating that the task entry code path is unsafe (CAS free)
        /// since we know that the underlying scheduler already takes care of atomic transitions from queued to non-queued.
        /// </summary>
        internal override bool RequiresAtomicStartTransition => false;

        /// <summary>
        /// Notifies the scheduler that work is progressing (no-op).
        /// </summary>
        internal override void NotifyWorkItemProgress()
        {
            // TODO ?
        }

        /// <summary>
        /// Schedules a task to the ThreadPool.
        /// </summary>
        /// <param name="task">The task to schedule.</param>
        [SecurityCritical]
        protected internal override void QueueTask(Task task)
        {
            if ((task.CreationOptions & TaskCreationOptions.LongRunning) != 0)
            {
                // Run LongRunning tasks on their own dedicated thread.
                var thread = new Thread(_longRunningThreadWork)
                {
                    IsBackground = true // Keep this thread from blocking process shutdown
                };
                thread.Start(task);
            }
            else
            {
                // TODO: TaskCreationOptions.PreferFairness ?
                ThreadPool.QueueUserWorkItem(_executeCallback, task);
            }
        }

        [SecurityCritical]
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // TODO ?
            yield break;
        }

        [SecurityCritical]
        protected override bool TryDequeue(Task task)
        {
            throw new InternalSpecialCancelException("ThreadPool");
        }

        [SecurityCritical]
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if ((task.CreationOptions & TaskCreationOptions.LongRunning) != 0)
            {
                // LongRunning task are going to run on a dedicated Thread.
                return false;
            }
            // Propagate the return value of Task.ExecuteEntry()
            bool result;
            try
            {
                result = task.ExecuteEntry(true); // handles switching Task.Current etc.
            }
            finally
            {
                //   Only call NWIP() if task was previously queued
                if (taskWasPreviouslyQueued)
                {
                    NotifyWorkItemProgress();
                }
            }

            return result;
        }

        private static void LongRunningThreadWork(object obj)
        {
            Contract.Requires(obj != null, "TaskScheduler.LongRunningThreadWork: obj is null");
            if (obj is Task task)
            {
                task.ExecuteEntry(false);
            }
            else
            {
                Contract.Assert(false, "TaskScheduler.LongRunningThreadWork: obj is null");
            }
        }

        private static void TaskExecuteCallback(object obj)
        {
            if (obj is Task task)
            {
                task.ExecuteEntry(true);
            }
        }
    }
}

#endif