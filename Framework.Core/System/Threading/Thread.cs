#if NETSTANDARD1_5 || NETSTANDARD1_6

using System.Threading.Tasks;
using Theraot;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace System
{
    public class SystemException : Exception
    {
        public SystemException()
        {
        }

        public SystemException(string message) : base(message)
        {
        }

        public SystemException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(false)]
    public delegate void ParameterizedThreadStart(object obj);

    [Runtime.InteropServices.ComVisible(true)]
    public delegate void ThreadStart();

    [Flags]
    [Runtime.InteropServices.ComVisible(true)]
    public enum ThreadState
    {
        Running = 0,
        StopRequested = 1,
        SuspendRequested = 2,
        Background = 4,
        Unstarted = 8,
        Stopped = 16,
        WaitSleepJoin = 32,
        Suspended = 64,
        AbortRequested = 128,
        Aborted = 256
    }

    public sealed class LocalDataStoreSlot
    {
        internal TrackingThreadLocal<object> ThreadLocal;

        internal LocalDataStoreSlot(TrackingThreadLocal<object> threadLocal)
        {
            ThreadLocal = threadLocal;
        }
    }

    public class Thread
    {
        private static readonly SafeCollection<LocalDataStoreSlot> _dataStoreSlots = new SafeCollection<LocalDataStoreSlot>();

        private static readonly SafeDictionary<string, LocalDataStoreSlot> _namedDataStoreSlots = new SafeDictionary<string, LocalDataStoreSlot>();

        [ThreadStatic]
        private static Thread _currentThread;

        private static int _lastId;

        [ThreadStatic]
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static object _threadProbe;

        private readonly WeakReference<object> _probe;
        private readonly ParameterizedThreadStart _start;
        private string _name;
        private Task _task;

        public Thread(ParameterizedThreadStart start)
        {
            _start = start == null
                ? throw new ArgumentNullException(nameof(start))
                : new ParameterizedThreadStart
                (
                    state =>
                    {
                        _currentThread = this;
                        start(state);
                    }
                );
            ManagedThreadId = Interlocked.Increment(ref _lastId);
        }

        public Thread(ThreadStart start)
        {
            _start = start == null
                ? throw new ArgumentNullException(nameof(start))
                : new ParameterizedThreadStart
                (
                    _ =>
                    {
                        _currentThread = this;
                        start();
                    }
                );
            ManagedThreadId = Interlocked.Increment(ref _lastId);
        }

        private Thread()
        {
            _start = null;
            _task = null;
            _currentThread = this;
            _threadProbe = new object();
            _probe = new WeakReference<object>(_threadProbe);
        }

        ~Thread()
        {
            try
            {
                // Empty
            }
            finally
            {
                try
                {
                    var task = Volatile.Read(ref _task);
                    if (task != null && !task.IsCompleted)
                    {
                        GC.ReRegisterForFinalize(this);
                    }
                }
                catch (Exception exception)
                {
                    // Catch them all - there shouldn't be exceptions here, yet we really don't want them
                    GC.KeepAlive(exception);
                }
            }
        }

        public static Thread CurrentThread => _currentThread ?? (_currentThread = new Thread());

        public bool IsAlive => _start == null && _probe.TryGetTarget(out _) || _task != null && !_task.IsCompleted;

        public bool IsBackground
        {
            get => _start == null;
            set => GC.KeepAlive(value);
        }

        public bool IsThreadPoolThread => _start == null;

        public int ManagedThreadId { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != null)
                {
                    throw new InvalidOperationException();
                }
                _name = value;
            }
        }

        public ThreadState ThreadState
        {
            get
            {
                if (_start == null)
                {
                    if (_probe.TryGetTarget(out _))
                    {
                        return ThreadState.Background;
                    }
                    return ThreadState.Stopped;
                }
                if (_task == null)
                {
                    return ThreadState.Unstarted;
                }
                switch (_task.Status)
                {
                    case TaskStatus.Canceled:
                    case TaskStatus.Faulted:
                        return ThreadState.Aborted;

                    case TaskStatus.RanToCompletion:
                        return ThreadState.Stopped;

                    case TaskStatus.Running:
                        return ThreadState.Running;

                    default:
                        return ThreadState.Unstarted;
                }
            }
        }

        public static LocalDataStoreSlot AllocateDataSlot()
        {
            var slot = new LocalDataStoreSlot
            (
                new TrackingThreadLocal<object>
                (
                    FuncHelper.GetDefaultFunc<object>()
                )
            );
            _dataStoreSlots.Add(slot);
            return slot;
        }

        public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
        {
            var slot = new LocalDataStoreSlot
            (
                new TrackingThreadLocal<object>
                (
                    FuncHelper.GetDefaultFunc<object>()
                )
            );
            _namedDataStoreSlots.AddNew(name, slot);
            return slot;
        }

        public static void FreeNamedDataSlot(string name)
        {
            _namedDataStoreSlots.Remove(name);
        }

        public static object GetData(LocalDataStoreSlot slot)
        {
            return slot.ThreadLocal.Value;
        }

        public static void SetData(LocalDataStoreSlot slot, object data)
        {
            slot.ThreadLocal.Value = data;
        }

        public static void Sleep(int millisecondsTimeout)
        {
            Task.Delay(millisecondsTimeout).Wait();
        }

        public static void Sleep(TimeSpan timeout)
        {
            Task.Delay(timeout).Wait();
        }

        public static void SpinWait(int iterations)
        {
            for (int index = 0; index < iterations; index++)
            {
                GC.KeepAlive(index);
            }
        }

        public void Abort()
        {
            throw new PlatformNotSupportedException();
        }

        public void Abort(object stateInfo)
        {
            throw new PlatformNotSupportedException();
        }

        public void Join()
        {
            if (this == CurrentThread)
            {
                // This is by definition a DeadLock
                var source = new TaskCompletionSource<VoidStruct>();
                source.Task.Wait();
            }
            if (_task != null)
            {
                _task.Wait();
            }
            else
            {
                var source = new TaskCompletionSource<VoidStruct>();
                var probe = _probe;
                GCMonitor.Collected += Handler;
                source.Task.Wait();
                void Handler(object sender, EventArgs args)
                {
                    if (!probe.TryGetTarget(out _))
                    {
                        source.SetResult(default);
                        GCMonitor.Collected -= Handler;
                    }
                }
            }
        }

        public void Start()
        {
            if (_start == null || _task != null)
            {
                throw new ThreadStateException();
            }
            var task = new Task(() => _start(null), TaskCreationOptions.LongRunning);
            task.Start();
            _task = task;
        }

        public void Start(object parameter)
        {
            if (_start == null || _task != null)
            {
                throw new ThreadStateException();
            }
            var task = new Task(() => _start(parameter), TaskCreationOptions.LongRunning);
            task.Start();
            _task = task;
        }
    }

    [Runtime.InteropServices.ComVisible(true)]
    public class ThreadStateException : SystemException
    {
        public ThreadStateException()
        {
        }

        public ThreadStateException(string message) : base(message)
        {
        }

        public ThreadStateException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

#endif