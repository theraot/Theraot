#if FAT

using System;

namespace Theraot.Threading
{
    public sealed class ReadWriteLock : IReadWriteLock
    {
        private readonly IReadWriteLock _wrapped;

        public ReadWriteLock()
        {
            _wrapped = new NoReentrantReadWriteLock();
        }

        public ReadWriteLock(bool reentrant)
        {
            _wrapped = (IReadWriteLock)(reentrant ? (object)new ReentrantReadWriteLock() : new NoReentrantReadWriteLock());
        }

        public bool HasReader
        {
            get { return _wrapped.HasReader; }
        }

        public bool HasWriter
        {
            get { return _wrapped.HasWriter; }
        }

        public bool IsCurrentThreadReader
        {
            get { return _wrapped.IsCurrentThreadReader; }
        }

        public bool IsCurrentThreadWriter
        {
            get { return _wrapped.IsCurrentThreadWriter; }
        }

        public void Dispose()
        {
            _wrapped.Dispose();
        }

        public IDisposable EnterRead()
        {
            return _wrapped.EnterRead();
        }

        public IDisposable EnterWrite()
        {
            return _wrapped.EnterWrite();
        }

        public bool TryEnterRead(out IDisposable engagement)
        {
            return _wrapped.TryEnterRead(out engagement);
        }

        public bool TryEnterWrite(out IDisposable engagement)
        {
            return _wrapped.TryEnterWrite(out engagement);
        }
    }
}

#endif