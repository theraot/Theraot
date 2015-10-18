#if FAT

using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class DisposableAkin : CriticalFinalizerObject, IDisposable
    {
        private Action _release;
        private Thread _thread;

        private DisposableAkin(Action release, Thread thread)
        {
            if (release == null)
            {
                throw new ArgumentNullException("release");
            }
            if (thread == null)
            {
                throw new ArgumentNullException("thread");
            }
            _release = release;
            _thread = thread;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~DisposableAkin()
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

        public bool IsDisposed
        {
            [System.Diagnostics.DebuggerNonUserCode]
            get
            {
                return _thread == null;
            }
        }

        public static DisposableAkin Create(Action release)
        {
            return new DisposableAkin(release, Thread.CurrentThread);
        }

        public static DisposableAkin Create()
        {
            return new DisposableAkin(ActionHelper.GetNoopAction(), Thread.CurrentThread);
        }

        public bool Dispose(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }
            if (ReferenceEquals(Interlocked.CompareExchange(ref _thread, null, Thread.CurrentThread), Thread.CurrentThread))
            {
                if (condition.Invoke())
                {
                    try
                    {
                        _release.Invoke();
                        return true;
                    }
                    finally
                    {
                        _release = null;
                    }
                }
                return false;
            }
            return false;
        }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive", Justification = "By Design")]
        private void Dispose(bool disposeManagedResources)
        {
            if
            (
                !disposeManagedResources ||
                ReferenceEquals(Interlocked.CompareExchange(ref _thread, null, Thread.CurrentThread), Thread.CurrentThread)
            )
            {
                try
                {
                    _release.Invoke();
                }
                finally
                {
                    _release = null;
                }
            }
        }
    }
}

#endif