// Needed for NET35 (TASK)

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable RCS1231 // Make parameter ref read-only.

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public class RootedTimeout : IPromise
    {
        protected Action Callback;
        private const int _canceled = 4;
        private const int _canceling = 3;
        private const int _changing = 6;
        private const int _created = 0;
        private const int _executed = 2;
        private const int _executing = 1;
        private static int _lastRootIndex = -1;
        private static readonly Bucket<RootedTimeout> _root = new Bucket<RootedTimeout>();
        private readonly int _hashcode;
        private int _rootIndex = -1;
        private long _startTime;
        private int _status;
        private long _targetTime;
        private Timer _wrapped;

        protected RootedTimeout()
        {
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        ~RootedTimeout()
        {
            Close();
        }

        public bool IsCanceled => Volatile.Read(ref _status) == _canceled;

        public bool IsCompleted => Volatile.Read(ref _status) == _executed;

        Exception IPromise.Exception => null;

        bool IPromise.IsFaulted => false;

        public static RootedTimeout Launch(Action callback, long dueTime)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }

            var timeout = new RootedTimeout();
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
            return timeout;
        }

        public static RootedTimeout Launch(Action callback, long dueTime, CancellationToken token)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }

            var timeout = new RootedTimeout();
            if (token.IsCancellationRequested)
            {
                timeout._status = _canceled;
                return timeout;
            }

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
            return timeout;
        }

        public static RootedTimeout Launch(Action callback, TimeSpan dueTime)
        {
            return Launch(callback, (long)dueTime.TotalMilliseconds);
        }

        public static RootedTimeout Launch(Action callback, TimeSpan dueTime, CancellationToken token)
        {
            return Launch(callback, (long)dueTime.TotalMilliseconds, token);
        }

        public void Cancel()
        {
            if (Interlocked.CompareExchange(ref _status, _canceling, _created) != _created)
            {
                return;
            }

            Close();
            Volatile.Write(ref _status, _canceled);
        }

        public bool Change(long dueTime)
        {
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }

            if (Interlocked.CompareExchange(ref _status, _changing, _created) != _created)
            {
                return false;
            }

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
                wrapped.Change(Finish, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
            }

            Volatile.Write(ref _status, _created);
            return true;
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
            if (remaining > 0)
            {
                return remaining;
            }

            Finish();
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is RootedTimeout)
            {
                return this == obj;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _hashcode;
        }

        protected static void Root(RootedTimeout rootedTimeout)
        {
            rootedTimeout._rootIndex = Interlocked.Increment(ref _lastRootIndex);
            _root.Set(rootedTimeout._rootIndex, rootedTimeout);
        }

        protected static void UnRoot(RootedTimeout rootedTimeout)
        {
            var rootIndex = Interlocked.Exchange(ref rootedTimeout._rootIndex, -1);
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
            _targetTime = dueTime == -1 ? -1 : _startTime + dueTime;
            _wrapped = Timer.GetTimer(Finish, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
        }

        private void Close()
        {
            Timer.Donate(_wrapped);
            Volatile.Write(ref Callback, null);
            GC.SuppressFinalize(this);
        }

        private void Finish()
        {
            ThreadingHelper.SpinWaitWhile(ref _status, _changing);
            if (Interlocked.CompareExchange(ref _status, _executing, _created) != _created)
            {
                return;
            }

            var callback = Volatile.Read(ref Callback);
            if (callback == null)
            {
                return;
            }

            callback.Invoke();
            Close();
            Volatile.Write(ref _status, _executed);
        }
    }
}