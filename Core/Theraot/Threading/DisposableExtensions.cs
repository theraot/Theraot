#if FAT

using System;

using Theraot.Core;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class DisposableExtensions
    {
        public static void DisposableSafeWith<T>(this T disposable, Action<T> action)
            where T : IExtendedDisposable
        {
            if (!ReferenceEquals(disposable, null))
            {
                disposable.DisposedConditional
                    (
                        null,
                        () => action.SafeInvoke(disposable)
                    );
            }
        }

        public static TReturn DisposableSafeWith<T, TReturn>(this T disposable, Func<T, TReturn> action, TReturn def)
            where T : IExtendedDisposable
        {
            if (!ReferenceEquals(disposable, null))
            {
                return disposable.DisposedConditional
                    (
                        null,
                        () => action.SafeInvoke(disposable, def)
                    );
            }
            else
            {
                return def;
            }
        }

        public static TReturn DisposableSafeWith<T, TReturn>(this T disposable, Func<T, TReturn> action, Func<TReturn> alternative, TReturn def)
            where T : IExtendedDisposable
        {
            if (!ReferenceEquals(disposable, null))
            {
                return disposable.DisposedConditional
                    (
                        null,
                        () => action.SafeInvoke(disposable, alternative, def)
                    );
            }
            else
            {
                return alternative.SafeInvoke(def);
            }
        }

        public static void DisposedConditional(this IExtendedDisposable disposable, string exceptionMessageWhenDisposed, Action whenNotDisposed)
        {
            Action whenDisposed =
                () =>
                {
                    throw new ObjectDisposedException(exceptionMessageWhenDisposed);
                };
            if (disposable == null)
            {
                whenDisposed();
            }
            else
            {
                disposable.DisposedConditional
                (
                    whenDisposed,
                    whenNotDisposed
                );
            }
        }

        public static TReturn DisposedConditional<TReturn>(this IExtendedDisposable disposable, string exceptionMessageWhenDisposed, Func<TReturn> whenNotDisposed)
        {
            Func<TReturn> whenDisposed =
                () =>
                {
                    throw new ObjectDisposedException(exceptionMessageWhenDisposed);
                };
            if (disposable == null)
            {
                return whenDisposed();
            }
            else
            {
                return disposable.DisposedConditional
                (
                    whenDisposed,
                    whenNotDisposed
                );
            }
        }

        public static void Run(Func<IDisposable> allocationCode, Action<IDisposable> bodyCode)
        {
            IDisposable resource = null;
            try
            {
                try
                {
                    //Empty
                }
                finally
                {
                    resource = allocationCode.SafeInvoke(null);
                }
                if (resource != null)
                {
                    bodyCode.SafeInvoke(resource);
                }
            }
            finally
            {
                if (resource != null)
                {
                    resource.Dispose();
                }
            }
        }

        public static void SafeWith<T>(this T obj, Action<T> action)
        {
            if (!ReferenceEquals(obj, null))
            {
                action.SafeInvoke(obj);
            }
        }

        public static TReturn SafeWith<T, TReturn>(this T obj, Func<T, TReturn> action, TReturn def)
        {
            if (!ReferenceEquals(obj, null))
            {
                return action.SafeInvoke(obj, def);
            }
            else
            {
                return def;
            }
        }

        public static TReturn SafeWith<T, TReturn>(this T obj, Func<T, TReturn> action, Func<TReturn> alternative, TReturn def)
        {
            if (!ReferenceEquals(obj, null))
            {
                return action.SafeInvoke(obj, alternative, def);
            }
            else
            {
                return alternative.SafeInvoke(def);
            }
        }

        public static void With<T>(this T obj, Action<T> action)
        {
            Check.NotNullArgument(action, "action")(obj);
        }

        public static TReturn With<T, TReturn>(this T obj, Func<T, TReturn> action)
        {
            return Check.NotNullArgument(action, "action")(obj);
        }
    }
}

#endif