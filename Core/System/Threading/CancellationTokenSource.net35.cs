// CancellationTokenSource.cs
//
// Authors:
//       Jérémie "Garuma" Laval <jeremie.laval@gmail.com>
//       Marek Safar (marek.safar@gmail.com)
//       Alfonso J. Ramos (theraot@gmail.com)
//
// Copyright (c) 2009 Jérémie "Garuma" Laval
// Copyright 2011 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Collections.ThreadSafe;

namespace System.Threading
{
    public class CancellationTokenSource : IDisposable
    {
        internal static readonly CancellationTokenSource CanceledSource = new CancellationTokenSource();
        internal static readonly CancellationTokenSource NoneSource = new CancellationTokenSource();
        private static readonly TimerCallback _timerCallback;
        private readonly ManualResetEvent _handle;
        private HashBucket<CancellationTokenRegistration, Action> _callbacks;
        private bool _canceled;
        private int _currentId = int.MinValue;
        private bool _disposed;
        private CancellationTokenRegistration[] _linkedTokens;
        private Timer _timer;

        static CancellationTokenSource()
        {
            CanceledSource._canceled = true;
            _timerCallback = token =>
            {
                var cancellationTokenSource = (CancellationTokenSource)token;
                cancellationTokenSource.Cancel();
            };
        }

        public CancellationTokenSource()
        {
            _callbacks = new HashBucket<CancellationTokenRegistration, Action>();
            _handle = new ManualResetEvent(false);
        }

        public CancellationTokenSource(int millisecondsDelay)
            : this()
        {
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }
            if (millisecondsDelay != Timeout.Infinite)
            {
                _timer = new Timer(_timerCallback, this, millisecondsDelay, Timeout.Infinite);
            }
        }

        public CancellationTokenSource(TimeSpan delay)
            : this(CheckTimeout(delay))
        {
            //Empty
        }

        public bool IsCancellationRequested
        {
            get
            {
                return _canceled;
            }
        }

        public CancellationToken Token
        {
            get
            {
                CheckDisposed();
                return new CancellationToken(this);
            }
        }

        internal WaitHandle WaitHandle
        {
            get
            {
                CheckDisposed();
                return _handle;
            }
        }

        public static CancellationTokenSource CreateLinkedTokenSource(CancellationToken token1, CancellationToken token2)
        {
            return CreateLinkedTokenSource(new[] { token1, token2 });
        }

        public static CancellationTokenSource CreateLinkedTokenSource(params CancellationToken[] tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException("tokens");
            }
            else
            {
                if (tokens.Length == 0)
                {
                    throw new ArgumentException("Empty tokens array");
                }
                else
                {
                    var src = new CancellationTokenSource();
                    Action action = src.SafeLinkedCancel;
                    var registrations = new List<CancellationTokenRegistration>(tokens.Length);
                    foreach (CancellationToken token in tokens)
                    {
                        if (token.CanBeCanceled)
                        {
                            registrations.Add(token.Register(action));
                        }
                    }
                    src._linkedTokens = registrations.ToArray();
                    return src;
                }
            }
        }

        public void Cancel()
        {
            Cancel(false);
        }

        // If parameter is true we throw exception as soon as they appear otherwise we aggregate them
        public void Cancel(bool throwOnFirstException)
        {
            CheckDisposed();
            if (!_canceled)
            {
                Thread.MemoryBarrier();
                _canceled = true;
                _handle.Set();
                UnregisterLinkedTokens();
                List<Exception> exceptions = null;
                try
                {
                    for (int id = int.MinValue + 1; id <= _currentId; id++)
                    {
                        Action callback;
                        if (_callbacks.Remove(new CancellationTokenRegistration(id, this), out callback) && callback != null)
                        {
                            if (throwOnFirstException)
                            {
                                callback();
                            }
                            else
                            {
                                try
                                {
                                    callback();
                                }
                                catch (Exception exception)
                                {
                                    if (object.ReferenceEquals(exceptions, null))
                                    {
                                        exceptions = new List<Exception>();
                                    }
                                    exceptions.Add(exception);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _callbacks.Clear();
                }
                if (exceptions != null)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        public void CancelAfter(TimeSpan delay)
        {
            CancelAfter(CheckTimeout(delay));
        }

        public void CancelAfter(int millisecondsDelay)
        {
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }
            else
            {
                CheckDisposed();
                if (!_canceled && millisecondsDelay != Timeout.Infinite)
                {
                    if (object.ReferenceEquals(_timer, null))
                    {
                        // Have to be careful not to create secondary background timer
                        var newTimer = new Timer(_timerCallback, this, Timeout.Infinite, Timeout.Infinite);
                        var oldTimer = Interlocked.CompareExchange(ref _timer, newTimer, null);
                        if (!ReferenceEquals(oldTimer, null))
                        {
                            newTimer.Dispose();
                        }
                    }
                    _timer.Change(millisecondsDelay, Timeout.Infinite);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        internal CancellationTokenRegistration Register(Action callback, bool useSynchronizationContext)
        {
            CheckDisposed();
            var tokenReg = new CancellationTokenRegistration(Interlocked.Increment(ref _currentId), this);
            /* If the source is already canceled we execute the callback immediately
             * if not, we try to add it to the queue and if it is currently being processed
             * we try to execute it back ourselves to be sure the callback is ran
             */
            if (_canceled)
            {
                callback();
            }
            else
            {
                _callbacks.TryAdd(tokenReg, callback);
                if (_canceled && _callbacks.Remove(tokenReg, out callback))
                {
                    callback();
                }
            }
            return tokenReg;
        }

        internal void RemoveCallback(CancellationTokenRegistration reg)
        {
            // Ignore call if the source has been disposed
            if (!_disposed)
            {
                var callbacks = _callbacks;
                if (!object.ReferenceEquals(callbacks, null))
                {
                    Action dummy;
                    callbacks.Remove(reg, out dummy);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Thread.MemoryBarrier();
                _disposed = true;
                if (!_canceled)
                {
                    Thread.MemoryBarrier();
                    UnregisterLinkedTokens();
                    _callbacks = null;
                }
                if (!object.ReferenceEquals(_timer, null))
                {
                    _timer.Dispose();
                    _timer = null;
                }
                _handle.Close();
            }
        }

        private static int CheckTimeout(TimeSpan delay)
        {
            try
            {
                return checked((int)delay.TotalMilliseconds);
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException("delay");
            }
        }

        private void SafeLinkedCancel()
        {
            try
            {
                Cancel();
            }
            catch (ObjectDisposedException)
            {
                //Empty
            }
        }

        private void UnregisterLinkedTokens()
        {
            var registrations = Interlocked.Exchange(ref _linkedTokens, null);
            if (!object.ReferenceEquals(registrations, null))
            {
                foreach (var linked in registrations)
                {
                    linked.Dispose();
                }
            }
        }
    }
}

#endif