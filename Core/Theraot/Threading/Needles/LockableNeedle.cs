using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed class LockableNeedle<T, TNeedle> : INeedle<T>
        where TNeedle : class, INeedle<T>
    {
        [ThreadStatic]
        private static LockSlot<Thread> _thread_slot;
        private readonly Context _context;
        private LockSlot<Thread> _slot;
        private int _count;
        private readonly TNeedle _needle;
        public sealed class Context
        {
            internal readonly LockContext<Thread> _context;

            public Context(int capacity)
            {
                _context = new LockContext<Thread>(capacity);
            }
        }

        public LockableNeedle(Context context, TNeedle needle)
        {
            _context = context;
            _needle = needle;
        }

        public IDisposable Lock()
        {
            if (ReferenceEquals(_thread_slot, null))
            {
                ThreadingHelper.SpinWaitUntil(() => _context._context.ClaimSlot(out _thread_slot));
                _thread_slot.Value = Thread.CurrentThread;
            }
            _slot = _thread_slot;
            Interlocked.Increment(ref _count);
            return DisposableAkin.Create(OnRelease);
        }
        private void OnRelease()
        {
            if (Interlocked.Decrement(ref _count) == 0 && !ReferenceEquals(_slot, null))
            {
                _slot.Free();
                _slot = null;
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
                using (Lock())
                {
                    _needle.Value = value;
                }
            }
        }

        void INeedle<T>.Free()
        {
            using (Lock())
            {
                _needle.Free();
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return _needle.IsAlive;
            }
        }
    }
}