#if NET20 || NET30 || NET35

using System;
using System.Collections.Generic;
using System.Text;
using Theraot.Threading;

namespace System.Threading
{
    public class ManualResetEventSlim : IDisposable
    {
        private const int INT_DefaultSpinCount = 10;
        private const int INT_LongTimeOutHint = 160;
        private const int IntSleepCountHint = 5;
        private const int IntSpinWaitHint = 20;

        private static readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(INT_LongTimeOutHint);

        private ManualResetEvent _handle;
        private int _requested;
        private int _spinCount;
        private int _state;
        public ManualResetEventSlim()
            : this(false)
        {
            //Empty
        }

        public ManualResetEventSlim(bool initialState)
        {
            _state = initialState ? 1 : 0;
            _spinCount = INT_DefaultSpinCount;
        }

        public ManualResetEventSlim(bool initialState, int spinCount)
        {
            if (spinCount < 0 || spinCount > 2047)
            {
                throw new ArgumentOutOfRangeException("spinCount");
            }
            else
            {
                _spinCount = spinCount;
                _state = initialState ? 1 : 0;
            }
        }

        public bool IsSet
        {
            get
            {
                if (Thread.VolatileRead(ref _state) == -1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                else
                {
                    if (Thread.VolatileRead(ref _requested) != 0)
                    {
                        var handle = WaitHandleExtracted();
                        return handle.WaitOne(0);
                    }
                    else
                    {
                        if (Thread.VolatileRead(ref _state) == 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }

        public int SpinCount
        {
            get
            {
                return _spinCount;
            }
        }

        public WaitHandle WaitHandle
        {
            get
            {
                return WaitHandleExtracted();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Reset()
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                Thread.VolatileWrite(ref _state, 0);
                if (Thread.VolatileRead(ref _requested) != 0)
                {
                    var handle = WaitHandleExtracted();
                    handle.Reset();
                }
            }
        }

        public void Set()
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                Thread.VolatileWrite(ref _state, 1);
                if (Thread.VolatileRead(ref _requested) != 0)
                {
                    var handle = WaitHandleExtracted();
                    handle.Set();
                }
            }
        }

        public bool Wait(TimeSpan timeout)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                if (!IsSet)
                {
                    if (Thread.VolatileRead(ref _requested) != 0)
                    {
                        var handle = WaitHandleExtracted();
                        return handle.WaitOne(timeout);
                    }
                    else
                    {
                        if (IsSet)
                        {
                            return true;
                        }
                        else
                        {
                            if (timeout > _timeout)
                            {
                                if (WaitExtracted(_timeout))
                                {
                                    return true;
                                }
                                else
                                {
                                    timeout -= _timeout;
                                    var handle = WaitHandleExtracted();
                                    return handle.WaitOne(timeout);
                                }
                            }
                            else
                            {
                                return WaitExtracted(timeout);
                            }
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public bool Wait(int millisecondsTimeout)
        {
            return Wait(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "False Positive")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Interlocked.Exchange(ref _state, -1) != -1)
                {
                    Thread.VolatileWrite(ref _requested, 0);
                    var handle = Interlocked.Exchange(ref _handle, null);
                    if (handle != null)
                    {
                        handle.Close();
                    }
                }
            }
        }

        private ManualResetEvent WaitHandleExtracted()
        {
            if (Interlocked.CompareExchange(ref _requested, 1, 0) == 0)
            {
                _handle = new ManualResetEvent(IsSet);
            }
            else if (_handle == null)
            {
                ThreadingHelper.SpinWaitWhileNull(ref _handle);
            }
            return _handle;
        }
        private bool WaitExtracted(TimeSpan timeout)
        {
            var start = DateTime.Now;
            var backCount = 0;
            if (Environment.ProcessorCount > 1)
            {
                backCount = IntSleepCountHint;
            }
            retry:
            if (IsSet)
            {
                return true;
            }
            else
            {
                if (timeout.CompareTo(DateTime.Now.Subtract(start)) > 0)
                {
                    if (backCount == 0)
                    {
                        Thread.Sleep(0);
                    }
                    else
                    {
                        Thread.SpinWait(IntSpinWaitHint);
                        backCount--;
                    }
                    goto retry;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

#endif