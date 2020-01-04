#if LESSTHAN_NET47 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable RCS1047 // Non-asynchronous method name should not end with 'Async'.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
    /// <summary>Provides a type that can be used to configure how awaits on an <see cref="IAsyncDisposable"/> are performed.</summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ConfiguredAsyncDisposable
    {
        private readonly IAsyncDisposable _source;
        private readonly bool _continueOnCapturedContext;

        internal ConfiguredAsyncDisposable(IAsyncDisposable source, bool continueOnCapturedContext)
        {
            _source = source;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public ConfiguredValueTaskAwaitable DisposeAsync() =>
            // as with other "configured" awaitable-related type in CompilerServices, we don't null check to defend against
            // misuse like default(ConfiguredAsyncDisposable).DisposeAsync(), which will null ref by design.
            _source.DisposeAsync().ConfigureAwait(_continueOnCapturedContext);
    }
}

#endif