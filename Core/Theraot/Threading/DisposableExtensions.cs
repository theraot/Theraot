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
            if (!ReferenceEquals(disposable, null))
            {
                disposable.DisposedConditional
                    (
                        null,
                        () =>
                        {
                            if (action != null)
                            {
                                action.Invoke(disposable);
                            }
                        });
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
                        () =>
                        {
                            if (action == null)
                            {
                                return def;
                            }
                            else
                            {
                                return action.Invoke(disposable);
                            }
                        }
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
                        () =>
                        {
                            if (action == null)
                            {
                                return def;
                            }
                            else
                            {
                                return action.Invoke(disposable);
                            }
                        }
                    );
            }
            else
            {
                return alternative == null ? alternative.Invoke() : def;
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
                    if (allocationCode != null)
                    {
                        resource = allocationCode.Invoke();
                    }
                }
                if ((bodyCode != null) && (resource != null))
                {
                    bodyCode.Invoke(resource);
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
            if ((action != null) && !ReferenceEquals(obj, null))
            {
                action.Invoke(obj);
            }
        }

        public static TReturn SafeWith<T, TReturn>(this T obj, Func<T, TReturn> action, TReturn def)
        {
            if (!ReferenceEquals(obj, null))
            {
                if (action == null)
                {
                    return def;
                }
                else
                {
                    return action.Invoke(obj);
                }
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
                if (action == null)
                {
                    if (alternative == null)
                    {
                        return def;
                    }
                    else
                    {
                        return alternative.Invoke();
                    }
                }
                else
                {
                    return action.Invoke(obj);
                }
            }
            else
            {
                if (alternative == null)
                {
                    return def;
                }
                else
                {
                    return alternative.Invoke();
                }
            }
        }

        public static void With<T>(this T obj, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            action.Invoke(obj);
        }

        public static TReturn With<T, TReturn>(this T obj, Func<T, TReturn> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            return action.Invoke(obj);
        }
    }
}

#endif