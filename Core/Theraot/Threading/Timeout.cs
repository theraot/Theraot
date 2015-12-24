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
        private bool _completed;
        private bool _rooted;
        private Timer _wrapped;

        static Timeout()
        {
            _root = new HashSet<Timeout>();
        }

        public Timeout(Action callback, long dueTime)
        {
            if (callback == null)
            {
                throw new NullReferenceException("callback");
            }
            _callback = callback;
            _wrapped = new Timer(Callback, null, dueTime, System.Threading.Timeout.Infinite);
            _hashcode = unchecked((int)DateTime.Now.Ticks);
        }

        public Timeout(Action callback, long dueTime, CancellationToken token)
        {
            if (callback == null)
            {
                throw new NullReferenceException("callback");
            }
            if (token.IsCancellationRequested)
            {
                _callback = null;
                _wrapped = null;
            }
            else
            {
                _callback = callback;
                _wrapped = new Timer(Callback, null, dueTime, System.Threading.Timeout.Infinite);
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
                return _wrapped == null;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _completed;
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

        public void Change(long dueTime)
        {
            _wrapped.Change(dueTime, System.Threading.Timeout.Infinite);
        }

        public void Change(TimeSpan dueTime)
        {
            Change((long)dueTime.TotalMilliseconds);
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

        public override bool Equals(object obj)
        {
            if (obj is Timeout)
            {
                return ReferenceEquals(this, obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _hashcode;
        }

        private void Callback(object state)
        {
            GC.KeepAlive(state);
            _callback.Invoke();
            _completed = true;
            Cancel();
        }
    }
}