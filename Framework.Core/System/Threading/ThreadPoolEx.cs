namespace System.Threading
{
    public static class ThreadPoolEx
    {
        public static bool QueueUserWorkItem(WaitCallback callBack)
        {
            return ThreadPool.QueueUserWorkItem(callBack);
        }

        public static bool QueueUserWorkItem(WaitCallback callBack, object? state)
        {
#if LESSTHAN_NETSTANDARD13
            return ThreadPool.QueueUserWorkItem(_ => callBack(state));
#else
            return ThreadPool.QueueUserWorkItem(callBack, state);
#endif
        }

        public static bool QueueUserWorkItem(Action<object?> callBack, object? state, bool preferLocal)
        {
#if LESSTHAN_NETSTANDARD13
            _ = preferLocal;
            return ThreadPool.QueueUserWorkItem(_ => callBack(state));
#elif TARGETS_NET || TARGETS_NETCORE || LESSTHAN_NETSTANDARD21
            _ = preferLocal;
            return ThreadPool.QueueUserWorkItem(obj => callBack(obj), state);
#else
            return ThreadPool.QueueUserWorkItem(callBack, state, preferLocal);
#endif
        }

        [System.Security.SecurityCritical]
        public static bool UnsafeQueueUserWorkItem(WaitCallback callBack, object state)
        {
#if TARGETS_NET || LESSTHAN_NETCORE12 || GREATERTHAN_NETSTANDARD16
            return ThreadPool.UnsafeQueueUserWorkItem(callBack, state);
#elif LESSTHAN_NETSTANDARD13
            return ThreadPool.QueueUserWorkItem(_ => callBack(state));
#else
            return ThreadPool.QueueUserWorkItem(callBack, state);
#endif
        }
    }
}