#if NETSTANDARD1_0 || NETSTANDARD1_1

using System.Threading.Tasks;

namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(true)]
    public delegate void TimerCallback(object state);

    public sealed class Timer : IDisposable
    {
        private TimerCallback _callback;
        private CancellationTokenSource _changeSource;
        private object _state;
        private Task _task;

        public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            _callback = callback;
            _state = state;
            Change(dueTime, period);
        }

        public Timer(TimerCallback callback, object state, int dueTime, int period)
        {
            _callback = callback;
            _state = state;
            Change(dueTime, period);
        }

        public void Change(TimeSpan dueTime, TimeSpan period)
        {
            if (_callback == null)
            {
                throw new ObjectDisposedException(nameof(Timer));
            }
            Stop();
            Task.Factory.StartNew(Function, TaskCreationOptions.LongRunning).Unwrap();
            async Task Function()
            {
                await Task.Delay(dueTime, _changeSource.Token).ConfigureAwait(false);
                if (_changeSource.IsCancellationRequested)
                {
                    return;
                }

                _callback(_state);
                if (_changeSource.IsCancellationRequested)
                {
                    return;
                }

                while (true)
                {
                    await Task.Delay(period, _changeSource.Token).ConfigureAwait(false);
                    if (_changeSource.IsCancellationRequested)
                    {
                        return;
                    }

                    _callback(_state);
                    if (_changeSource.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        public void Change(int dueTime, int period)
        {
            if (_callback == null)
            {
                throw new ObjectDisposedException(nameof(Timer));
            }
            if (period < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(period));
            }
            if (dueTime < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime));
            }
            Stop();
            if (dueTime != -1)
            {
                Task.Factory.StartNew(Function, TaskCreationOptions.LongRunning).Unwrap();
            }
            async Task Function()
            {
                await Task.Delay(dueTime, _changeSource.Token);
                if (_changeSource.IsCancellationRequested)
                {
                    return;
                }

                _callback(_state);
                if (_changeSource.IsCancellationRequested)
                {
                    return;
                }

                if (period != -1)
                {
                    while (true)
                    {
                        await Task.Delay(period, _changeSource.Token);
                        if (_changeSource.IsCancellationRequested)
                        {
                            return;
                        }

                        _callback(_state);
                        if (_changeSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _callback = null;
            _state = null;
            Stop();
        }

        private void Stop()
        {
            _changeSource?.Cancel();
            if (_task != null)
            {
                _task.Wait();
                _task = null;
            }
            _changeSource = new CancellationTokenSource();
        }
    }
}

#endif