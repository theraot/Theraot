#if LESSTHAN_NET47 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

#pragma warning disable CA1815 // Override equals and operator equals on value types

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Theraot.Threading.Needles;

namespace System.Threading.Tasks.Sources
{
    /// <summary>Provides the core logic for implementing a manual-reset <see cref="IValueTaskSource"/> or <see cref="IValueTaskSource{TResult}"/>.</summary>
    /// <typeparam name="TResult"></typeparam>
    [StructLayout(LayoutKind.Auto)]
    public struct ManualResetValueTaskSourceCore<TResult>
    {
        /// <summary>
        /// The callback to invoke when the operation completes if <see cref="OnCompleted"/> was called before the operation completed,
        /// or <see cref="ManualResetValueTaskSourceCoreShared.Sentinel"/> if the operation completed before a callback was supplied,
        /// or null if a callback hasn't yet been provided and the operation hasn't yet completed.
        /// </summary>
        private Action<object?>? _continuation;

        /// <summary>State to pass to <see cref="_continuation"/>.</summary>
        private object? _continuationState;

#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD13

        /// <summary><see cref="ExecutionContext"/> to flow to the callback, or null if no flowing is required.</summary>
        private ExecutionContext? _executionContext;

#endif

        /// <summary>
        /// A "captured" <see cref="SynchronizationContext"/> or <see cref="TaskScheduler"/> with which to invoke the callback,
        /// or null if no special context is required.
        /// </summary>
        private object? _capturedContext;

        /// <summary>The result with which the operation succeeded, or the default value if it hasn't yet completed or failed.</summary>
        private INeedle<TResult>? _result;

        /// <summary>Gets or sets whether to force continuations to run asynchronously.</summary>
        /// <remarks>Continuations may run asynchronously if this is false, but they'll never run synchronously if this is true.</remarks>
        public bool RunContinuationsAsynchronously { get; set; }

        /// <summary>Resets to prepare for the next operation.</summary>
        public void Reset()
        {
            // Reset/update state for the next use/await of this instance.
            Version++;
            _result = null;
            _capturedContext = null;
            _continuation = null;
            _continuationState = null;
#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD13
            _executionContext = null;
#endif
        }

        /// <summary>Completes with a successful result.</summary>
        /// <param name="result">The result.</param>
        public void SetResult(TResult result)
        {
            _result = new StructNeedle<TResult>(result);
            SignalCompletion();
        }

        /// <summary>Completes with an error.</summary>
        /// <param name="error">The exception.</param>
        public void SetException(Exception error)
        {
            _result = new ExceptionStructNeedle<TResult>(error);
            SignalCompletion();
        }

        /// <summary>Gets the operation version.</summary>
        public short Version { get; private set; }

        /// <summary>Gets the status of the operation.</summary>
        /// <param name="token">Opaque value that was provided to the <see cref="ValueTask"/>'s constructor.</param>
        public ValueTaskSourceStatus GetStatus(short token)
        {
            ValidateToken(token);
            if (_continuation == null || _result == null)
            {
                return ValueTaskSourceStatus.Pending;
            }
            if (_result is ExceptionStructNeedle<TResult> error)
            {
                if (error.Exception is OperationCanceledException)
                {
                    return ValueTaskSourceStatus.Canceled;
                }
                return ValueTaskSourceStatus.Faulted;
            }
            return ValueTaskSourceStatus.Succeeded;
        }

        /// <summary>Gets the result of the operation.</summary>
        /// <param name="token">Opaque value that was provided to the <see cref="ValueTask"/>'s constructor.</param>
        public TResult GetResult(short token)
        {
            ValidateToken(token);
            if (_result == null)
            {
                throw new InvalidOperationException();
            }

            return _result.Value;
        }

        /// <summary>Schedules the continuation action for this operation.</summary>
        /// <param name="continuation">The continuation to invoke when the operation has completed.</param>
        /// <param name="state">The state object to pass to <paramref name="continuation"/> when it's invoked.</param>
        /// <param name="token">Opaque value that was provided to the <see cref="ValueTask"/>'s constructor.</param>
        /// <param name="flags">The flags describing the behavior of the continuation.</param>
        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }
            ValidateToken(token);

#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD13
            if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
            {
                _executionContext = ExecutionContext.Capture();
            }
#endif

            if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
            {
                var sc = SynchronizationContext.Current;
                if (sc != null && sc.GetType() != typeof(SynchronizationContext))
                {
                    _capturedContext = sc;
                }
                else
                {
                    var ts = TaskScheduler.Current;
                    if (ts != TaskScheduler.Default)
                    {
                        _capturedContext = ts;
                    }
                }
            }

            // We need to set the continuation state before we swap in the delegate, so that
            // if there's a race between this and SetResult/Exception and SetResult/Exception
            // sees the _continuation as non-null, it'll be able to invoke it with the state
            // stored here.  However, this also means that if this is used incorrectly (e.g.
            // awaited twice concurrently), _continuationState might get erroneously overwritten.
            // To minimize the chances of that, we check preemptively whether _continuation
            // is already set to something other than the completion sentinel.

            object? oldContinuation = _continuation;
            if (oldContinuation == null)
            {
                _continuationState = state;
                oldContinuation = Interlocked.CompareExchange(ref _continuation, continuation, null);
            }

            if (oldContinuation == null)
            {
                return;
            }

            // Operation already completed, so we need to queue the supplied callback.
            if (!ReferenceEquals(oldContinuation, ManualResetValueTaskSourceCoreShared.Sentinel))
            {
                throw new InvalidOperationException();
            }

            switch (_capturedContext)
            {
                case null:
#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD13
                    if (_executionContext != null)
                    {
                        ThreadPoolEx.QueueUserWorkItem(continuation, state, true);
                        return;
                    }
#endif
                    ThreadPoolEx.UnsafeQueueUserWorkItem(continuation, state, true);
                    return;

                case SynchronizationContext sc:
                    sc.Post(s =>
                    {
                        var tuple = (Tuple<Action<object?>, object?>)s!;
                        tuple.Item1(tuple.Item2);
                    }, Tuple.Create(continuation, state));
                    return;

                case TaskScheduler ts:
#if NET40
                    Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.None, ts);
#else
                    Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, ts);
#endif
                    return;

                default:
                    break;
            }
        }

        /// <summary>Ensures that the specified token matches the current version.</summary>
        /// <param name="token">The token supplied by <see cref="ValueTask"/>.</param>
        private void ValidateToken(short token)
        {
            if (token != Version)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>Signals that the operation has completed.  Invoked after the result or error has been set.</summary>
        private void SignalCompletion()
        {
            if (_continuation == null && Interlocked.CompareExchange(ref _continuation, ManualResetValueTaskSourceCoreShared.Sentinel, null) == null)
            {
                return;
            }
#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD13
            if (_executionContext != null)
            {
                ExecutionContext.Run(_executionContext, s => ((ManualResetValueTaskSourceCore<TResult>)s).InvokeContinuation(), this);
                return;
            }
#endif
            InvokeContinuation();
        }

        private void InvokeContinuation()
        {
            Debug.Assert(_continuation != null);
            var continuation = _continuation!;

            switch (_capturedContext)
            {
                case null:
                    if (RunContinuationsAsynchronously)
                    {
#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD13
                        if (_executionContext != null)
                        {
                            ThreadPoolEx.QueueUserWorkItem(continuation, _continuationState, true);
                            return;
                        }
#endif
                        ThreadPoolEx.QueueUserWorkItem(continuation, _continuationState, true);
                        return;
                    }

                    continuation(_continuationState);
                    return;

                case SynchronizationContext sc:
                    sc.Post(s =>
                    {
                        var state = (Tuple<Action<object?>, object?>)s!;
                        state.Item1(state.Item2);
                    }, Tuple.Create(continuation, _continuationState));
                    return;

                case TaskScheduler ts:
#if NET40
                    Task.Factory.StartNew(continuation, _continuationState, CancellationToken.None, TaskCreationOptions.None, ts);
#else
                    Task.Factory.StartNew(continuation, _continuationState, CancellationToken.None, TaskCreationOptions.DenyChildAttach, ts);
#endif
                    return;

                default:
                    break;
            }
        }
    }

    internal static class ManualResetValueTaskSourceCoreShared // separated out of generic to avoid unnecessary duplication
    {
        internal static readonly Action<object?> Sentinel = CompletionSentinel;

        private static void CompletionSentinel(object? _) // named method to aid debugging
        {
            DebugEx.Fail("The sentinel delegate should never be invoked.");
            throw new InvalidOperationException();
        }
    }
}

#endif