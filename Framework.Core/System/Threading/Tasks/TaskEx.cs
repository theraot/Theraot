#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable RCS1231 // Make parameter ref read-only.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Theraot;
using Theraot.Threading;

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
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FromCanceled(CancellationToken cancellationToken)
        {
            return FromCanceled<VoidStruct>(cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task FromCancellation(CancellationToken token)
        {
            return FromCancellation<VoidStruct>(token);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> FromCancellation<TResult>(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return FromCanceled<TResult>(token);
            }
            var taskCompleteSource = new TaskCompletionSource<TResult>();
            if (token.CanBeCanceled)
            {
#if LESSTHAN_NETSTANDARD13
                token.Register(() => taskCompleteSource.TrySetCanceled());
#else
                token.Register(() => taskCompleteSource.TrySetCanceled(token));
#endif
            }
            return taskCompleteSource.Task;
        }

        /// <summary>
        ///     Creates an already completed <see cref="System.Threading.Tasks.Task{TResult}" /> from the specified result.
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
#if NET40
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
#else
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
#endif
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The function to execute asynchronously.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            return Run<Task<TResult>>(function, cancellationToken).Unwrap();
        }

        /// <summary>
        ///     Creates a task that runs the specified function.
        /// </summary>
        /// <param name="function">The action to execute asynchronously.</param>
        /// <returns>
        ///     A task that represents the completion of the action.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
#if NET40
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
#else
            return Task.Factory.StartNew(function, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
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
        ///     Creates a Task that will complete when any of the tasks in the provided collection completes.
        /// </summary>
        /// <param name="tasks">The Tasks to be monitored.</param>
        /// <returns>
        ///     A Task that represents the completion of any of the provided Tasks.  The completed Task is this Task's result.
        /// </returns>
        /// <remarks>
        ///     Any Tasks that fault will need to have their exceptions observed elsewhere.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="tasks" /> argument is null.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="tasks" /> argument contains a null reference.</exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAny((IEnumerable<Task<TResult>>)tasks);
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

    public static partial class TaskEx
    {
        /// <summary>
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="millisecondsDelay">The delay in milliseconds before the returned task completes.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The <paramref name="millisecondsDelay" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Delay(int millisecondsDelay)
        {
            return Delay(millisecondsDelay, CancellationToken.None);
        }

        /// <summary>
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="millisecondsDelay">The delay in milliseconds before the returned task completes.</param>
        /// <param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The <paramref name="millisecondsDelay" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
#if NET40
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");
            }
            if (cancellationToken.IsCancellationRequested)
            {
                return FromCanceled(cancellationToken);
            }
            if (millisecondsDelay == 0)
            {
                return CompletedTask;
            }
            var source = new TaskCompletionSource<bool>();
            var timeout = RootedTimeout.Launch
            (
                () => source.TrySetResult(true),
                () => source.TrySetCanceled(),
                millisecondsDelay,
                cancellationToken
            );

            return source.Task;
#else
            // Missing in .NET 4.0
            return Task.Delay(millisecondsDelay, cancellationToken);
#endif
        }

        /// <summary>
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="millisecondsDelay">The delay before the returned task completes.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The <paramref name="millisecondsDelay" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Delay(TimeSpan millisecondsDelay)
        {
            return Delay(millisecondsDelay, CancellationToken.None);
        }

        /// <summary>
        ///     Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="millisecondsDelay">The delay before the returned task completes.</param>
        /// <param name="cancellationToken">A CancellationToken that may be used to cancel the task before the due time occurs.</param>
        /// <returns>
        ///     The timed Task.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The <paramref name="millisecondsDelay" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task Delay(TimeSpan millisecondsDelay, CancellationToken cancellationToken)
        {
#if NET40
            var timeoutMs = (long)millisecondsDelay.TotalMilliseconds;
            if (timeoutMs < Timeout.Infinite || timeoutMs > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay), _argumentOutOfRangeTimeoutNonNegativeOrMinusOne);
            }

            return Delay((int)timeoutMs, cancellationToken);
#else
            // Missing in .NET 4.0
            return Task.Delay(millisecondsDelay, cancellationToken);
#endif
        }
    }

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

        public static Task<bool> FromWaitHandle(WaitHandle waitHandle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException(nameof(waitHandle));
            }

            return FromWaitHandleInternal(waitHandle, millisecondsTimeout, cancellationToken);
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
    }
}