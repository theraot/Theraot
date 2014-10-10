using System;

namespace Theraot.Threading
{
    internal interface IReadWriteLock : IDisposable
    {
        bool CurrentThreadIsReader { get; }

        bool CurrentThreadIsWriter { get; }

        bool HasReader { get; }

        bool HasWriter { get; }

        IDisposable EnterRead();

        IDisposable EnterWrite();

        bool TryEnterRead(out IDisposable engagement);

        bool TryEnterWrite(out IDisposable engagement);
    }
}