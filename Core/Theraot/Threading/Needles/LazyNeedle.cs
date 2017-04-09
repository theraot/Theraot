#if FAT

using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class LazyNeedle<T> : PromiseNeedle<T>, IEquatable<LazyNeedle<T>>
    {
        [NonSerialized]
        private Thread _runnerThread;

        private Func<T> _valueFactory;

        public LazyNeedle()
            : base(true)
        {
            _valueFactory = null;
        }

        public LazyNeedle(T target)
            : base(target)
        {
            _valueFactory = null;
        }

        public LazyNeedle(Func<T> valueFactory)
            : base(false)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _valueFactory = valueFactory;
        }

        public LazyNeedle(Func<T> valueFactory, bool cacheExceptions)
            : base(false)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _valueFactory = valueFactory;
            if (cacheExceptions)
            {
                _valueFactory = () =>
                {
                    try
                    {
                        return valueFactory.Invoke();
                    }
                    catch (Exception exc)
                    {
                        _valueFactory = FuncHelper.GetThrowFunc<T>(exc);
                        throw;
                    }
                };
            }
        }

        public override T Value
        {
            get
            {
                Initialize();
                return base.Value;
            }
            set
            {
                base.Value = value;
                ReleaseValueFactory();
            }
        }

        protected Thread RunnerThread
        {
            get { return _runnerThread; }
        }

        public override bool Equals(object obj)
        {
            return obj is LazyNeedle<T> && base.Equals(obj);
        }

        public bool Equals(LazyNeedle<T> other)
        {
            return other != null && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Initialize()
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            var valueFactory = Interlocked.Exchange(ref _valueFactory, null);
            if (valueFactory == null)
            {
                base.Wait();
            }
            else
            {
                InitializeExtracted(valueFactory);
            }
        }

        public void ReleaseValueFactory()
        {
            Volatile.Write(ref _valueFactory, null);
        }

        public override void Wait()
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait();
        }

        public override void Wait(CancellationToken cancellationToken)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(cancellationToken);
        }

        public override void Wait(int milliseconds)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(milliseconds);
        }

        public override void Wait(TimeSpan timeout)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(timeout);
        }

        public override void Wait(int milliseconds, CancellationToken cancellationToken)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(milliseconds, cancellationToken);
        }

        public override void Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(timeout, cancellationToken);
        }

        protected virtual void Initialize(Action beforeInitialize)
        {
            if (beforeInitialize == null)
            {
                throw new ArgumentNullException("beforeInitialize");
            }
            if (_runnerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            var valueFactory = Interlocked.Exchange(ref _valueFactory, null);
            if (valueFactory == null)
            {
                base.Wait();
            }
            else
            {
                try
                {
                    beforeInitialize.Invoke();
                }
                finally
                {
                    InitializeExtracted(valueFactory);
                }
            }
        }

        private void InitializeExtracted(Func<T> valueFactory)
        {
            _runnerThread = Thread.CurrentThread;
            try
            {
                base.Value = valueFactory.Invoke();
            }
            catch (Exception exception)
            {
                Interlocked.CompareExchange(ref _valueFactory, valueFactory, null);
                SetError(exception);
                throw;
            }
            finally
            {
                _runnerThread = null;
            }
        }
    }
}

#endif