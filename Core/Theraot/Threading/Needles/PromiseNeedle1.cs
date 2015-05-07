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

        public AggregateException Exception
        {
            get
            {
                return _promised.Exception;
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
            return _promised.IsCompleted && _promised.Value.Equals(obj);
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{{Promise: {0}}}", _promised);
        }

        public bool TryGetValue(out T target)
        {
            return _promised.TryGetValue(out target);
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
            return left.Equals(right);
        }

        private static bool NotEqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            return !left.Equals(right);
        }

        [Serializable]
        public sealed class Promised : ICacheNeedle<T>, IObserver<T>, IEquatable<Promised>
        {
            private readonly int _hashCode;
            private AggregateException _exception;
            private T _target;
            private StructNeedle<ManualResetEventSlim> _waitHandle;

            public Promised()
            {
                _hashCode = base.GetHashCode();
                _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
            }

            public Promised(T target)
            {
                _target = target;
                _hashCode = ReferenceEquals(target, null) ? base.GetHashCode() : target.GetHashCode();
                _waitHandle = null;
            }

            public Promised(Exception exception)
            {
                _exception = new AggregateException(exception);
                _hashCode = exception.GetHashCode();
                _waitHandle = null;
            }

            ~Promised()
            {
                ReleaseWaitHandle();
            }

            public AggregateException Exception
            {
                get
                {
                    Wait();
                    return _exception;
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
                    return !_waitHandle.IsAlive;
                }
            }

            public bool IsFaulted
            {
                get
                {
                    return !ReferenceEquals(_exception, null);
                }
            }

            public T Value
            {
                get
                {
                    Wait();
                    if (ReferenceEquals(_exception, null))
                    {
                        return _target;
                    }
                    throw _exception;
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
                        if (ReferenceEquals(_exception, null))
                        {
                            return ReferenceEquals(other._exception, null);
                        }
                        return !ReferenceEquals(other._exception, null) && _target.Equals(other._target);
                    }
                    return false;
                }
                return !other.IsCompleted;
            }

            public override bool Equals(object obj)
            {
                var _obj = obj as Promised;
                return _obj != null && Equals(_obj);
            }

            public void Free()
            {
                if (_waitHandle.IsAlive)
                {
                    _waitHandle.Value.Reset();
                }
                else
                {
                    _waitHandle.Value = new ManualResetEventSlim(false);
                }
                _target = default(T);
                _exception = null;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public void OnCompleted()
            {
                _target = default(T);
                _exception = null;
                ReleaseWaitHandle();
            }

            public void OnError(Exception error)
            {
                _exception = ReferenceEquals(_exception, null) ? new AggregateException(error) : (new AggregateException(error, _exception)).Flatten();
                _target = default(T);
                ReleaseWaitHandle();
            }

            public void OnNext(T value)
            {
                _target = value;
                _exception = null;
                ReleaseWaitHandle();
            }

            public override string ToString()
            {
                if (IsCompleted)
                {
                    if (ReferenceEquals(_exception, null))
                    {
                        return _target.ToString();
                    }
                    return _exception.ToString();
                }
                return "[Not Created]";
            }

            public bool TryGetValue(out T target)
            {
                var result = IsCompleted;
                target = _target;
                return result;
            }

            public void Wait()
            {
                var handle = _waitHandle.Value;
                if (handle != null)
                {
                    try
                    {
                        handle.Wait();
                    }
                    catch (ObjectDisposedException exception)
                    {
                        // Came late to the party, initialization was done
                        GC.KeepAlive(exception);
                    }
                }
            }

            private void ReleaseWaitHandle()
            {
                var waitHandle = _waitHandle.Value;
                if (!ReferenceEquals(waitHandle, null))
                {
                    waitHandle.Set();
                    waitHandle.Dispose();
                }
                _waitHandle.Value = null;
            }
        }
    }
}