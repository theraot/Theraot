// Needed for NET35 (TASK)

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public class Timeout : IPromise
    {
        protected Action Callback;
        private const int _canceled = 4;
        private const int _canceling = 3;
        private const int _changing = 6;
        private const int _created = 0;
        private const int _executed = 2;
        private const int _executing = 1;
        private static readonly Bucket<Timeout> _root = new Bucket<Timeout>();
        private static int _lastRootIndex = -1;
        private readonly int _hashcode;
        private int _rootIndex = -1;
        private long _startTime;
        private int _status;
        private long _targetTime;
        private Timer _wrapped;

        public Timeout(Action callback, long dueTime)
        {
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Start(dueTime);
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        public Timeout(Action callback, long dueTime, CancellationToken token)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            if (token.IsCancellationRequested)
            {
                Callback = null;
                _wrapped = null;
                _status = _canceled;
            }
            else
            {
                Callback = callback;
                token.Register(Cancel);
                Start(dueTime);
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

        protected Timeout()
        {
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        ~Timeout()
        {
            Close();
        }

        Exception IPromise.Exception => null;

        public bool IsCanceled => Volatile.Read(ref _status) == _canceled;

        public bool IsCompleted => Volatile.Read(ref _status) == _executed;

        bool IPromise.IsFaulted => false;

        public static void Launch(Action callback, long dueTime)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            var timeout = new Timeout();
            timeout.Callback = () =>
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
            Root(timeout);
            timeout.Start(dueTime);
        }

        public static void Launch(Action callback, long dueTime, CancellationToken token)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            if (token.IsCancellationRequested)
            {
                return;
            }
            var timeout = new Timeout();
            timeout.Callback = () =>
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
            token.Register(timeout.Cancel);
            Root(timeout);
            timeout.Start(dueTime);
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
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            if (Interlocked.CompareExchange(ref _status, _changing, _created) == _created)
            {
                _startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
                var wrapped = Interlocked.CompareExchange(ref _wrapped, null, null);
                if (wrapped == null)
                {
                    return false;
                }
                if (dueTime == -1)
                {
                    _targetTime = -1;
                }
                else
                {
                    _targetTime = _startTime + dueTime;
                    wrapped.Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
                }
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
            if (_targetTime == -1)
            {
                return -1;
            }
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

        protected static void Root(Timeout timeout)
        {
            timeout._rootIndex = Interlocked.Increment(ref _lastRootIndex);
            _root.Set(timeout._rootIndex, timeout);
        }

        protected static void UnRoot(Timeout timeout)
        {
            var rootIndex = Interlocked.Exchange(ref timeout._rootIndex, -1);
            if (rootIndex != -1)
            {
                _root.RemoveAt(rootIndex);
            }
        }

        protected void Start(long dueTime)
        {
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            _startTime = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
            if (dueTime == -1)
            {
                _targetTime = -1;
            }
            else
            {
                _targetTime = _startTime + dueTime;
            }
            _wrapped = new Timer(Finish, null, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
        }

        private void Close()
        {
            var wrapped = Interlocked.Exchange(ref _wrapped, null);
            wrapped?.Dispose();
            Volatile.Write(ref Callback, null);
            GC.SuppressFinalize(this);
        }

        private void Finish(object state)
        {
            GC.KeepAlive(state);
            ThreadingHelper.SpinWaitWhile(ref _status, _changing);
            if (Interlocked.CompareExchange(ref _status, _executing, _created) == _created)
            {
                var callback = Volatile.Read(ref Callback);
                if (callback != null)
                {
                    callback.Invoke();
                    Close();
                    Volatile.Write(ref _status, _executed);
                }
            }
        }
    }

    public class Timeout<T> : Timeout
    {
        public Timeout(Action<T> callback, long dueTime, T target)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            Callback = new ValueActionClosure<T>(callback, target).Invoke;
            Start(dueTime);
        }

        public Timeout(Action<T> callback, long dueTime, CancellationToken token, T target)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            if (token.IsCancellationRequested)
            {
                Callback = null;
                Cancel();
            }
            else
            {
                Callback = new ValueActionClosure<T>(callback, target).Invoke;
                Start(dueTime);
                token.Register(Cancel);
            }
        }

        public Timeout(Action<T> callback, TimeSpan dueTime, T target)
            : this(callback, (long)dueTime.TotalMilliseconds, target)
        {
            // Empty
        }

        public Timeout(Action<T> callback, TimeSpan dueTime, CancellationToken token, T target)
            : this(callback, (long)dueTime.TotalMilliseconds, token, target)
        {
            // Empty
        }

        private Timeout()
        {
            // Empty
        }

        public static void Launch(Action<T> callback, long dueTime, T target)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            var timeout = new Timeout<T>();
            timeout.Callback = () =>
            {
                try
                {
                    callback(target);
                }
                finally
                {
                    UnRoot(timeout);
                }
            };
            timeout.Start(dueTime);
            Root(timeout);
        }

        public static void Launch(Action<T> callback, long dueTime, CancellationToken token, T target)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            if (token.IsCancellationRequested)
            {
                return;
            }
            var timeout = new Timeout<T>();
            timeout.Callback = () =>
            {
                try
                {
                    callback(target);
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

        public static void Launch(Action<T> callback, TimeSpan dueTime, T target)
        {
            Launch(callback, (long)dueTime.TotalMilliseconds, target);
        }

        public static void Launch(Action<T> callback, TimeSpan dueTime, CancellationToken token, T target)
        {
            Launch(callback, (long)dueTime.TotalMilliseconds, token, target);
        }
    }
}