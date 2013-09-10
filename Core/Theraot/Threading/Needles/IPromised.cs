using System;

namespace Theraot.Threading.Needles
{
    public interface IPromised : IPromised<object>
    {
        //Empty
    }

    public interface IPromised<T> : IPromise, IObserver<T>, ICacheNeedle<T>
    {
        //Empty
    }
}