#if LESSTHAN_NET45

#pragma warning disable CA1815 // Override equals and operator equals on value types

using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.Runtime.CompilerServices
{
    public readonly struct ValueTaskAwaiter : ICriticalNotifyCompletion
    {
        internal static readonly Action<object> InvokeActionDelegate = state =>
        {
            if (!(state is Action action))
            {
                throw new ArgumentNullException(nameof(state));
            }

            action();
        };

        private readonly ValueTask _value;

        internal ValueTaskAwaiter(ValueTask value)
        {
            _value = value;
        }

        public bool IsCompleted
        {
            get
            {
                return _value.IsCompleted;
            }
        }

        public void GetResult()
        {
            _value.ThrowIfCompletedUnsuccessfully();
        }

        public void OnCompleted(Action continuation)
        {
            var obj = _value.Obj;
            switch (obj)
            {
                case null:
                    ValueTask.CompletedTask.GetAwaiter().OnCompleted(continuation);
                    return;

                case Task task:
                    task.GetAwaiter().OnCompleted(continuation);
                    return;

                default:
                    ((IValueTaskSource)obj).OnCompleted(InvokeActionDelegate, continuation, _value.Token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext | ValueTaskSourceOnCompletedFlags.FlowExecutionContext);
                    break;
            }
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            var obj = _value.Obj;
            switch (obj)
            {
                case null:
                    ValueTask.CompletedTask.GetAwaiter().UnsafeOnCompleted(continuation);
                    return;

                case Task task:
                    task.GetAwaiter().UnsafeOnCompleted(continuation);
                    return;

                default:
                    ((IValueTaskSource)obj).OnCompleted(InvokeActionDelegate, continuation, _value.Token, ValueTaskSourceOnCompletedFlags.UseSchedulingContext);
                    break;
            }
        }
    }

    /// <summary>Provides an awaiter for a <see cref="ValueTask{TResult}" />.</summary>
    public readonly struct ValueTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        /// <summary>The value being awaited.</summary>
        private readonly ValueTask<TResult> _value;

        /// <summary>Initializes the awaiter.</summary>
        /// <param name="value">The value to be awaited.</param>
        internal ValueTaskAwaiter(ValueTask<TResult> value)
        {
            _value = value;
        }

        /// <summary>Gets whether the <see cref="ValueTask{TResult}" /> has completed.</summary>
        public bool IsCompleted => _value.IsCompleted;

        /// <summary>Gets the result of the ValueTask.</summary>
        public TResult GetResult()
        {
            return _value._task == null ? _value._result : _value._task.GetAwaiter().GetResult();
        }

        public void OnCompleted(Action continuation)
        {
            (_value._task ?? TaskEx.FromResult(_value._result)).ConfigureAwait(true).GetAwaiter().OnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            (_value._task ?? TaskEx.FromResult(_value._result)).ConfigureAwait(true).GetAwaiter().UnsafeOnCompleted(continuation);
        }
    }
}

#endif