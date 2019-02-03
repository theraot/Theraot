#if NET40

#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.

using System.Runtime.CompilerServices;
using Theraot;

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Provides extension methods for threading-related types.
    /// </summary>
    /// <summary>
    ///     Asynchronous wrappers for .NET Framework operations.
    /// </summary>
    /// <summary>
    ///     Provides extension methods for threading-related types.
    /// </summary>
    public static partial class TaskTheraotExtensions
    {
        /// <summary>
        ///     Creates and configures an awaitable object for awaiting the specified task.
        /// </summary>
        /// <param name="task">The task to be awaited.</param>
        /// <param name="continueOnCapturedContext">
        ///     true to automatic marshal back to the original call site's current SynchronizationContext
        ///     or TaskScheduler; otherwise, false.
        /// </param>
        /// <returns>
        ///     The instance to be awaited.
        /// </returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task,
            bool continueOnCapturedContext)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new ConfiguredTaskAwaitable<TResult>(task, continueOnCapturedContext);
        }

        /// <summary>
        ///     Creates and configures an awaitable object for awaiting the specified task.
        /// </summary>
        /// <param name="task">The task to be awaited.</param>
        /// <param name="continueOnCapturedContext">
        ///     true to automatic marshal back to the original call site's current SynchronizationContext
        ///     or TaskScheduler; otherwise, false.
        /// </param>
        /// <returns>
        ///     The instance to be awaited.
        /// </returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConfiguredTaskAwaitable ConfigureAwait(this Task task, bool continueOnCapturedContext)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new ConfiguredTaskAwaitable(task, continueOnCapturedContext);
        }

        /// <summary>
        ///     Gets an awaiter used to await this <see cref="T:System.Threading.Tasks.Task" />.
        /// </summary>
        /// <param name="task">The task to await.</param>
        /// <returns>
        ///     An awaiter instance.
        /// </returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static TaskAwaiter GetAwaiter(this Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new TaskAwaiter(task);
        }

        /// <summary>
        ///     Gets an awaiter used to await this <see cref="T:System.Threading.Tasks.Task" />.
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of data returned by the task.</typeparam>
        /// <param name="task">The task to await.</param>
        /// <returns>
        ///     An awaiter instance.
        /// </returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Task<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return new TaskAwaiter<TResult>(task);
        }
    }

    public static partial class TaskTheraotExtensions
    {
        private const TaskContinuationOptions _conditionMask = TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.OnlyOnRanToCompletion;

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var source = new TaskCompletionSource<VoidStruct>(state);
            var condition = RemoveConditions(ref continuationOptions);
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() => source.TrySetCanceled());
                if (cancellationToken.IsCancellationRequested)
                {
                    return source.Task;
                }
            }

            task.ContinueWith
            (
                t =>
                {
                    if (ValidateConditions(t, condition))
                    {
                        try
                        {
                            continuationAction(t, state);
                            source.TrySetResult(default);
                        }
                        catch (Exception exception)
                        {
                            source.TrySetException(exception);
                        }
                    }
                    else
                    {
                        source.TrySetCanceled();
                    }
                },
                cancellationToken,
                continuationOptions,
                scheduler
            );
            return source.Task;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith(task, continuationAction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var source = new TaskCompletionSource<TResult>(state);
            var condition = RemoveConditions(ref continuationOptions);
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() => source.TrySetCanceled());
                if (cancellationToken.IsCancellationRequested)
                {
                    return source.Task;
                }
            }

            task.ContinueWith
            (
                t =>
                {
                    if (ValidateConditions(t, condition))
                    {
                        try
                        {
                            source.TrySetResult(continuationFunction(t, state));
                        }
                        catch (Exception exception)
                        {
                            source.TrySetException(exception);
                        }
                    }
                    else
                    {
                        source.TrySetCanceled();
                    }
                },
                cancellationToken,
                continuationOptions,
                scheduler
            );
            return source.Task;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            return ContinueWith(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith(task, continuationFunction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(task, continuationFunction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state)
        {
            return ContinueWith(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var source = new TaskCompletionSource<VoidStruct>(state);
            var condition = RemoveConditions(ref continuationOptions);
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() => source.TrySetCanceled());
                if (cancellationToken.IsCancellationRequested)
                {
                    return source.Task;
                }
            }

            task.ContinueWith
            (
                t =>
                {
                    if (ValidateConditions(t, condition))
                    {
                        try
                        {
                            continuationAction(t, state);
                            source.TrySetResult(default);
                        }
                        catch (Exception exception)
                        {
                            source.TrySetException(exception);
                        }
                    }
                    else
                    {
                        source.TrySetCanceled();
                    }
                },
                cancellationToken,
                continuationOptions,
                scheduler
            );
            return source.Task;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith(task, continuationAction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state)
        {
            return ContinueWith(task, continuationAction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var source = new TaskCompletionSource<TResult>(state);
            var condition = RemoveConditions(ref continuationOptions);
            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() => source.TrySetCanceled());
                if (cancellationToken.IsCancellationRequested)
                {
                    return source.Task;
                }
            }

            task.ContinueWith
            (
                t =>
                {
                    if (ValidateConditions(t, condition))
                    {
                        try
                        {
                            source.TrySetResult(continuationFunction(t, state));
                        }
                        catch (Exception exception)
                        {
                            source.TrySetException(exception);
                        }
                    }
                    else
                    {
                        source.TrySetCanceled();
                    }
                },
                cancellationToken,
                continuationOptions,
                scheduler
            );
            return source.Task;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            return ContinueWith(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            return ContinueWith(task, continuationFunction, state, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            return ContinueWith(task, continuationFunction, state, CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state)
        {
            return ContinueWith(task, continuationFunction, state, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private static TaskContinuationOptions RemoveConditions(ref TaskContinuationOptions continuationOptions)
        {
            var result = continuationOptions & _conditionMask;
            continuationOptions &= ~_conditionMask;
            return result;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private static bool ValidateConditions(Task task, TaskContinuationOptions condition)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    return (condition & TaskContinuationOptions.NotOnRanToCompletion) == TaskContinuationOptions.None;
                case TaskStatus.Canceled:
                    return (condition & TaskContinuationOptions.NotOnCanceled) == TaskContinuationOptions.None;
                case TaskStatus.Faulted:
                    return (condition & TaskContinuationOptions.NotOnFaulted) == TaskContinuationOptions.None;
                default:
                    return false;
            }
        }
    }
}

#endif