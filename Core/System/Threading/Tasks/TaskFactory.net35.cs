#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    public class TaskFactory
    {
        internal static TaskFactory _defaultInstance = new TaskFactory();

        public Task StartNew(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            var result = new Task(action, null, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default); ;
            result.Start();
            return result;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("action");
            }
            var result = new Task<TResult>(function, null, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
            result.Start();
            return result;
        }
    }
}

#endif