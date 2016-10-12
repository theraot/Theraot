#if NETCF

using System.Runtime.InteropServices;

namespace System.Runtime.ConstrainedExecution
{
    [ComVisible(true)]
    // [Security.Permissions.SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    public abstract class CriticalFinalizerObject
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected CriticalFinalizerObject()
        {
            // Empty
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        ~CriticalFinalizerObject()
        {
            // Empty
        }
    }
}

#endif