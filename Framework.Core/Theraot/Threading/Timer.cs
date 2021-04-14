// Needed for NET35 (TASK)

#pragma warning disable CA2213 // Disposable fields should be disposed

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    internal sealed class Timer : IDisposable
    {
        private static readonly Pool<Timer> _pool = new(64, time => time.Stop());

        private Action? _callback;
        private System.Threading.Timer? _timer;

        private Timer(Action callback, TimeSpan dueTime, TimeSpan period)
        {
            _callback = callback;
            _timer = new System.Threading.Timer(Callback, state: null, dueTime, period);
        }

        public static void Donate(ref Timer? timer)
        {
            _pool.Donate(Interlocked.Exchange(ref timer, value: null));
        }

        public static Timer GetTimer(Action callback, TimeSpan dueTime, TimeSpan period)
        {
            if (_pool.TryGet(out var timer))
            {
                timer.Change(callback, dueTime, period);
            }
            else
            {
                timer = new Timer(callback, dueTime, period);
            }

            return timer;
        }

        public void Change(Action callback, TimeSpan dueTime, TimeSpan period)
        {
            var timer = _timer;
            if (timer == null)
            {
                throw new ObjectDisposedException(nameof(Timer));
            }

            timer.Change(dueTime, period);
            _callback = callback;
        }

        void IDisposable.Dispose()
        {
            Interlocked.Exchange(ref _timer, value: null)?.Dispose();
            _callback = null;
        }

        public void Stop()
        {
            var timer = _timer;
            if (timer == null)
            {
                throw new ObjectDisposedException(nameof(Timer));
            }

            timer.Change(Timeout.Infinite, Timeout.Infinite);
            _callback = null;
        }

        private void Callback(object? state)
        {
            _callback?.Invoke();
        }
    }
}