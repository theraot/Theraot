#if FAT

using System;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact : IDisposable
    {
        private int _status;

        [System.Diagnostics.DebuggerNonUserCode]
        ~Transact()
        {
            try
            {
                // Empty
            }
            finally
            {
                try
                {
                    Dispose(false);
                }
                catch (Exception exception)
                {
                    // Fields may be partially collected.
                    GC.KeepAlive(exception);
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                if (disposeManagedResources)
                {
                    Release(true);
                }
            }
        }

        private bool TakeDisposalExecution()
        {
            return _status != -1 && ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }
    }
}

#endif