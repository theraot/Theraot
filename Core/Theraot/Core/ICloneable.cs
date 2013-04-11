using System;

namespace Theraot.Core
{
    public interface ICloneable<T> : ICloneable
    {
        new T Clone();
    }
}