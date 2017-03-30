// Needed for NET35 (TASK)

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public class Timeout : IPromise
    {
        private static readonly HashSet<Timeout> _root;

        private readonly int _hashcode;
        private Action _callback;
        private int _completed;
        private int _executed;
        private bool _rooted;
        private long _start;
        private long _targetTime;
        private Timer _wrapped;

        static Timeout()
        {
            _root = new HashSet<Timeout>();
        }

        public Timeout(Action callback, long dueTime)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            _callback = callback;
            Initialize(dueTime);
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        public Timeout(Action callback, long dueTime, CancellationToken token)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            _start = ThreadingHelper.TicksNow();
            if (token.IsCancellationRequested)
            {
                _callback = null;
                _wrapped = null;
            }
            else
            {
                _callback = callback;
                Initialize(dueTime);
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

        ~Timeout()
        {
            Cancel();
        }

        Exception IPromise.Exception
        {
            get
            {
                return null;
            }
        }

        bool IPromise.IsFaulted
        {
            get
            {
                return false;
            }
        }

        public bool IsCanceled
        {
            get
            {
                return Volatile.Read(ref _completed) == 0 && _wrapped == null;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Volatile.Read(ref _completed) == 1;
            }
        }

        public bool Rooted
        {
            get
            {
                return _rooted;
            }
            set
            {
                if (value == _rooted)
                {
                    return;
                }
                if (value)
                {
                    _root.Add(this);
                }
                else
                {
                    _root.Remove(this);
                }
                _rooted = value;
            }
        }

        public void Cancel()
        {
            var wrapped = Interlocked.Exchange(ref _wrapped, null);
            if (wrapped != null)
            {
                wrapped.Dispose();
            }
            _callback = null;
            _root.Remove(this);
            GC.SuppressFinalize(this);
        }

        public void Change(long dueTime)
        {
            Initialize(dueTime);
        }

        public void Change(TimeSpan dueTime)
        {
            Initialize((long)dueTime.TotalMilliseconds);
        }

        public long CheckRemaining()
        {
            var remaining = _targetTime - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
            if (remaining <= 0)
            {
                Callback(null);
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

        private void Callback(object state)
        {
            if (Interlocked.CompareExchange(ref _executed, 1, 0) == 0)
            {
                GC.KeepAlive(state);
                _callback.Invoke();
                Cancel();
                Volatile.Write(ref _completed, 1);
            }
        }

        private void Initialize(long dueTime)
        {
            if (Volatile.Read(ref _executed) == 1)
            {
                ThreadingHelper.SpinWaitWhile(ref _completed, 1);
                Volatile.Write(ref _executed, 0);
            }
            _start = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow());
            _targetTime = _start + dueTime;
            if (_wrapped == null)
            {
                _wrapped = new Timer(Callback, null, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
            }
            else
            {
                _wrapped.Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(-1));
            }
        }
    }
}