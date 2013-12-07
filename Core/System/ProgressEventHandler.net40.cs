#if NET20 || NET30 || NET35 ||NET40

using System.Collections.Generic;
using System.Text;

namespace System
{
    public delegate void ProgressEventHandler<T>(object sender, T value);
}

#endif