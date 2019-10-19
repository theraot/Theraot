#if LESSTHAN_NET40

#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        private int _waitNotificationEnabled;

        /// <summary>Gets whether the task's debugger notification for wait completion bit is set.</summary>
        /// <returns>true if the bit is set; false if it's not set.</returns>
        internal bool IsWaitNotificationEnabled // internal only to enable unit tests; would otherwise be private
            => Volatile.Read(ref _waitNotificationEnabled) == 1;

        /// <summary>
        ///     Determines whether we should inform the debugger that we're ending a join with a task.
        ///     This should only be called if the debugger notification bit is set, as it is has some cost,
        ///     namely it is a virtual call (however calling it if the bit is not set is not functionally
        ///     harmful).  Derived implementations may choose to only conditionally call down to this base
        ///     implementation.
        /// </summary>
        internal virtual bool ShouldNotifyDebuggerOfWaitCompletion // ideally would be familyAndAssembly, but that can't be done in C#
        {
            get
            {
                // It's theoretically possible but extremely rare that this assert could fire because the
                // bit was unset between the time that it was checked and this method was called.
                // It's so remote a chance that it's worth having the assert to protect against misuse.
                var isWaitNotificationEnabled = IsWaitNotificationEnabled;
                Contract.Assert(isWaitNotificationEnabled, "Should only be called if the wait completion bit is set.");
                return isWaitNotificationEnabled;
            }
        }

        /// <summary>
        ///     Waits for all of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="AggregateException">
        ///     At least one of the <see cref="Task" /> instances was canceled -or- an exception was thrown during
        ///     the execution of at least one of the <see cref="Task" /> instances.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static void WaitAll(params Task[] tasks)
        {
#if DEBUG
            var waitResult =
#endif
                WaitAll(tasks, Timeout.Infinite);

#if DEBUG
            Contract.Assert(waitResult, "expected wait to succeed");
#endif
        }

        /// <summary>
        ///     Waits for all of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <returns>
        ///     true if all of the <see cref="Task" /> instances completed execution within the allotted time;
        ///     otherwise, false.
        /// </returns>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="timeout">
        ///     A <see cref="TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="TimeSpan" /> that
        ///     represents -1 milliseconds to wait indefinitely.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="AggregateException">
        ///     At least one of the <see cref="Task" /> instances was canceled -or- an exception was thrown during
        ///     the execution of at least one of the <see cref="Task" /> instances.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an
        ///     infinite time-out -or- timeout is greater than
        ///     <see cref="int.MaxValue" />.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static bool WaitAll(Task[] tasks, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1 || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            return WaitAll(tasks, (int)milliseconds);
        }

        /// <summary>
        ///     Waits for all of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <returns>
        ///     true if all of the <see cref="Task" /> instances completed execution within the allotted time;
        ///     otherwise, false.
        /// </returns>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="millisecondsTimeout">
        ///     The number of milliseconds to wait, or <see cref="Timeout.Infinite" /> (-1) to
        ///     wait indefinitely.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="AggregateException">
        ///     At least one of the <see cref="Task" /> instances was canceled -or- an exception was thrown during
        ///     the execution of at least one of the <see cref="Task" /> instances.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an
        ///     infinite time-out.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static bool WaitAll(Task[] tasks, int millisecondsTimeout)
        {
            return WaitAll(tasks, millisecondsTimeout, default);
        }

        /// <summary>
        ///     Waits for all of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <returns>
        ///     true if all of the <see cref="Task" /> instances completed execution within the allotted time;
        ///     otherwise, false.
        /// </returns>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the tasks to complete.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="AggregateException">
        ///     At least one of the <see cref="Task" /> instances was canceled -or- an exception was thrown during
        ///     the execution of at least one of the <see cref="Task" /> instances.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        ///     The <paramref name="cancellationToken" /> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static void WaitAll(Task[] tasks, CancellationToken cancellationToken)
        {
            WaitAll(tasks, Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        ///     Waits for all of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <returns>
        ///     true if all of the <see cref="Task" /> instances completed execution within the allotted time;
        ///     otherwise, false.
        /// </returns>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="millisecondsTimeout">
        ///     The number of milliseconds to wait, or <see cref="Timeout.Infinite" /> (-1) to
        ///     wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the tasks to complete.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="AggregateException">
        ///     At least one of the <see cref="Task" /> instances was canceled -or- an exception was thrown during
        ///     the execution of at least one of the <see cref="Task" /> instances.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an
        ///     infinite time-out.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        ///     The <paramref name="cancellationToken" /> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static bool WaitAll(Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

            Contract.EndContractBlock();
            cancellationToken.ThrowIfCancellationRequested(); // early check before we make any allocations
            //
            // In this WaitAll() implementation we have 2 alternate code paths for a task to be handled:
            // CODE PATH 1: skip an already completed task, CODE PATH 2: actually wait on tasks
            // We make sure that the exception behavior of Task.Wait() is replicated the same for tasks handled in either of these code paths
            //
            List<Exception>? exceptions = null;
            List<Task>? waitedOnTaskList = null;
            // If any of the waited-upon tasks end as Faulted or Canceled, set these to true.
            var exceptionSeen = false;
            var cancellationSeen = false;
            var allCompleted = true;
            // try inlining the task only if we have an infinite timeout and an empty cancellation token
            var canInline = millisecondsTimeout == Timeout.Infinite && !cancellationToken.CanBeCanceled;
            // Collects incomplete tasks in "waitedOnTaskList"
            for (var taskIndex = tasks.Length - 1; taskIndex >= 0; taskIndex--)
            {
                var task = tasks[taskIndex];
                if (task == null)
                {
                    throw new ArgumentException("The tasks array included at least one null element.");
                }

                var taskIsCompleted = task.IsCompleted;
                if (canInline && !taskIsCompleted)
                {
                    // We are eligible for inlining.  If it doesn't work, we'll do a full wait.
                    // A successful TryStart doesn't guarantee completion
                    taskIsCompleted = task.TryStart(task.ExecutingTaskScheduler, true) && task.IsCompleted;
                }

                if (!taskIsCompleted)
                {
                    (waitedOnTaskList ??= new List<Task>(tasks.Length)).Add(task);
                }
            }

            if (waitedOnTaskList != null)
            {
                // Block waiting for the tasks to complete.
                allCompleted = WaitAllBlockingCore(waitedOnTaskList, millisecondsTimeout, cancellationToken);
            }

            if (!allCompleted)
            {
                return false;
            }

            {
                for (var taskIndex = tasks.Length - 1; taskIndex >= 0; taskIndex--)
                {
                    var task = tasks[taskIndex];
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        var exception = task.GetExceptions(true);
                        if (exception != null)
                        {
                            // make sure the task's exception observed status is set appropriately
                            // it's possible that WaitAll was called by the parent of an attached child,
                            // this will make sure it won't throw again in the implicit wait
                            task.UpdateExceptionObservedStatus();
                            (exceptions ??= new List<Exception>(exception.InnerExceptions.Count)).AddRange(exception.InnerExceptions);
                        }
                    }

                    if (task.IsFaulted)
                    {
                        exceptionSeen = true;
                    }
                    else
                    {
                        cancellationSeen |= task.IsCanceled;
                    }

                    if (task.IsWaitNotificationEnabled && task.NotifyDebuggerOfWaitCompletionIfNecessary())
                    {
                        break;
                    }
                }

                if (cancellationSeen && !exceptionSeen)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // If one or more threw exceptions, aggregate and throw them.
                if (!exceptionSeen && !cancellationSeen)
                {
                    return true;
                }

                Contract.Assert(exceptions != null, "Should have seen at least one exception");
                throw new AggregateException(exceptions!);
            }
        }

        /// <summary>
        ///     Waits for any of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <returns>The index of the completed task in the <paramref name="tasks" /> array argument.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static int WaitAny(params Task[] tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            var waitResult = WaitAny(tasks, Timeout.Infinite);
            Contract.Assert(tasks.Length == 0 || waitResult != -1, "expected wait to succeed");
            return waitResult;
        }

        /// <summary>
        ///     Waits for any of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="timeout">
        ///     A <see cref="TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="TimeSpan" /> that
        ///     represents -1 milliseconds to wait indefinitely.
        /// </param>
        /// <returns>
        ///     The index of the completed task in the <paramref name="tasks" /> array argument, or -1 if the
        ///     timeout occurred.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an
        ///     infinite time-out -or- timeout is greater than
        ///     <see cref="int.MaxValue" />.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static int WaitAny(Task[] tasks, TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1 || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            return WaitAny(tasks, (int)milliseconds);
        }

        /// <summary>
        ///     Waits for any of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for a task to complete.
        /// </param>
        /// <returns>
        ///     The index of the completed task in the <paramref name="tasks" /> array argument.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        ///     The <paramref name="cancellationToken" /> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static int WaitAny(Task[] tasks, CancellationToken cancellationToken)
        {
            return WaitAny(tasks, Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        ///     Waits for any of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="millisecondsTimeout">
        ///     The number of milliseconds to wait, or <see cref="Timeout.Infinite" /> (-1) to
        ///     wait indefinitely.
        /// </param>
        /// <returns>
        ///     The index of the completed task in the <paramref name="tasks" /> array argument, or -1 if the
        ///     timeout occurred.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an
        ///     infinite time-out.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static int WaitAny(Task[] tasks, int millisecondsTimeout)
        {
            return WaitAny(tasks, millisecondsTimeout, default);
        }

        /// <summary>
        ///     Waits for any of the provided <see cref="Task" /> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        ///     An array of <see cref="Task" /> instances on which to wait.
        /// </param>
        /// <param name="millisecondsTimeout">
        ///     The number of milliseconds to wait, or <see cref="Timeout.Infinite" /> (-1) to
        ///     wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for a task to complete.
        /// </param>
        /// <returns>
        ///     The index of the completed task in the <paramref name="tasks" /> array argument, or -1 if the
        ///     timeout occurred.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> argument contains a null element.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an
        ///     infinite time-out.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        ///     The <paramref name="cancellationToken" /> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.NoOptimization)] // this is needed for the parallel debugger
        public static int WaitAny(Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

            Contract.EndContractBlock();
            cancellationToken.ThrowIfCancellationRequested(); // early check before we make any allocations
            var signaledTaskIndex = -1;
            // Make a pass through the loop to check for any tasks that may have
            // already been completed, and to verify that no tasks are null.
            for (var taskIndex = 0; taskIndex < tasks.Length; taskIndex++)
            {
                var task = tasks[taskIndex];
                if (task == null)
                {
                    throw new ArgumentException("The tasks array included at least one null element.", nameof(tasks));
                }

                if (signaledTaskIndex == -1 && task.IsCompleted)
                {
                    // We found our first completed task.  Store it, but we can't just return here,
                    // as we still need to validate the whole array for nulls.
                    signaledTaskIndex = taskIndex;
                }
            }

            if (signaledTaskIndex == -1 && tasks.Length != 0)
            {
                PrivateWaitAny(tasks, millisecondsTimeout, cancellationToken, ref signaledTaskIndex);
            }

            // We need to prevent the tasks array from being garbage collected until we come out of the wait.
            // This is necessary so that the Parallel Debugger can traverse it during the long wait
            // and deduce waiter/waitee relationships
            GC.KeepAlive(tasks);
            // Return the index
            return signaledTaskIndex;
        }

        /// <summary>
        ///     Calls the debugger notification method if the right bit is set and if
        ///     the task itself allows for the notification to proceed.
        /// </summary>
        /// <returns>true if the debugger was notified; otherwise, false.</returns>
        internal bool NotifyDebuggerOfWaitCompletionIfNecessary()
        {
            // Notify the debugger if of any of the tasks we've waited on requires notification
            if (!IsWaitNotificationEnabled || !ShouldNotifyDebuggerOfWaitCompletion)
            {
                return false;
            }

            NotifyDebuggerOfWaitCompletion();
            return true;
        }

        /// <summary>
        ///     Sets or clears the TASK_STATE_WAIT_COMPLETION_NOTIFICATION state bit.
        ///     The debugger sets this bit to aid it in "stepping out" of an async method body.
        ///     If enabled is true, this must only be called on a task that has not yet been completed.
        ///     If enabled is false, this may be called on completed tasks.
        ///     Either way, it should only be used for promise-style tasks.
        /// </summary>
        /// <param name="enabled">true to set the bit; false to unset the bit.</param>
        internal void SetNotificationForWaitCompletion(bool enabled)
        {
            Contract.Assert(IsPromiseTask, "Should only be used for promise-style tasks"); // hasn't been vetted on other kinds as there hasn't been a need
            Volatile.Write(ref _waitNotificationEnabled, enabled ? 1 : 0);
        }

        private static void PrivateWaitAny(Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken, ref int signaledTaskIndex)
        {
            var firstCompleted = PrivateWhenAny(tasks, ref signaledTaskIndex);
            if (signaledTaskIndex != -1)
            {
                return;
            }

            var waitCompleted = firstCompleted.Wait(millisecondsTimeout, cancellationToken);
            if (waitCompleted)
            {
                signaledTaskIndex = Array.IndexOf(tasks, firstCompleted.Result);
            }
        }

        private static Task<Task> PrivateWhenAny(IList<Task> tasks, ref int signaledTaskIndex)
        {
            var firstCompleted = new CompleteOnInvokePromise(tasks);
            for (var taskIndex = 0; taskIndex < tasks.Count; taskIndex++)
            {
                var task = tasks[taskIndex];
                // If a task has already completed, complete the promise.
                if (task.IsCompleted)
                {
                    firstCompleted.Invoke(task);
                    signaledTaskIndex = taskIndex;
                    break;
                }

                // Otherwise, add the completion action and keep going.
                task.AddTaskContinuation(firstCompleted, false);
            }

            //--
            return firstCompleted;
        }

        /// <summary>Performs a blocking WaitAll on the vetted list of tasks.</summary>
        /// <param name="tasks">The tasks, which have already been checked and filtered for completion.</param>
        /// <param name="millisecondsTimeout">The timeout.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>true if all of the tasks completed; otherwise, false.</returns>
        private static bool WaitAllBlockingCore(List<Task> tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (tasks == null)
            {
                Contract.Assert(false, "Expected a non-null list of tasks");
                throw new ArgumentNullException(nameof(tasks));
            }

            Contract.Assert(tasks.Count > 0, "Expected at least one task");
            bool waitCompleted;
            ManualResetEventSlim? manualResetEventSlim = null;
            WhenAllCore? core = null;
            try
            {
                manualResetEventSlim = new ManualResetEventSlim(false);
                core = new WhenAllCore(tasks, manualResetEventSlim.Set);
                waitCompleted = core.IsDone || manualResetEventSlim.Wait(millisecondsTimeout, cancellationToken);
            }
            finally
            {
                if (core != null)
                {
                    core.Dispose();
                    waitCompleted = core.IsDone;
                }

                manualResetEventSlim?.Dispose();
            }

            return waitCompleted;
        }

        /// <summary>Placeholder method used as a breakpoint target by the debugger.  Must not be inlined or optimized.</summary>
        /// <remarks>All joins with a task should end up calling this if their debugger notification bit is set.</remarks>
        [MethodImpl(MethodImplOptionsEx.NoOptimization | MethodImplOptions.NoInlining)]
        private void NotifyDebuggerOfWaitCompletion()
        {
            // It's theoretically possible but extremely rare that this assert could fire because the
            // bit was unset between the time that it was checked and this method was called.
            // It's so remote a chance that it's worth having the assert to protect against misuse.
            Contract.Assert(IsWaitNotificationEnabled, "Should only be called if the wait completion bit is set.");

            // Now that we're notifying the debugger, clear the bit.  The debugger should do this anyway,
            // but this adds a bit of protection in case it fails to, and given that the debugger is involved,
            // the overhead here for the interlocked is negligible.  We do still rely on the debugger
            // to clear bits, as this doesn't recursively clear bits in the case of, for example, WhenAny.
            SetNotificationForWaitCompletion( /*enabled:*/ false);
        }
    }
}

#endif