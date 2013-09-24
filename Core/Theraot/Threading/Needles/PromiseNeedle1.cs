using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class PromiseNeedle<T> : IPromise<T>, IReadOnlyNeedle<T>, ICacheNeedle<T>, IEquatable<PromiseNeedle<T>>
    {
        private int _hashCode;
        private Internal _internal;

        public PromiseNeedle(bool done)
        {
            _internal = new Internal();
            if (done)
            {
                _internal.OnCompleted();
            }
            _hashCode = base.GetHashCode();
        }

        public PromiseNeedle(T target)
        {
            _internal = new Internal(target);
            _hashCode = target.GetHashCode();
        }

        public PromiseNeedle(Exception exception)
        {
            _internal = new Internal(exception);
            _hashCode = exception.GetHashCode();
        }

        public PromiseNeedle(out IPromised<T> promised, bool done)
        {
            _internal = new Internal();
            if (done)
            {
                _internal.OnCompleted();
            }
            promised = _internal;
            _hashCode = base.GetHashCode();
        }

        public PromiseNeedle(out IPromised<T> promised, T target)
        {
            _internal = new Internal(target);
            promised = _internal;
            _hashCode = target.GetHashCode();
        }

        public PromiseNeedle(out IPromised<T> promised, Exception exception)
        {
            _internal = new Internal(exception);
            promised = _internal;
            _hashCode = exception.GetHashCode();
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
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            else
            {
                return needle.Value;
            }
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
                if (_internal.IsCompleted)
                {
                    return _internal.Value.Equals(obj);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
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
                if (ReferenceEquals(right, null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
                if (ReferenceEquals(right, null))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return !left.Equals(right);
            }
        }

        [Serializable]
        private class Internal : IPromised<T>, IObserver<T>, IReadOnlyNeedle<T>, ICacheNeedle<T>, IEquatable<Internal>
        {
            private Exception _error;
            private int _isCompleted;
            private T _target;

            private StructNeedle<ManualResetEvent> _waitHandle;

            public Internal()
            {
                _waitHandle = new ManualResetEvent(false);
            }

            public Internal(T value)
            {
                _target = value;
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle = new ManualResetEvent(true);
            }

            public Internal(Exception error)
            {
                _error = error;
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

            public bool Equals(Internal other)
            {
                if (IsCompleted)
                {
                    if (other.IsCompleted)
                    {
                        if (ReferenceEquals(_error, null))
                        {
                            if (ReferenceEquals(other._error, null))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (ReferenceEquals(other._error, null))
                            {
                                return false;
                            }
                            else
                            {
                                return _target.Equals(other._target);
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (other.IsCompleted)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
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