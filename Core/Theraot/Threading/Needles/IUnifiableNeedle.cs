using System;
using System.Collections.Generic;
using System.Text;

namespace Theraot.Threading.Needles
{
    interface IUnifiableNeedle<T> : INeedle<T>
    {
        bool TryUnify<TNeedle>(ref TNeedle value) where TNeedle: INeedle<T>;
    }
}
