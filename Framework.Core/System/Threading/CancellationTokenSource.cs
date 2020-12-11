#if LESSTHAN_NET40

#pragma warning disable CC0031 // Check for null before calling a delegate

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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading
{
    public class CancellationTokenSource : IDisposable
    {
        internal static readonly CancellationTokenSource CanceledSource = new CancellationTokenSource
        {
            _cancelRequested = 1
        }; // Leaked

        internal static readonly CancellationTokenSource NoneSource = new CancellationTokenSource(); // Leaked
        private static readonly Action<CancellationTokenSource> _timerCallback = TimerCallback;
        private readonly ManualResetEvent _handle;
        private Bucket<Action>? _callbacks;
        private int _cancelRequested;
        private int _currentId = int.MaxValue;
        private int _disposeRequested;
        private CancellationTokenRegistration[]? _linkedTokens;
        private RootedTimeout? _timeout;

        public CancellationTokenSource()
        {
            _callbacks = new Bucket<Action>();
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
                _timeout = RootedTimeout.Launch(Callback, millisecondsDelay);
            }
        }

        public CancellationTokenSource(TimeSpan delay)
            : this(CheckTimeout(delay))
        {
            // Empty
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
            registrations.AddRange(tokens.Where(token => token.CanBeCanceled).Select(token => token.Register(action)));
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
            if (Volatile.Read(ref _cancelRequested) != 0 || millisecondsDelay == Timeout.Infinite || _timeout != null)
            {
                return;
            }

            // Have to be careful not to create secondary background timer
            var newTimer = RootedTimeout.Launch(Callback, Timeout.Infinite);
            var oldTimer = Interlocked.CompareExchange(ref _timeout, newTimer, null);
            if (oldTimer != null)
            {
                newTimer.Cancel();
            }

            _timeout.Change(millisecondsDelay);
        }

        [DebuggerNonUserCode]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void CheckDisposed()
        {
            if (Volatile.Read(ref _disposeRequested) == 1)
            {
                throw new ObjectDisposedException(nameof(CancellationTokenSource));
            }
        }

        internal Bucket<Action> CheckDisposedGetCallbacks()
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

                callbacks.Insert(id, callback);
                // Check if the source was just canceled and if so, it may be that it executed the callbacks except the one just added...
                // So try to inline the callback
                if (Volatile.Read(ref _cancelRequested) == 1 && callbacks.RemoveAt(id, out var recovered))
                {
                    recovered!();
                }
            }

            return tokenReg;
        }

        internal bool RemoveCallback(int reg)
        {
            // Ignore call if the source has been disposed
            if (Volatile.Read(ref _disposeRequested) != 0)
            {
                return true;
            }

            var callbacks = _callbacks;
            return callbacks == null || callbacks.RemoveAt(reg, out var dummy);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Interlocked.CompareExchange(ref _disposeRequested, 1, 0) != 0)
            {
                return;
            }

            if (Volatile.Read(ref _cancelRequested) == 0)
            {
                UnregisterLinkedTokens();
                _callbacks = null;
            }

            var timer = Interlocked.Exchange(ref _timeout, null);
            timer?.Cancel();
            _handle.Close();
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

        private static void RunCallback(bool throwOnFirstException, Action callback, ref List<Exception>? exceptions)
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
                    (exceptions ??= new List<Exception>()).Add(exception);
                }
            }
        }

        private static void TimerCallback(CancellationTokenSource cancellationTokenSource)
        {
            var callbacks = cancellationTokenSource._callbacks;
            if (callbacks != null)
            {
                cancellationTokenSource.CancelExtracted(false, callbacks, true);
            }
        }

        private void Callback()
        {
            _timerCallback(this);
        }

        private void CancelExtracted(bool throwOnFirstException, Bucket<Action> callbacks, bool ignoreDisposedException)
        {
            if (Interlocked.CompareExchange(ref _cancelRequested, 1, 0) != 0)
            {
                return;
            }

            try
            {
                // The CancellationTokenSource may have been disposed just before this call
                _handle.Set();
            }
            catch (ObjectDisposedException exception) when (ignoreDisposedException)
            {
                _ = exception;
            }

            UnregisterLinkedTokens();
            List<Exception>? exceptions = null;
            try
            {
                var id = _currentId;
                do
                {
                    if (callbacks.RemoveAt(id, out var callback) && callback != null)
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
                    callbacks.RemoveWhereEnumerable(_ => true))
                {
                    RunCallback(throwOnFirstException, callback, ref exceptions);
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
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
            if (registrations == null)
            {
                return;
            }

            foreach (var linked in registrations)
            {
                linked.Dispose();
            }
        }
    }
}

#endif