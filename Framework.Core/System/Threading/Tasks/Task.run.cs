#if LESSTHAN_NET40

namespace System.Threading.Tasks
{
    public partial class Task
    {
        public static Task Run(Action action)
        {
            var result = new Task(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach);
            result.Start();
            return result;
        }

        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            var result = new Task(action, cancellationToken, TaskCreationOptions.DenyChildAttach);
            if (!cancellationToken.IsCancellationRequested)
            {
                result.Start(result.ExecutingTaskScheduler, inline: false);
            }

            return result;
        }

        public static Task Run(Func<Task> action)
        {
            return Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
        }

        public static Task Run(Func<Task> action, CancellationToken cancellationToken)
        {
            return Factory.StartNew(action, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
        }

        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var result = new Task<TResult>(function, CancellationToken.None, TaskCreationOptions.DenyChildAttach);
            result.Start();
            return result;
        }

        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            var result = new Task<TResult>(function, cancellationToken, TaskCreationOptions.DenyChildAttach);
            if (!cancellationToken.IsCancellationRequested)
            {
                result.Start(result.ExecutingTaskScheduler, inline: false);
            }

            return result;
        }

        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            var source = new TaskCompletionSource<TResult>();
            var result = source.Task;
            ThreadPool.QueueUserWorkItem
            (
                _ => function().ContinueWith
                (
                    task => source.SetResult(task.InternalResult),
                    TaskScheduler.Current
                )
            );
            return result;
        }

        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            var source = new TaskCompletionSource<TResult>();
            var result = source.Task;
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                    function().ContinueWith
                    (
                        task => source.SetResult(task.InternalResult),
                        cancellationToken,
                        TaskContinuationOptions.None,
                        TaskScheduler.Current
                    )
            );
            return result;
        }
    }
}

#endif