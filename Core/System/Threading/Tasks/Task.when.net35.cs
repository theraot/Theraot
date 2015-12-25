#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using Theraot;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        /// <para>
        /// If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted state,
        /// where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied tasks.
        /// </para>
        /// <para>
        /// If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the Canceled state.
        /// </para>
        /// <para>
        /// If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the RanToCompletion state.
        /// </para>
        /// <para>
        /// If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a RanToCompletion
        /// state before it's returned to the caller.
        /// </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> collection contained a null task.
        /// </exception>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
            // Take a more efficient path if tasks is actually an array
            var taskArray = tasks as Task[];
            if (taskArray != null)
            {
                return WhenAll(taskArray);
            }
            // Skip a List allocation/copy if tasks is a collection
            var taskCollection = tasks as ICollection<Task>;
            if (taskCollection != null)
            {
                var index = 0;
                taskArray = new Task[taskCollection.Count];
                foreach (var task in tasks)
                {
                    if (task == null) throw new ArgumentException("The tasks argument included a null value.", "tasks");
                    taskArray[index++] = task;
                }
                return InternalWhenAll(taskArray);
            }
            // Do some argument checking and convert tasks to a List (and later an array).
            if (tasks == null) throw new ArgumentNullException("tasks");
            var taskList = new List<Task>();
            foreach (var task in tasks)
            {
                if (task == null) throw new ArgumentException("The tasks argument included a null value.", "tasks");
                taskList.Add(task);
            }
            // Delegate the rest to InternalWhenAll()
            return InternalWhenAll(taskList.ToArray());
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        /// <para>
        /// If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted state,
        /// where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied tasks.
        /// </para>
        /// <para>
        /// If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the Canceled state.
        /// </para>
        /// <para>
        /// If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the RanToCompletion state.
        /// </para>
        /// <para>
        /// If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a RanToCompletion
        /// state before it's returned to the caller.
        /// </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> array contained a null task.
        /// </exception>
        public static Task WhenAll(params Task[] tasks)
        {
            // Do some argument checking and make a defensive copy of the tasks array
            if (tasks == null) throw new ArgumentNullException("tasks");
            Contract.EndContractBlock();
            var taskCount = tasks.Length;
            if (taskCount == 0) return InternalWhenAll(tasks); // Small optimization in the case of an empty array.
            var tasksCopy = new Task[taskCount];
            for (var i = 0; i < taskCount; i++)
            {
                var task = tasks[i];
                if (task == null) throw new ArgumentException("The tasks argument included a null value.", "tasks");
                tasksCopy[i] = task;
            }
            // The rest can be delegated to InternalWhenAll()
            return InternalWhenAll(tasksCopy);
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        /// <para>
        /// If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted state,
        /// where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied tasks.
        /// </para>
        /// <para>
        /// If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the Canceled state.
        /// </para>
        /// <para>
        /// If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the RanToCompletion state.
        /// The Result of the returned task will be set to an array containing all of the results of the
        /// supplied tasks in the same order as they were provided (e.g. if the input tasks array contained t1, t2, t3, the output
        /// task's Result will return an TResult[] where arr[0] == t1.Result, arr[1] == t2.Result, and arr[2] == t3.Result).
        /// </para>
        /// <para>
        /// If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a RanToCompletion
        /// state before it's returned to the caller.  The returned TResult[] will be an array of 0 elements.
        /// </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> collection contained a null task.
        /// </exception>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            // Take a more efficient route if tasks is actually an array
            var taskArray = tasks as Task<TResult>[];
            if (taskArray != null)
            {
                return WhenAll(taskArray);
            }
            // Skip a List allocation/copy if tasks is a collection
            var taskCollection = tasks as ICollection<Task<TResult>>;
            if (taskCollection != null)
            {
                var index = 0;
                taskArray = new Task<TResult>[taskCollection.Count];
                foreach (var task in tasks)
                {
                    if (task == null) throw new ArgumentException("The tasks argument included a null value.", "tasks");
                    taskArray[index++] = task;
                }
                return InternalWhenAll(taskArray);
            }
            // Do some argument checking and convert tasks into a List (later an array)
            if (tasks == null) throw new ArgumentNullException("tasks");
            var taskList = new List<Task<TResult>>();
            foreach (var task in tasks)
            {
                if (task == null) throw new ArgumentException("Task_MultiTaskContinuation_NullTask", "tasks");
                taskList.Add(task);
            }
            // Delegate the rest to InternalWhenAll<TResult>().
            return InternalWhenAll(taskList.ToArray());
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <remarks>
        /// <para>
        /// If any of the supplied tasks completes in a faulted state, the returned task will also complete in a Faulted state,
        /// where its exceptions will contain the aggregation of the set of unwrapped exceptions from each of the supplied tasks.
        /// </para>
        /// <para>
        /// If none of the supplied tasks faulted but at least one of them was canceled, the returned task will end in the Canceled state.
        /// </para>
        /// <para>
        /// If none of the tasks faulted and none of the tasks were canceled, the resulting task will end in the RanToCompletion state.
        /// The Result of the returned task will be set to an array containing all of the results of the
        /// supplied tasks in the same order as they were provided (e.g. if the input tasks array contained t1, t2, t3, the output
        /// task's Result will return an TResult[] where arr[0] == t1.Result, arr[1] == t2.Result, and arr[2] == t3.Result).
        /// </para>
        /// <para>
        /// If the supplied array/enumerable contains no tasks, the returned task will immediately transition to a RanToCompletion
        /// state before it's returned to the caller.  The returned TResult[] will be an array of 0 elements.
        /// </para>
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> array contained a null task.
        /// </exception>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            // Do some argument checking and make a defensive copy of the tasks array
            if (tasks == null) throw new ArgumentNullException("tasks");
            Contract.EndContractBlock();
            var taskCount = tasks.Length;
            if (taskCount == 0) return InternalWhenAll(tasks); // small optimization in the case of an empty task array
            var tasksCopy = new Task<TResult>[taskCount];
            for (var i = 0; i < taskCount; i++)
            {
                var task = tasks[i];
                if (task == null) throw new ArgumentException("The tasks argument included a null value.", "tasks");
                tasksCopy[i] = task;
            }
            // Delegate the rest to InternalWhenAll<TResult>()
            return InternalWhenAll(tasksCopy);
        }

        /// <summary>Returns true if any of the supplied tasks require wait notification.</summary>
        /// <param name="tasks">The tasks to check.</param>
        /// <returns>true if any of the tasks require notification; otherwise, false.</returns>
        internal static bool AnyTaskRequiresNotifyDebuggerOfWaitCompletion(IEnumerable<Task> tasks)
        {
            if (tasks == null)
            {
                Contract.Assert(false, "Expected non-null array of tasks");
                throw new ArgumentNullException("tasks");
            }
            foreach (var task in tasks)
            {
                if
                (
                    task != null &&
                    task.IsWaitNotificationEnabled &&
                    task.ShouldNotifyDebuggerOfWaitCompletion
                ) // potential recursion
                {
                    return true;
                }
            }
            return false;
        }

        // Some common logic to support WhenAll() methods
        // tasks should be a defensive copy.
        private static Task InternalWhenAll(Task[] tasks)
        {
            Contract.Requires(tasks != null, "Expected a non-null tasks array");
            // take shortcut if there are no tasks upon which to wait
            if (tasks.Length == 0)
            {
                return CompletedTask;
            }
            return new WhenAllPromise(tasks);
        }

        // Some common logic to support WhenAll<TResult> methods
        private static Task<TResult[]> InternalWhenAll<TResult>(Task<TResult>[] tasks)
        {
            Contract.Requires(tasks != null, "Expected a non-null tasks array");
            // take shortcut if there are no tasks upon which to wait
            if (tasks.Length == 0)
            {
                return FromResult(new TResult[0]);
            }
            return new WhenAllPromise<TResult>(tasks);
        }

        // A Task<VoidTaskResult> that gets completed when all of its constituent tasks complete.
        // Completion logic will analyze the antecedents in order to choose completion status.
        // This type allows us to replace this logic:
        //      Task<VoidTaskResult> promise = new Task<VoidTaskResult>(...);
        //      Action<Task> completionAction = delegate { <completion logic>};
        //      TaskFactory.CommonCWAllLogic(tasksCopy).AddCompletionAction(completionAction);
        //      return promise;
        // which involves several allocations, with this logic:
        //      return new WhenAllPromise(tasksCopy);
        // which saves a couple of allocations and enables debugger notification specialization.
        //
        // Used in InternalWhenAll(Task[])
        private sealed class WhenAllPromise : Task<VoidStruct>, ITaskCompletionAction
        {
            /// <summary>
            /// Stores all of the constituent tasks.  Tasks clear themselves out of this
            /// array as they complete, but only if they don't have their wait notification bit set.
            /// </summary>
            private readonly Task[] _tasks;

            /// <summary>The number of tasks remaining to complete.</summary>
            private int _count;
            internal WhenAllPromise(Task[] tasks)
            {
                Contract.Requires(tasks != null, "Expected a non-null task array");
                Contract.Requires(tasks.Length > 0, "Expected a non-zero length task array");
                _tasks = tasks;
                _count = tasks.Length;
                foreach (var task in tasks)
                {
                    if (task.IsCompleted) Invoke(task); // short-circuit the completion action, if possible
                    else task.AddTaskContinuation(this, false); // simple completion action
                }
            }

            /// <summary>
            /// Returns whether we should notify the debugger of a wait completion.  This returns
            /// true iff at least one constituent task has its bit set.
            /// </summary>
            internal override bool ShouldNotifyDebuggerOfWaitCompletion
            {
                get
                {
                    return
                        base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks);
                }
            }

            public void Invoke(Task completedTask)
            {
                // Decrement the count, and only continue to complete the promise if we're the last one.
                if (Interlocked.Decrement(ref _count) == 0)
                {
                    // Set up some accounting variables
                    List<ExceptionDispatchInfo> observedExceptions = null;
                    Task canceledTask = null;
                    // Loop through antecedents:
                    //   If any one of them faults, the result will be faulted
                    //   If none fault, but at least one is canceled, the result will be canceled
                    //   If none fault or are canceled, then result will be RanToCompletion
                    for (var index = 0; index < _tasks.Length; index++)
                    {
                        var task = _tasks[index];
                        if (task == null)
                        {
                            Contract.Assert(false, "Constituent task in WhenAll should never be null");
                            throw new InvalidOperationException("Constituent task in WhenAll should never be null");
                        }
                        if (task.IsFaulted)
                        {
                            if (observedExceptions == null)
                            {
                                observedExceptions = new List<ExceptionDispatchInfo>();
                            }
                            observedExceptions.AddRange(task._exceptionsHolder.GetExceptionDispatchInfos());
                        }
                        else if (task.IsCanceled)
                        {
                            if (canceledTask == null) canceledTask = task; // use the first task that's canceled
                        }
                        // Regardless of completion state, if the task has its debug bit set, transfer it to the
                        // WhenAll task.  We must do this before we complete the task.
                        if (task.IsWaitNotificationEnabled) SetNotificationForWaitCompletion(enabled: true);
                        else _tasks[index] = null; // avoid holding onto tasks unnecessarily
                    }
                    if (observedExceptions != null)
                    {
                        Contract.Assert(observedExceptions.Count > 0, "Expected at least one exception");
                        //We don't need to TraceOperationCompleted here because TrySetException will call Finish and we'll log it there
                        TrySetException(observedExceptions);
                    }
                    else if (canceledTask != null)
                    {
                        TrySetCanceled(canceledTask.CancellationToken, canceledTask._exceptionsHolder.GetCancellationExceptionDispatchInfo());
                    }
                    else
                    {
                        TrySetResult(default(VoidStruct));
                    }
                }
                Contract.Assert(_count >= 0, "Count should never go below 0");
            }
        }

        // A Task<T> that gets completed when all of its constituent tasks complete.
        // Completion logic will analyze the antecedents in order to choose completion status.
        // See comments for non-generic version of WhenAllPromise class.
        //
        // Used in InternalWhenAll<TResult>(Task<TResult>[])
        private sealed class WhenAllPromise<T> : Task<T[]>, ITaskCompletionAction
        {
            /// <summary>
            /// Stores all of the constituent tasks.  Tasks clear themselves out of this
            /// array as they complete, but only if they don't have their wait notification bit set.
            /// </summary>
            private readonly Task<T>[] _tasks;

            /// <summary>The number of tasks remaining to complete.</summary>
            private int _count;
            internal WhenAllPromise(Task<T>[] tasks)
            {
                Contract.Requires(tasks != null, "Expected a non-null task array");
                Contract.Requires(tasks.Length > 0, "Expected a non-zero length task array");
                _tasks = tasks;
                _count = tasks.Length;
                foreach (var task in tasks)
                {
                    if (task.IsCompleted) Invoke(task); // short-circuit the completion action, if possible
                    else task.AddTaskContinuation(this, false); // simple completion action
                }
            }

            /// <summary>
            /// Returns whether we should notify the debugger of a wait completion.  This returns true
            /// iff at least one constituent task has its bit set.
            /// </summary>
            internal override bool ShouldNotifyDebuggerOfWaitCompletion
            {
                get
                {
                    return base.ShouldNotifyDebuggerOfWaitCompletion && AnyTaskRequiresNotifyDebuggerOfWaitCompletion(_tasks);
                }
            }

            public void Invoke(Task ignored)
            {
                // Decrement the count, and only continue to complete the promise if we're the last one.
                if (Interlocked.Decrement(ref _count) == 0)
                {
                    // Set up some accounting variables
                    var results = new T[_tasks.Length];
                    List<ExceptionDispatchInfo> observedExceptions = null;
                    Task canceledTask = null;
                    // Loop through antecedents:
                    //   If any one of them faults, the result will be faulted
                    //   If none fault, but at least one is canceled, the result will be canceled
                    //   If none fault or are canceled, then result will be RanToCompletion
                    for (var index = 0; index < _tasks.Length; index++)
                    {
                        var task = _tasks[index];
                        if (task == null)
                        {
                            Contract.Assert(false, "Constituent task in WhenAll should never be null");
                            throw new InvalidOperationException("Constituent task in WhenAll should never be null");
                        }
                        if (task.IsFaulted)
                        {
                            if (observedExceptions == null)
                            {
                                observedExceptions = new List<ExceptionDispatchInfo>();
                            }
                            observedExceptions.AddRange(task._exceptionsHolder.GetExceptionDispatchInfos());
                        }
                        else if (task.IsCanceled)
                        {
                            if (canceledTask == null) canceledTask = task; // use the first task that's canceled
                        }
                        else
                        {
                            Contract.Assert(task.Status == TaskStatus.RanToCompletion);
                            results[index] = task.Result;
                        }
                        // Regardless of completion state, if the task has its debug bit set, transfer it to the
                        // WhenAll task.  We must do this before we complete the task.
                        if (task.IsWaitNotificationEnabled) SetNotificationForWaitCompletion(enabled: true);
                        else _tasks[index] = null; // avoid holding onto tasks unnecessarily
                    }
                    if (observedExceptions != null)
                    {
                        Contract.Assert(observedExceptions.Count > 0, "Expected at least one exception");

                        //We don't need to TraceOperationCompleted here because TrySetException will call Finish and we'll log it there

                        TrySetException(observedExceptions);
                    }
                    else if (canceledTask != null)
                    {
                        TrySetCanceled(canceledTask.CancellationToken, canceledTask._exceptionsHolder.GetCancellationExceptionDispatchInfo());
                    }
                    else
                    {
                        TrySetResult(results);
                    }
                }
                Contract.Assert(_count >= 0, "Count should never go below 0");
            }
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that completed.</returns>
        /// <remarks>
        /// The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in the RanToCompletion state 
        /// with its Result set to the first task to complete.  This is true even if the first task to complete ended in the Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> array contained a null task, or was empty.
        /// </exception>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }
            if (tasks.Length == 0)
            {
                throw new ArgumentException("The tasks argument contains no tasks.", "tasks");
            }
            Contract.EndContractBlock();
            // Make a defensive copy, as the user may manipulate the tasks array
            // after we return but before the WhenAny asynchronously completes.
            var taskCount = tasks.Length;
            var tasksCopy = new Task[taskCount];
            for (var index = 0; index < taskCount; index++)
            {
                var task = tasks[index];
                if (task == null)
                {
                    throw new ArgumentException("The tasks argument included a null value.", "tasks");
                }
                tasksCopy[index] = task;
            }
            var signaledTaskIndex = -1;
            return PrivateWhenAny(tasksCopy, ref signaledTaskIndex);
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that completed.</returns>
        /// <remarks>
        /// The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in the RanToCompletion state 
        /// with its Result set to the first task to complete.  This is true even if the first task to complete ended in the Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> collection contained a null task, or was empty.
        /// </exception>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }
            Contract.EndContractBlock();
            // Make a defensive copy, as the user may manipulate the tasks collection
            // after we return but before the WhenAny asynchronously completes.
            var taskList = new List<Task>();
            foreach (var task in tasks)
            {
                if (task == null)
                {
                    throw new ArgumentException("The tasks argument included a null value.", "tasks");
                }
                taskList.Add(task);
            }
            if (taskList.Count == 0)
            {
                throw new ArgumentException("The tasks argument contains no tasks.", "tasks");
            }
            var signaledTaskIndex = -1;
            return PrivateWhenAny(taskList, ref signaledTaskIndex);
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that completed.</returns>
        /// <remarks>
        /// The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in the RanToCompletion state 
        /// with its Result set to the first task to complete.  This is true even if the first task to complete ended in the Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> array contained a null task, or was empty.
        /// </exception>
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            // We would just like to do this:
            //    return (Task<Task<TResult>>) WhenAny( (Task[]) tasks);
            // but classes are not covariant to enable casting Task<TResult> to Task<Task<TResult>>.
            // Call WhenAny(Task[]) for basic functionality
            var intermediate = WhenAny((Task[])tasks);
            // Return a continuation task with the correct result type
            return intermediate.ContinueWith(Task<TResult>.ContinuationConvertion, default(CancellationToken), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return Task's Result is the task that completed.</returns>
        /// <remarks>
        /// The returned task will complete when any of the supplied tasks has completed.  The returned task will always end in the RanToCompletion state 
        /// with its Result set to the first task to complete.  This is true even if the first task to complete ended in the Canceled or Faulted state.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="tasks"/> argument was null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="tasks"/> collection contained a null task, or was empty.
        /// </exception>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            // We would just like to do this:
            //    return (Task<Task<TResult>>) WhenAny( (IEnumerable<Task>) tasks);
            // but classes are not covariant to enable casting Task<TResult> to Task<Task<TResult>>.
            // Call WhenAny(IEnumerable<Task>) for basic functionality
            var intermediate = WhenAny((IEnumerable<Task>)tasks);
            // Return a continuation task with the correct result type
            return intermediate.ContinueWith(Task<TResult>.ContinuationConvertion, default(CancellationToken), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }
    }
}

#endif