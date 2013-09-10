using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

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
                return IsLoaded;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return _internal.IsLoaded;
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
                if (_internal.IsLoaded)
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
            return string.Format("{Promise: {0}}", _internal.ToString());
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

        private class Internal : IPromised<T>, IObserver<T>, IReadOnlyNeedle<T>, ICacheNeedle<T>, IEquatable<Internal>
        {
            private Exception _error;

            private int _isValueCreated;

            private T _target;

            private ManualResetEvent _waitHandle;

            public Internal()
            {
                _waitHandle = new ManualResetEvent(false);
            }

            public Internal(T value)
            {
                _target = value;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle = new ManualResetEvent(true);
            }

            public Internal(Exception error)
            {
                _error = error;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle = new ManualResetEvent(true);
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
                    return Thread.VolatileRead(ref _isValueCreated) == 1;
                }
            }

            public bool IsLoaded
            {
                get
                {
                    return Thread.VolatileRead(ref _isValueCreated) == 1;
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
                if (IsLoaded)
                {
                    if (other.IsLoaded)
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
                    if (other.IsLoaded)
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
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle.Set();
            }

            public void OnError(Exception error)
            {
                _target = default(T);
                _error = error;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle.Set();
            }

            public void OnNext(T value)
            {
                _target = value;
                _error = null;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle.Set();
            }

            public void Release()
            {
                Thread.VolatileWrite(ref _isValueCreated, 0);
                _waitHandle.Reset();
                _target = default(T);
                _error = null;
            }

            public override string ToString()
            {
                if (IsLoaded)
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
                _waitHandle.WaitOne();
            }
        }
    }

    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class PromiseNeedle : IPromise
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

        public PromiseNeedle(Exception exception)
        {
            _internal = new Internal(exception);
            _hashCode = exception.GetHashCode();
        }

        public PromiseNeedle(out IPromised promised, bool done)
        {
            _internal = new Internal();
            if (done)
            {
                _internal.OnCompleted();
            }
            promised = _internal;
            _hashCode = base.GetHashCode();
        }

        public PromiseNeedle(out IPromised promised, Exception exception)
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

        public bool IsLoaded
        {
            get
            {
                return _internal.IsLoaded;
            }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return string.Format("{Promise: {0}}", _internal.ToString());
        }

        public void Wait()
        {
            _internal.Wait();
        }

        private class Internal : IPromised
        {
            private Exception _error;
            private int _isValueCreated;
            private ManualResetEvent _waitHandle;

            public Internal()
            {
                _waitHandle = new ManualResetEvent(false);
            }

            public Internal(Exception error)
            {
                _error = error;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle = new ManualResetEvent(true);
            }

            public Exception Error
            {
                get
                {
                    Wait();
                    return _error;
                }
            }

            object INeedle<object>.Value
            {
                get
                {
                    throw new NotSupportedException();
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            bool IReadOnlyNeedle<object>.IsAlive
            {
                get
                {
                    return false;
                }
            }

            object IReadOnlyNeedle<object>.Value
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            public bool IsLoaded
            {
                get
                {
                    return Thread.VolatileRead(ref _isValueCreated) == 1;
                }
            }

            void INeedle<object>.Release()
            {
                //Empty
            }

            void IObserver<object>.OnNext(object value)
            {
                //Empty
            }

            public void OnCompleted()
            {
                _error = null;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle.Set();
            }

            public void OnError(Exception error)
            {
                _error = error;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle.Set();
            }

            public void Release()
            {
                Thread.VolatileWrite(ref _isValueCreated, 0);
                _waitHandle.Reset();
                _error = null;
            }

            public override string ToString()
            {
                if (IsLoaded)
                {
                    if (ReferenceEquals(_error, null))
                    {
                        return "[Done]";
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
                _waitHandle.WaitOne();
            }
        }
    }
}
