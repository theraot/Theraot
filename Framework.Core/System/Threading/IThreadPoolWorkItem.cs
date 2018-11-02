#if NET20 || NET30 || NET35

using System.Security;

namespace System.Threading
{
    internal interface IThreadPoolWorkItem
    {
        [SecurityCritical]
        void ExecuteWorkItem();

        [SecurityCritical]
        void MarkAborted(ThreadAbortException exception);
    }

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