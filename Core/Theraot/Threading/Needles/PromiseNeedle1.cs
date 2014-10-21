using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class PromiseNeedle<T> : IPromise<T>, ICacheNeedle<T>, IEquatable<PromiseNeedle<T>>
    {
        private readonly Promised _promised;

        public PromiseNeedle(bool done)
        {
            _promised = new Promised();
            if (done)
            {
                _promised.OnCompleted();
            }
        }

        public PromiseNeedle(T target)
        {
            _promised = new Promised(target);
        }

        public PromiseNeedle(Exception exception)
        {
            _promised = new Promised(exception);
        }

        public PromiseNeedle(out Promised promised, bool done)
        {
            _promised = new Promised();
            if (done)
            {
                _promised.OnCompleted();
            }
            promised = _promised;
        }

        public PromiseNeedle(out Promised promised, T target)
        {
            _promised = new Promised(target);
            promised = _promised;
        }

        public PromiseNeedle(out Promised promised, Exception exception)
        {
            _promised = new Promised(exception);
            promised = _promised;
        }

        public Exception Error
        {
            get
            {
                return _promised.Error;
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

        public bool IsAlive
        {
            get
            {
                return _promised.IsAlive;
            }
        }

        public bool IsCanceled
        {
            get
            {
                return _promised.IsCanceled;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _promised.IsCompleted;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return _promised.IsFaulted;
            }
        }

        public T Value
        {
            get
            {
                return _promised.Value;
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
                return _promised.IsCompleted && _promised.Value.Equals(obj);
            }
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        void INeedle<T>.Free()
        {
            //Empty
        }

        public override string ToString()
        {
            return string.Format("{{Promise: {0}}}", _promised);
        }

        public bool TryGet(out T target)
        {
            return _promised.TryGet(out target);
        }

        public void Wait()
        {
            _promised.Wait();
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
        public sealed class Promised : ICacheNeedle<T>, IObserver<T>, IEquatable<Promised>
        {
            private readonly int _hashCode;
            private Exception _error;
            private int _isCompleted;
            private T _target;
            private StructNeedle<ManualResetEvent> _waitHandle;

            public Promised()
            {
                _waitHandle = new ManualResetEvent(false);
                _hashCode = base.GetHashCode();
            }

            public Promised(T target)
            {
                _target = target;
                _hashCode = ReferenceEquals(target, null) ? base.GetHashCode() : target.GetHashCode();
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle = new ManualResetEvent(true);
            }

            public Promised(Exception error)
            {
                _error = error;
                _hashCode = error.GetHashCode();
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle = new ManualResetEvent(true);
            }

            ~Promised()
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

            public bool IsAlive
            {
                get
                {
                    return !ReferenceEquals(_target, null);
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

            public bool Equals(Promised other)
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

            public override bool Equals(object obj)
            {
                var _obj = obj as Promised;
                return _obj != null && Equals(_obj);
            }

            public void Free()
            {
                Thread.VolatileWrite(ref _isCompleted, 0);
                _waitHandle.Value.Reset();
                _target = default(T);
                _error = null;
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

            public bool TryGet(out T target)
            {
                var result = IsCompleted;
                target = _target;
                return result;
            }

            public void Wait()
            {
                _waitHandle.Value.WaitOne();
            }
        }
    }
}