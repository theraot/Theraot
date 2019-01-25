#if LESSTHAN_NET45

namespace System.Runtime.CompilerServices
{
    /// <inheritdoc />
    /// <summary>
    /// Identities the async state machine type for this method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    [Serializable]
    public sealed class AsyncStateMachineAttribute : StateMachineAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes the attribute.
        /// </summary>
        /// <param name="stateMachineType">The type that implements the state machine.</param>
        public AsyncStateMachineAttribute(Type stateMachineType)
            : base(stateMachineType)
        {
        }
    }
}

#endif