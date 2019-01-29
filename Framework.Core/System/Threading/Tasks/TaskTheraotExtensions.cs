#if NET40
using System.Runtime.CompilerServices;

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
        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                cancellationToken,
                continuationOptions,
                scheduler
            );
        }

        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                scheduler
            );
        }

        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                cancellationToken
            );
        }

        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                continuationOptions
            );
        }

        public static Task ContinueWith(this Task task, Action<Task, object> continuationAction, object state)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state)
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                cancellationToken,
                continuationOptions,
                scheduler
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                scheduler
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                cancellationToken
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                continuationOptions
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, object, TResult> continuationFunction, object state)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state)
            );
        }

        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                cancellationToken,
                continuationOptions,
                scheduler
            );
        }

        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                scheduler
            );
        }

        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                cancellationToken
            );
        }

        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state),
                continuationOptions
            );
        }

        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>, object> continuationAction, object state)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return task.ContinueWith
            (
                t => continuationAction(t, state)
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                cancellationToken,
                continuationOptions,
                scheduler
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                scheduler
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                cancellationToken
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state),
                continuationOptions
            );
        }

        public static Task<TResult> ContinueWith<TResult>(this Task<TResult> task, Func<Task<TResult>, object, TResult> continuationFunction, object state)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return task.ContinueWith
            (
                t => continuationFunction(t, state)
            );
        }
    }
}

#endif