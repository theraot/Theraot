#if NET35

namespace System.Threading.Tasks
{
    internal interface IContinuationTask
    {
        Task Antecedent { get; }
    }
}

#endif