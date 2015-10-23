#if FAT

namespace Theraot.Threading.Needles
{
    public partial class CacheNeedle<T>
    {
        [System.Diagnostics.DebuggerNonUserCode]
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
                        ReleaseWaitHandle();
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