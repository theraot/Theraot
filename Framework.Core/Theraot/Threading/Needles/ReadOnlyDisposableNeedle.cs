#if FAT
using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class ReadOnlyDisposableNeedle<T> : IReadOnlyNeedle<T>
    {
        private readonly int _hashCode;
        private int _status;

        public ReadOnlyDisposableNeedle()
        {
            IsAlive = false;
            _hashCode = EqualityComparer<T>.Default.GetHashCode(default);
        }

        public ReadOnlyDisposableNeedle(T target)
        {
            IsAlive = true;
            Value = target;
            _hashCode = EqualityComparer<T>.Default.GetHashCode(target);
        }

        public bool IsAlive { get; private set; }

        public bool IsDisposed => _status == -1;

        public T Value { get; private set; }

        public static explicit operator T(ReadOnlyDisposableNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static implicit operator ReadOnlyDisposableNeedle<T>(T field)
        {
            return new ReadOnlyDisposableNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            if (left is null && right is null)
            {
                return false;
            }
            if (left is null || right is null)
            {
                return true;
            }
            return !EqualityComparer<T>.Default.Equals(left.Value, right.Value);
        }

        public static bool operator ==(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            if (left is null && right is null)
            {
                return true;
            }
            if (left is null || right is null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(left.Value, right.Value);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            if (TakeDisposalExecution())
            {
                Kill();
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
        {
            if (_status == -1)
            {
                whenDisposed?.Invoke();
            }
            else
            {
                if (whenNotDisposed != null)
                {
                    if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
                    {
                        try
                        {
                            whenNotDisposed.Invoke();
                        }
                        finally
                        {
                            System.Threading.Interlocked.Decrement(ref _status);
                        }
                    }
                    else
                    {
                        whenDisposed?.Invoke();
                    }
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_status == -1)
            {
                if (whenDisposed == null)
                {
                    return default;
                }
                return whenDisposed.Invoke();
            }
            if (whenNotDisposed == null)
            {
                return default;
            }
            if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
            {
                try
                {
                    return whenNotDisposed.Invoke();
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref _status);
                }
            }
            if (whenDisposed == null)
            {
                return default;
            }
            return whenDisposed.Invoke();
        }

        public override bool Equals(object obj)
        {
            var needle = obj as ReadOnlyDisposableNeedle<T>;
            if (needle != null)
            {
                return EqualityComparer<T>.Default.Equals(Value, needle.Value);
            }
            if (obj is T variable)
            {
                return EqualityComparer<T>.Default.Equals(Value, variable);
            }
            return false;
        }

        public bool Equals(ReadOnlyDisposableNeedle<T> other)
        {
            return other != null && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            if (IsAlive)
            {
                return target.ToString();
            }
            return "<Dead Needle>";
        }

        private void Kill()
        {
            IsAlive = false;
            Value = default;
        }

        private bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }
    }
}

#endif