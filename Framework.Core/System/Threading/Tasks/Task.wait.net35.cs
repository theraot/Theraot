#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        private int _waitNotificationEnabled;

        /// <summary>
        /// Determines whether we should inform the debugger that we're ending a join with a task.
        /// This should only be called if the debugger notification bit is set, as it is has some cost,
        /// namely it is a virtual call (however calling it if the bit is not set is not functionally
        /// harmful).  Derived implementations may choose to only conditionally call down to this base
        /// implementation.
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
        /// Calls the debugger notification method if the right bit is set and if
        /// the task itself allows for the notification to proceed.
        /// </summary>
        /// <returns>true if the debugger was notified; otherwise, false.</returns>
        internal bool NotifyDebuggerOfWaitCompletionIfNecessary()
        {
            // Notify the debugger if of any of the tasks we've waited on requires notification
            if (IsWaitNotificationEnabled && ShouldNotifyDebuggerOfWaitCompletion)
            {
                NotifyDebuggerOfWaitCompletion();
                return true;
            }
            return false;
        }

        /// <summary>Placeholder method used as a breakpoint target by the debugger.  Must not be inlined or optimized.</summary>
        /// <remarks>All joins with a task should end up calling this if their debugger notification bit is set.</remarks>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private void NotifyDebuggerOfWaitCompletion()
        {
            // It's theoretically possible but extremely rare that this assert could fire because the
            // bit was unset between the time that it was checked and this method was called.
            // It's so remote a chance that it's worth having the assert to protect against misuse.
            Contract.Assert(IsWaitNotificationEnabled, "Should only be called if the wait completion bit is set.");

            // Now that we're notifying the debugger, clear the bit.  The debugger should do this anyway,
            // but this adds a bit of protection in case it fails to, and given that the debugger is involved,
            // the overhead here for the interlocked is negligable.  We do still rely on the debugger
            // to clear bits, as this doesn't recursively clear bits in the case of, for example, WhenAny.
            SetNotificationForWaitCompletion(/*enabled:*/ false);
        }

        /// <summary>
        /// Sets or clears the TASK_STATE_WAIT_COMPLETION_NOTIFICATION state bit.
        /// The debugger sets this bit to aid it in "stepping out" of an async method body.
        /// If enabled is true, this must only be called on a task that has not yet been completed.
        /// If enabled is false, this may be called on completed tasks.
        /// Either way, it should only be used for promise-style tasks.
        /// </summary>
        /// <param name="enabled">true to set the bit; false to unset the bit.</param>
        internal void SetNotificationForWaitCompletion(bool enabled)
        {
            Contract.Assert(IsPromiseTask, "Should only be used for promise-style tasks"); // hasn't been vetted on other kinds as there hasn't been a need
            Thread.VolatileWrite(ref _waitNotificationEnabled, enabled ? 1 : 0);
        }

        /// <summary>Gets whether the task's debugger notification for wait completion bit is set.</summary>
        /// <returns>true if the bit is set; false if it's not set.</returns>
        internal bool IsWaitNotificationEnabled // internal only to enable unit tests; would otherwise be private
        {
            get { return Thread.VolatileRead(ref _waitNotificationEnabled) == 1; }
        }

        /// <summary>
        /// Waits for all of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.AggregateException">
        /// At least one of the <see cref="Task"/> instances was canceled -or- an exception was thrown during
        /// the execution of at least one of the <see cref="Task"/> instances.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
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
        /// Waits for all of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <returns>
        /// true if all of the <see cref="Task"/> instances completed execution within the allotted time;
        /// otherwise, false.
        /// </returns>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <param name="timeout">
        /// A <see cref="System.TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.AggregateException">
        /// At least one of the <see cref="Task"/> instances was canceled -or- an exception was thrown during
        /// the execution of at least one of the <see cref="Task"/> instances.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="timeout"/> is a negative number other than -1 milliseconds, which represents an
        /// infinite time-out -or- timeout is greater than
        /// <see cref="int.MaxValue"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
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
        /// Waits for all of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <returns>
        /// true if all of the <see cref="Task"/> instances completed execution within the allotted time;
        /// otherwise, false.
        /// </returns>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to
        /// wait indefinitely.</param>
        /// <param name="tasks">An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.AggregateException">
        /// At least one of the <see cref="Task"/> instances was canceled -or- an exception was thrown during
        /// the execution of at least one of the <see cref="Task"/> instances.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an
        /// infinite time-out.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
        public static bool WaitAll(Task[] tasks, int millisecondsTimeout)
        {
            return WaitAll(tasks, millisecondsTimeout, default(CancellationToken));
        }

        /// <summary>
        /// Waits for all of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <returns>
        /// true if all of the <see cref="Task"/> instances completed execution within the allotted time;
        /// otherwise, false.
        /// </returns>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the tasks to complete.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.AggregateException">
        /// At least one of the <see cref="Task"/> instances was canceled -or- an exception was thrown during
        /// the execution of at least one of the <see cref="Task"/> instances.
        /// </exception>
        /// <exception cref="T:System.OperationCanceledException">
        /// The <paramref name="cancellationToken"/> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
        public static void WaitAll(Task[] tasks, CancellationToken cancellationToken)
        {
            WaitAll(tasks, Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        /// Waits for all of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <returns>
        /// true if all of the <see cref="Task"/> instances completed execution within the allotted time;
        /// otherwise, false.
        /// </returns>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to
        /// wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the tasks to complete.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.AggregateException">
        /// At least one of the <see cref="Task"/> instances was canceled -or- an exception was thrown during
        /// the execution of at least one of the <see cref="Task"/> instances.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an
        /// infinite time-out.
        /// </exception>
        /// <exception cref="T:System.OperationCanceledException">
        /// The <paramref name="cancellationToken"/> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
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
            // CODEPATH1: skip an already completed task, CODEPATH2: actually wait on tasks
            // We make sure that the exception behavior of Task.Wait() is replicated the same for tasks handled in either of these codepaths
            //
            List<Exception> exceptions = null;
            List<Task> waitedOnTaskList = null;
            List<Task> notificationTasks = null;
            // If any of the waited-upon tasks end as Faulted or Canceled, set these to true.
            var exceptionSeen = false;
            var cancellationSeen = false;
            var allCompleted = true;
            // Collects incomplete tasks in "waitedOnTaskList"
            for (var taskIndex = tasks.Length - 1; taskIndex >= 0; taskIndex--)
            {
                var task = tasks[taskIndex];
                if (task == null)
                {
                    throw new ArgumentException("The tasks array included at least one null element.");
                }
                var taskIsCompleted = task.IsCompleted;
                if (!taskIsCompleted)
                {
                    // try inlining the task only if we have an infinite timeout and an empty cancellation token
                    if (millisecondsTimeout != Timeout.Infinite || cancellationToken.CanBeCanceled)
                    {
                        // We either didn't attempt inline execution because we had a non-infinite timeout or we had a cancellable token.
                        // In all cases we need to do a full wait on the task (=> add its event into the list.)
                        AddToList(task, ref waitedOnTaskList, /*initSize:*/ tasks.Length);
                    }
                    else
                    {
                        // We are eligible for inlining.  If it doesn't work, we'll do a full wait.
                        taskIsCompleted = task.TryStart(task.ExecutingTaskScheduler, true) && task.IsCompleted; // A successful TryRunInline doesn't guarantee completion
                        if (!taskIsCompleted)
                        {
                            AddToList(task, ref waitedOnTaskList, /*initSize:*/ tasks.Length);
                        }
                    }
                }
                if (taskIsCompleted)
                {
                    if (task.IsFaulted)
                    {
                        exceptionSeen = true;
                    }
                    else
                    {
                        cancellationSeen |= task.IsCanceled;
                    }

                    if (task.IsWaitNotificationEnabled)
                    {
                        AddToList(task, ref notificationTasks, /*initSize:*/ 1);
                    }
                }
            }
            if (waitedOnTaskList != null)
            {
                // Block waiting for the tasks to complete.
                allCompleted = WaitAllBlockingCore(waitedOnTaskList, millisecondsTimeout, cancellationToken);
                // If the wait didn't time out, ensure exceptions are propagated, and if a debugger is
                // attached and one of these tasks requires it, that we notify the debugger of a wait completion.
                if (allCompleted)
                {
                    // Add any exceptions for this task to the collection, and if it's wait
                    // notification bit is set, store it to operate on at the end.
                    foreach (var task in waitedOnTaskList)
                    {
                        if (task.IsFaulted)
                        {
                            exceptionSeen = true;
                        }
                        else
                        {
                            cancellationSeen |= task.IsCanceled;
                        }

                        if (task.IsWaitNotificationEnabled)
                        {
                            AddToList(task, ref notificationTasks, /*initSize:*/ 1);
                        }
                    }
                }
                // We need to prevent the tasks array from being GC'ed until we come out of the wait.
                // This is necessary so that the Parallel Debugger can traverse it during the long wait and
                // deduce waiter/waitee relationships
                GC.KeepAlive(tasks);
            }
            // Now that we're done and about to exit, if the wait completed and if we have
            // any tasks with a notification bit set, signal the debugger if any requires it.
            if (allCompleted && notificationTasks != null)
            {
                // Loop through each task tha that had its bit set, and notify the debugger
                // about the first one that requires it.  The debugger will reset the bit
                // for any tasks we don't notify of as soon as we break, so we only need to notify
                // for one.
                foreach (var task in notificationTasks)
                {
                    if (task.NotifyDebuggerOfWaitCompletionIfNecessary())
                    {
                        break;
                    }
                }
            }
            // If one or more threw exceptions, aggregate and throw them.
            if (allCompleted && (exceptionSeen || cancellationSeen))
            {
                // If the WaitAll was canceled and tasks were canceled but not faulted,
                // prioritize throwing an OCE for canceling the WaitAll over throwing an
                // AggregateException for all of the canceled Tasks.  This helps
                // to bring determinism to an otherwise non-determistic case of using
                // the same token to cancel both the WaitAll and the Tasks.
                if (!exceptionSeen)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                // Now gather up and throw all of the exceptions.
                foreach (var task in tasks)
                {
                    AddExceptionsForCompletedTask(ref exceptions, task);
                }
                Contract.Assert(exceptions != null, "Should have seen at least one exception");
                throw new AggregateException(exceptions);
            }
            return allCompleted;
        }

        /// <summary>Adds an element to the list, initializing the list if it's null.</summary>
        /// <typeparam name="T">Specifies the type of data stored in the list.</typeparam>
        /// <param name="item">The item to add.</param>
        /// <param name="list">The list.</param>
        /// <param name="initSize">The size to which to initialize the list if the list is null.</param>
        private static void AddToList<T>(T item, ref List<T> list, int initSize)
        {
            if (list == null)
            {
                list = new List<T>(initSize);
            }

            list.Add(item);
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
            ManualResetEventSlim mres = null;
            WhenAllCore core = null;
            try
            {
                mres = new ManualResetEventSlim(false);
                core = new WhenAllCore(tasks, mres.Set);
                if (core.IsDone)
                {
                    waitCompleted = true;
                }
                else
                {
                    waitCompleted = mres.Wait(millisecondsTimeout, cancellationToken);
                }
            }
            finally
            {
                if (core != null)
                {
                    core.Dispose();
                    waitCompleted = core.IsDone;
                }
                if (mres != null)
                {
                    mres.Dispose();
                }
            }
            return waitCompleted;
        }

        internal static void FastWaitAll(Task[] tasks)
        {
            Contract.Requires(tasks != null);
            List<Exception> exceptions = null;
            // Collects incomplete tasks in "waitedOnTaskList" and their cooperative events in "cooperativeEventList"
            for (var taskIndex = tasks.Length - 1; taskIndex >= 0; taskIndex--)
            {
                ref var current = ref tasks[taskIndex];
                if (!current.IsCompleted)
                {
                    // Just attempting to inline here... result doesn't matter.
                    // We'll do a second pass to do actual wait on each task, and to aggregate their exceptions.
                    // If the task is inlined here, it will register as IsCompleted in the second pass
                    // and will just give us the exception.
                    current.TryStart(current.ExecutingTaskScheduler, true);
                }
            }
            // Wait on the tasks.
            for (var taskIndex = tasks.Length - 1; taskIndex >= 0; taskIndex--)
            {
                ref var current = ref tasks[taskIndex];
                current.Wait();
                AddExceptionsForCompletedTask(ref exceptions, current);
                // Note that unlike other wait code paths, we do not check
                // task.NotifyDebuggerOfWaitCompletionIfNecessary() here, because this method is currently
                // only used from contexts where the tasks couldn't have that bit set, namely
                // Parallel.Invoke.  If that ever changes, such checks should be added here.
            }
            // If one or more threw exceptions, aggregate them.
            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }

        internal static void AddExceptionsForCompletedTask(ref List<Exception> exceptions, Task t)
        {
            var ex = t.GetExceptions(true);
            if (ex != null)
            {
                // make sure the task's exception observed status is set appropriately
                // it's possible that WaitAll was called by the parent of an attached child,
                // this will make sure it won't throw again in the implicit wait
                t.UpdateExceptionObservedStatus();

                if (exceptions == null)
                {
                    exceptions = new List<Exception>(ex.InnerExceptions.Count);
                }

                exceptions.AddRange(ex.InnerExceptions);
            }
        }

        /// <summary>
        /// Waits for any of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <returns>The index of the completed task in the <paramref name="tasks"/> array argument.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
        public static int WaitAny(params Task[] tasks)
        {
            var waitResult = WaitAny(tasks, Timeout.Infinite);
            Contract.Assert(tasks.Length == 0 || waitResult != -1, "expected wait to succeed");
            return waitResult;
        }

        /// <summary>
        /// Waits for any of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <param name="timeout">
        /// A <see cref="System.TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely.
        /// </param>
        /// <returns>
        /// The index of the completed task in the <paramref name="tasks"/> array argument, or -1 if the
        /// timeout occurred.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="timeout"/> is a negative number other than -1 milliseconds, which represents an
        /// infinite time-out -or- timeout is greater than
        /// <see cref="int.MaxValue"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
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
        /// Waits for any of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for a task to complete.
        /// </param>
        /// <returns>
        /// The index of the completed task in the <paramref name="tasks"/> array argument.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.OperationCanceledException">
        /// The <paramref name="cancellationToken"/> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
        public static int WaitAny(Task[] tasks, CancellationToken cancellationToken)
        {
            return WaitAny(tasks, Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        /// Waits for any of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to
        /// wait indefinitely.
        /// </param>
        /// <returns>
        /// The index of the completed task in the <paramref name="tasks"/> array argument, or -1 if the
        /// timeout occurred.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an
        /// infinite time-out.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
        public static int WaitAny(Task[] tasks, int millisecondsTimeout)
        {
            return WaitAny(tasks, millisecondsTimeout, default(CancellationToken));
        }

        /// <summary>
        /// Waits for any of the provided <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">
        /// An array of <see cref="Task"/> instances on which to wait.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to
        /// wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for a task to complete.
        /// </param>
        /// <returns>
        /// The index of the completed task in the <paramref name="tasks"/> array argument, or -1 if the
        /// timeout occurred.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> argument contains a null element.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an
        /// infinite time-out.
        /// </exception>
        /// <exception cref="T:System.OperationCanceledException">
        /// The <paramref name="cancellationToken"/> was canceled.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoOptimization)]  // this is needed for the parallel debugger
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
            // We need to prevent the tasks array from being GC'ed until we come out of the wait.
            // This is necessary so that the Parallel Debugger can traverse it during the long wait
            // and deduce waiter/waitee relationships
            GC.KeepAlive(tasks);
            // Return the index
            return signaledTaskIndex;
        }

        private static void PrivateWaitAny(Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken, ref int signaledTaskIndex)
        {
            var firstCompleted = PrivateWhenAny(tasks, ref signaledTaskIndex);
            if (signaledTaskIndex == -1)
            {
                var waitCompleted = firstCompleted.Wait(millisecondsTimeout, cancellationToken);
                if (waitCompleted)
                {
                    signaledTaskIndex = Array.IndexOf(tasks, firstCompleted.Result);
                }
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
    }
}

#endif