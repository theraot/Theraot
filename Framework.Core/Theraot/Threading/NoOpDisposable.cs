using System;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class NoOpDisposable : IDisposable
    {
        private NoOpDisposable()
        {
            // Empty
        }

        public static IDisposable Instance { get; } = new NoOpDisposable();

        public void Dispose()
        {
            // Empty
        }
    }
}