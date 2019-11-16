#if LESSTHAN_NET40

#pragma warning disable CA1068 // CancellationToken parameters must come last

using System.Diagnostics.Contracts;

namespace System.Threading.Tasks
{
    public partial class Task<TResult> : Task
    {
        internal TResult InternalResult = default!;

        static Task()
        {
            ContinuationConversion = done => (Task<TResult>)done.Result;
        }

        public Task(Func<TResult> function)
            : base(function, null, null, default, TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, CancellationToken cancellationToken)
            : base(function, null, null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, TaskCreationOptions creationOptions)
            : base(function, null, null, default, creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
            : base(function, null, null, cancellationToken, creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        internal Task(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : base(function, null, null, cancellationToken, creationOptions, InternalTaskOptions.None, scheduler)
        {
            // Empty
        }

        internal Task(Func<TResult> function, Task? parent, CancellationToken cancellationToken, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions, TaskScheduler scheduler)
            : this(function, null, parent, cancellationToken, creationOptions, internalOptions, scheduler)
        {
            CapturedContext = ExecutionContext.Capture();
        }

        internal Task(Func<object?, TResult> function, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : base(function, state, null, cancellationToken, creationOptions, InternalTaskOptions.None, scheduler)
        {
            // Empty
        }

        internal Task(Delegate function, object? state, Task? parent, CancellationToken cancellationToken, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions, TaskScheduler scheduler)
            : base(function, state, parent, cancellationToken, creationOptions, internalOptions, scheduler)
        {
            // Empty
        }

        public TResult Result
        {
            get
            {
                Wait();
                if (IsFaulted)
                {
                    throw Exception!;
                }

                if (IsCanceled)
                {
                    throw new AggregateException(new TaskCanceledException(this));
                }

                return InternalResult;
            }
        }

        internal static Func<Task<Task>, Task<TResult>> ContinuationConversion { get; }

        internal override void InnerInvoke()
        {
            switch (Action)
            {
                case Func<TResult> action:
                    InternalResult = action();
                    return;

                case Func<object?, TResult> withState:
                    InternalResult = withState(State);
                    return;

                default:
                    Contract.Assert(false, "Invalid Action in Task");
                    break;
            }
        }
    }
}

#endif