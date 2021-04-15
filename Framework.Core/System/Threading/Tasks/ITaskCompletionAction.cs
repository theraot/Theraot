#if LESSTHAN_NET40

namespace System.Threading.Tasks
{
    internal interface ITaskCompletionAction
    {
        void Invoke(Task completingTask);
    }
}

#endif