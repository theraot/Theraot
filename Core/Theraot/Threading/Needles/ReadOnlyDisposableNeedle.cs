#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class ReadOnlyDisposableNeedle<T> : IReadOnlyNeedle<T>
    {
        private readonly int _hashCode;
        private bool _isAlive;
        private int _status;
        private T _target;

        public ReadOnlyDisposableNeedle()
        {
            _isAlive = false;
            _hashCode = EqualityComparer<T>.Default.GetHashCode(default(T));
        }

        public ReadOnlyDisposableNeedle(T target)
        {
            _isAlive = true;
            _target = target;
            _hashCode = EqualityComparer<T>.Default.GetHashCode(target);
        }

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        public bool IsDisposed
        {
            get { return _status == -1; }
        }

        public T Value
        {
            get { return _target; }
        }

        public static explicit operator T(ReadOnlyDisposableNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            return needle.Value;
        }

        public static implicit operator ReadOnlyDisposableNeedle<T>(T field)
        {
            return new ReadOnlyDisposableNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            if (left == null && right == null)
            {
                return false;
            }
            if (left == null || right == null)
            {
                return true;
            }
            return !EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public static bool operator ==(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            if (left == null || right == null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(left._target, right._target);
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
                if (whenDisposed == null)
                {
                    whenDisposed.Invoke();
                }
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
                        if (whenDisposed == null)
                        {
                            whenDisposed.Invoke();
                        }
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
                    return default(TReturn);
                }
                else
                {
                    return whenDisposed.Invoke();
                }
            }
            else
            {
                if (whenNotDisposed == null)
                {
                    return default(TReturn);
                }
                else
                {
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
                    else
                    {
                        if (whenDisposed == null)
                        {
                            return default(TReturn);
                        }
                        else
                        {
                            return whenDisposed.Invoke();
                        }
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            var needle = obj as ReadOnlyDisposableNeedle<T>;
            if (needle != null)
            {
                return EqualityComparer<T>.Default.Equals(_target, needle._target);
            }
            // Keep the "is" operator
            if (obj is T)
            {
                return EqualityComparer<T>.Default.Equals(_target, (T)obj);
            }
            return false;
        }

        public bool Equals(ReadOnlyDisposableNeedle<T> other)
        {
            return !ReferenceEquals(_target, null) && EqualityComparer<T>.Default.Equals(_target, other.Value);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            if (_isAlive)
            {
                return target.ToString();
            }
            return "<Dead Needle>";
        }

        private void Kill()
        {
            _isAlive = false;
            _target = default(T);
        }

        private bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            else
            {
                return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
            }
        }
    }
}

#endif