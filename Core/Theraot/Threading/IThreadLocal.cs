using System;
using System.Collections.Generic;

namespace Theraot.Threading
{
    internal interface IThreadLocal<T> : IDisposable
    {
        bool IsValueCreated
        {
            get;
        }

        T Value
        {
            get;
            set;
        }

        IList<T> Values
        {
            get;
        }
    }
}