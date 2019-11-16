#if LESSTHAN_NET47 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

namespace System.Runtime.CompilerServices
{
    /// <summary>Indicates whether a method is an asynchronous iterator.</summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class AsyncIteratorStateMachineAttribute : StateMachineAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute" /> class.</summary>
        /// <param name="stateMachineType">The type object for the underlying state machine type that's used to implement a state machine method.</param>
        public AsyncIteratorStateMachineAttribute(Type stateMachineType)
            : base(stateMachineType)
        {
            // Empty
        }
    }
}

#endif