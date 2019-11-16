#if LESSTHAN_NET40

#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable CC0091 // Use static method

using Theraot;

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

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), TaskScheduler.Current);
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
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

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current);
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

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
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

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), TaskScheduler.Current);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
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

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), TaskScheduler.Current);
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

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction)
        {
            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, TaskContinuationOptions.None, TaskScheduler.Current);
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            if (continuationFunction == null)
            {
                throw new ArgumentNullException(nameof(continuationFunction));
            }

            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), CancellationToken.None, continuationOptions, TaskScheduler.Current);
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

        public Task StartNew(Action<object?> action, object? state)
        {
            var result = new Task(action, state, null, CancellationToken.None, TaskCreationOptions.None, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action<object?> action, object? state, CancellationToken cancellationToken)
        {
            var result = new Task(action, state, null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action<object?> action, object? state, TaskCreationOptions creationOptions)
        {
            var result = new Task(action, state, Task.InternalCurrentIfAttached(creationOptions), CancellationToken.None, creationOptions, InternalTaskOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task StartNew(Action<object?> action, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
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

        public Task<TResult> StartNew<TResult>(Func<object?, TResult> function, object? state)
        {
            var result = new Task<TResult>(function, state, CancellationToken.None, TaskCreationOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object?, TResult> function, object? state, CancellationToken cancellationToken)
        {
            var result = new Task<TResult>(function, state, cancellationToken, TaskCreationOptions.None, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object?, TResult> function, object? state, TaskCreationOptions creationOptions)
        {
            var result = new Task<TResult>(function, state, CancellationToken.None, creationOptions, _scheduler);
            result.InternalStart(_scheduler, false, true);
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object?, TResult> function, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            // Should not be static
            var result = new Task<TResult>(function, state, cancellationToken, creationOptions, scheduler);
            result.InternalStart(scheduler, false, true);
            return result;
        }
    }

    public partial class TaskFactory
    {
        public Task<TResult> FromAsync<TResult>(IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                asyncResult,
                endMethod,
                TaskCreationOptions.None,
                _scheduler
            );
        }

        public Task<TResult> FromAsync<TResult>(IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod, TaskCreationOptions creationOptions)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                asyncResult,
                endMethod,
                creationOptions,
                _scheduler
            );
        }

        public Task<TResult> FromAsync<TResult>(IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod, TaskCreationOptions creationOptions, TaskScheduler scheduler)
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

            return FromAsyncCore
            (
                asyncResult,
                endMethod,
                creationOptions,
                scheduler
            );
        }

        public Task FromAsync<TArg1>(Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                (callback, obj) => beginMethod(arg1, callback, obj),
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                TaskCreationOptions.None
            );
        }

        public Task FromAsync<TArg1>(Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                (callback, obj) => beginMethod(arg1, callback, obj),
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                creationOptions
            );
        }

        public Task<TResult> FromAsync<TArg1, TResult>(Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore((callback, obj) => beginMethod(arg1, callback, obj), endMethod, state, default);
        }

        public Task<TResult> FromAsync<TArg1, TResult>(Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore((callback, obj) => beginMethod(arg1, callback, obj), endMethod, state, creationOptions);
        }

        public Task FromAsync<TArg1, TArg2>(Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                (callback, obj) => beginMethod(arg1, arg2, callback, obj),
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                TaskCreationOptions.None
            );
        }

        public Task FromAsync<TArg1, TArg2>(Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                (callback, obj) => beginMethod(arg1, arg2, callback, obj),
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                creationOptions
            );
        }

        public Task<TResult> FromAsync<TArg1, TArg2, TResult>(Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore((callback, obj) => beginMethod(arg1, arg2, callback, obj), endMethod, state, default);
        }

        public Task<TResult> FromAsync<TArg1, TArg2, TResult>(Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore((callback, obj) => beginMethod(arg1, arg2, callback, obj), endMethod, state, creationOptions);
        }

        public Task FromAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                (callback, obj) => beginMethod(arg1, arg2, arg3, callback, obj),
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                TaskCreationOptions.None
            );
        }

        public Task FromAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                (callback, obj) => beginMethod(arg1, arg2, arg3, callback, obj),
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                creationOptions
            );
        }

        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore((callback, obj) => beginMethod(arg1, arg2, arg3, callback, obj), endMethod, state, default);
        }

        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore((callback, obj) => beginMethod(arg1, arg2, arg3, callback, obj), endMethod, state, creationOptions);
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

            return FromAsyncCore
                (
                    asyncResult,
                    result =>
                    {
                        endMethod(result);
                        return default(VoidStruct);
                    },
                    TaskCreationOptions.None,
                    _scheduler
                );
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

            return FromAsyncCore
            (
                asyncResult,
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                creationOptions,
                _scheduler
            );
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

            return FromAsyncCore
            (
                asyncResult,
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                creationOptions,
                scheduler
            );
        }

        public Task FromAsync(Func<AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                beginMethod,
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                TaskCreationOptions.None
            );
        }

        public Task FromAsync(Func<AsyncCallback, object?, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore
            (
                beginMethod,
                result =>
                {
                    endMethod(result);
                    return default(VoidStruct);
                },
                state,
                creationOptions
            );
        }

        public Task<TResult> FromAsync<TResult>(Func<AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object? state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore(beginMethod, endMethod, state, default);
        }

        public Task<TResult> FromAsync<TResult>(Func<AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object? state, TaskCreationOptions creationOptions)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException(nameof(beginMethod));
            }

            if (endMethod == null)
            {
                throw new ArgumentNullException(nameof(endMethod));
            }

            return FromAsyncCore(beginMethod, endMethod, state, creationOptions);
        }
    }

    public partial class TaskFactory
    {
        private static async Task<TResult> FromAsyncCore<TResult>(IAsyncResult asyncResult, Func<IAsyncResult, TResult> endMethod, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            var task = new Task<TResult>(() => endMethod(asyncResult), Task.InternalCurrentIfAttached(creationOptions), CancellationToken.None, creationOptions, InternalTaskOptions.None, scheduler);
            if (asyncResult.IsCompleted)
            {
                task.RunSynchronously(scheduler);
            }
            else
            {
                await TaskExEx.FromWaitHandleInternal(asyncResult.AsyncWaitHandle).ConfigureAwait(false);
                task.InternalStart(scheduler, false, true);
            }

            var result = await task.ConfigureAwait(false);
            task.Dispose();
            return result;
        }

        private static async Task<TResult> FromAsyncCore<TResult>(Func<AsyncCallback, object?, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, object? state, TaskCreationOptions creationOptions)
        {
            return endMethod(await FromBeginMethod(beginMethod, state, creationOptions).ConfigureAwait(false));
        }

        private static Task<IAsyncResult> FromBeginMethod(Func<AsyncCallback, object?, IAsyncResult> beginMethod, object? state, TaskCreationOptions creationOptions)
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