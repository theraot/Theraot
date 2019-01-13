#if LESSTHAN_NET45

using System.Security;

namespace System.Runtime.CompilerServices
{
    public interface ICriticalNotifyCompletion : INotifyCompletion
    {
        [SecurityCritical]
        void UnsafeOnCompleted(Action continuation);
    }
}

#endif