#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
namespace System.Runtime.ConstrainedExecution
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Assembly, Inherited = false)]
    public sealed class ReliabilityContractAttribute : Attribute
    {
        public ReliabilityContractAttribute(Consistency consistencyGuarantee, Cer cer)
        {
            ConsistencyGuarantee = consistencyGuarantee;
            Cer = cer;
        }

        public Cer Cer { get; }

        public Consistency ConsistencyGuarantee { get; }
    }
}

#endif