#if FAT

using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class LazyNeedle<T> : Needle<T>, ICacheNeedle<T>, IEquatable<LazyNeedle<T>>, IPromise<T>
    {
        // TODO: Free and valueFactory?
        [NonSerialized]
        private Thread _initializerThread;

        private Func<T> _valueFactory;
        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public LazyNeedle(Func<T> valueFactory)
            : base(default(T))
        {
            _valueFactory = Check.NotNullArgument(valueFactory, "valueFactory");
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
        }

        public LazyNeedle(Func<T> valueFactory, bool cacheExceptions)
            : base(default(T))
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

        public LazyNeedle(T target)
            : base(target)
        {
            _valueFactory = null;
            _waitHandle = null;
        }

        public LazyNeedle()
            : base(default(T))
        {
            _valueFactory = null;
            _waitHandle = null;
        }

        ~LazyNeedle()
        {
            ReleaseWaitHandle();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool IExpected.IsCanceled
        {
            get
            {
                return false;
            }
        }

        bool IExpected.IsFaulted
        {
            get
            {
                return IsFaulted;
            }
        }

        Exception IPromise.Error
        {
            get
            {
                return Error;
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
                _valueFactory = null;
                ReleaseWaitHandle();
            }
        }

        protected INeedle<ManualResetEventSlim> WaitHandle
        {
            get
            {
                return _waitHandle;
            }
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as LazyNeedle<T>;
            return !ReferenceEquals(null, _obj) && base.Equals(obj);
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
            InitializeExtracted();
        }

        public bool TryGet(out T target)
        {
            var result = IsCompleted;
            target = base.Value;
            return result;
        }

        public void Wait()
        {
            if (_initializerThread == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }
            else
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
                else
                {
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
                            else
                            {
                                ReleaseWaitHandle();
                            }
                        }
                        catch (ObjectDisposedException exception)
                        {
                            // Came late to the party, initialization is done
                            GC.KeepAlive(exception);
                        }
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

#endif