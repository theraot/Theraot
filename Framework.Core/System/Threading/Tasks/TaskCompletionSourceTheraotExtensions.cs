#if GREATERTHAN_NET35 && LESSTHAN_NET46

#pragma warning disable RECS0108 // Warns about static fields in generic types
#pragma warning disable RECS0146 // Member hides static member from outer class

using System.Diagnostics;
using System.Reflection;

namespace System.Threading.Tasks
{
    public static class TaskCompletionSourceTheraotExtensions
    {
        public static bool TrySetCanceled<T>(this TaskCompletionSource<T> taskCompletionSource, CancellationToken token)
        {
            if (taskCompletionSource == null)
            {
                throw new ArgumentNullException(nameof(taskCompletionSource));
            }
            return TrySetCanceledCachedDelegate<T>.TrySetCanceled(taskCompletionSource, token);
        }

        /// <summary>
        /// Calls TaskCompletionSource<typeparamref name="T"/>.TrySetCanceled internal method.
        /// </summary>
        private static class TrySetCanceledCachedDelegate<T>
        {
            public static Func<TaskCompletionSource<T>, CancellationToken, bool> TrySetCanceled =>
                _trySetCanceled ??= CreateTrySetCanceledDelegate();

            private static Func<TaskCompletionSource<T>, CancellationToken, bool>? _trySetCanceled;

            private static Func<TaskCompletionSource<T>, CancellationToken, bool> CreateTrySetCanceledDelegate()
            {
                var trySetCanceled = typeof(TaskCompletionSource<T>).GetMethod(
                    nameof(TrySetCanceled),
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                    null, CallingConventions.Any, new[] { typeof(CancellationToken) }, null);

                Debug.Assert(trySetCanceled != null, nameof(trySetCanceled) + " != null");
                return (Func<TaskCompletionSource<T>, CancellationToken, bool>)Delegate.CreateDelegate(
                    typeof(Func<TaskCompletionSource<T>, CancellationToken, bool>), trySetCanceled);
            }
        }
    }
}

#endif