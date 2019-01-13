#if TARGETS_NETSTANDARD

using System.Threading.Tasks;
using Theraot;
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

    public class Thread
    {
        [ThreadStatic]
        private static Thread _currentThread;

        private static int _lastId;

        [ThreadStatic]
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
                    No.Op(exception);
                }
            }
        }

        public static Thread CurrentThread => _currentThread ?? (_currentThread = new Thread());

        public bool IsAlive => _start == null && _probe.TryGetTarget(out _) || _task != null && !_task.IsCompleted;

        public bool IsBackground
        {
            get => _start == null;
            set => No.Op(value);
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

                    default:
                        return ThreadState.Running;
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

        public static void SpinWait(int iterations)
        {
            for (var index = 0; index < iterations; index++)
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
            No.Op(stateInfo);
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
            if (_start == null)
            {
                if (_probe.TryGetTarget(out _))
                {
                    Wait();
                }
                return;
            }
            if (_task == null)
            {
                throw new ThreadStateException("Unable to Join not started Thread.");
            }
            switch (_task.Status)
            {
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                case TaskStatus.RanToCompletion:
                    return;

                default:
                    _task.Wait();
                    return;
            }
            void Wait()
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
                throw new ThreadStateException($"Unable to Start started Thread (Internal Task: {_task}) (Internal Delegate: {_start}).");
            }
            var task = new Task(() => _start(null), TaskCreationOptions.LongRunning);
            task.Start();
            _task = task;
        }

        public void Start(object parameter)
        {
            if (_start == null || _task != null)
            {
                throw new ThreadStateException($"Unable to Start started Thread (Internal Task: {_task}) (Internal Delegate: {_start}).");
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