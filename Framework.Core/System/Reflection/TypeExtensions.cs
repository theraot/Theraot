#if LESSTHAN_NETSTANDARD13

// ReSharper disable LoopCanBeConvertedToQuery

using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class TypeExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConstructorInfo GetConstructor(this Type type, Type[] typeArguments)
        {
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
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConstructorInfo[] GetConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static FieldInfo[] GetFields(this Type type)
        {
            return type.GetTypeInfo().DeclaredFields.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo GetMethod(this Type type, string name, Type[] typeArguments)
        {
            foreach (var methodInfo in type.GetTypeInfo().DeclaredMethods)
            {
                if (!string.Equals(methodInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                var parameters = methodInfo.GetParameters();
                if (parameters.Length != typeArguments.Length)
                {
                    continue;
                }

                var ok = !typeArguments.Where((t, index) => parameters[index].GetType() != t).Any();

                if (!ok)
                {
                    continue;
                }

                return methodInfo;
            }

            return null;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo GetMethod(this Type type, string name)
        {
            MethodInfo found = null;
            foreach (var methodInfo in type.GetTypeInfo().DeclaredMethods)
            {
                if (!string.Equals(methodInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                if (found != null)
                {
                    throw new AmbiguousMatchException();
                }

                found = methodInfo;
            }

            return found;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo[] GetMethods(this Type type)
        {
            return type.GetTypeInfo().DeclaredMethods.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo GetProperty(this Type type, string name, Type[] typeArguments)
        {
            foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties)
            {
                if (!string.Equals(propertyInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                var parameters = propertyInfo.GetIndexParameters();
                if (parameters.Length != typeArguments.Length)
                {
                    continue;
                }

                var ok = !typeArguments.Where((t, index) => parameters[index].GetType() != t).Any();

                if (!ok)
                {
                    continue;
                }

                return propertyInfo;
            }

            return null;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            PropertyInfo found = null;
            foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties)
            {
                if (!string.Equals(propertyInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                if (found != null)
                {
                    throw new AmbiguousMatchException();
                }

                found = propertyInfo;
            }

            return found;
        }
    }
}

#endif