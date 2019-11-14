using System.Runtime.CompilerServices;

#if NET40

using System.Linq;

#endif

namespace System.Threading.Tasks
{
    public static partial class TaskExEx
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
                    _completedTask = completedTask =
                        new Task(TaskStatus.RanToCompletion, InternalTaskOptions.DoNotDispose);
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