#if FAT
#if NET20 || NET30 || NET35

using Theraot.Core;

namespace System.Threading.Tasks
{
    public class Task<TResult> : Task
    {
        private readonly IErsatz<TResult> _erzatz;

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
                    throw new AggregateException((Exception)new TaskCanceledException(this));
                }
                return _erzatz.Result;
            }
        }

        private Task(IErsatz<TResult> erzatz)
            : base(erzatz.InvokeAction(), null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
        {
            _erzatz = erzatz;
        }

        private Task(IErsatz<TResult> erzatz, CancellationToken cancellationToken)
            : base(erzatz.InvokeAction(), null, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default)
        {
            _erzatz = erzatz;
        }

        private Task(IErsatz<TResult> erzatz, TaskCreationOptions creationOptions)
            : base(erzatz.InvokeAction(), null, CancellationToken.None, creationOptions, TaskScheduler.Default)
        {
            _erzatz = erzatz;
        }

        private Task(IErsatz<TResult> erzatz, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(erzatz.InvokeAction(), null, cancellationToken, creationOptions, TaskScheduler.Default)
        {
            _erzatz = erzatz;
        }

        private Task(IErsatz<TResult> erzatz, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : base(erzatz.InvokeAction(), state, cancellationToken, creationOptions, scheduler)
        {
            _erzatz = erzatz;
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

        internal Task(Func<TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : this(new ErsatzFunc<TResult>(function), state, cancellationToken, creationOptions, scheduler)
        {
            // Empty
        }

        internal Task(Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : this(new ErsatzFunc<object, TResult>(function, state), state, cancellationToken, creationOptions, scheduler)
        {
            // Empty
        }
    }
}

#endif
#endif