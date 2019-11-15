#if LESSTHAN_NET45

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable CS0649 // Field is never assigned

using System.Security;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    public struct AsyncValueTaskMethodBuilder
    {
        private bool _haveResult;
        private AsyncTaskMethodBuilder? _methodBuilder;

        public ValueTask Task
        {
            get
            {
                return _haveResult ? new ValueTask() : new ValueTask(GetMethodBuilder().Task);
            }
        }

        public static AsyncValueTaskMethodBuilder Create()
        {
            return new AsyncValueTaskMethodBuilder();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            GetMethodBuilder().AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            GetMethodBuilder().AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetException(Exception exception)
        {
            GetMethodBuilder().SetException(exception);
        }

        public void SetResult()
        {
            if (!_methodBuilder.HasValue)
            {
                _haveResult = true;
            }
            else
            {
                _methodBuilder.Value.SetResult();
            }
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            GetMethodBuilder().SetStateMachine(stateMachine);
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            GetMethodBuilder().Start(ref stateMachine);
        }

        private AsyncTaskMethodBuilder GetMethodBuilder()
        {
            return _methodBuilder ??= new AsyncTaskMethodBuilder();
        }
    }
}

#endif