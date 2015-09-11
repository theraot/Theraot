#if NET20 || NET30 || NET35

namespace System.Threading
{
    public static class LazyInitializer
    {
        public static T EnsureInitialized<T>(ref T target)
            where T : class
        {
            var found = target;
            Thread.MemoryBarrier();
            if (found != null)
            {
                return found;
            }
            var value = GetDefaultCtorValue<T>();
            if (value == null)
            {
                throw new InvalidOperationException();
            }
            return Interlocked.CompareExchange(ref target, value, null) ?? value;
        }

        public static T EnsureInitialized<T>(ref T target, Func<T> valueFactory)
            where T : class
        {
            var found = target;
            Thread.MemoryBarrier();
            if (found != null)
            {
                return found;
            }
            var value = valueFactory();
            if (value == null)
            {
                throw new InvalidOperationException("valueFactory returned null");
            }
            return Interlocked.CompareExchange(ref target, value, null) ?? value;
        }

        public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock)
        {
            if (syncLock == null)
            {
                Interlocked.CompareExchange(ref syncLock, new object(), null);
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
                target = GetDefaultCtorValue<T>();
                Volatile.Write(ref initialized, true);
            }
            return target;
        }

        public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock, Func<T> valueFactory)
        {
            if (syncLock == null)
            {
                Interlocked.CompareExchange(ref syncLock, new object(), null);
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
                Volatile.Write(ref initialized, true);
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