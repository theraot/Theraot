using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class PromiseNeedle : IPromise
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

        public PromiseNeedle(Exception exception)
        {
            _internal = new Internal(exception);
        }

        public PromiseNeedle(out IPromised promised, bool done)
        {
            _internal = new Internal();
            if (done)
            {
                _internal.OnCompleted();
            }
            promised = _internal;
        }

        public PromiseNeedle(out IPromised promised, Exception exception)
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

        public override int GetHashCode()
        {
            return _internal.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{{Promise: {0}}}", _internal);
        }

        public void Wait()
        {
            _internal.Wait();
        }

        [Serializable]
        private class Internal : IPromised, IEquatable<Internal>
        {
            private readonly int _hashCode;
            private Exception _error;
            private int _isCompleted;
            private StructNeedle<ManualResetEvent> _waitHandle;

            public Internal()
            {
                _hashCode = base.GetHashCode();
                _waitHandle = new ManualResetEvent(false);
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
                            return !ReferenceEquals(other._error, null);
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
                var _obj = obj as Internal;
                return _obj != null && Equals(_obj);
            }

            public void Free()
            {
                Thread.VolatileWrite(ref _isCompleted, 0);
                _waitHandle.Value.Reset();
                _error = null;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            void IObserver<object>.OnNext(object value)
            {
                //Empty
            }

            public void OnCompleted()
            {
                _error = null;
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle.Value.Set();
            }

            public void OnError(Exception error)
            {
                _error = error;
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle.Value.Set();
            }

            public override string ToString()
            {
                if (IsCompleted)
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
                _waitHandle.Value.WaitOne();
            }
        }
    }
}