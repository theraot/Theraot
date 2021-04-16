#if LESSTHAN_NET40

#pragma warning disable CA1062 // Validate arguments of public methods

using Theraot.Reflection;

namespace System.Threading
{
    public static class LazyInitializer
    {
        public static T EnsureInitialized<T>(ref T target)
            where T : class
        {
            return TypeHelper.LazyCreate(ref target);
        }

        public static T EnsureInitialized<T>(ref T target, Func<T> valueFactory)
            where T : class
        {
            return TypeHelper.LazyCreate(ref target, valueFactory);
        }

        public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock)
        {
            if (syncLock == null)
            {
                Interlocked.CompareExchange(ref syncLock, new object(), comparand: null);
            }

            if (Volatile.Read(ref initialized))
            {
                return target;
            }

            lock (syncLock)
            {
                if (Volatile.Read(ref initialized))
                {
                    return target;
                }

                try
                {
                    target = Activator.CreateInstance<T>();
                }
                catch
                {
                    throw new MissingMemberException("The type being lazily initialized does not have a public, parameterless constructor.");
                }

                Volatile.Write(ref initialized, value: true);
            }

            return target;
        }

        public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock, Func<T> valueFactory)
        {
            // MICROSOFT doesn't do a null check for valueFactory
            if (syncLock == null)
            {
                Interlocked.CompareExchange(ref syncLock, new object(), comparand: null);
            }

            if (Volatile.Read(ref initialized))
            {
                return target;
            }

            lock (syncLock)
            {
                if (Volatile.Read(ref initialized))
                {
                    return target;
                }

                target = valueFactory();
                Volatile.Write(ref initialized, value: true);
            }

            return target;
        }
    }
}

#endif