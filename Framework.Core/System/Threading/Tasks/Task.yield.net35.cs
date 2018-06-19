#if NET20 || NET30 || NET35

using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public partial class Task
    {
        /// <summary>
        /// Creates an awaitable that asynchronously yields back to the current context when awaited.
        /// </summary>
        ///
        /// <returns>
        /// A context that, when awaited, will asynchronously transition back into the current context.
        ///             If SynchronizationContext.Current is non-null, that is treated as the current context.
        ///             Otherwise, TaskScheduler.Current is treated as the current context.
        ///
        /// </returns>
        public static YieldAwaitable Yield()
        {
            return new YieldAwaitable();
        }
    }
}

#endif