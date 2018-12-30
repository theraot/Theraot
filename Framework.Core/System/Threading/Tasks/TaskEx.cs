using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if NET40

using System.Linq;

#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Task Delay(TimeSpan dueTime, CancellationToken cancellationToken)
        {
#if NET40
            var timeoutMs = (long)dueTime.TotalMilliseconds;
            if (timeoutMs < Timeout.Infinite || timeoutMs > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime), _argumentOutOfRangeTimeoutNonNegativeOrMinusOne);
            }

            return Delay((int)timeoutMs, cancellationToken);
#else
            // Missing in .NET 4.0
            return Task.Delay(dueTime, cancellationToken);
#endif
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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Task Delay(int dueTime, CancellationToken cancellationToken)
        {
#if NET40
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime), _argumentOutOfRangeTimeoutNonNegativeOrMinusOne);
            }
            if (cancellationToken.IsCancellationRequested)
            {
                return _preCanceledTask;
            }
            if (dueTime == 0)
            {
                return _preCompletedTask;
            }
            var tcs = new TaskCompletionSource<bool>();
            var timerBox = new Timer[] { null };
            var registration = GetRegistrationToken();
            var timer = new Timer(_ =>
            {
                registration.Dispose();
                Interlocked.Exchange(ref timerBox[0], null)?.Dispose();
                tcs.TrySetResult(true);
            }, null, Timeout.Infinite, Timeout.Infinite);
            Volatile.Write(ref timerBox[0], timer);
            try
            {
                timer.Change(dueTime, Timeout.Infinite);
            }
            catch (ObjectDisposedException exception)
            {
                GC.KeepAlive(exception);
            }
            return tcs.Task;
            CancellationTokenRegistration GetRegistrationToken()
            {
                if (cancellationToken.CanBeCanceled)
                {
                    var newRegistration = cancellationToken.Register
                    (
                        () =>
                        {
                            Interlocked.Exchange(ref timerBox[0], null)?.Dispose();
                            tcs.TrySetCanceled();
                        }
                    );
                    return newRegistration;
                }
                return default;
            }
#else
            // Missing in .NET 4.0
            return Task.Delay(dueTime, cancellationToken);
#endif
        }

        /// <summary>
        /// Creates an already completed <see cref="T:System.Threading.Tasks.Task`1"/> from the specified result.
        /// </summary>
        /// <param name="result">The result from which to create the completed task.</param>
        /// <returns>
        /// The completed task.
        /// </returns>
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Task<TResult> FromResult<TResult>(TResult result)
        {
#if NET40
            var completionSource = new TaskCompletionSource<TResult>(result);
            completionSource.TrySetResult(result);
            return completionSource.Task;
#else
            // Missing in .NET 4.0
            return Task.FromResult(result);
#endif
        }

        /// <summary>
        /// Creates a task that runs the specified action.
        /// </summary>
        /// <param name="action">The action to execute asynchronously.</param>
        /// <returns>
        /// A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action"/> argument is null.</exception>
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Task WhenAll(IEnumerable<Task> tasks)
        {
#if NET40
            return WhenAllCore(tasks, (Action<Task[], TaskCompletionSource<object>>)((completedTasks, tcs) => tcs.TrySetResult(null)));
#else
            // Missing in .NET 4.0
            return Task.WhenAll(tasks);
#endif
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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
#if NET40
            return WhenAllCore<TResult[]>(tasks, (completedTasks, tcs) =>
                                                 tcs.TrySetResult(completedTasks
                                                                      .Cast<Task<TResult>>()
                                                                      .Select(t => t.Result)
                                                                      .ToArray()));
#else
            // Missing in .NET 4.0
            return Task.WhenAll(tasks);
#endif
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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
#if NET40
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            var tcs = new TaskCompletionSource<Task>();
            Task.Factory.ContinueWhenAny(tasks as Task[] ?? tasks.ToArray(), tcs.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs.Task;
#else
            // Missing in .NET 4.0
            return Task.WhenAny(tasks);
#endif
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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
#if NET40
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            var tcs = new TaskCompletionSource<Task<TResult>>();
            Task.Factory.ContinueWhenAny(tasks as Task<TResult>[] ?? tasks.ToArray(), tcs.TrySetResult, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs.Task;
#else
            // Missing in .NET 4.0
            return Task.WhenAny(tasks);
#endif
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
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

        public static YieldAwaitable Yield()
        {
            return new YieldAwaitable();
        }

#if NET40

        private const string _argumentOutOfRangeTimeoutNonNegativeOrMinusOne = "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.";

        /// <summary>
        /// An already canceled task.
        /// </summary>
        private static readonly Task _preCanceledTask = ((Func<Task>)(() =>
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.TrySetCanceled();
            return tcs.Task;
        }))();

        /// <summary>
        /// An already completed task.
        /// </summary>
        private static readonly Task _preCompletedTask = FromResult(false);

        /// <summary>
        /// Adds the target exception to the list, initializing the list if it's null.
        /// </summary>
        /// <param name="targetList">The list to which to add the exception and initialize if the list is null.</param><param name="exception">The exception to add, and unwrap if it's an aggregate.</param>
        private static void AddPotentiallyUnwrappedExceptions(ref List<Exception> targetList, Exception exception)
        {
            if (targetList == null)
            {
                targetList = new List<Exception>();
            }

            if (exception is AggregateException aggregateException)
            {
                targetList.Add(aggregateException.InnerExceptions.Count == 1 ? exception.InnerException : exception);
            }
            else
            {
                targetList.Add(exception);
            }
        }

        /// <summary>
        /// Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param><param name="setResultAction">A callback invoked when all of the tasks complete successfully in the RanToCompletion state.
        ///             This callback is responsible for storing the results into the TaskCompletionSource.
        ///             </param>
        /// <returns>
        /// A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks"/> argument is null.</exception><exception cref="T:System.ArgumentException">The <paramref name="tasks"/> argument contains a null reference.</exception>
        private static Task<TResult> WhenAllCore<TResult>(IEnumerable<Task> tasks, Action<Task[], TaskCompletionSource<TResult>> setResultAction)
        {
#if DEBUG
            if (setResultAction == null)
            {
                throw new ArgumentException("setResultAction");
            }
#endif
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            var tcs = new TaskCompletionSource<TResult>();
            var taskArray = tasks as Task[] ?? tasks.ToArray();
            if (taskArray.Length == 0)
            {
                setResultAction(taskArray, tcs);
            }
            else
            {
                Task.Factory.ContinueWhenAll(taskArray, completedTasks =>
                {
                    List<Exception> exceptions = null;
                    var canceled = false;
                    foreach (var task in completedTasks)
                    {
                        if (task.IsFaulted)
                        {
                            AddPotentiallyUnwrappedExceptions(ref exceptions, task.Exception);
                        }
                        else if (task.IsCanceled)
                        {
                            canceled = true;
                        }
                    }
                    if (exceptions != null && exceptions.Count > 0)
                    {
                        tcs.TrySetException(exceptions);
                    }
                    else if (canceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        setResultAction(completedTasks, tcs);
                    }
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }

            return tcs.Task;
        }
#endif
    }
}