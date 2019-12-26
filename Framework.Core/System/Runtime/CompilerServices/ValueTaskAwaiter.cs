#if LESSTHAN_NET45

#pragma warning disable CA1815 // Override equals and operator equals on value types

using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.Runtime.CompilerServices
{
    public struct ValueTaskAwaiter : ICriticalNotifyCompletion
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
}

#endif