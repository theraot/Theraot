#if FAT

using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Threading.Needles
{
    public sealed class LockableNeedle<T> : Needle<T>, IDisposable
    {
        private readonly LockableContext _context;
        private readonly Pin _pin;
        private int _status;

        public LockableNeedle(T value, LockableContext context)
            : base(value)
        {
            if (ReferenceEquals(context, null))
            {
                throw new NullReferenceException("context");
            }
            _context = context;
            _pin = new Pin(_context.Context);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~LockableNeedle()
        {
            try
            {
                // Empty
            }
            finally
            {
                try
                {
                    Dispose(false);
                }
                catch (Exception exception)
                {
                    // Pokemon - fields may be partially collected.
                    GC.KeepAlive(exception);
                }
            }
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
                    Wait();
                    base.Value = value;
                    Thread.MemoryBarrier();
                }
                else
                {
                    throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
                }
            }
        }

        public void Capture()
        {
            var slot = _context.Slot;
            var lockslot = slot.LockSlot;
            if (_pin.Capture(lockslot))
            {
                slot.Add(_pin);
            }
            else
            {
                throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
            }
        }

        public bool Check()
        {
            return _pin.CheckCapture();
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        public bool TryUpdate(T newValue, T expectedValue)
        {
            if (_context.HasSlot)
            {
                Wait();
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
                Wait();
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
                Wait();
                var result = updateValueFactory(base.Value);
                base.Value = result;
                Thread.MemoryBarrier();
                return result;
            }
            throw new InvalidOperationException("The current thread has not entered the LockableContext of this LockableNeedle.");
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                if (disposeManagedResources)
                {
                    _pin.Dispose();
                }
            }
        }

        private bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }

        private void Wait()
        {
            if (!_pin.CheckCapture())
            {
                ThreadingHelper.SpinWaitUntil(() => _pin.CheckCapture());
            }
        }
    }
}

#endif