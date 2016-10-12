#if NETCF

namespace System.Runtime.ConstrainedExecution
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Interface, Inherited=false)]
    public sealed class ReliabilityContractAttribute : Attribute
    {
        private readonly Cer _cer;
        private readonly Consistency _consistency;
        public ReliabilityContractAttribute(Consistency consistencyGuarantee, Cer cer)
        {
            _consistency = consistencyGuarantee;
            _cer = cer;
        }

        public Cer Cer
        {
            get
            {
                return _cer;
            }
        }

        public Consistency ConsistencyGuarantee
        {
            get
            {
                return _consistency;
            }
        }
    }
}

#endif