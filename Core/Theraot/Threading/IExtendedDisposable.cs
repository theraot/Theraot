#if FAT

using System;

namespace Theraot.Threading
{
    public interface IExtendedDisposable : IDisposable
    {
        bool IsDisposed { get; }

        void DisposedConditional(Action whenDisposed, Action whenNotDisposed);

        TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed);
    }
}

#endif