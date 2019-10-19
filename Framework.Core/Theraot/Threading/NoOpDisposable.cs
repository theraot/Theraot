using System;
using System.Diagnostics;

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public sealed class NoOpDisposable : IDisposable
    {
        private NoOpDisposable()
        {
        }

        public static IDisposable Instance { get; } = new NoOpDisposable();

        public void Dispose()
        {
        }
    }
}