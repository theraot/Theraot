using System;
using System.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal class LockNeedleSlot<T> : IComparable<LockNeedleSlot<T>>, INeedle<T>
    {
        private readonly LockNeedleContext<T> _context;
        private readonly int _id;
        private T _target;
        private Thread _thread;
        private VersionProvider.VersionToken _versionToken;

        internal LockNeedleSlot(LockNeedleContext<T> context, int id)
        {
            _context = context;
            _id = id;
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return !ReferenceEquals(_target, null);
            }
        }

        public T Value
        {
            get
            {
                return _target;
            }
            set
            {
                _versionToken = _context.Advance();
                _target = value;
            }
        }

        internal int Id
        {
            get
            {
                return _id;
            }
        }

        public void Capture(LockNeedle<T> token)
        {
            token.Capture(_id);
        }

        public int CompareTo(LockNeedleSlot<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            else
            {
                if (ReferenceEquals(_versionToken, null))
                {
                    return ReferenceEquals(other._versionToken, null) ? 0 : -1;
                }
                else
                {
                    if (ReferenceEquals(other._versionToken, null))
                    {
                        return 1;
                    }
                    else
                    {
                        return _versionToken.CompareTo(other._versionToken);
                    }
                }
            }
        }

        public void Free()
        {
            ThreadingHelper.VolatileWrite(ref _thread, null);
            _versionToken.Reset();
        }

        public bool Lock(LockNeedle<T> token)
        {
            return token.Lock(_id);
        }

        public void Release(LockNeedle<T> token)
        {
            token.Release(_id);
            _context.Release(this);
        }

        public bool Unlock(LockNeedle<T> token)
        {
            return token.Unlock(_id);
        }

        internal bool Claim()
        {
            return Interlocked.CompareExchange(ref _thread, Thread.CurrentThread, null) == null;
        }
    }
}