using System;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class CacheNeedle<T> : WeakNeedle<T>, ICacheNeedle<T>, IEquatable<CacheNeedle<T>>, IPromise<T>
        where T : class
    {
        [NonSerialized]
        private Thread _initializerThread;

        private Func<T> _valueFactory;
        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public CacheNeedle(Func<T> valueFactory)
            : this(valueFactory, false)
        {
            // Empty
        }

        public CacheNeedle(Func<T> valueFactory, bool trackResurrection)
            : base(default(T), trackResurrection)
        {
            _valueFactory = Check.NotNullArgument(valueFactory, "valueFactory");
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
        }

        public CacheNeedle(Func<T> valueFactory, T target)
            : this(valueFactory, target, false)
        {
            // Empty
        }

        public CacheNeedle(Func<T> valueFactory, T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
            _valueFactory = Check.NotNullArgument(valueFactory, "valueFactory");
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
        }

        public CacheNeedle(Func<T> valueFactory, T target, bool trackResurrection, bool cacheExceptions)
            : base(target, trackResurrection)
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
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
        }

        public CacheNeedle(T target)
            : base(target)
        {
            _valueFactory = null;
            _waitHandle = null;
        }

        public CacheNeedle()
            : base(default(T))
        {
            _valueFactory = null;
            _waitHandle = null;
        }

        public T CachedTarget
        {
            get
            {
                return base.Value;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return !_waitHandle.IsAlive;
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
                SetTargetValue(value);
                ReleaseWaitHandle();
            }
        }

        Exception IPromise.Error
        {
            get
            {
                return Error;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool IPromise.IsCanceled
        {
            get
            {
                return false;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool IPromise.IsFaulted
        {
            get
            {
                return IsFaulted;
            }
        }

        protected INeedle<ManualResetEventSlim> WaitHandle
        {
            get
            {
                return _waitHandle;
            }
        }

        public bool Equals(CacheNeedle<T> other)
        {
            return !ReferenceEquals(other, null) && base.Equals(other);
        }

        public virtual void Initialize()
        {
            InitializeExtracted();
        }

        public void ReleaseValueFactory()
        {
            ThreadingHelper.VolatileWrite(ref _valueFactory, null);
        }

        public override bool TryGetValue(out T target)
        {
            target = default(T);
            return IsCompleted && base.TryGetValue(out target);
        }

        public void Wait()
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
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

        protected virtual void Initialize(Action beforeInitialize)
        {
            var _beforeInitialize = Check.NotNullArgument(beforeInitialize, "beforeInitialize");
            if (ThreadingHelper.VolatileRead(ref _valueFactory) != null)
            {
                try
                {
                    _beforeInitialize.Invoke();
                }
                finally
                {
                    InitializeExtracted();
                }
            }
        }

        private void InitializeExtracted()
        {
            back:
            var valueFactory = Interlocked.Exchange(ref _valueFactory, null);
            if (valueFactory == null)
            {
                if (_initializerThread == Thread.CurrentThread)
                {
                    throw new InvalidOperationException();
                }
                var handle = _waitHandle.Value;
                if (handle != null)
                {
                    try
                    {
                        _waitHandle.Value.Wait();
                        if (ThreadingHelper.VolatileRead(ref _valueFactory) != null)
                        {
                            goto back;
                        }
                        ReleaseWaitHandle();
                    }
                    catch (ObjectDisposedException exception)
                    {
                        // Came late to the party, initialization is done
                        GC.KeepAlive(exception);
                    }
                }
            }
            else
            {
                _initializerThread = Thread.CurrentThread;
                try
                {
                    SetTargetValue(valueFactory.Invoke());
                    ReleaseWaitHandle();
                }
                catch (Exception exception)
                {
                    SetTargetError(exception);
                    Interlocked.CompareExchange(ref _valueFactory, valueFactory, null);
                    _waitHandle.Value.Set();
                    throw;
                }
                finally
                {
                    _initializerThread = null;
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