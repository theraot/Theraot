#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable MA0040 // Flow the cancellation token

namespace System.Threading.Tasks
{
#if TARGETS_NETSTANDARD

    public static partial class TaskExEx
    {
        private sealed class WaitHandleCancellableTaskCompletionSourceManager
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
                // ReSharper disable once MethodSupportsCancellation
                _ = Task.Run(result.CallbackWithoutTimeout);
            }

            public static void CreateWithTimeout(WaitHandle waitHandle, CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleCancellableTaskCompletionSourceManager(waitHandle, cancellationToken, taskCompletionSource);
                // ReSharper disable once MethodSupportsCancellation
                _ = Task.Run(() => result.CallbackWithTimeout(millisecondsTimeout));
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
    }

#else

    public static partial class TaskExEx
    {
        private sealed class WaitHandleCancellableTaskCompletionSourceManager
        {
            private readonly CancellationToken _cancellationToken;

            private readonly RegisteredWaitHandle?[] _registeredWaitHandle;

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
                result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithoutTimeout, state: null, -1, executeOnlyOnce: true);
                cancellationToken.Register(() => result.Unregister());
            }

            public static void CreateWithTimeout(WaitHandle waitHandle, CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleCancellableTaskCompletionSourceManager(cancellationToken, taskCompletionSource);
                result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithTimeout, state: null, millisecondsTimeout, executeOnlyOnce: true);
                cancellationToken.Register(() => result.Unregister());
            }

            private void CallbackWithoutTimeout(object? state, bool timeOut)
            {
                if (Unregister())
                {
                    _taskCompletionSource.TrySetCanceled();
                    return;
                }

                _taskCompletionSource.TrySetResult(true);
            }

            private void CallbackWithTimeout(object? state, bool timeOut)
            {
                if (Unregister())
                {
                    return;
                }

                if (timeOut)
                {
                    _taskCompletionSource.TrySetResult(false);
                    return;
                }

                _taskCompletionSource.TrySetResult(true);
            }

            private bool Unregister()
            {
                Volatile.Read(ref _registeredWaitHandle[0])!.Unregister(waitObject: null);
                if (!_cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                _taskCompletionSource.TrySetCanceled();
                return true;
            }
        }
    }

#endif
}