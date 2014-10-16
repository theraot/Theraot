#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed class LockableNeedle<T, TNeedle> : INeedle<T>, ILockable
        where TNeedle : class, INeedle<T>
    {
        private readonly Lockable _lockable;
        private readonly TNeedle _needle;

        public LockableNeedle(LockableContext context, TNeedle needle)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            else
            {
                _lockable = new Lockable(context);
                _needle = needle;
            }
        }

        public bool HasOwner
        {
            get
            {
                return _lockable.HasOwner;
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return _needle.IsAlive;
            }
        }

        public T Value
        {
            get
            {
                return _needle.Value;
            }
            set
            {
                if (_lockable.CheckAccess(Thread.CurrentThread))
                {
                    _needle.Value = value;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool Capture()
        {
            return _lockable.Capture();
        }

        public bool CheckAccess(Thread thread)
        {
            return _lockable.CheckAccess(thread);
        }

        void INeedle<T>.Free()
        {
            if (_lockable.CheckAccess(Thread.CurrentThread))
            {
                _needle.Free();
            }
        }

        public void Uncapture()
        {
            _lockable.Uncapture();
        }
    }
}

#endif