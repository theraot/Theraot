#if FAT

using System;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact : IDisposable
    {
        private int _status;

        [System.Diagnostics.DebuggerNonUserCode]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
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
                    // Pokemon - fields may be partially collected.
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
            if (_status == -1)
            {
                return false;
            }
            else
            {
                return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
            }
        }
    }
}

#endif