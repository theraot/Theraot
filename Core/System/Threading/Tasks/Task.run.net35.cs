#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    public partial class Task : IDisposable, IAsyncResult
    {
        public static Task Run(Action action)
        {
            var result = new Task(action);
            result.Start();
            return result;
        }

        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            var result = new Task(action, cancellationToken);
            result.Start();
            return result;
        }

        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var result = new Task<TResult>(function);
            result.Start();
            return result;
        }

        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
            var result = new Task<TResult>(function, cancellationToken);
            result.Start();
            return result;
        }
    }
}

#endif