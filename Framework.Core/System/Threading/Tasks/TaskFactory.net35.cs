#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    public class TaskFactory
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
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAll<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAll<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return Task.WhenAll(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAny(Task[] tasks, Action<Task[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(Task<TAntecedentResult>[] tasks, Func<Task<TAntecedentResult>[], TResult> continuationFunction)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken, continuationOptions, scheduler);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), continuationOptions);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction, CancellationToken cancellationToken)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks), cancellationToken);
        }

        public Task ContinueWhenAny<TAntecedentResult>(Task<TAntecedentResult>[] tasks, Action<Task<TAntecedentResult>[]> continuationAction)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationAction(tasks));
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks));
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), cancellationToken);
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        {
            return Task.WhenAny(tasks).ContinueWith(_ => continuationFunction(tasks), continuationOptions);
        }

        public Task<TResult> ContinueWhenAny<TResult>(Task[] tasks, Func<Task[], TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        {
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
}

#endif