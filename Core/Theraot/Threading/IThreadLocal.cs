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

        T ValueForDebugDisplay
        {
            get;
        }

        IList<T> Values
        {
            get;
        }
    }
}