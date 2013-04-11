using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class SilentProgressor<T> : ProgressorBase<T>, IObservable<T>, IProgressor<T>
    {
        public SilentProgressor(IEnumerable<T> wrapped)
            : base(wrapped)
        {
            //Empty
        }

        public SilentProgressor(TryTake<T> trytake)
            : base(trytake)
        {
            //Empty
        }

        internal SilentProgressor(IEnumerable<T> wrapped, IProxyObservable<T> observable)
            : base(wrapped, observable)
        {
            //Empty
        }

        internal SilentProgressor(IProgressor<T> wrapped, IProxyObservable<T> observable)
            : base(wrapped, observable)
        {
            //Empty
        }

        internal SilentProgressor(TryTake<T> trytake, IProxyObservable<T> observable)
            : base(trytake, observable)
        {
            //Empty
        }
    }
}