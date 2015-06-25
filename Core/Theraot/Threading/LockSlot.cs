using System;
using System.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public sealed class LockSlot<T> : IComparable<LockSlot<T>>, INeedle<T>
    {
        internal readonly int Id;
        private readonly LockContext<T> _context;
        private int _free;
        private T _target;
        private VersionProvider.VersionToken _versionToken;

        internal LockSlot(LockContext<T> context, int id, VersionProvider.VersionToken versionToken)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
            _versionToken = versionToken;
            Id = id;
        }

        public T Value
        {
            get
            {
                Thread.MemoryBarrier();
                return _target;
            }
            set
            {
                _target = value;
                Thread.MemoryBarrier();
            }
        }

        internal bool IsOpen
        {
            get
            {
                return Thread.VolatileRead(ref _free) == 0;
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return !ReferenceEquals(_target, null);
            }
        }

        public void Close()
        {
            if (Interlocked.CompareExchange(ref _free, 1, 0) == 0)
            {
                _target = default(T);
                _context.Close(this);
            }
        }

        public int CompareTo(LockSlot<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            return _versionToken.CompareTo(other._versionToken);
        }

        internal void Open(VersionProvider.VersionToken versionToken)
        {
            if (Interlocked.CompareExchange(ref _free, 0, 1) == 1)
            {
                _versionToken = versionToken;
            }
        }
    }
}