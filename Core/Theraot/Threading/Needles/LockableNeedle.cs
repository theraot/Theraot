using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed class LockableNeedle<T> : Needle<T>
    {
        private readonly LockableContext _context;
        private readonly NeedleLock<Thread> _needleLock;

        public LockableNeedle(T value, LockableContext context)
            : base(value)
        {
            if (ReferenceEquals(context, null))
            {
                throw new NullReferenceException("context");
            }
            _context = context;
            _needleLock = new NeedleLock<Thread>(_context.Context);
        }

        public override T Value
        {
            get
            {
                Thread.MemoryBarrier();
                return base.Value;
            }
            set
            {
                if (_context.HasSlot)
                {
                    CaptureAndWait();
                    base.Value = value;
                    Thread.MemoryBarrier();
                }
                else
                {
                    throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
                }
            }
        }

        public void CaptureAndWait()
        {
            Capture();
            var thread = Thread.CurrentThread;
            // The reason while I cannot make an smarter wait function:
            // If another thread changed _needleLock.Value after the check but before the starting to wait, the wait will not finish.
            ThreadingHelper.SpinWaitUntil(() => _needleLock.Value == thread);
        }

        public bool TryUpdate(T newValue, T expectedValue)
        {
            if (_context.HasSlot)
            {
                CaptureAndWait();
                if (EqualityComparer<T>.Default.Equals(base.Value, expectedValue))
                {
                    base.Value = newValue;
                    Thread.MemoryBarrier();
                    return true;
                }
                return false;
            }
            throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
        }

        public bool TryUpdate(T newValue, T expectedValue, IEqualityComparer<T> comparer)
        {
            if (_context.HasSlot)
            {
                CaptureAndWait();
                if (comparer.Equals(base.Value, expectedValue))
                {
                    base.Value = newValue;
                    Thread.MemoryBarrier();
                    return true;
                }
                return false;
            }
            throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
        }

        public T Update(Func<T, T> updateValueFactory)
        {
            if (_context.HasSlot)
            {
                CaptureAndWait();
                var result = updateValueFactory(base.Value);
                base.Value = result;
                Thread.MemoryBarrier();
                return result;
            }
            throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
        }

        private void Capture()
        {
            var slot = _context.Slot;
            var lockslot = slot.LockSlot;
            if (ReferenceEquals(lockslot, null))
            {
                throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
            }
            _needleLock.Capture(lockslot);
            slot.Add(_needleLock);
        }
    }
}