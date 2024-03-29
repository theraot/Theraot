﻿#if LESSTHAN_NET40

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        /// <summary>
        ///     Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        ///     <para>
        ///         If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted
        ///         state,
        ///         where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied
        ///         tasks.
        ///     </para>
        ///     <para>
        ///         If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the
        ///         Canceled state.
        ///     </para>
        ///     <para>
        ///         If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the
        ///         RanToCompletion state.
        ///     </para>
        ///     <para>
        ///         If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a
        ///         RanToCompletion
        ///         state before it's returned to the caller.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> collection contained a null task.
        /// </exception>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (tasks is Task[] array)
            {
                // Take a more efficient path if tasks is actually an array
                return WhenAll(array);
            }

            if (tasks is ICollection<Task> collection)
            {
                var index = 0;
                Task[] taskArray = new Task[collection.Count];
                foreach (var task in collection)
                {
                    taskArray[index++] =
                        task ?? throw new ArgumentException("The tasks argument included a null value.", nameof(tasks));
                }

                return InternalWhenAll(taskArray);
            }

            var taskList = new List<Task>();
            foreach (var task in tasks)
            {
                if (task == null)
                {
                    throw new ArgumentException("The tasks argument included a null value.", nameof(tasks));
                }

                taskList.Add(task);
            }

            // Delegate the rest to InternalWhenAll()
            return InternalWhenAll(taskList.ToArray());
        }

        /// <summary>
        ///     Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        ///     <para>
        ///         If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted
        ///         state,
        ///         where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied
        ///         tasks.
        ///     </para>
        ///     <para>
        ///         If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the
        ///         Canceled state.
        ///     </para>
        ///     <para>
        ///         If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the
        ///         RanToCompletion state.
        ///     </para>
        ///     <para>
        ///         If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a
        ///         RanToCompletion
        ///         state before it's returned to the caller.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> array contained a null task.
        /// </exception>
        public static Task WhenAll(params Task[] tasks)
        {
            // Do some argument checking and make a defensive copy of the tasks array
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            Contract.EndContractBlock();
            var taskCount = tasks.Length;
            if (taskCount == 0)
            {
                return InternalWhenAll(tasks); // Small optimization in the case of an empty array.
            }

            var tasksCopy = new Task[taskCount];
            for (var i = 0; i < taskCount; i++)
            {
                var task = tasks[i];

                tasksCopy[i] = task ?? throw new ArgumentException("The tasks argument included a null value.", nameof(tasks));
            }

            // The rest can be delegated to InternalWhenAll()
            return InternalWhenAll(tasksCopy);
        }

        /// <summary>
        ///     Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        ///     <para>
        ///         If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted
        ///         state,
        ///         where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied
        ///         tasks.
        ///     </para>
        ///     <para>
        ///         If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the
        ///         Canceled state.
        ///     </para>
        ///     <para>
        ///         If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the
        ///         RanToCompletion state.
        ///         The Result of the returned task will be set to an array containing all of the results of the
        ///         supplied tasks in the same order as they were provided (e.g. if the input tasks array contained t1, t2, t3, the
        ///         output
        ///         task's Result will return an TResult[] where arr[0] == t1.Result, arr[1] == t2.Result, and arr[2] ==
        ///         t3.Result).
        ///     </para>
        ///     <para>
        ///         If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a
        ///         RanToCompletion
        ///         state before it's returned to the caller.  The returned TResult[] will be an array of 0 elements.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> collection contained a null task.
        /// </exception>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (tasks is Task<TResult>[] array)
            {
                // Take a more efficient route if tasks is actually an array
                return WhenAll(array);
            }

            if (tasks is ICollection<Task<TResult>> collection)
            {
                var index = 0;
                Task<TResult>[] taskArray = new Task<TResult>[collection.Count];
                foreach (var task in collection)
                {
                    taskArray[index++] =
                        task ?? throw new ArgumentException("The tasks argument included a null value.", nameof(tasks));
                }

                return InternalWhenAll(taskArray);
            }

            var taskList = new List<Task<TResult>>();
            foreach (var task in tasks)
            {
                if (task == null)
                {
                    throw new ArgumentException("Task_MultiTaskContinuation_NullTask", nameof(tasks));
                }

                taskList.Add(task);
            }

            // Delegate the rest to InternalWhenAll<TResult>().
            return InternalWhenAll(taskList.ToArray());
        }

        /// <summary>
        ///     Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        ///     <para>
        ///         If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted
        ///         state,
        ///         where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied
        ///         tasks.
        ///     </para>
        ///     <para>
        ///         If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the
        ///         Canceled state.
        ///     </para>
        ///     <para>
        ///         If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the
        ///         RanToCompletion state.
        ///         The Result of the returned task will be set to an array containing all of the results of the
        ///         supplied tasks in the same order as they were provided (e.g. if the input tasks array contained t1, t2, t3, the
        ///         output
        ///         task's Result will return an TResult[] where arr[0] == t1.Result, arr[1] == t2.Result, and arr[2] ==
        ///         t3.Result).
        ///     </para>
        ///     <para>
        ///         If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a
        ///         RanToCompletion
        ///         state before it's returned to the caller.  The returned TResult[] will be an array of 0 elements.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> array contained a null task.
        /// </exception>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            // Do some argument checking and make a defensive copy of the tasks array
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            Contract.EndContractBlock();
            var taskCount = tasks.Length;
            if (taskCount == 0)
            {
                return InternalWhenAll(tasks); // small optimization in the case of an empty task array
            }

            var tasksCopy = new Task<TResult>[taskCount];
            for (var i = 0; i < taskCount; i++)
            {
                var task = tasks[i];

                tasksCopy[i] = task ?? throw new ArgumentException("The tasks argument included a null value.", nameof(tasks));
            }

            // Delegate the rest to InternalWhenAll<TResult>()
            return InternalWhenAll(tasksCopy);
        }

        /// <summary>
        ///     Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        ///     A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that
        ///     completed.
        /// </returns>
        /// <remarks>
        ///     The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in
        ///     the RanToCompletion state
        ///     with its Result set to the first task to complete.  This is true even if the first task to complete ended in the
        ///     Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> array contained a null task, or was empty.
        /// </exception>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (tasks.Length == 0)
            {
                throw new ArgumentException("The tasks argument contains no tasks.", nameof(tasks));
            }

            Contract.EndContractBlock();
            // Make a defensive copy, as the user may manipulate the tasks array
            // after we return but before the WhenAny asynchronously completes.
            var taskCount = tasks.Length;
            var tasksCopy = new Task[taskCount];
            for (var index = 0; index < taskCount; index++)
            {
                var task = tasks[index];
                tasksCopy[index] = task ?? throw new ArgumentException("The tasks argument included a null value.", nameof(tasks));
            }

            var signaledTaskIndex = -1;
            return PrivateWhenAny(tasksCopy, ref signaledTaskIndex);
        }

        /// <summary>
        ///     Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        ///     A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that
        ///     completed.
        /// </returns>
        /// <remarks>
        ///     The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in
        ///     the RanToCompletion state
        ///     with its Result set to the first task to complete.  This is true even if the first task to complete ended in the
        ///     Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> collection contained a null task, or was empty.
        /// </exception>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            Contract.EndContractBlock();
            // Make a defensive copy, as the user may manipulate the tasks collection
            // after we return but before the WhenAny asynchronously completes.
            var taskList = new List<Task>();
            foreach (var task in tasks)
            {
                if (task == null)
                {
                    throw new ArgumentException("The tasks argument included a null value.", nameof(tasks));
                }

                taskList.Add(task);
            }

            if (taskList.Count == 0)
            {
                throw new ArgumentException("The tasks argument contains no tasks.", nameof(tasks));
            }

            var signaledTaskIndex = -1;
            return PrivateWhenAny(taskList, ref signaledTaskIndex);
        }

        /// <summary>
        ///     Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        ///     A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that
        ///     completed.
        /// </returns>
        /// <remarks>
        ///     The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in
        ///     the RanToCompletion state
        ///     with its Result set to the first task to complete.  This is true even if the first task to complete ended in the
        ///     Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> array contained a null task, or was empty.
        /// </exception>
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            // ReSharper disable once CoVariantArrayConversion
            var intermediate = WhenAny((Task[])tasks);
            return intermediate.ContinueWith(Task<TResult>.ContinuationConversion, default, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        ///     Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>
        ///     A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that
        ///     completed.
        /// </returns>
        /// <remarks>
        ///     The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in
        ///     the RanToCompletion state
        ///     with its Result set to the first task to complete.  This is true even if the first task to complete ended in the
        ///     Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="tasks" /> argument was null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="tasks" /> collection contained a null task, or was empty.
        /// </exception>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            var intermediate = WhenAny((IEnumerable<Task>)tasks);
            return intermediate.ContinueWith(Task<TResult>.ContinuationConversion, default, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>Returns true if any of the supplied tasks require wait notification.</summary>
        /// <param name="tasks">The tasks to check.</param>
        /// <returns>true if any of the tasks require notification; otherwise, false.</returns>
        internal static bool AnyTaskRequiresNotifyDebuggerOfWaitCompletion(IEnumerable<Task?> tasks)
        {
            if (tasks != null)
            {
                return tasks.Any(task => task?.IsWaitNotificationEnabled == true && task.ShouldNotifyDebuggerOfWaitCompletion);
            }

            Contract.Assert(condition: false, "Expected non-null array of tasks");
            throw new ArgumentNullException(nameof(tasks));
        }

        // Some common logic to support WhenAll() methods
        // tasks should be a defensive copy.
        private static Task InternalWhenAll(Task[] tasks)
        {
            // take shortcut if there are no tasks upon which to wait
            return tasks.Length == 0 ? TaskExEx.CompletedTask : new WhenAllPromise(tasks);
        }

        // Some common logic to support WhenAll<TResult> methods
        private static Task<TResult[]> InternalWhenAll<TResult>(Task<TResult>[] tasks)
        {
            // take shortcut if there are no tasks upon which to wait
            return tasks.Length == 0 ? FromResult(new TResult[0]) : new WhenAllPromise<TResult>(tasks);
        }
    }
}

#endif