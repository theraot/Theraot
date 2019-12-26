using System.Runtime.CompilerServices;
using Theraot;

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Provides methods for creating and manipulating tasks.
    /// </summary>
    /// <remarks>
    ///     TaskEx is a placeholder.
    /// </remarks>
    public static partial class TaskExEx
    {
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
    }

    public static partial class TaskExEx
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