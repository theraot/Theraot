#if LESSTHAN_NETSTANDARD12
using System.Threading.Tasks;

namespace System.Threading
{
    public sealed class Timer : IDisposable
    {
        private TimerCallback? _callback;
        private CancellationTokenSource? _changeSource;
        private object? _state;

        public Timer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            _callback = callback;
            _state = state;
            Change(dueTime, period);
        }

        public Timer(TimerCallback callback, object? state, int dueTime, int period)
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
            var callback = _callback;
            Stop();
            Task.Factory.StartNew(Function, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
            async Task Function()
            {
                await Task.Delay(dueTime, _changeSource!.Token).ConfigureAwait(false);
                if (_changeSource.IsCancellationRequested)
                {
                    return;
                }

                callback(_state);
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

                    callback(_state);
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
            var callback = _callback;
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
                Task.Factory.StartNew(Function, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
            }
            async Task Function()
            {
                await Task.Delay(dueTime, _changeSource!.Token).ConfigureAwait(false);
                if (_changeSource.IsCancellationRequested)
                {
                    return;
                }

                callback(_state);
                if (_changeSource.IsCancellationRequested)
                {
                    return;
                }

                if (period != -1)
                {
                    while (true)
                    {
                        await Task.Delay(period, _changeSource.Token).ConfigureAwait(false);
                        if (_changeSource.IsCancellationRequested)
                        {
                            return;
                        }

                        callback(_state);
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
            _changeSource?.Cancel();
            _changeSource?.Dispose();
        }

        private void Stop()
        {
            _changeSource?.Cancel();
            _changeSource?.Dispose();
            _changeSource = new CancellationTokenSource();
        }
    }
}

#endif