using System;

namespace Theraot.Threading.Needles
{
    public interface IPromise
    {
        Exception Error { get; }
    }

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
    {
        //Empty
    }
}
