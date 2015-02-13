#if FAT

using System;
using System.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public sealed class LockSlot<T> : IComparable<LockSlot<T>>, INeedle<T>
    {
        private readonly LockContext<T> _context;
        private readonly int _id;
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
            _id = id;
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

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return !ReferenceEquals(_target, null);
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

        public void Release()
        {
            _target = default(T);
            _context.Free(this);
        }

        internal void Capture(NeedleLock<T> needle)
        {
            needle.Capture(_id);
        }

        internal void Uncapture(NeedleLock<T> needle)
        {
            needle.Uncapture(_id);
        }

        internal void Unfree(VersionProvider.VersionToken versionToken)
        {
            _versionToken = versionToken;
        }
    }
}

#endif