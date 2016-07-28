#if NET35

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Provides methods for creating and manipulating tasks.
    /// </summary>
    ///
    /// <remarks>
    /// TaskEx is a placeholder.
    /// </remarks>
    public static class TaskEx
    {
        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay in milliseconds before the returned task completes.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(int dueTime)
        {
            return Delay(dueTime, CancellationToken.None);
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(TimeSpan dueTime)
        {
            return Delay(dueTime, CancellationToken.None);
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param><param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(TimeSpan dueTime, CancellationToken cancellationToken)
        {
            return Task.Delay(dueTime, cancellationToken);
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay in milliseconds before the returned task completes.</param><param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        /// The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime"/> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        ///             </exception>
        public static Task Delay(int dueTime, CancellationToken cancellationToken)
        {
            return Task.Delay(dueTime, cancellationToken);
        }

        /// <summary>
        /// Creates an already completed <see cref="T:System.Threading.Tasks.Task`1"/> from the specified result.
        /// </summary>
        /// <param name="result">The result from which to create the completed task.</param>
        /// <returns>
        /// The completed task.
        /// </returns>
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            return Task.FromResult(result);
        }

        /// <summary>
        /// Creates a task that runs the specified action.
        /// </summary>
        /// <param name="action">The action to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action"/> argument is null.</exception>
        public static Task Run(Action action)
        {
            return Run(action, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified action.
        /// </summary>
        /// <param name="action">The action to execute.</param><param name="cancellationToken">The CancellationToken to use to request cancellation of this task.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action"/> argument is null.</exception>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute.</param><param name="cancellationToken">The CancellationToken to use to cancel the task.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task Run(Func<Task> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute.</param><param name="cancellationToken">The CancellationToken to use to request cancellation of this task.</param>
        /// <returns>
        /// A task that represents the completion of the function.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
            return Run<Task>(function, cancellationToken).Unwrap();
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        /// Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute.</param><param name="cancellationToken">The CancellationToken to use to cancel the task.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function"/> argument is null.</exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            return Run<Task<TResult>>(function, cancellationToken).Unwrap();
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        ///
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        ///
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task WhenAll(params Task[] tasks)
        {
            return WhenAll((IEnumerable<Task>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        ///
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        ///
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAll((IEnumerable<Task<TResult>>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        ///
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        ///
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        ///
        /// <remarks>
        /// If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///             about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///             Task will also be canceled.
        ///
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        ///
        /// </returns>
        ///
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
            return WhenAny((IEnumerable<Task>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        ///
        /// </returns>
        ///
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
            return Task.WhenAny(tasks);
        }

        /// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        ///
        /// </returns>
        ///
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAny((IEnumerable<Task<TResult>>)tasks);
        }

        /// <summary>
        /// Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        /// A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        ///
        /// </returns>
        ///
        /// <remarks>
        /// Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            return Task.WhenAny(tasks);
        }

        /// <summary>
        /// Creates an awaitable that asynchronously yields back to the current context when awaited.
        /// </summary>
        ///
        /// <returns>
        /// A context that, when awaited, will asynchronously transition back into the current context.
        ///             If SynchronizationContext.Current is non-null, that is treated as the current context.
        ///             Otherwise, TaskScheduler.Current is treated as the current context.
        ///
        /// </returns>
        public static YieldAwaitable Yield()
        {
            return new YieldAwaitable();
        }
    }
}

#endif