using System.Threading;

namespace System
{
    public interface IAsyncResult
    {
        Object AsyncState
        {
            get;
        }

        WaitHandle AsyncWaitHandle
        {
            get;
        }

        Boolean CompletedSynchronously
        {
            get;
        }

        Boolean IsCompleted
        {
            get;
        }
    }
}