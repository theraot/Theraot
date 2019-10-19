#pragma warning disable CA1068 // CancellationToken parameters must come last
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
#pragma warning disable RCS1231 // Make parameter ref read-only.

using System.Runtime.CompilerServices;

#if NET40

using System.Linq;

#endif

namespace System.Threading.Tasks
{
    public static partial class TaskEx
    {
#if LESSTHAN_NET46 || LESSTHAN_NETSTANDARD13

        /// <summary>A task that's already been completed successfully.</summary>
        private static Task? _completedTask;

#endif

        /// <summary>Gets a task that's already been completed successfully.</summary>
        /// <remarks>May not always return the same instance.</remarks>
        public static Task CompletedTask
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
#if LESSTHAN_NET40
                var completedTask = _completedTask;
                if (completedTask == null)
                {
                    _completedTask = completedTask = new Task(TaskStatus.RanToCompletion, InternalTaskOptions.DoNotDispose)
                    {
                        CancellationToken = default
                    };
                }

                return completedTask;
#endif
#if (GREATERTHAN_NET35 && LESSTHAN_NET46) || LESSTHAN_NETSTANDARD13
                var completedTask = _completedTask;
                if (completedTask == null)
                {
                    _completedTask = completedTask = FromResult(default(Theraot.VoidStruct));
                }

                return completedTask;
#endif
#if GREATERTHAN_NET45 || GREATERTHAN_NETSTANDARD12 || TARGETS_NETCORE
                return Task.CompletedTask;
#endif
            }
        }
    }
}