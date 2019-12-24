#if NET40

#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable S112 // General exceptions should never be thrown

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
        public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task, bool continueOnCapturedContext)
        {
            if (task == null)
            {
                throw new NullReferenceException();
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
                throw new NullReferenceException();
            }

            return new ConfiguredTaskAwaitable(task, continueOnCapturedContext);
        }

        /// <summary>
        ///     Gets an awaiter used to await this <see cref="Task" />.
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
                throw new NullReferenceException();
            }

            return new TaskAwaiter(task);
        }

        /// <summary>
        ///     Gets an awaiter used to await this <see cref="Task" />.
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
                throw new NullReferenceException();
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
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationAction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, scheduler, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state)
        {
            if (task == null)
            {
                throw new NullReferenceException();
            }

            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            var continuationOptions = TaskContinuationOptions.None;
            var cancellationToken = CancellationToken.None;
            return ContinueWithExtracted(task, continuationFunction, state, ref continuationOptions, TaskScheduler.Current, cancellationToken);
        }

        private static Task ContinueWithExtracted(Task task, Action<Task, object> continuationAction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
        {
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

        private static Task ContinueWithExtracted<TResult>(Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
        {
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

        private static Task<TResult> ContinueWithExtracted<TResult>(Task task, Func<Task, object, TResult> continuationFunction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
        {
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

        private static Task<TResult> ContinueWithExtracted<TResult>(Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, ref TaskContinuationOptions continuationOptions, TaskScheduler scheduler, CancellationToken cancellationToken)
        {
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