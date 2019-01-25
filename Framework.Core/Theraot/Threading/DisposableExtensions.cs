#if FAT

using System;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class DisposableExtensions
    {
        public static void DisposableSafeWith<T>(this T disposable, Action<T> action)
            where T : IExtendedDisposable
        {
            if (disposable != null)
            {
                disposable.DisposedConditional
                    (
                        null,
                        () =>
                        {
                            action?.Invoke(disposable);
                        });
            }
        }

        public static TReturn DisposableSafeWith<T, TReturn>(this T disposable, Func<T, TReturn> action, TReturn def)
            where T : IExtendedDisposable
        {
            if (disposable != null)
            {
                return disposable.DisposedConditional
                    (
                        null,
                        () =>
                        {
                            if (action == null)
                            {
                                return def;
                            }
                            return action.Invoke(disposable);
                        }
                    );
            }
            return def;
        }

        public static TReturn DisposableSafeWith<T, TReturn>(this T disposable, Func<T, TReturn> action, Func<TReturn> alternative, TReturn def)
            where T : IExtendedDisposable
        {
            if (disposable != null)
            {
                return disposable.DisposedConditional
                    (
                        null,
                        () =>
                        {
                            if (action == null)
                            {
                                return def;
                            }
                            return action.Invoke(disposable);
                        }
                    );
            }
            return alternative != null ? alternative.Invoke() : def;
        }

        public static void DisposedConditional(this IExtendedDisposable disposable, string exceptionMessageWhenDisposed, Action whenNotDisposed)
        {
            void WhenDisposed()
            {
                throw new ObjectDisposedException(exceptionMessageWhenDisposed);
            }
            if (disposable == null)
            {
                WhenDisposed();
            }
            else
            {
                disposable.DisposedConditional
                (
                    WhenDisposed,
                    whenNotDisposed
                );
            }
        }

        public static TReturn DisposedConditional<TReturn>(this IExtendedDisposable disposable, string exceptionMessageWhenDisposed, Func<TReturn> whenNotDisposed)
        {
            TReturn WhenDisposed()
            {
                throw new ObjectDisposedException(exceptionMessageWhenDisposed);
            }
            if (disposable == null)
            {
                return WhenDisposed();
            }
            return disposable.DisposedConditional
            (
                WhenDisposed,
                whenNotDisposed
            );
        }

        public static void Run(Func<IDisposable> allocationCode, Action<IDisposable> bodyCode)
        {
            IDisposable resource = null;
            try
            {
                try
                {
                    // Empty
                }
                finally
                {
                    if (allocationCode != null)
                    {
                        resource = allocationCode.Invoke();
                    }
                }
                if (bodyCode != null && resource != null)
                {
                    bodyCode.Invoke(resource);
                }
            }
            finally
            {
                resource?.Dispose();
            }
        }

        public static void SafeWith<T>(this T obj, Action<T> action)
        {
            if (action != null && obj != null)
            {
                action.Invoke(obj);
            }
        }

        public static TReturn SafeWith<T, TReturn>(this T obj, Func<T, TReturn> action, TReturn def)
        {
            if (obj != null)
            {
                if (action == null)
                {
                    return def;
                }
                return action.Invoke(obj);
            }
            return def;
        }

        public static TReturn SafeWith<T, TReturn>(this T obj, Func<T, TReturn> action, Func<TReturn> alternative, TReturn def)
        {
            if (obj != null)
            {
                if (action == null)
                {
                    if (alternative == null)
                    {
                        return def;
                    }
                    return alternative.Invoke();
                }
                return action.Invoke(obj);
            }
            if (alternative == null)
            {
                return def;
            }
            return alternative.Invoke();
        }

        public static void With<T>(this T obj, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            action.Invoke(obj);
        }

        public static TReturn With<T, TReturn>(this T obj, Func<T, TReturn> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            return action.Invoke(obj);
        }
    }
}

#endif