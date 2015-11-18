#if NET20 || NET30 || NET35

using Theraot.Core;

namespace System.Threading.Tasks
{
    public class Task<TResult> : Task
    {
        private readonly IErsatz<TResult> _ersatz;

        public TResult Result
        {
            [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Microsoft's Design")]
            get
            {
                Wait();
                if (IsFaulted)
                {
                    throw Exception;
                }
                if (IsCanceled)
                {
                    throw new AggregateException(new TaskCanceledException(this));
                }
                return _ersatz.Result;
            }
        }

        private Task(IErsatz<TResult> ersatz)
            : base(ersatz.InvokeAction(), null, CancellationToken.None, TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            _ersatz = ersatz;
        }

        private Task(IErsatz<TResult> ersatz, CancellationToken cancellationToken)
            : base(ersatz.InvokeAction(), null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            _ersatz = ersatz;
        }

        private Task(IErsatz<TResult> ersatz, TaskCreationOptions creationOptions)
            : base(ersatz.InvokeAction(), null, CancellationToken.None, creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            _ersatz = ersatz;
        }

        private Task(IErsatz<TResult> ersatz, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(ersatz.InvokeAction(), null, cancellationToken, creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            _ersatz = ersatz;
        }

        private Task(IErsatz<TResult> ersatz, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : base(ersatz.InvokeAction(), null, cancellationToken, creationOptions, InternalTaskOptions.None, scheduler)
        {
            _ersatz = ersatz;
        }

        public Task(Func<TResult> function)
            : this(new ErsatzFunc<TResult>(function))
        {
            // Empty
        }

        public Task(Func<TResult> function, CancellationToken cancellationToken)
            : this(new ErsatzFunc<TResult>(function), cancellationToken)
        {
            // Empty
        }

        public Task(Func<TResult> function, TaskCreationOptions creationOptions)
            : this(new ErsatzFunc<TResult>(function), creationOptions)
        {
            // Empty
        }

        public Task(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : this(new ErsatzFunc<TResult>(function), cancellationToken, creationOptions)
        {
            // Empty
        }

        internal Task(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : this(new ErsatzFunc<TResult>(function), cancellationToken, creationOptions, scheduler)
        {
            // Empty
        }

        internal Task(Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : this(new ErsatzFunc<object, TResult>(function, state), cancellationToken, creationOptions, scheduler)
        {
            // Empty
        }
    }
}

#endif