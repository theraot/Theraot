#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed class Pin
    {
        private readonly NeedleLock<Thread> _lock;

        private int _status;

        internal Pin(LockContext<Thread> context)
        {
            _lock = new NeedleLock<Thread>(context);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~Pin()
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

        public bool CheckCapture()
        {
            var thread = Thread.CurrentThread;
            var check = _lock.Value;
            return ReferenceEquals(check, thread);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        internal bool Capture(LockSlot<Thread> slot)
        {
            if (ReferenceEquals(slot, null))
            {
                return false;
            }
            slot.Capture(_lock);
            return true;
        }

        internal void Release(LockSlot<Thread> slot)
        {
            if (!ReferenceEquals(slot, null))
            {
                slot.Uncapture(_lock);
            }
            _lock.Free();
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                if (disposeManagedResources)
                {
                    _lock.Free();
                }
            }
        }

        private bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }
    }
}

#endif