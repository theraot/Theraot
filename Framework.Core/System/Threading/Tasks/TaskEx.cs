#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable RCS1231 // Make parameter ref read-only.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theraot;

#if NET40
using System.Linq;

#endif

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Provides methods for creating and manipulating tasks.
    /// </summary>
    /// <remarks>
    ///     TaskEx is a placeholder.
    /// </remarks>
    public static partial class TaskEx
    {
        /// <summary>Gets a task that's already been completed successfully.</summary>
        /// <remarks>May not always return the same instance.</remarks>
        public static Task CompletedTask
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13
                var completedTask = _completedTask;
                if (completedTask == null)
                {
                    _completedTask = completedTask = CreateCompletedTask();
                }

                return completedTask;
#else
                return Task.CompletedTask;
#endif
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FromCanceled(CancellationToken cancellationToken)
        {
            return FromCanceled<VoidStruct>(cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
        {
#if LESSTHAN_NET40
            // Microsoft says Task.FromCancellation throws ArgumentOutOfRangeException when cancellation has not been requested for cancellationToken
            if (!cancellationToken.IsCancellationRequested)
            {
                throw new ArgumentOutOfRangeException(nameof(cancellationToken));
            }

            var task = new Task<TResult>();
            var value = task.TrySetCanceled(cancellationToken);
            if (value || task.IsCompleted)
            {
                return task;
            }

            var sw = new SpinWait();
            while (!task.IsCompleted)
            {
                sw.SpinOnce();
            }

            return task;
#elif LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13
            // Microsoft says Task.FromCancellation throws ArgumentOutOfRangeException when cancellation has not been requested for cancellationToken
            if (!cancellationToken.IsCancellationRequested)
            {
                throw new ArgumentOutOfRangeException(nameof(cancellationToken));
            }
            var taskCompleteSource = new TaskCompletionSource<TResult>();
            taskCompleteSource.TrySetCanceled();
            return taskCompleteSource.Task;
#else
            return Task.FromCanceled<TResult>(cancellationToken);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FromCancellation(CancellationToken token)
        {
            return FromCancellation<VoidStruct>(token);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromCancellation<TResult>(CancellationToken token)
        {
#if LESSTHAN_NET40
            var result = new Task<TResult>(TaskStatus.WaitingForActivation, InternalTaskOptions.PromiseTask)
            {
                CancellationToken = token,
                ExecutingTaskScheduler = TaskScheduler.Default
            };
            if (token.IsCancellationRequested)
            {
                result.InternalCancel(false);
            }
            else if (token.CanBeCanceled)
            {
                token.Register(() => result.InternalCancel(false));
            }

            return result;
#else
            var taskCompleteSource = new TaskCompletionSource<TResult>();
            if (token.IsCancellationRequested)
            {
                taskCompleteSource.TrySetCanceled();
                return taskCompleteSource.Task;
            }
            if (token.CanBeCanceled)
            {
                token.Register(() => taskCompleteSource.TrySetCanceled());
            }
            return taskCompleteSource.Task;
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FromException(Exception exception)
        {
            return FromException<VoidStruct>(exception);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromException<TResult>(Exception exception)
        {
#if LESSTHAN_NET40
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var task = new Task<TResult>();
            var value = task.TrySetException(exception);
            if (value || task.IsCompleted)
            {
                return task;
            }

            var sw = new SpinWait();
            while (!task.IsCompleted)
            {
                sw.SpinOnce();
            }

            return task;
#elif LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            var taskCompleteSource = new TaskCompletionSource<TResult>();
            taskCompleteSource.TrySetException(exception);
            return taskCompleteSource.Task;
#else
            return Task.FromException<TResult>(exception);
#endif
        }

        /// <summary>
        ///     Creates an already completed <see cref="T:System.Threading.Tasks.Task`1" /> from the specified result.
        /// </summary>
        /// <param name="result">The result from which to create the completed task.</param>
        /// <returns>
        ///     The completed task.
        /// </returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
#if NET40
            var completionSource = new TaskCompletionSource<TResult>();
            completionSource.TrySetResult(result);
            return completionSource.Task;
#else
            // Missing in .NET 4.0
            return Task.FromResult(result);
#endif
        }

        /// <summary>
        ///     Creates a task that runs the specified action.
        /// </summary>
        /// <param name="action">The action to execute asynchronously.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Run(Action action)
        {
            return Run(action, CancellationToken.None);
        }

        /// <summary>
        ///     Creates a task that runs the specified action.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="cancellationToken">The CancellationToken to use to request cancellation of this task.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the task.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute asynchronously.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Run(Func<Task> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        /// <param name="cancellationToken">The CancellationToken to use to request cancellation of this task.</param>
        /// <returns>
        ///     A task that represents the completion of the function.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
            return Run<Task>(function, cancellationToken).Unwrap();
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            return Run(function, CancellationToken.None);
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the task.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            return Run<Task<TResult>>(function, cancellationToken).Unwrap();
        }

        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WhenAll(params Task[] tasks)
        {
            return WhenAll((IEnumerable<Task>)tasks);
        }

        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAll((IEnumerable<Task<TResult>>)tasks);
        }

        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
#if NET40
            return WhenAllCore(tasks, (Action<Task[], TaskCompletionSource<object>>)((_, tcs) => tcs.TrySetResult(null)));
#else
            // Missing in .NET 4.0
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        ///     Creates a Task that will complete only when all of the provided collection of Tasks has completed.
        /// </summary>
        /// <param name="tasks">The Tasks to monitor for completion.</param>
        /// <returns>
        ///     A Task that represents the completion of all of the provided tasks.
        /// </returns>
        /// <remarks>
        ///     If any of the provided Tasks faults, the returned Task will also fault, and its Exception will contain information
        ///     about all of the faulted tasks.  If no Tasks fault but one or more Tasks is canceled, the returned
        ///     Task will also be canceled.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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
        ///     Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        ///     A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// </returns>
        /// <remarks>
        ///     Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<Task> WhenAny(params Task[] tasks)
        {
            return WhenAny((IEnumerable<Task>)tasks);
        }

        /// <summary>
        ///     Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        ///     A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// </returns>
        /// <remarks>
        ///     Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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
        ///     Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        ///     A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// </returns>
        /// <remarks>
        ///     Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAny((IEnumerable<Task<TResult>>)tasks);
        }

        /// <summary>
        ///     Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        ///     A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// </returns>
        /// <remarks>
        ///     Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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
        ///     Creates an awaitable that asynchronously yields back to the current context when awaited.
        /// </summary>
        /// <returns>
        ///     A context that, when awaited, will asynchronously transition back into the current context.
        ///     If SynchronizationContext.Current is non-null, that is treated as the current context.
        ///     Otherwise, TaskScheduler.Current is treated as the current context.
        /// </returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static YieldAwaitable Yield()
        {
            return new YieldAwaitable();
        }
    }

#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

#if NET40
    public static partial class TaskEx
    {
        private const string _argumentOutOfRangeTimeoutNonNegativeOrMinusOne = "The timeout must be non-negative or -1, and it must be less than or equal to Int32.MaxValue.";

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
                throw new ArgumentNullException(nameof(setResultAction));
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
                        else
                        {
                            canceled |= task.IsCanceled;
                        }
                    }
                    if (exceptions?.Count > 0)
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
    }
#endif

    public static partial class TaskEx
    {
        /// <summary>A task that's already been completed successfully.</summary>
        private static Task _completedTask;

        private static Task CreateCompletedTask()
        {
#if LESSTHAN_NET40
            return new Task(TaskStatus.RanToCompletion, InternalTaskOptions.DoNotDispose)
            {
                CancellationToken = default
            };
#else
            return FromResult(default(VoidStruct));
#endif
        }
    }
#endif

    public static partial class TaskEx
    {
        /// <summary>
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay in milliseconds before the returned task completes.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="dueTime" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Delay(int dueTime)
        {
            return Delay(dueTime, CancellationToken.None);
        }

        /// <summary>
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="dueTime" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Delay(TimeSpan dueTime)
        {
            return Delay(dueTime, CancellationToken.None);
        }

        /// <summary>
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param>
        /// <param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="dueTime" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay in milliseconds before the returned task completes.</param>
        /// <param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="dueTime" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Delay(int dueTime, CancellationToken cancellationToken)
        {
#if NET40
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime), _argumentOutOfRangeTimeoutNonNegativeOrMinusOne);
            }
            if (cancellationToken.IsCancellationRequested)
            {
                return FromCanceled(cancellationToken);
            }
            if (dueTime == 0)
            {
                return _completedTask;
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
                No.Op(exception);
            }
            return tcs.Task;
            CancellationTokenRegistration GetRegistrationToken()
            {
                if (!cancellationToken.CanBeCanceled)
                {
                    return default;
                }

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
#else
            // Missing in .NET 4.0
            return Task.Delay(dueTime, cancellationToken);
#endif
        }
    }

#if TARGETS_NETSTANDARD
    public static partial class TaskEx
    {
        private class WaitHandleCancellableTaskCompletionSourceManager
        {
            private readonly WaitHandle[] _handles;
            private readonly TaskCompletionSource<bool> _taskCompletionSource;

            private WaitHandleCancellableTaskCompletionSourceManager(WaitHandle waitHandle, CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
                _handles = new[] { waitHandle, cancellationToken.WaitHandle };
            }

            public static void CreateWithoutTimeout(WaitHandle waitHandle, CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource)
            {
                var result = new WaitHandleCancellableTaskCompletionSourceManager(waitHandle, cancellationToken, taskCompletionSource);
                var thread = new Thread(result.CallbackWithoutTimeout);
                thread.Start();
            }

            public static void CreateWithTimeout(WaitHandle waitHandle, CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleCancellableTaskCompletionSourceManager(waitHandle, cancellationToken, taskCompletionSource);
                var thread = new Thread(result.CallbackWithTimeout);
                thread.Start(millisecondsTimeout);
            }

            private void CallbackWithoutTimeout()
            {
                var index = WaitHandle.WaitAny(_handles);
                if (index == 0)
                {
                    _taskCompletionSource.SetResult(true);
                }
                _taskCompletionSource.TrySetCanceled();
            }

            private void CallbackWithTimeout(object state)
            {
                var index = WaitHandle.WaitAny(_handles, (int)state);
                if (index == 0)
                {
                    _taskCompletionSource.SetResult(true);
                }
                _taskCompletionSource.TrySetCanceled();
            }
        }

        private class WaitHandleTaskCompletionSourceManager
        {
            private readonly WaitHandle _handle;
            private readonly TaskCompletionSource<bool> _taskCompletionSource;

            private WaitHandleTaskCompletionSourceManager(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
                _handle = waitHandle;
            }

            public static void CreateWithoutTimeout(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource)
            {
                var result = new WaitHandleTaskCompletionSourceManager(waitHandle, taskCompletionSource);
                var thread = new Thread(result.CallbackWithoutTimeout);
                thread.Start();
            }

            public static void CreateWithTimeout(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleTaskCompletionSourceManager(waitHandle, taskCompletionSource);
                var thread = new Thread(result.CallbackWithTimeout);
                thread.Start(millisecondsTimeout);
            }

            private void CallbackWithoutTimeout()
            {
                _handle.WaitOne();
                _taskCompletionSource.SetResult(true);
            }

            private void CallbackWithTimeout(object state)
            {
                _taskCompletionSource.SetResult(_handle.WaitOne((int)state));
            }
        }
    }

#else

    public static partial class TaskEx
    {
        private class WaitHandleCancellableTaskCompletionSourceManager
        {
            private readonly CancellationToken _cancellationToken;
            private readonly RegisteredWaitHandle[] _registeredWaitHandle;
            private readonly TaskCompletionSource<bool> _taskCompletionSource;

            private WaitHandleCancellableTaskCompletionSourceManager(CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource)
            {
                _cancellationToken = cancellationToken;
                _taskCompletionSource = taskCompletionSource;
                _registeredWaitHandle = new RegisteredWaitHandle[1];
            }

            public static void CreateWithoutTimeout(WaitHandle waitHandle, CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource)
            {
                var result = new WaitHandleCancellableTaskCompletionSourceManager(cancellationToken, taskCompletionSource);
                result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithoutTimeout, null, -1, true);
                cancellationToken.Register(result.Unregister);
            }

            public static void CreateWithTimeout(WaitHandle waitHandle, CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleCancellableTaskCompletionSourceManager(cancellationToken, taskCompletionSource);
                result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithTimeout, null, millisecondsTimeout, true);
                cancellationToken.Register(result.Unregister);
            }

            private void CallbackWithoutTimeout(object state, bool timeOut)
            {
                Unregister();
                if (_cancellationToken.IsCancellationRequested)
                {
                    _taskCompletionSource.TrySetCanceled();
                    return;
                }

                _taskCompletionSource.TrySetResult(true);
            }

            private void CallbackWithTimeout(object state, bool timeOut)
            {
                Unregister();
                if (_cancellationToken.IsCancellationRequested)
                {
                    _taskCompletionSource.TrySetCanceled();
                    return;
                }

                if (timeOut)
                {
                    _taskCompletionSource.TrySetResult(false);
                    return;
                }

                _taskCompletionSource.TrySetResult(true);
            }

            private void Unregister()
            {
                Volatile.Read(ref _registeredWaitHandle[0]).Unregister(null);
            }
        }

        private class WaitHandleTaskCompletionSourceManager
        {
            private readonly RegisteredWaitHandle[] _registeredWaitHandle;
            private readonly TaskCompletionSource<bool> _taskCompletionSource;

            private WaitHandleTaskCompletionSourceManager(TaskCompletionSource<bool> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
                _registeredWaitHandle = new RegisteredWaitHandle[1];
            }

            public static void CreateWithoutTimeout(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource)
            {
                var result = new WaitHandleTaskCompletionSourceManager(taskCompletionSource);
                result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithoutTimeout, null, -1, true);
            }

            public static void CreateWithTimeout(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleTaskCompletionSourceManager(taskCompletionSource);
                result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithTimeout, null, millisecondsTimeout, true);
            }

            private void CallbackWithoutTimeout(object state, bool timeOut)
            {
                Unregister();
                _taskCompletionSource.TrySetResult(true);
            }

            private void CallbackWithTimeout(object state, bool timeOut)
            {
                Unregister();
                if (timeOut)
                {
                    _taskCompletionSource.TrySetResult(false);
                    return;
                }

                _taskCompletionSource.TrySetResult(true);
            }

            private void Unregister()
            {
                Volatile.Read(ref _registeredWaitHandle[0]).Unregister(null);
            }
        }
    }

#endif

    public static partial class TaskEx
    {
        public static Task FromWaitHandle(WaitHandle waitHandle)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            return FromWaitHandleInternal(waitHandle);
        }

        public static Task FromWaitHandle(WaitHandle waitHandle, CancellationToken cancellationToken)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            return FromWaitHandleInternal(waitHandle, cancellationToken);
        }

        public static Task<bool> FromWaitHandle(WaitHandle waitHandle, int millisecondsTimeout)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            return FromWaitHandleInternal(waitHandle, millisecondsTimeout);
        }

        public static Task<bool> FromWaitHandle(WaitHandle waitHandle, TimeSpan timeout)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            return FromWaitHandleInternal(waitHandle, (int)timeout.TotalMilliseconds);
        }

        public static Task<bool> FromWaitHandle(WaitHandle waitHandle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            return FromWaitHandleInternal(waitHandle, (int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static Task<bool> FromWaitHandle(WaitHandle waitHandle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            return FromWaitHandleInternal(waitHandle, millisecondsTimeout, cancellationToken);
        }

        internal static Task FromWaitHandleInternal(WaitHandle waitHandle)
        {
            var source = new TaskCompletionSource<bool>();
            if (waitHandle.WaitOne(0))
            {
                source.SetResult(true);
            }

            WaitHandleTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, source);
            return source.Task;
        }

        internal static Task FromWaitHandleInternal(WaitHandle waitHandle, TaskCreationOptions creationOptions)
        {
            var source = new TaskCompletionSource<bool>(creationOptions);
            if (waitHandle.WaitOne(0))
            {
                source.SetResult(true);
            }

            WaitHandleTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, source);
            return source.Task;
        }

        internal static Task<bool> FromWaitHandleInternal(WaitHandle waitHandle, int millisecondsTimeout)
        {
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

            var source = new TaskCompletionSource<bool>();
            if (waitHandle.WaitOne(0))
            {
                source.SetResult(true);
            }
            else if (millisecondsTimeout == -1)
            {
                WaitHandleTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, source);
            }
            else
            {
                WaitHandleTaskCompletionSourceManager.CreateWithTimeout(waitHandle, source, millisecondsTimeout);
            }

            return source.Task;
        }

        internal static Task FromWaitHandleInternal(WaitHandle waitHandle, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return FromCanceled<bool>(cancellationToken);
            }

            var source = new TaskCompletionSource<bool>();
            if (waitHandle.WaitOne(0))
            {
                source.SetResult(true);
            }

            WaitHandleCancellableTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, cancellationToken, source);
            return source.Task;
        }

        internal static Task<bool> FromWaitHandleInternal(WaitHandle waitHandle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return FromCanceled<bool>(cancellationToken);
            }

            var source = new TaskCompletionSource<bool>();
            if (waitHandle.WaitOne(0))
            {
                source.SetResult(true);
            }
            else if (millisecondsTimeout == -1)
            {
                WaitHandleCancellableTaskCompletionSourceManager.CreateWithoutTimeout(waitHandle, cancellationToken, source);
            }
            else
            {
                WaitHandleCancellableTaskCompletionSourceManager.CreateWithTimeout(waitHandle, cancellationToken, source, millisecondsTimeout);
            }

            return source.Task;
        }
    }
}