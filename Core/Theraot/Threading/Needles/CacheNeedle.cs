#if FAT

using System;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class CacheNeedle<T> : WeakNeedle<T>, ICacheNeedle<T>, IEquatable<CacheNeedle<T>>, IWaitablePromise<T>
        where T : class
    {
        [NonSerialized]
        private Thread _initializerThread;

        private Func<T> _valueFactory;

        // Can be null
        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public CacheNeedle(Func<T> valueFactory)
            : base(default(T))
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _valueFactory = valueFactory;
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
        }

        public CacheNeedle(Func<T> valueFactory, T target)
            : base(target)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _valueFactory = valueFactory;
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
        }

        public CacheNeedle(Func<T> valueFactory, T target, bool cacheExceptions)
            : base(target)
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
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
        }

        public CacheNeedle(T target)
            : base(target)
        {
            _valueFactory = null;
            _waitHandle = new StructNeedle<ManualResetEventSlim>(null);
        }

        public CacheNeedle()
            : base(default(T))
        {
            _valueFactory = null;
            _waitHandle = new StructNeedle<ManualResetEventSlim>(null);
        }

        public T CachedTarget
        {
            get { return base.Value; }
        }

        Exception IPromise.Exception
        {
            get { return Exception; }
        }

        bool IPromise.IsCanceled
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return !_waitHandle.IsAlive; }
        }

        bool IPromise.IsFaulted
        {
            get { return IsFaulted; }
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

        protected INeedle<ManualResetEventSlim> WaitHandle
        {
            get { return _waitHandle; }
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
            Volatile.Write(ref _valueFactory, null);
        }

        public override bool TryGetValue(out T value)
        {
            value = default(T);
            return IsCompleted && base.TryGetValue(out value);
        }

        public void Wait()
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            var waitHandle = _waitHandle.Value;
            if (waitHandle != null)
            {
                try
                {
                    waitHandle.Wait();
                }
                catch (ObjectDisposedException exception)
                {
                    // Came late to the party, initialization was done
                    GC.KeepAlive(exception);
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        protected override void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                try
                {
                    if (disposeManagedResources)
                    {
                        //Empty
                    }
                }
                finally
                {
                    try
                    {
                        ReleaseWaitHandle();
                    }
                    finally
                    {
                        _valueFactory = null;
                    }
                    base.Dispose(disposeManagedResources);
                }
            }
        }

        protected virtual void Initialize(Action beforeInitialize)
        {
            if (beforeInitialize == null)
            {
                throw new ArgumentNullException("beforeInitialize");
            }
            if (Volatile.Read(ref _valueFactory) == null)
            {
                // If unable to initialize do nothing
                // This happens if
                // - initialization is done
                // - ReleaseValueFactory was called
                // Even if ReleaseValueFactory was called before initialization,
                // _target can still be set by SetTargetValue or the Value property
                return;
            }
            try
            {
                beforeInitialize.Invoke();
            }
            finally
            {
                InitializeExtracted();
            }
        }

        private void InitializeExtracted()
        {
            back:
            var valueFactory = Interlocked.Exchange(ref _valueFactory, null);
            if (valueFactory == null)
            {
                // Many threads may enter here
                // Prevent reentry
                if (_initializerThread == Thread.CurrentThread)
                {
                    throw new InvalidOperationException();
                }
                var waitHandle = _waitHandle.Value;
                // While _waitHandle.Value is not null it means that we have to wait initialization to complete
                if (waitHandle != null)
                {
                    try
                    {
                        // Another thread may have called ReleaseWaitHandle just before the next instruction
                        waitHandle.Wait();
                        // Finished waiting...
                        if (Volatile.Read(ref _valueFactory) != null)
                        {
                            // There was an error in the initialization, go back
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
                // Only one thread enters here
                _initializerThread = Thread.CurrentThread;
                try
                {
                    // Initialize from the value factory
                    SetTargetValue(valueFactory.Invoke());
                    // Initialization done, let any wating thread go
                    ReleaseWaitHandle();
                }
                catch (Exception exception)
                {
                    // There was an error during initialization
                    // Set error state
                    SetTargetError(exception);
                    // Restore the valueFactory
                    Interlocked.CompareExchange(ref _valueFactory, valueFactory, null);
                    // Let any waiting threads go, but don't get rid of the wait handle
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
                // If another thread is currently waiting, awake it
                waitHandle.Set();
                // If another thread is about to wait
                // Or if another thread started waiting just after the last instruction
                // let it throw
                waitHandle.Dispose();
            }
            _waitHandle.Value = null;
        }
    }
}

#endif