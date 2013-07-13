using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class WeakNeedle<T> : INeedle<T>
        where T : class
    {
        private GCHandle _handle;
        private int _managedDisposal;
        private bool _trackResurrection;

        public WeakNeedle()
        {
            //Empty
        }

        public WeakNeedle(bool trackResurrection)
        {
            _trackResurrection = trackResurrection;
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public WeakNeedle(T target)
            : this(target, false)
        {
            //Empty
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public WeakNeedle(T target, bool trackResurrection)
        {
            _trackResurrection = trackResurrection;
            Allocate(target, _trackResurrection);
        }

        public virtual bool IsAlive
        {
            get
            {
                return DisposedConditional
                    (
                        () =>
                        {
                            return false;
                        },
                        () =>
                        {
                            if (!_handle.IsAllocated)
                            {
                                return false;
                            }
                            else
                            {
                                return !ReferenceEquals(_handle.Target, null);
                            }
                        }
                    );
            }
        }

        public virtual bool TrackResurrection
        {
            get
            {
                return _trackResurrection;
            }
        }

        public virtual T Value
        {
            get
            {
                return DisposedConditional<T>
                    (
                        () =>
                        {
                            return default(T);
                        },
                        () =>
                        {
                            try
                            {
                                object obj = _handle.Target;
                                if (obj == null)
                                {
                                    return default(T);
                                }
                                else
                                {
                                    return (T)obj;
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                return default(T);
                            }
                        }
                    );
            }
            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            set
            {
                Allocate(value, _trackResurrection);
            }
        }

        public void Release()
        {
            Dispose();
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected void Allocate(T value, bool trackResurrection)
        {
            DisposedConditional
                (
                    () =>
                    {
                        _handle = GetNewHandle(value, trackResurrection);
                        if (Interlocked.CompareExchange(ref _managedDisposal, 0, 1) == 1)
                        {
                            GC.ReRegisterForFinalize(this);
                        }
                        UnDispose();
                    },
                    () =>
                    {
                        var newHandle = GetNewHandle(value, trackResurrection);
                        GCHandle oldHandle = _handle;
                        _handle = newHandle;
                        if (oldHandle.IsAllocated)
                        {
                            try
                            {
                                oldHandle.Free();
                            }
                            catch (InvalidOperationException)
                            {
                                //Empty
                            }
                        }
                    }
                );
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        protected virtual GCHandle GetNewHandle(T value, bool trackResurrection)
        {
            return GCHandle.Alloc(value, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private void ReleaseExtracted()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }

        private void ReportManagedDisposal()
        {
            Thread.VolatileWrite(ref _managedDisposal, 1);
        }
    }
}