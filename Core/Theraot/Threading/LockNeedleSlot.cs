using System;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    internal class LockNeedleSlot<T> : IComparable<LockNeedleSlot<T>>, INeedle<T>
    {
        private readonly int _id;
        private T _target;
        private VersionProvider.VersionToken _versionToken;

        internal LockNeedleSlot(int id)
        {
            _versionToken = LockNeedleContext<T>.VersionToken.Clone();
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
                var versionToken = ThreadingHelper.VolatileRead(ref _versionToken);
                if (versionToken == null)
                {
                    throw new InvalidOperationException(string.Format("The {0} has been freed", GetType().Name));
                }
                else
                {
                    versionToken.UpdateTo(LockNeedleContext<T>.VersionToken);
                    _target = value;
                    versionToken = ThreadingHelper.VolatileRead(ref _versionToken);
                    if (versionToken == null)
                    {
                        _target = default(T);
                    }
                }
            }
        }

        public void Capture(LockNeedle<T> needle)
        {
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
            _target = default(T);
            ThreadingHelper.VolatileWrite(ref _versionToken, null);
            LockNeedleContext<T>.Free(this);
        }

        public bool Lock(LockNeedle<T> needle)
        {
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

        internal void Unfree()
        {
            _versionToken = LockNeedleContext<T>.VersionToken.Clone();
        }
    }
}