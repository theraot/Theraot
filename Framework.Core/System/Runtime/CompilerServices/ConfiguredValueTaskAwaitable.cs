#if LESSTHAN_NET45

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1815 // Override equals and operator equals on value types

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks.Sources;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides an awaitable type that enables configured awaits on a <see cref="ValueTask" />.</summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredValueTaskAwaitable
    {
        /// <summary>The wrapped <see cref="ValueTask{TResult}" />.</summary>
        private readonly ValueTask _value;

        /// <summary>Initializes the awaitable.</summary>
        /// <param name="value">The wrapped <see cref="ValueTask{TResult}" />.</param>
        internal ConfiguredValueTaskAwaitable(ValueTask value)
        {
            _value = value;
        }

        /// <summary>Returns an awaiter for this <see cref="ConfiguredValueTaskAwaitable{TResult}" /> instance.</summary>
        public ConfiguredValueTaskAwaiter GetAwaiter()
        {
            return new ConfiguredValueTaskAwaiter(_value);
        }

        /// <summary>Provides an awaiter for a <see cref="ConfiguredValueTaskAwaitable{TResult}" />.</summary>
        [StructLayout(LayoutKind.Auto)]
        public readonly struct ConfiguredValueTaskAwaiter : ICriticalNotifyCompletion
        {
            /// <summary>The value being awaited.</summary>
            private readonly ValueTask _value;

            /// <summary>Initializes the awaiter.</summary>
            /// <param name="value">The value to be awaited.</param>
            internal ConfiguredValueTaskAwaiter(ValueTask value)
            {
                _value = value;
            }

            /// <summary>Gets whether the <see cref="ConfiguredValueTaskAwaitable{TResult}" /> has completed.</summary>
            public bool IsCompleted => _value.IsCompleted;

            /// <summary>Gets the result of the ValueTask.</summary>
            public void GetResult()
            {
                _value.ThrowIfCompletedUnsuccessfully();
            }

            public void OnCompleted(Action continuation)
            {
                var obj = _value.Obj;
                switch (obj)
                {
                    case Task task:
                        task.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                        return;

                    case null:
                        ValueTask.CompletedTask.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
                        return;

                    default:
                        ((IValueTaskSource)obj).OnCompleted(ValueTaskAwaiter.InvokeActionDelegate, continuation, _value.Token, (ValueTaskSourceOnCompletedFlags)(2 | (_value.ContinueOnCapturedContext ? 1 : 0)));
                        break;
                }
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                var obj = _value.Obj;
                switch (obj)
                {
                    case null:
                        ValueTask.CompletedTask.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                        return;

                    case Task task:
                        task.ConfigureAwait(_value.ContinueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
                        return;

                    default:
                        ((IValueTaskSource)obj).OnCompleted(ValueTaskAwaiter.InvokeActionDelegate, continuation, _value.Token, _value.ContinueOnCapturedContext ? ValueTaskSourceOnCompletedFlags.UseSchedulingContext : ValueTaskSourceOnCompletedFlags.None);
                        break;
                }
            }
        }
    }
}

#endif