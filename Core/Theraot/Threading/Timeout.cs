// Needed for NET35 (TASK)

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public class Timeout : IPromise
    {
        private const int _canceled = 4;
        private const int _canceling = 3;
        private const int _created = 0;
        private const int _executed = 2;
        private const int _executing = 1;
        private const int _changing = 6;
        private static readonly Bucket<Timeout> _root = new Bucket<Timeout>();
        private static int _lastRootIndex = -1;
        private readonly int _hashcode;
        private Action _callback;
        private int _rootIndex = -1;
        private long _startTime;
        private int _status;
        private long _targetTime;
        private Timer _wrapped;

        public Timeout(Action callback, long dueTime)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            _callback = callback;
            Start(dueTime);
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        public Timeout(Action callback, long dueTime, CancellationToken token)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            if (token.IsCancellationRequested)
            {
                _callback = null;
                _wrapped = null;
                _status = _canceled;
            }
            else
            {
                _callback = callback;
                Start(dueTime);
                token.Register(Cancel);
            }
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        public Timeout(Action callback, TimeSpan dueTime)
            : this(callback, (long)dueTime.TotalMilliseconds)
        {
            // Empty
        }

        public Timeout(Action callback, TimeSpan dueTime, CancellationToken token)
            : this(callback, (long)dueTime.TotalMilliseconds, token)
        {
            // Empty
        }

        private Timeout()
        {
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        ~Timeout()
        {
            Close();
        }

        Exception IPromise.Exception
        {
            get
            {
                return null;
            }
        }

        public bool IsCanceled
        {
            get
            {
                return Volatile.Read(ref _status) == _canceled;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Volatile.Read(ref _status) == _executed;
            }
        }

        bool IPromise.IsFaulted
        {
            get
            {
                return false;
            }
        }

        public static void Launch(Action callback, long dueTime)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            var timeout = new Timeout();
            timeout._callback = () =>
            {
                try
                {
                    callback();
                }
                finally
                {
                    UnRoot(timeout);
                }
            };
            timeout.Start(dueTime);
            Root(timeout);
        }

        public static void Launch(Action callback, long dueTime, CancellationToken token)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            if (token.IsCancellationRequested)
            {
                return;
            }
            var timeout = new Timeout();
            timeout._callback = () =>
            {
                try
                {
                    callback();
                }
                finally
                {
                    UnRoot(timeout);
                }
            };
            timeout.Start(dueTime);
            token.Register(timeout.Cancel);
            Root(timeout);
        }

        public static void Launch(Action callback, TimeSpan dueTime)
        {
            Launch(callback, (long)dueTime.TotalMilliseconds);
        }

        public static void Launch(Action callback, TimeSpan dueTime, CancellationToken token)
        {
            Launch(callback, (long)dueTime.TotalMilliseconds, token);
        }

        public void Cancel()
        {
            if (Interlocked.CompareExchange(ref _status, _canceling, _created) == _created)
            {
                Close();
                Volatile.Write(ref _status, _canceled);
            }
        }

        public bool Change(long dueTime)
        {
            if (Interlocked.CompareExchange(ref _status, _changing, _created) == _created)
            {
                _startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
                _targetTime = _startTime + dueTime;
                var wrapped = Interlocked.CompareExchange(ref _wrapped, null, null);
                if (wrapped == null)
                {
                    return false;
                }
                wrapped.Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
                Volatile.Write(ref _status, _created);
                return true;
            }
            return false;
        }

        public void Change(TimeSpan dueTime)
        {
            Change((long)dueTime.TotalMilliseconds);
        }

        public long CheckRemaining()
        {
            var remaining = _targetTime - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
            if (remaining <= 0)
            {
                Finish(null);
                return 0;
            }
            return remaining;
        }

        public override bool Equals(object obj)
        {
            if (obj is Timeout)
            {
                return this == obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _hashcode;
        }

        private static void Root(Timeout timeout)
        {
            timeout._rootIndex = Interlocked.Increment(ref _lastRootIndex);
            _root.Set(timeout._rootIndex, timeout);
        }

        private static void UnRoot(Timeout timeout)
        {
            var rootIndex = Interlocked.Exchange(ref timeout._rootIndex, -1);
            if (rootIndex != -1)
            {
                _root.RemoveAt(rootIndex);
            }
        }

        private void Close()
        {
            var wrapped = Interlocked.Exchange(ref _wrapped, null);
            if (wrapped != null)
            {
                wrapped.Dispose();
            }
            Volatile.Write(ref _callback, null);
            GC.SuppressFinalize(this);
        }

        private void Finish(object state)
        {
            GC.KeepAlive(state);
            ThreadingHelper.SpinWaitWhile(ref _status, _changing);
            if (Interlocked.CompareExchange(ref _status, _executing, _created) == _created)
            {
                var callback = Volatile.Read(ref _callback);
                if (callback != null)
                {
                    callback.Invoke();
                    Close();
                    Volatile.Write(ref _status, _executed);
                }
            }
        }

        private void Start(long dueTime)
        {
            _startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
            _targetTime = _startTime + dueTime;
            _wrapped = new Timer(Finish, null, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
        }
    }
}