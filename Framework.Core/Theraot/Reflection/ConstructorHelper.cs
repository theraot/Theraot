// Needed for NET40

using System;
using System.Reflection;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Reflection
{
    public static class ConstructorHelper
    {
        private static readonly CacheDict<Type, bool> _hasConstructorCache = new CacheDict<Type, bool>(256);
        private static readonly CacheDict<Type, object> _constructorCache = new CacheDict<Type, object>(256);

        public static TReturn Create<TReturn>()
        {
            return GetCreate<TReturn>()();
        }

        public static TReturn CreateOrDefault<TReturn>()
        {
            if (TryGetCreate<TReturn>(out var result))
            {
                return result();
            }
            return default;
        }

        public static Func<TReturn> GetCreate<TReturn>()
        {
            if (TryGetCreate<TReturn>(out var result))
            {
                return result;
            }
            throw new InvalidOperationException($"There is no constructor for {typeof(TReturn)} with no type arguments.");
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
            var canCache = TypeExtensions.CanCache(type);
            if (canCache && _constructorCache.TryGetValue(type, out var result))
            {
                if (result == null)
                {
                    create = null;
                    return false;
                }
                create = (Func<TReturn>)result;
                return true;
            }
            if (canCache && _hasConstructorCache.TryGetValue(type, out var has) && !has)
            {
                _constructorCache[type] = null;
                create = null;
                return false;
            }
            var typeArguments = Type.EmptyTypes;
            var constructorInfo = typeof(TReturn).GetConstructor(typeArguments);
            if (constructorInfo == null)
            {
                if (canCache)
                {
                    _hasConstructorCache[type] = false;
                    _constructorCache[type] = null;
                }
                create = null;
                return false;
            }
            TReturn Create() => (TReturn) constructorInfo.Invoke(ArrayReservoir<object>.EmptyArray);
            if (canCache)
            {
                _hasConstructorCache[type] = true;
                _constructorCache[type] = (Func<TReturn>) Create;
            }
            create = Create;
            return true;
        }

        public static bool HasConstructor<TReturn>()
        {
            var type = typeof(TReturn);
            var canCache = TypeExtensions.CanCache(type);
            if (canCache && _constructorCache.TryGetValue(type, out _))
            {
                return true;
            }
            if (canCache && _hasConstructorCache.TryGetValue(type, out var has))
            {
                return has;
            }
            var typeArguments = Type.EmptyTypes;
            var constructorInfo = typeof(TReturn).GetConstructor(typeArguments);
            if (constructorInfo == null)
            {
                if (canCache)
                {
                    _hasConstructorCache[type] = false;
                }
                return false;
            }
            if (canCache)
            {
                _hasConstructorCache[type] = true;
            }
            return true;
        }
    }
}