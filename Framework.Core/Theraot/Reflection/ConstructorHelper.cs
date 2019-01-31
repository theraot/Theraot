// Needed for NET40

using System;
using System.Reflection;
using Theraot.Collections.ThreadSafe;

#if GREATERTHAN_NETSTANDARD12
using System.Linq;
#endif

namespace Theraot.Reflection
{
    public static class ConstructorHelper
    {
        private static readonly CacheDict<Type, object> _constructorCache = new CacheDict<Type, object>(256);

        public static TReturn Create<TReturn>()
        {
            if (TryGetCreate<TReturn>(out var result))
            {
                return result();
            }

            throw new MissingMemberException($"There is no constructor for {typeof(TReturn)} with no type arguments.");
        }

        public static TReturn CreateOrDefault<TReturn>()
        {
            return TryGetCreate<TReturn>(out var result) ? result() : default;
        }

        public static bool TryGetCreate<TReturn>(out Func<TReturn> create)
        {
            var type = typeof(TReturn);
            var info = type.GetTypeInfo();
            if (info.IsValueType)
            {
                create = () => default;
                return true;
            }

            if (_constructorCache.TryGetValue(type, out var result))
            {
                if (result == null)
                {
                    create = null;
                    return false;
                }

                create = (Func<TReturn>)result;
                return true;
            }

            var typeArguments = ArrayEx.Empty<Type>();
            var constructorInfo = GetConstructor(typeof(TReturn), typeArguments);
            if (constructorInfo == null)
            {
                _constructorCache[type] = null;
                create = null;
                return false;
            }

            TReturn Create()
            {
                return (TReturn)constructorInfo.Invoke(ArrayEx.Empty<object>());
            }

            _constructorCache[type] = (Func<TReturn>)Create;
            create = Create;
            return true;
        }

        private static ConstructorInfo GetConstructor(Type type, Type[] typeArguments)
        {
#if GREATERTHAN_NETSTANDARD12
            foreach (var constructorInfo in type.GetTypeInfo().DeclaredConstructors)
            {
                var parameters = constructorInfo.GetParameters();
                if (parameters.Length != typeArguments.Length)
                {
                    continue;
                }

                var ok = !typeArguments.Where((t, index) => parameters[index].GetType() != t).Any();

                if (!ok)
                {
                    continue;
                }

                return constructorInfo;
            }

            return null;
#else
            return type.GetConstructor(typeArguments);
#endif
        }
    }
}