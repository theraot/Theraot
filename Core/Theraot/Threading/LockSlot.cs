using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal sealed class LockSlot<T> : IComparable<LockSlot<T>>, INeedle<T>
    {
        private readonly LockContext<T> _context;
        private readonly int _id;
        private readonly VersionProvider.VersionToken _versionToken;
        private T _target;

        internal LockSlot(LockContext<T> context, int id, VersionProvider.VersionToken versionToken)
        {
            if (ReferenceEquals(versionToken, null))
            {
                throw new ArgumentNullException("versionToken");
            }
            else if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            else
            {
                _context = context;
                _versionToken = versionToken.Clone();
                _id = id;
            }
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
                _target = value;
            }
        }

        public void Capture(NeedleLock<T> needle)
        {
            needle.Capture(_id);
        }

        public int CompareTo(LockSlot<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            else
            {
                return _versionToken.CompareTo(other._versionToken);
            }
        }

        public void Free()
        {
            _target = default(T);
            _context.Free(this);
        }

        public bool Lock(NeedleLock<T> needle)
        {
            return needle.Lock(_id);
        }

        public void Uncapture(NeedleLock<T> needle)
        {
            needle.Uncapture(_id);
        }

        public bool Unlock(NeedleLock<T> needle)
        {
            return needle.Unlock(_id);
        }

        internal void Unfree()
        {
            _versionToken.Update();
        }
    }
}