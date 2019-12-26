#if LESSTHAN_NET40

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Core;
using Theraot.Reflection;

namespace System
{
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    [Serializable]
    public class Lazy<T>
    {
        private int _isValueCreated;
        private Func<T> _valueFactory;

        public Lazy()
            : this(LazyThreadSafetyMode.ExecutionAndPublication)
        {
            // Empty
        }

        public Lazy(Func<T> valueFactory)
            : this(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication)
        {
            // Empty
        }

        public Lazy(bool isThreadSafe)
            : this(isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None)
        {
            // Empty
        }

        public Lazy(LazyThreadSafetyMode mode)
            : this(ConstructorHelper.Create<T>, mode, false)
        {
            // Empty
        }

        public Lazy(Func<T> valueFactory, bool isThreadSafe)
            : this(valueFactory, isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None)
        {
            // Empty
        }

        public Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
            : this(valueFactory, mode, true)
        {
            // Empty
        }

        private Lazy(Func<T> valueFactory, LazyThreadSafetyMode mode, bool cacheExceptions)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            switch (mode)
            {
                case LazyThreadSafetyMode.None:
                    if (cacheExceptions)
                    {
                        var threads = new HashSet<Thread>();
                        _valueFactory =
                            () => CachingNoneMode(threads);
                    }
                    else
                    {
                        var threads = new HashSet<Thread>();
                        _valueFactory =
                            () => NoneMode(threads);
                    }

                    break;

                case LazyThreadSafetyMode.PublicationOnly:
                    _valueFactory = PublicationOnlyMode;
                    break;

                default: /*LazyThreadSafetyMode.ExecutionAndPublication*/
                    if (cacheExceptions)
                    {
                        Thread? thread = null;
                        ManualResetEvent? manualResetEvent = null;
                        _valueFactory =
                            () => CachingFullMode(valueFactory, ref manualResetEvent, ref thread);
                    }
                    else
                    {
                        Thread? thread = null;
                        ManualResetEvent? manualResetEvent = null;
                        _valueFactory =
                            () => FullMode(valueFactory, ref manualResetEvent, ref thread);
                    }

                    break;
            }

            T CachingNoneMode(HashSet<Thread> threads)
            {
                if (Volatile.Read(ref _isValueCreated) != 0)
                {
                    return _valueFactory.Invoke();
                }

                try
                {
                    AddThread(threads);
                    ValueForDebugDisplay = valueFactory();
                    _valueFactory = FuncHelper.GetReturnFunc(ValueForDebugDisplay);
                    Volatile.Write(ref _isValueCreated, 1);
                    return ValueForDebugDisplay;
                }
                catch (Exception exception)
                {
                    _valueFactory = FuncHelper.GetThrowFunc<T>(exception);
                    throw;
                }
                finally
                {
                    RemoveThread(threads);
                }
            }

            T NoneMode(HashSet<Thread> threads)
            {
                if (Volatile.Read(ref _isValueCreated) != 0)
                {
                    return _valueFactory.Invoke();
                }

                try
                {
                    AddThread(threads);
                    ValueForDebugDisplay = valueFactory();
                    _valueFactory = FuncHelper.GetReturnFunc(ValueForDebugDisplay);
                    Volatile.Write(ref _isValueCreated, 1);
                    return ValueForDebugDisplay;
                }
                catch (Exception)
                {
                    Volatile.Write(ref _isValueCreated, 0);
                    throw;
                }
                finally
                {
                    RemoveThread(threads);
                }
            }

            T PublicationOnlyMode()
            {
                ValueForDebugDisplay = valueFactory();
                if (Interlocked.CompareExchange(ref _isValueCreated, 1, 0) == 0)
                {
                    _valueFactory = FuncHelper.GetReturnFunc(ValueForDebugDisplay);
                }

                return ValueForDebugDisplay;
            }
        }

        public bool IsValueCreated => _isValueCreated == 1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value => _valueFactory.Invoke();

        internal T ValueForDebugDisplay { get; private set; } = default!;

        private static void AddThread(HashSet<Thread> threads)
        {
            // lock (threads) // This is meant to not be thread-safe
            var currentThread = Thread.CurrentThread;
            if (threads.Contains(currentThread))
            {
                throw new InvalidOperationException();
            }

            threads.Add(currentThread);
        }

        private static void RemoveThread(HashSet<Thread> threads)
        {
            // lock (threads) // This is meant to not be thread-safe
            threads.Remove(Thread.CurrentThread);
        }

        private T CachingFullMode(Func<T> valueFactory, ref ManualResetEvent? waitHandle, ref Thread? thread)
        {
            if (waitHandle == null)
            {
                waitHandle = new ManualResetEvent(false);
            }

            if (Interlocked.CompareExchange(ref _isValueCreated, 1, 0) == 0)
            {
                Volatile.Write(ref thread, Thread.CurrentThread);
                try
                {
                    ValueForDebugDisplay = valueFactory.Invoke();
                    _valueFactory = FuncHelper.GetReturnFunc(ValueForDebugDisplay);
                    return ValueForDebugDisplay;
                }
                catch (Exception exc)
                {
                    _valueFactory = FuncHelper.GetThrowFunc<T>(exc);
                    throw;
                }
                finally
                {
                    Volatile.Write(ref thread, null);
                    waitHandle.Set();
                    waitHandle.Close();
                }
            }

            if (Volatile.Read(ref thread) == Thread.CurrentThread)
            {
                throw new InvalidOperationException();
            }

            if (waitHandle.SafeWaitHandle.IsClosed)
            {
                return _valueFactory.Invoke();
            }

            try
            {
                waitHandle.WaitOne();
            }
            catch (ObjectDisposedException exception)
            {
                _ = exception;
            }

            return _valueFactory.Invoke();
        }

        private T FullMode(Func<T> valueFactory, ref ManualResetEvent? waitHandle, ref Thread? thread)
        {
            if (waitHandle == null)
            {
                waitHandle = new ManualResetEvent(false);
            }

            while (Volatile.Read(ref _isValueCreated) != 1)
            {
                var foundThread = Interlocked.CompareExchange(ref thread, Thread.CurrentThread, null);
                if (foundThread == null)
                {
                    try
                    {
                        ValueForDebugDisplay = valueFactory.Invoke();
                        _valueFactory = FuncHelper.GetReturnFunc(ValueForDebugDisplay);
                        Volatile.Write(ref _isValueCreated, 1);
                        return ValueForDebugDisplay;
                    }
                    finally
                    {
                        Volatile.Write(ref thread, null);
                        waitHandle.Set();
                        if (Volatile.Read(ref _isValueCreated) == 1)
                        {
                            waitHandle.Close();
                        }
                    }
                }

                if (foundThread == Thread.CurrentThread)
                {
                    throw new InvalidOperationException();
                }

                if (waitHandle.SafeWaitHandle.IsClosed)
                {
                    continue;
                }

                try
                {
                    waitHandle.WaitOne();
                }
                catch (ObjectDisposedException exception)
                {
                    _ = exception;
                }
            }

            return _valueFactory.Invoke();
        }
    }

    public class Lazy<T, TMetadata> : Lazy<T>
    {
        public Lazy(TMetadata metadata)
        {
            Metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata)
            : base(valueFactory)
        {
            Metadata = metadata;
        }

        public Lazy(TMetadata metadata, bool isThreadSafe)
            : base(isThreadSafe)
        {
            Metadata = metadata;
        }

        public Lazy(TMetadata metadata, LazyThreadSafetyMode mode)
            : base(mode)
        {
            Metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata, bool isThreadSafe)
            : base(valueFactory, isThreadSafe)
        {
            Metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata, LazyThreadSafetyMode mode)
            : base(valueFactory, mode)
        {
            Metadata = metadata;
        }

        public TMetadata Metadata { get; }
    }
}

#endif