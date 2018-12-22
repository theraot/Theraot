#if NETSTANDARD1_5 || NETSTANDARD1_6

using System.Threading.Tasks;
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

    public sealed class LocalDataStoreSlot
    {
        internal TrackingThreadLocal<object> ThreadLocal;

        internal LocalDataStoreSlot(TrackingThreadLocal<object> threadLocal)
        {
            ThreadLocal = threadLocal;
        }
    }

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

    [Runtime.InteropServices.ComVisible(false)]
    public delegate void ParameterizedThreadStart(object obj);

    [Runtime.InteropServices.ComVisible(true)]
    public delegate void ThreadStart();

    public class Thread
    {
        [ThreadStatic]
        private static Thread _currentThread;
        private static int _lastId;
        private static readonly SafeCollection<LocalDataStoreSlot> _dataStoreSlots = new SafeCollection<LocalDataStoreSlot>();
        private static readonly SafeDictionary<string, LocalDataStoreSlot> _namedDataStoreSlots = new SafeDictionary<string, LocalDataStoreSlot>();

        private Task _task;

        private readonly ParameterizedThreadStart _start;

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

        private Thread()
        {
            _task = null;
        }

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

        public Thread(ParameterizedThreadStart start, int maxStackSize)
            :this (start)
        {
            GC.KeepAlive(maxStackSize);
        }

        public Thread(ThreadStart start, int maxStackSize)
            :this (start)
        {
            GC.KeepAlive(maxStackSize);
        }

        public bool IsAlive => _start == null || _task != null && !_task.IsCompleted;

        public bool IsBackground
        {
            get => _start == null;
            set => GC.KeepAlive(value);
        }

        public bool IsThreadPoolThread => _start == null;

        public int ManagedThreadId { get; }

        public string Name { get; set; }

        public ThreadState ThreadState
        {
            get
            {
                if (_start == null)
                {
                    return ThreadState.Background;
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

        public static void Sleep(int millisecondsTimeout)
        {
            Task.Delay(millisecondsTimeout).Wait();
        }

        public static void Sleep(TimeSpan timeout)
        {
            Task.Delay(timeout).Wait();
        }

        public void Abort()
        {
            throw new PlatformNotSupportedException();
        }

        public void Abort(object stateInfo)
        {
            throw new PlatformNotSupportedException();
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

        public static Thread CurrentThread => _currentThread ?? (_currentThread = new Thread());

        public static object GetData(LocalDataStoreSlot slot)
        {
            return slot.ThreadLocal.Value;
        }

        public static void SetData(LocalDataStoreSlot slot, object data)
        {
            slot.ThreadLocal.Value = data;
        }

        public static void SpinWait(int iterations)
        {
            for (int index = 0; index < iterations; index++)
            {
                GC.KeepAlive(index);
            }
        }

        public void Start()
        {
            if (_start == null)
            {
                throw new ThreadStateException();
            }
            try
            {
                _task = new Task(() => _start(null));
            }
            catch (InvalidOperationException)
            {
                throw new ThreadStateException();
            }
        }

        public void Start (object parameter)
        {
            if (_task == null)
            {
                throw new ThreadStateException();
            }
            try
            {
                _task.Start();
            }
            catch (InvalidOperationException)
            {
                throw new ThreadStateException();
            }
        }
    }
}

#endif