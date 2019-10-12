#if LESSTHAN_NET40

namespace System.Threading.Tasks
{
    internal interface IContinuationTask
    {
        Task? Antecedent { get; }
    }
}

#endif