#if NET20 || NET30 || NET35

namespace System.Threading
{
    public static class LazyInitializer
    {
        public static T EnsureInitialized<T>(ref T target) where T : class
        {
            return target ?? EnsureInitialized(ref target, GetDefaultCtorValue<T>);
        }

        public static T EnsureInitialized<T>(ref T target, Func<T> valueFactory) where T : class
        {
            if (target == null)
            {
                var value = valueFactory();
                if (value == null)
                    throw new InvalidOperationException();

                Interlocked.CompareExchange(ref target, value, null);
            }

            return target;
        }

        public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock)
        {
            return EnsureInitialized(ref target, ref initialized, ref syncLock, GetDefaultCtorValue<T>);
        }

        public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock, Func<T> valueFactory)
        {
            if (initialized)
                return target;

            if (syncLock == null)
                Interlocked.CompareExchange(ref syncLock, new object(), null);

            lock (syncLock)
            {
                if (initialized)
                    return target;

                initialized = true;
                Thread.MemoryBarrier();
                target = valueFactory();
            }

            return target;
        }

        private static T GetDefaultCtorValue<T>()
        {
            try
            {
                return Activator.CreateInstance<T>();
            }
            catch
            {
                throw new MissingMemberException("The type being lazily initialized does not have a "
                                                  + "public, parameterless constructor.");
            }
        }
    }
}

#endif