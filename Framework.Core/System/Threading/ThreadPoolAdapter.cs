#if LESSTHAN_NET40

namespace System.Threading
{
    internal static class ThreadPoolAdapter
    {
        internal static void QueueWorkItem(IThreadPoolWorkItem item)
        {
            ThreadPool.QueueUserWorkItem
            (
                _ =>
                {
                    try
                    {
                        item.ExecuteWorkItem();
                    }
                    catch (ThreadAbortException exception)
                    {
                        item.MarkAborted(exception);
                    }
                }
            );
        }
    }
}

#endif