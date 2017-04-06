#if NET40

using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Provides extension methods for threading-related types.
    /// </summary>
    ///
    /// <summary>
    /// Asynchronous wrappers for .NET Framework operations.
    /// </summary>
    ///
    /// <summary>
    /// Provides extension methods for threading-related types.
    /// </summary>
    ///
    /// <remarks>
    /// AsyncCtpThreadingExtensions is a placeholder.
    /// </remarks>
    public static class AsyncCompatLibExtensions
    {
        /// <summary>
        /// Creates and configures an awaitable object for awaiting the specified task.
        /// </summary>
        /// <param name="task">The task to be awaited.</param>
        /// <param name="continueOnCapturedContext">true to automatic marshal back to the original call site's current SynchronizationContext
        ///             or TaskScheduler; otherwise, false.</param>
        /// <returns>
        /// The instance to be awaited.
        /// </returns>
        public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(this Task<TResult> task,
            bool continueOnCapturedContext)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return new ConfiguredTaskAwaitable<TResult>(task, continueOnCapturedContext);
        }

        /// <summary>
        /// Creates and configures an awaitable object for awaiting the specified task.
        /// </summary>
        /// <param name="task">The task to be awaited.</param>
        /// <param name="continueOnCapturedContext">true to automatic marshal back to the original call site's current SynchronizationContext
        ///             or TaskScheduler; otherwise, false.</param>
        /// <returns>
        /// The instance to be awaited.
        /// </returns>
        public static ConfiguredTaskAwaitable ConfigureAwait(this Task task, bool continueOnCapturedContext)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return new ConfiguredTaskAwaitable(task, continueOnCapturedContext);
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="T:System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="task">The task to await.</param>
        /// <returns>
        /// An awaiter instance.
        /// </returns>
        public static TaskAwaiter GetAwaiter(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return new TaskAwaiter(task);
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="T:System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of data returned by the task.</typeparam>
        /// <param name="task">The task to await.</param>
        /// <returns>
        /// An awaiter instance.
        /// </returns>
        public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Task<TResult> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return new TaskAwaiter<TResult>(task);
        }
    }
}

#endif