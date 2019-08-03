#if LESSTHAN_NET40

#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable CC0091 // Use static method

namespace System.Threading.Tasks
{
    public partial class TaskFactory
    {
        internal static readonly TaskFactory DefaultInstance = new TaskFactory();

        private readonly TaskScheduler _scheduler;

        public TaskFactory()
        {
            _scheduler = TaskScheduler.Default;
        }

        public TaskFactory(TaskScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task StartNew(Action action)
        {
            var result = new Task(action, null, CancellationToken.None, TaskCreationOptions.None, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action action, CancellationToken cancellationToken)
        {
            var result = new Task(action, null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action action, TaskCreationOptions creationOptions)
        {
            var result = new Task(action, Task.InternalCurrentIfAttached(creationOptions), CancellationToken.None, creationOptions, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            // Should not be static
            var result = new Task(action, Task.InternalCurrentIfAttached(creationOptions), cancellationToken, creationOptions, InternalTaskOptions.None, scheduler);
            result.InternalStart(scheduler, false, true);
            return result;
        }

        public Task StartNew(Action<object> action, object state)
        {
            var result = new Task(action, state, null, CancellationToken.None, TaskCreationOptions.None, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action<object> action, object state, CancellationToken cancellationToken)
        {
            var result = new Task(action, state, null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action<object> action, object state, TaskCreationOptions creationOptions)
        {
            var result = new Task(action, state, Task.InternalCurrentIfAttached(creationOptions), CancellationToken.None, creationOptions, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            // Should not be static
            var result = new Task(action, state, Task.InternalCurrentIfAttached(creationOptions), cancellationToken, creationOptions, InternalTaskOptions.None, scheduler);
            result.InternalStart(scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function)
        {
            var result = new Task<TResult>(function, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            var result = new Task<TResult>(function, cancellationToken, TaskCreationOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function, TaskCreationOptions creationOptions)
        {
            var result = new Task<TResult>(function, CancellationToken.None, creationOptions, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            // Should not be static
            var result = new Task<TResult>(function, cancellationToken, creationOptions, scheduler);
            result.InternalStart(scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state)
        {
            var result = new Task<TResult>(function, state, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state, CancellationToken cancellationToken)
        {
            var result = new Task<TResult>(function, state, cancellationToken, TaskCreationOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state, TaskCreationOptions creationOptions)
        {
            var result = new Task<TResult>(function, state, CancellationToken.None, creationOptions, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            // Should not be static
            var result = new Task<TResult>(function, state, cancellationToken, creationOptions, scheduler);
            result.InternalStart(scheduler, false, true);
            return result;
        }
    }

    public partial class TaskFactory
    {
        public Task FromAsync(IAsyncResult asyncResult, Action<IAsyncResult> endMethod)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncInternal(asyncResult, endMethod, TaskCreationOptions.None, _scheduler);
        }

        public Task FromAsync(Func<AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncInternal(beginMethod, endMethod, state);
        }

        public Task FromAsync(IAsyncResult asyncResult, Action<IAsyncResult> endMethod, TaskCreationOptions creationOptions)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncInternal(asyncResult, endMethod, creationOptions, _scheduler);
        }

        public Task FromAsync(Func<AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncInternal(beginMethod, endMethod, state, creationOptions);
        }

        public Task FromAsync(IAsyncResult asyncResult, Action<IAsyncResult> endMethod, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return FromAsyncInternal(asyncResult, endMethod, creationOptions, scheduler);
        }

        public Task<TResult> FromAsync<TResult>(Func<AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncInternal(beginMethod, endMethod, state, creationOptions);
        }

        public Task<TResult> FromAsync<TResult>(Func<AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncInternal(beginMethod, endMethod, state, default);
        }
    }

#pragma warning disable CC0031 // Check for null before calling a delegate

    public partial class TaskFactory
    {
        internal static async Task FromAsyncInternal(Func<AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object state)
        {
            endMethod(await FromBeginMethod(beginMethod, state).ConfigureAwait(false));
        }

        internal static async Task FromAsyncInternal(Func<AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object state, TaskCreationOptions creationOptions)
        {
            endMethod(await FromBeginMethod(beginMethod, state, creationOptions).ConfigureAwait(false));
        }

        internal static async Task<TResult> FromAsyncInternal<TResult>(Func<AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object state, TaskCreationOptions creationOptions)
        {
            return endMethod(await FromBeginMethod(beginMethod, state, creationOptions).ConfigureAwait(false));
        }

        internal static async Task FromAsyncInternal(IAsyncResult asyncResult, Action<IAsyncResult> endMethod, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            var task = new Task(() => endMethod(asyncResult), Task.InternalCurrentIfAttached(creationOptions), CancellationToken.None, creationOptions, InternalTaskOptions.None, scheduler);
            if (asyncResult.IsCompleted)
            {
                task.RunSynchronously(scheduler);
            }
            else
            {
                await TaskEx.FromWaitHandleInternal(asyncResult.AsyncWaitHandle).ConfigureAwait(false);
                task.InternalStart(scheduler, false, true);
            }

            await task;
            task.Dispose();
        }

        private static Task<IAsyncResult> FromBeginMethod(Func<AsyncCallback, object, IAsyncResult> beginMethod, object state)
        {
            var source = new TaskCompletionSource<IAsyncResult>();
            var canInvokeEnd = new[] { 0 };
            var asyncResult = beginMethod(AsyncCallback, state);
            if (asyncResult?.CompletedSynchronously == true)
            {
                AsyncCallback(asyncResult);
            }

            return source.Task;

            void AsyncCallback(IAsyncResult r)
            {
                if (Interlocked.CompareExchange(ref canInvokeEnd[0], 1, 0) != 0)
                {
                    return;
                }

                source.TrySetResult(r);
            }
        }

        private static Task<IAsyncResult> FromBeginMethod(Func<AsyncCallback, object, IAsyncResult> beginMethod, object state, TaskCreationOptions creationOptions)
        {
            var source = new TaskCompletionSource<IAsyncResult>(creationOptions);
            var canInvokeEnd = new[] { 0 };
            var asyncResult = beginMethod(AsyncCallback, state);
            if (asyncResult?.CompletedSynchronously == true)
            {
                AsyncCallback(asyncResult);
            }

            return source.Task;

            void AsyncCallback(IAsyncResult r)
            {
                if (Interlocked.CompareExchange(ref canInvokeEnd[0], 1, 0) != 0)
                {
                    return;
                }

                source.TrySetResult(r);
            }
        }
    }
}

#endif