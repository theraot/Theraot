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
using System.Diagnostics;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class CancellationTokenSource : IDisposable
    {
        internal static readonly CancellationTokenSource CanceledSource = new CancellationTokenSource(); // Leaked
        internal static readonly CancellationTokenSource NoneSource = new CancellationTokenSource(); // Leaked
        private static readonly Action<CancellationTokenSource> _timerCallback;
        private readonly ManualResetEvent _handle;
        private SafeDictionary<int, Action> _callbacks;
        private int _cancelRequested;
        private int _currentId = int.MaxValue;
        private int _disposeRequested;
        private CancellationTokenRegistration[] _linkedTokens;
        private Timeout<CancellationTokenSource> _timeout;

        static CancellationTokenSource()
        {
            CanceledSource._cancelRequested = 1;
            _timerCallback = cancellationTokenSource =>
            {
                var callbacks = cancellationTokenSource._callbacks;
                if (callbacks != null)
                {
                    cancellationTokenSource.CancelExtracted(false, callbacks, true);
                }
            };
        }

        public CancellationTokenSource()
        {
            _callbacks = new SafeDictionary<int, Action>();
            _handle = new ManualResetEvent(false);
        }

        public CancellationTokenSource(int millisecondsDelay)
            : this()
        {
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));
            }
            if (millisecondsDelay != Timeout.Infinite)
            {
                _timeout = new Timeout<CancellationTokenSource>(_timerCallback, millisecondsDelay, this);
            }
        }

        public CancellationTokenSource(TimeSpan delay)
            : this(CheckTimeout(delay))
        {
            //Empty
        }

        public bool IsCancellationRequested => _cancelRequested == 1;

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
                throw new ArgumentNullException(nameof(tokens));
            }
            if (tokens.Length == 0)
            {
                throw new ArgumentException("Empty tokens array");
            }
            var src = new CancellationTokenSource();
            Action action = src.SafeLinkedCancel;
            var registrations = new List<CancellationTokenRegistration>(tokens.Length);
            foreach (var token in tokens)
            {
                if (token.CanBeCanceled)
                {
                    registrations.Add(token.Register(action));
                }
            }
            src._linkedTokens = registrations.ToArray();
            return src;
        }

        public void Cancel()
        {
            Cancel(false);
        }

        public void Cancel(bool throwOnFirstException)
        {
            // If throwOnFirstException is true we throw exception as soon as they appear otherwise we aggregate them
            var callbacks = CheckDisposedGetCallbacks();
            CancelExtracted(throwOnFirstException, callbacks, false);
        }

        public void CancelAfter(TimeSpan delay)
        {
            CancelAfter(CheckTimeout(delay));
        }

        public void CancelAfter(int millisecondsDelay)
        {
            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsDelay));
            }
            CheckDisposed();
            if (Volatile.Read(ref _cancelRequested) == 0 && millisecondsDelay != Timeout.Infinite)
            {
                if (_timeout == null)
                {
                    // Have to be careful not to create secondary background timer
                    var newTimer = new Timeout<CancellationTokenSource>(_timerCallback, Timeout.Infinite, this);
                    var oldTimer = Interlocked.CompareExchange(ref _timeout, newTimer, null);
                    if (oldTimer != null)
                    {
                        newTimer.Cancel();
                    }
                    _timeout.Change(millisecondsDelay);
                }
            }
        }

        [DebuggerNonUserCode]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        internal void CheckDisposed()
        {
            if (Volatile.Read(ref _disposeRequested) == 1)
            {
                throw new ObjectDisposedException(nameof(CancellationTokenSource));
            }
        }

        internal SafeDictionary<int, Action> CheckDisposedGetCallbacks()
        {
            var result = _callbacks;
            if (result == null || Volatile.Read(ref _disposeRequested) == 1)
            {
                throw new ObjectDisposedException(nameof(CancellationTokenSource));
            }
            return result;
        }

        internal CancellationTokenRegistration Register(Action callback, bool useSynchronizationContext)
        {
            // NOTICE this method has no null check
            var callbacks = CheckDisposedGetCallbacks();
            var id = Interlocked.Decrement(ref _currentId);
            var tokenReg = new CancellationTokenRegistration(id, this);
            // If the source is already canceled run the callback inline.
            // if not, we try to add it to the queue and if it is currently being processed.
            // we try to execute it back ourselves to be sure the callback is ran.
            if (Volatile.Read(ref _cancelRequested) == 1)
            {
                callback();
            }
            else
            {
                // Capture execution contexts if the callback may not run inline.
                if (useSynchronizationContext)
                {
                    var capturedSyncContext = SynchronizationContext.Current;
                    var originalCallback = callback;
                    callback = () => capturedSyncContext.Send(_ => originalCallback(), null);
                }
                callbacks.TryAdd(id, callback);
                // Check if the source was just canceled and if so, it may be that it executed the callbacks except the one just added...
                // So try to inline the callback
                if (Volatile.Read(ref _cancelRequested) == 1 && callbacks.Remove(id, out callback))
                {
                    callback();
                }
            }
            return tokenReg;
        }

        internal bool RemoveCallback(int reg)
        {
            // Ignore call if the source has been disposed
            if (Volatile.Read(ref _disposeRequested) == 0)
            {
                var callbacks = _callbacks;
                if (callbacks != null)
                {
                    return callbacks.Remove(reg, out var dummy);
                }
            }
            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Interlocked.CompareExchange(ref _disposeRequested, 1, 0) == 0)
            {
                if (Volatile.Read(ref _cancelRequested) == 0)
                {
                    UnregisterLinkedTokens();
                    _callbacks = null;
                }
                var timer = Interlocked.Exchange(ref _timeout, null);
                timer?.Cancel();
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
                throw new ArgumentOutOfRangeException(nameof(delay));
            }
        }

        private static void RunCallback(bool throwOnFirstException, Action callback, ref List<Exception> exceptions)
        {
            // NOTICE this method has no null check
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
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }
                    exceptions.Add(exception);
                }
            }
        }

        private void CancelExtracted(bool throwOnFirstException, SafeDictionary<int, Action> callbacks, bool ignoreDisposedException)
        {
            if (Interlocked.CompareExchange(ref _cancelRequested, 1, 0) == 0)
            {
                try
                {
                    // The CancellationTokenSource may have been disposed just before this call
                    _handle.Set();
                }
                catch (ObjectDisposedException)
                {
                    if (!ignoreDisposedException)
                    {
                        throw;
                    }
                }
                UnregisterLinkedTokens();
                List<Exception> exceptions = null;
                try
                {
                    var id = _currentId;
                    do
                    {
                        if (callbacks.Remove(id, out var callback) && callback != null)
                        {
                            RunCallback(throwOnFirstException, callback, ref exceptions);
                        }
                    } while (id++ != int.MaxValue);
                }
                finally
                {
                    // Whatever was added after the cancellation process started, it should run inline in Register... if they don't, handle then here.
                    foreach (
                        var callback in
                            callbacks.RemoveWhereKeyEnumerable(_ => true))
                    {
                        RunCallback(throwOnFirstException, callback, ref exceptions);
                    }
                }
                if (exceptions != null)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        private void SafeLinkedCancel()
        {
            var callbacks = _callbacks;
            if (callbacks == null || Volatile.Read(ref _disposeRequested) == 1)
            {
                return;
            }
            CancelExtracted(false, callbacks, true);
        }

        private void UnregisterLinkedTokens()
        {
            var registrations = Interlocked.Exchange(ref _linkedTokens, null);
            if (registrations != null)
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