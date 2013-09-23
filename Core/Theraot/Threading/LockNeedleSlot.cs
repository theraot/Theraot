using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal class LockNeedleSlot<T> : IComparable<LockNeedleSlot<T>>, INeedle<T>
    {
        private LockNeedleContext<T> _context;
        private int _id;
        private T _target;
        private Thread _thread;
        private VersionProvider.VersionToken _token;

        internal LockNeedleSlot(LockNeedleContext<T> context)
        {
            _context = context;
        }

        public bool IsAlive
        {
            get { throw new NotImplementedException(); }
        }

        public T Value
        {
            get
            {
                return _target;
            }
            set
            {
                _token = _context.Advance();
                _target = value;
            }
        }

        internal int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
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
                if (ReferenceEquals(_token, null))
                {
                    return ReferenceEquals(other._token, null) ? 0 : -1;
                }
                else
                {
                    if (ReferenceEquals(other._token, null))
                    {
                        return 1;
                    }
                    else
                    {
                        return _token.CompareTo(other._token);
                    }
                }
            }
        }

        public void Release()
        {
            ThreadingHelper.VolatileWrite(ref _thread, null);
            _token.Reset();
        }

        public void Release(LockNeedle<T> token)
        {
            token.Release(_id);
        }

        internal bool Claim()
        {
            return Interlocked.CompareExchange(ref _thread, Thread.CurrentThread, null) == null;
        }
    }
}
