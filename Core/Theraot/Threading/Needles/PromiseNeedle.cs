using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class PromiseNeedle : IPromise
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

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{{Promise: {0}}}", _promised);
        }

        public void Wait()
        {
            _promised.Wait();
        }

        [Serializable]
        public class Promised : IEquatable<Promised>, IObserver<object>
        {
            private readonly int _hashCode;
            private Exception _error;
            private int _isCompleted;
            private StructNeedle<ManualResetEvent> _waitHandle;

            public Promised()
            {
                _hashCode = base.GetHashCode();
                _waitHandle = new ManualResetEvent(false);
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
                var _obj = obj as Promised;
                return _obj != null && Equals(_obj);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            void IObserver<object>.OnNext(object value)
            {
                OnCompleted();
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