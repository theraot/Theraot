#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    public class TaskFactory
    {
        internal static TaskFactory _defaultInstance = new TaskFactory();

        public Task StartNew(Action action)
        {
            var result = new Task(action, null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task StartNew(Action action, CancellationToken cancellationToken)
        {
            var result = new Task(action, null, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task StartNew(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            var result = new Task(action, null, cancellationToken, creationOptions, scheduler);
            result.Start();
            return result;
        }

        public Task StartNew(Action<object> action, object state)
        {
            var result = new Task(action, state, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task StartNew(Action<object> action, object state, CancellationToken cancellationToken)
        {
            var result = new Task(action, state, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task StartNew(Action<object> action, object state, TaskCreationOptions creationOptions)
        {
            var result = new Task(action, state, CancellationToken.None, creationOptions, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task StartNew(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            var result = new Task(action, state, cancellationToken, creationOptions, scheduler);
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function)
        {
            var result = new Task<TResult>(function, null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            var result = new Task<TResult>(function, null, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            var result = new Task<TResult>(function, null, cancellationToken, creationOptions, scheduler);
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state)
        {
            var result = new Task<TResult>(function, state, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state, CancellationToken cancellationToken)
        {
            var result = new Task<TResult>(function, state, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state, TaskCreationOptions creationOptions)
        {
            var result = new Task<TResult>(function, state, CancellationToken.None, creationOptions, TaskScheduler.Default);
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
        {
            var result = new Task<TResult>(function, state, cancellationToken, creationOptions, scheduler);
            result.Start();
            return result;
        }
    }
}

#endif