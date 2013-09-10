using System;

namespace Theraot.Threading.Needles
{
    public interface IPromise : IExpected
    {
        Exception Error { get; }

        void Wait();
    }

    public interface IPromise<out T> : IPromise, IReadOnlyNeedle<T>
    {
        //Empty
    }
}