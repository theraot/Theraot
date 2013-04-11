using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [Serializable]
    public sealed class Progressor<T> : ProgressorBase<T>, IObservable<T>, IProgressor<T>
    {
        public Progressor(IEnumerable<T> wrapped)
            : base(wrapped)
        {
            //Empty
        }

        public Progressor(TryTake<T> trytake)
            : base(trytake)
        {
            //Empty
        }

        internal Progressor(IEnumerable<T> wrapped, IProxyObservable<T> observable)
            : base(wrapped, observable)
        {
            //Empty
        }

        internal Progressor(IProgressor<T> wrapped, IProxyObservable<T> observable)
            : base(wrapped, observable)
        {
            //Empty
        }

        internal Progressor(TryTake<T> trytake, IProxyObservable<T> observable)
            : base(trytake, observable)
        {
            //Empty
        }

        protected override void OnClose()
        {
            Observable.OnCompleted();
        }

        protected override void OnTake(T item)
        {
            Observable.OnNext(item);
        }
    }
}