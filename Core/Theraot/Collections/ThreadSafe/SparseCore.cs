using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections.ThreadSafe
{
    internal class SparseCore<T> : ICore<T>
    {
        public SparseCore(int capacity)
        {
            throw new NotImplementedException();

        }

        public int Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Do(int index, bool grow, DoAction<T> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}