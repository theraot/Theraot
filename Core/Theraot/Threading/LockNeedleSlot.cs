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
                _target = value;
            }
        }

        public void Capture(LockNeedle<T> needle)
        {
            _context.Advance();
            needle.Capture(_id);
        }

        public int CompareTo(LockNeedleSlot<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            else
            {
                if (ReferenceEquals(_context.VersionToken, null))
                {
                    return ReferenceEquals(other._context.VersionToken, null) ? 0 : -1;
                }
                else
                {
                    if (ReferenceEquals(other._context.VersionToken, null))
                    {
                        return 1;
                    }
                    else
                    {
                        return _context.VersionToken.CompareTo(other._context.VersionToken);
                    }
                }
            }
        }

        public void Free()
        {
            _target = default(T);
            _context.Free(this);
        }

        public bool Lock(LockNeedle<T> needle)
        {
            _context.Advance();
            return needle.Lock(_id);
        }

        public void Uncapture(LockNeedle<T> needle)
        {
            needle.Uncapture(_id);
        }

        public bool Unlock(LockNeedle<T> needle)
        {
            return needle.Unlock(_id);
        }
    }
}