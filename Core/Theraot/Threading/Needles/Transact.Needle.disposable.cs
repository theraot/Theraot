#if FAT

using System;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        public sealed partial class Needle<T> : IDisposable
        {
            private int _status;

            [global::System.Diagnostics.DebuggerNonUserCode]
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
            ~Needle()
            {
                try
                {
                    //Empty
                }
                finally
                {
                    try
                    {
                        Dispose(false);
                    }
                    catch
                    {
                        //Pokemon
                    }
                }
            }

            [global::System.Diagnostics.DebuggerNonUserCode]
            public void Dispose()
            {
                try
                {
                    Dispose(true);
                }
                finally
                {
                    GC.SuppressFinalize(this);
                }
            }

            [global::System.Diagnostics.DebuggerNonUserCode]
            private void Dispose(bool disposeManagedResources)
            {
                if (TakeDisposalExecution())
                {
                    if (disposeManagedResources)
                    {
                        this.OnDispose();
                    }
                }
            }

            private bool TakeDisposalExecution()
            {
                if (_status == -1)
                {
                    return false;
                }
                else
                {
                    return ThreadingHelper.SpinWaitSet(ref _status, -1, 0, -1);
                }
            }
        }
    }
}

#endif