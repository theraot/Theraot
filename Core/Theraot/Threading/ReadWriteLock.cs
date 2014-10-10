using System;

namespace Theraot.Threading
{
    public sealed class ReadWriteLock : Theraot.Threading.IReadWriteLock, IDisposable
    {
        private readonly IReadWriteLock _wrapped;

        public ReadWriteLock()
        {
            _wrapped = new NoReentrantReadWriteLock();
        }

        public ReadWriteLock(bool reentrant)
        {
            if (reentrant)
            {
                _wrapped = new ReentrantReadWriteLock();
            }
            else
            {
                _wrapped = new NoReentrantReadWriteLock();
            }
        }

        public bool CurrentThreadIsReader
        {
            get
            {
                return _wrapped.CurrentThreadIsReader;
            }
        }

        public bool CurrentThreadIsWriter
        {
            get
            {
                return _wrapped.CurrentThreadIsWriter;
            }
        }

        public bool HasReader
        {
            get
            {
                return _wrapped.HasReader;
            }
        }

        public bool HasWriter
        {
            get
            {
                return _wrapped.HasWriter;
            }
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