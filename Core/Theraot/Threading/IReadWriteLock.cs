#if FAT

using System;

namespace Theraot.Threading
{
    internal interface IReadWriteLock : IDisposable
    {
        bool HasReader { get; }

        bool HasWriter { get; }

        bool IsCurrentThreadReader { get; }

        bool IsCurrentThreadWriter { get; }

        IDisposable EnterRead();

        IDisposable EnterWrite();

        bool TryEnterRead(out IDisposable engagement);

        bool TryEnterWrite(out IDisposable engagement);
    }
}

#endif