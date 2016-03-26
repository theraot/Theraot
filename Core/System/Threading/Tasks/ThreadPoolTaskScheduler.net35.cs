#if NET20 || NET30 || NET35

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

using System;
using System.Security;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Text;

namespace System.Threading.Tasks
{
    /// <summary>
    /// An implementation of TaskScheduler that uses the ThreadPool scheduler
    /// </summary>
    internal sealed class ThreadPoolTaskScheduler: TaskScheduler
    {
        private static readonly WaitCallback _executeCallback = TaskExecuteCallback;

        private static void TaskExecuteCallback(object obj)
        {
            var task = obj as Task;
            if (task != null)
            {
                task.ExecuteEntry(true);
            }
        }

        /// <summary>
        /// Constructs a new ThreadPool task scheduler object
        /// </summary>
        internal ThreadPoolTaskScheduler()
        {
            int id = base.Id; // force ID creation of the default scheduler
        }

        // static delegate for threads allocated to handle LongRunning tasks.
        private static readonly ParameterizedThreadStart s_longRunningThreadWork = new ParameterizedThreadStart(LongRunningThreadWork);

        private static void LongRunningThreadWork(object obj)
        {
            Contract.Requires(obj != null, "TaskScheduler.LongRunningThreadWork: obj is null");
            Task t = obj as Task;
            Contract.Assert(t != null, "TaskScheduler.LongRunningThreadWork: t is null");
            t.ExecuteEntry(false);
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
                Thread thread = new Thread(s_longRunningThreadWork);
                thread.IsBackground = true; // Keep this thread from blocking process shutdown
                thread.Start(task);
            }
            else
            {
                // TODO: TaskCreationOptions.PreferFairness ?
                ThreadPool.QueueUserWorkItem(_executeCallback, task);
            }
        }
        
        /// <summary>
        /// This internal function will do this:
        ///   (1) If the task had previously been queued, attempt to pop it and return false if that fails.
        ///   (2) Propagate the return value from Task.ExecuteEntry() back to the caller.
        /// 
        /// IMPORTANT NOTE: TryExecuteTaskInline will NOT throw task exceptions itself. Any wait code path using this function needs
        /// to account for exceptions that need to be propagated, and throw themselves accordingly.
        /// </summary>
        [SecurityCritical]
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // Propagate the return value of Task.ExecuteEntry()
            bool result;
            try
            {
                result = task.ExecuteEntry(false); // handles switching Task.Current etc.
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

        [SecurityCritical]
        protected override bool TryDequeue(Task task)
        {
            throw new Theraot.Core.InternalSpecialCancelException("ThreadPool");
        }

        [SecurityCritical]
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // TODO ?
            yield break;
        }

        /// <summary>
        /// Notifies the scheduler that work is progressing (no-op).
        /// </summary>
        internal override void NotifyWorkItemProgress()
        {
            // TODO ?
        }

        /// <summary>
        /// This is the only scheduler that returns false for this property, indicating that the task entry codepath is unsafe (CAS free)
        /// since we know that the underlying scheduler already takes care of atomic transitions from queued to non-queued.
        /// </summary>
        internal override bool RequiresAtomicStartTransition
        {
            get { return false; }
        }
    }
}

#endif