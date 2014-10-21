#if FAT

using System;

namespace Theraot.Threading.Needles
{
    public partial class CacheNeedle<T> : IDisposable, IExtendedDisposable
    {
        [global::System.Diagnostics.DebuggerNonUserCode]
        protected override void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                try
                {
                    if (disposeManagedResources)
                    {
                        //Empty
                    }
                }
                finally
                {
                    try
                    {
                        this.UnmanagedDispose();
                    }
                    finally
                    {
                        _valueFactory = null;
                    }
                    base.Dispose(disposeManagedResources);
                }
            }
        }
    }
}

#endif