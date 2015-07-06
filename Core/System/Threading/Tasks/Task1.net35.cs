#if NET20 || NET30 || NET35

using Theraot.Core;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks
{
    public partial class Task<TResult> : Task
    {
        private IErsatz<TResult> _erzatz;

        public TResult Result
        {
            get
            {
                Wait();
                if (IsFaulted)
                {
                    throw Exception;
                }
                return _erzatz.Result;
            }
        }

        private Task(IErsatz<TResult> erzatz)
            : base(erzatz.InvokeAction(), null, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
        {
            _erzatz = erzatz;
        }

        private Task(IErsatz<TResult> erzatz, TaskCreationOptions creationOptions)
            : base(erzatz.InvokeAction(), null, CancellationToken.None, creationOptions, TaskScheduler.Default)
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

        public Task(Func<TResult> function, TaskCreationOptions creationOptions)
            : this(new ErsatzFunc<TResult>(function), creationOptions)
        {
            // Empty
        }

        internal Task(Func<TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskScheduler scheduler)
            : this(new ErsatzFunc<TResult>(function), state, cancellationToken, creationOptions, scheduler)
        {
            // Empty
        }
    }
}

#endif