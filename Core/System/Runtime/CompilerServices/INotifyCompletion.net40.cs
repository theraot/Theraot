#if NET20 || NET30 || NET35 || NET40

namespace System.Runtime.CompilerServices
{
    public interface INotifyCompletion
    {
        void OnCompleted(Action continuation);
    }
}

#endif