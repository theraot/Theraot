#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable RCS1231 // Make parameter ref read-only.

#if NET40

using System.Linq;

#endif

namespace System.Threading.Tasks
{
#if TARGETS_NETSTANDARD

    public static partial class TaskEx
    {
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
                // ReSharper disable once MethodSupportsCancellation
                Task.Run(result.CallbackWithoutTimeout);
            }

            public static void CreateWithTimeout(WaitHandle waitHandle, TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleTaskCompletionSourceManager(waitHandle, taskCompletionSource);
                // ReSharper disable once MethodSupportsCancellation
                Task.Run(() => result.CallbackWithTimeout(millisecondsTimeout));
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
        private class WaitHandleTaskCompletionSourceManager
        {
            private readonly RegisteredWaitHandle?[] _registeredWaitHandle;

            private readonly TaskCompletionSource<bool> _taskCompletionSource;

            private WaitHandleTaskCompletionSourceManager(TaskCompletionSource<bool> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
                _registeredWaitHandle = new RegisteredWaitHandle[1];
            }

            public static void CreateWithoutTimeout(WaitHandle waitHandle,
                TaskCompletionSource<bool> taskCompletionSource)
            {
                var result = new WaitHandleTaskCompletionSourceManager(taskCompletionSource);
                result._registeredWaitHandle[0] =
                    ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithoutTimeout, null, -1, true);
            }

            public static void CreateWithTimeout(WaitHandle waitHandle,
                TaskCompletionSource<bool> taskCompletionSource, int millisecondsTimeout)
            {
                var result = new WaitHandleTaskCompletionSourceManager(taskCompletionSource);
                result._registeredWaitHandle[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, result.CallbackWithTimeout, null, millisecondsTimeout, true);
            }

            private void CallbackWithoutTimeout(object? state, bool timeOut)
            {
                Unregister();
                _taskCompletionSource.TrySetResult(true);
            }

            private void CallbackWithTimeout(object? state, bool timeOut)
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
                Volatile.Read(ref _registeredWaitHandle[0])!.Unregister(null);
            }
        }
    }

#endif
}