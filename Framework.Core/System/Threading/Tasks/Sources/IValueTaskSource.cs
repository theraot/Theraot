#if LESSTHAN_NET45

namespace System.Threading.Tasks.Sources
{
    public interface IValueTaskSource
    {
        void GetResult(short token);

        ValueTaskSourceStatus GetStatus(short token);

        void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags);
    }
}

#endif