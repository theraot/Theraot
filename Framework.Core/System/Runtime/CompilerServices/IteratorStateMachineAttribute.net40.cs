#if LESSTHAN_NET45

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Identities the iterator state machine type for this method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    [Serializable]
    public sealed class IteratorStateMachineAttribute : StateMachineAttribute
    {
        /// <summary>
        /// Initializes the attribute.
        /// </summary>
        /// <param name="stateMachineType">The type that implements the state machine.</param>
        public IteratorStateMachineAttribute(Type stateMachineType)
            : base(stateMachineType)
        {
        }
    }
}

#endif