using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public interface ILockable
    {
        bool HasOwner { get; }

        bool Capture();

        bool CheckAccess(Thread thread);

        void Uncapture();
    }

    public sealed class Lockable : ILockable
    {
        private readonly LockableContext _context;
        private readonly NeedleLock<Thread> _lock;

        public Lockable(LockableContext context)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            else
            {
                _context = context;
                _lock = new NeedleLock<Thread>(context._context);
            }
        }

        public bool HasOwner
        {
            get
            {
                return !ReferenceEquals(_lock.Value, null);
            }
        }

        public bool Capture()
        {
            _context._slot.Value.Capture(_lock);
            return ReferenceEquals(_lock.Value, Thread.CurrentThread);
        }

        public bool CheckAccess(Thread thread)
        {
            var value = _lock.Value;
            return ReferenceEquals(value, thread) || ReferenceEquals(value, null);
        }

        public void Uncapture()
        {
            LockSlot<Thread> slot;
            if (_context._slot.TryGet(out slot))
            {
                slot.Uncapture(_lock);
            }
            _lock.Free();
        }
    }

    public sealed class LockableContext
    {
        internal readonly LockContext<Thread> _context;
        internal TrackingThreadLocal<LockSlot<Thread>> _slot;

        public LockableContext(int capacity)
        {
            _context = new LockContext<Thread>(capacity);
            _slot = new TrackingThreadLocal<LockSlot<Thread>>
                (
                    () =>
                    {
                        LockSlot<Thread> _lockSlot = null;
                        ThreadingHelper.SpinWaitUntil(() => _context.ClaimSlot(out _lockSlot));
                        _lockSlot.Value = Thread.CurrentThread;
                        return _lockSlot;
                    }
                );
        }
    }

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
                    throw new InvalidOperationException("");
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