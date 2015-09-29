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
        private Thread _initializerThread;

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
            _valueFactory = Check.NotNullArgument(valueFactory, "valueFactory");
        }

        public LazyNeedle(Func<T> valueFactory, bool cacheExceptions)
           : base(false)
        {
            _valueFactory = Check.NotNullArgument(valueFactory, "valueFactory");
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

        public override bool Equals(object obj)
        {
            return !ReferenceEquals(null, obj as LazyNeedle<T>) && base.Equals(obj);
        }

        public bool Equals(LazyNeedle<T> other)
        {
            return !ReferenceEquals(other, null) && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Initialize()
        {
            if (_initializerThread == Thread.CurrentThread)
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
            ThreadingHelper.VolatileWrite(ref _valueFactory, null);
        }

        public override void Wait()
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait();
        }

        public override void Wait(CancellationToken cancellationToken)
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(cancellationToken);
        }

        public override void Wait(int milliseconds)
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(milliseconds);
        }

        public override void Wait(TimeSpan timeout)
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(timeout);
        }

        public override void Wait(int milliseconds, CancellationToken cancellationToken)
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            base.Wait(milliseconds, cancellationToken);
        }

        public override void Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (_initializerThread == Thread.CurrentThread)
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
            if (_initializerThread == Thread.CurrentThread)
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
            _initializerThread = Thread.CurrentThread;
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
                _initializerThread = null;
            }
        }
    }
}

#endif