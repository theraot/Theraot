using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class PromiseNeedle<T> : IPromise<T>, IReadOnlyNeedle<T>, ICacheNeedle<T>, IEquatable<PromiseNeedle<T>>
    {
        private readonly Internal _internal;

        public PromiseNeedle(bool done)
        {
            _internal = new Internal();
            if (done)
            {
                _internal.OnCompleted();
            }
        }

        public PromiseNeedle(T target)
        {
            _internal = new Internal(target);
        }

        public PromiseNeedle(Exception exception)
        {
            _internal = new Internal(exception);
        }

        public PromiseNeedle(out IPromised<T> promised, bool done)
        {
            _internal = new Internal();
            if (done)
            {
                _internal.OnCompleted();
            }
            promised = _internal;
        }

        public PromiseNeedle(out IPromised<T> promised, T target)
        {
            _internal = new Internal(target);
            promised = _internal;
        }

        public PromiseNeedle(out IPromised<T> promised, Exception exception)
        {
            _internal = new Internal(exception);
            promised = _internal;
        }

        public Exception Error
        {
            get
            {
                return _internal.Error;
            }
        }

        T INeedle<T>.Value
        {
            get
            {
                return Value;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return IsCompleted;
            }
        }

        public bool IsCanceled
        {
            get
            {
                return _internal.IsCanceled;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _internal.IsCompleted;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return _internal.IsFaulted;
            }
        }

        public T Value
        {
            get
            {
                return _internal.Value;
            }
        }

        public static explicit operator T(PromiseNeedle<T> needle)
        {
            return Check.NotNullArgument(needle, "needle").Value;
        }

        public static bool operator !=(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as PromiseNeedle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return EqualsExtracted(this, _obj);
            }
            else
            {
                return _internal.IsCompleted && _internal.Value.Equals(obj);
            }
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _internal.GetHashCode();
        }

        void INeedle<T>.Release()
        {
            //Empty
        }

        public override string ToString()
        {
            return string.Format("{{Promise: {0}}}", _internal.ToString());
        }

        public void Wait()
        {
            _internal.Wait();
        }

        private static bool EqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            else
            {
                return left.Equals(right);
            }
        }

        private static bool NotEqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            else
            {
                return !left.Equals(right);
            }
        }

        [Serializable]
        private class Internal : IPromised<T>, IObserver<T>, IReadOnlyNeedle<T>, ICacheNeedle<T>, IEquatable<Internal>
        {
            private readonly int _hashCode;
            private Exception _error;
            private int _isCompleted;
            private T _target;
            private StructNeedle<ManualResetEvent> _waitHandle;

            public Internal()
            {
                _waitHandle = new ManualResetEvent(false);
                _hashCode = base.GetHashCode();
            }

            public Internal(T target)
            {
                _target = target;
                if (ReferenceEquals(target, null))
                {
                    _hashCode = base.GetHashCode();
                }
                else
                {
                    _hashCode = target.GetHashCode();
                }
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle = new ManualResetEvent(true);
            }

            public Internal(Exception error)
            {
                _error = error;
                _hashCode = error.GetHashCode();
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle = new ManualResetEvent(true);
            }

            ~Internal()
            {
                var waitHandle = _waitHandle.Value;
                if (!ReferenceEquals(waitHandle, null))
                {
                    waitHandle.Close();
                }
                _waitHandle.Value = null;
            }

            public Exception Error
            {
                get
                {
                    Wait();
                    return _error;
                }
            }

            bool IReadOnlyNeedle<T>.IsAlive
            {
                get
                {
                    return Thread.VolatileRead(ref _isCompleted) == 1;
                }
            }

            public bool IsCanceled
            {
                get
                {
                    return false;
                }
            }

            public bool IsCompleted
            {
                get
                {
                    return Thread.VolatileRead(ref _isCompleted) == 1;
                }
            }

            public bool IsFaulted
            {
                get
                {
                    return !ReferenceEquals(_error, null);
                }
            }

            public T Value
            {
                get
                {
                    Wait();
                    if (ReferenceEquals(_error, null))
                    {
                        return _target;
                    }
                    else
                    {
                        throw _error;
                    }
                }
                set
                {
                    OnNext(value);
                }
            }

            public override bool Equals(object obj)
            {
                var _obj = obj as Internal;
                if (_obj != null)
                {
                    return Equals(_obj);
                }
                else
                {
                    return false;
                }
            }

            public bool Equals(Internal other)
            {
                if (IsCompleted)
                {
                    if (other.IsCompleted)
                    {
                        if (ReferenceEquals(_error, null))
                        {
                            return ReferenceEquals(other._error, null);
                        }
                        else
                        {
                            return !ReferenceEquals(other._error, null) && _target.Equals(other._target);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return !other.IsCompleted;
                }
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public void OnCompleted()
            {
                _target = default(T);
                _error = null;
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle.Value.Set();
            }

            public void OnError(Exception error)
            {
                _target = default(T);
                _error = error;
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle.Value.Set();
            }

            public void OnNext(T value)
            {
                _target = value;
                _error = null;
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle.Value.Set();
            }

            public void Release()
            {
                Thread.VolatileWrite(ref _isCompleted, 0);
                _waitHandle.Value.Reset();
                _target = default(T);
                _error = null;
            }

            public override string ToString()
            {
                if (IsCompleted)
                {
                    if (ReferenceEquals(_error, null))
                    {
                        return _target.ToString();
                    }
                    else
                    {
                        return _error.ToString();
                    }
                }
                else
                {
                    return "[Not Created]";
                }
            }

            public void Wait()
            {
                _waitHandle.Value.WaitOne();
            }
        }
    }
}