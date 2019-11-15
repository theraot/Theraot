#if LESSTHAN_NET45

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1815 // Override equals and operator equals on value types

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="ValueTask{TResult}" />.</summary>
    /// <typeparam name="TResult">The type of the result produced.</typeparam>
    [StructLayout(LayoutKind.Auto)]
    public struct ConfiguredValueTaskAwaitable<TResult>
    {
        /// <summary>true to attempt to marshal the continuation back to the original context captured; otherwise, false.</summary>
        private readonly bool _continueOnCapturedContext;

        /// <summary>The wrapped <see cref="ValueTask{TResult}" />.</summary>
        private readonly ValueTask<TResult> _value;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="ValueTask{TResult}" />.</param>
        /// <param name="continueOnCapturedContext">
        ///     true to attempt to marshal the continuation back to the original synchronization context captured; otherwise,
        ///     false.
        /// </param>
        internal ConfiguredValueTaskAwaitable(ValueTask<TResult> value, bool continueOnCapturedContext)
        {
            _value = value;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        /// <summary>Returns an awaiter for this <see cref="ConfiguredValueTaskAwaitable{TResult}" /> instance.</summary>
        public ConfiguredValueTaskAwaiter GetAwaiter()
        {
            return new ConfiguredValueTaskAwaiter(_value, _continueOnCapturedContext);
        }

        /// <summary>Provides an awaiter for a <see cref="ConfiguredValueTaskAwaitable{TResult}" />.</summary>
        [StructLayout(LayoutKind.Auto)]
        public struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>The value to pass to ConfigureAwait.</summary>
            private readonly bool _continueOnCapturedContext;

            /// <summary>The value being awaited.</summary>
            private readonly ValueTask<TResult> _value;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            /// <param name="continueOnCapturedContext">The value to pass to ConfigureAwait.</param>
            internal ConfiguredValueTaskAwaiter(ValueTask<TResult> value, bool continueOnCapturedContext)
            {
                _value = value;
                _continueOnCapturedContext = continueOnCapturedContext;
            }

            /// <summary>Gets whether the <see cref="ConfiguredValueTaskAwaitable{TResult}" /> has completed.</summary>
            public bool IsCompleted => _value.IsCompleted;

            /// <summary>Gets the result of the ValueTask.</summary>
            public TResult GetResult()
            {
                return _value._task == null ? _value._result : _value._task.GetAwaiter().GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                (_value._task ?? TaskEx.FromResult(_value._result)).ConfigureAwait(_continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                (_value._task ?? TaskEx.FromResult(_value._result)).ConfigureAwait(_continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
            }
        }
    }
}

#endif