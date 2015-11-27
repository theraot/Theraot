using System.Diagnostics.Contracts;
using Theraot.Core;

#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    public class Task<TResult> : Task
    {
        protected TResult ProtectedResult;

        public TResult Result
        {
            [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Microsoft's Design")]
            get
            {
                try
                {
                    Wait();
                }
                catch(NewOperationCanceledException)
                {
                    throw new AggregateException(new TaskCanceledException(this));
                }
                if (IsFaulted)
                {
                    throw Exception;
                }
                if (IsCanceled)
                {
                    throw new AggregateException(new TaskCanceledException(this));
                }
                return ProtectedResult;
            }
        }

        internal override void InnerInvoke()
        {
            var action = Action as Func<TResult>;
            if (action != null)
            {
                ProtectedResult = action();
                return;
            }
            var withState = Action as Func<object, TResult>;
            if (withState != null)
            {
                ProtectedResult = withState(State);
                return;
            }
            Contract.Assert(false, "Invalid Action in Task");
        }

        public Task(Func<TResult> function)
            : base(function, null, null, default(CancellationToken), TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, CancellationToken cancellationToken)
            : base(function, null, null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, TaskScheduler.Default)
        {
            // Empty
        }

        public Task(Func<TResult> function, TaskCreationOptions creationOptions)
            : base(function, null, null, default(CancellationToken), creationOptions, InternalTaskOptions.None, TaskScheduler.Default)
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

        internal Task(Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : base(function, state, null, cancellationToken, creationOptions, InternalTaskOptions.None, scheduler)
        {
            // Empty
        }

        internal Task(Delegate function, object state, Task parent, CancellationToken cancellationToken, TaskCreationOptions creationOptions, InternalTaskOptions internalOptions, TaskScheduler scheduler)
            : base(function, state, parent, cancellationToken, creationOptions, internalOptions, scheduler)
        {
            // Empty
        }
    }
}

#endif