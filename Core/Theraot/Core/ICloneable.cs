using System;

namespace Theraot.Core
{
    public interface ICloneable<out T> : ICloneable
    {
        new T Clone();
    }
}