#if NET20 || NET30 || NET35

namespace System.Threading.Tasks
{
    public partial class Task : IDisposable, IAsyncResult
    {
        /// <summary>
        /// Runs all of the continuations, as appropriate.
        /// </summary>
        internal void FinishContinuations()
        {
            // TODO
        }

        // Removes a continuation task from m_continuations
        internal void RemoveContinuation(object continuationObject) // could be TaskContinuation or Action<Task>
        {
            // TODO
        }

        internal void CancelContinuations()
        {

        }
    }
}

#endif