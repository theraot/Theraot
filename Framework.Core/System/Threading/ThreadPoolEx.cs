using System.Runtime.CompilerServices;

namespace System.Threading
{
    public static class ThreadPoolEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool QueueUserWorkItem(WaitCallback callBack)
        {
            return ThreadPool.QueueUserWorkItem(callBack);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool QueueUserWorkItem(WaitCallback callBack, object? state)
        {
#if LESSTHAN_NETSTANDARD13
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }
            return ThreadPool.QueueUserWorkItem(_ => callBack(state));
#else
            return ThreadPool.QueueUserWorkItem(callBack, state);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool QueueUserWorkItem(Action<object?> callBack, object? state, bool preferLocal)
        {
#if LESSTHAN_NETSTANDARD13
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }
            _ = preferLocal;
            return ThreadPool.QueueUserWorkItem(_ => callBack(state));
#elif TARGETS_NET || TARGETS_NETCORE || LESSTHAN_NETSTANDARD21
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }
            _ = preferLocal;
            return ThreadPool.QueueUserWorkItem(obj => callBack(obj), state);
#else
            return ThreadPool.QueueUserWorkItem(callBack, state, preferLocal);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [Security.SecurityCritical]
        public static bool UnsafeQueueUserWorkItem(WaitCallback callBack, object? state)
        {
#if TARGETS_NET || LESSTHAN_NETCORE12 || GREATERTHAN_NETSTANDARD16
            return ThreadPool.UnsafeQueueUserWorkItem(callBack, state);
#elif LESSTHAN_NETSTANDARD13
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }
            return ThreadPool.QueueUserWorkItem(_ => callBack(state));
#else
            return ThreadPool.QueueUserWorkItem(callBack, state);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [Security.SecurityCritical]
        public static bool UnsafeQueueUserWorkItem(Action<object?> callBack, object? state, bool preferLocal)
        {
#if LESSTHAN_NETCORE12
            return ThreadPool.UnsafeQueueUserWorkItem(callBack, state, preferLocal);
#elif TARGETS_NET || GREATERTHAN_NETSTANDARD16
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }
            _ = preferLocal;
            return ThreadPool.UnsafeQueueUserWorkItem(obj => callBack(obj), state);
#elif LESSTHAN_NETSTANDARD13
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }
            _ = preferLocal;
            return ThreadPool.QueueUserWorkItem(_ => callBack(state));
#else
            _ = preferLocal;
            return ThreadPool.QueueUserWorkItem(obj => callBack(obj), state);
#endif
        }
    }
}