#if LESSTHAN_NETSTANDARD15

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class TypeExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConstructorInfo GetConstructor(this Type type, Type[] typeArguments)
        {
            var members = type.GetTypeInfo().DeclaredMembers;
            foreach (var member in members)
            {
                if (!(member is ConstructorInfo constructorInfo))
                {
                    continue;
                }

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
            var members = type.GetTypeInfo().DeclaredMembers;
            var result = new List<ConstructorInfo>();
            foreach (var member in members)
            {
                if (member is ConstructorInfo constructorInfo)
                {
                    result.Add(constructorInfo);
                }
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static FieldInfo[] GetFields(this Type type)
        {
            var members = type.GetTypeInfo().DeclaredMembers;
            var result = new List<FieldInfo>();
            foreach (var member in members)
            {
                if (member is FieldInfo fieldInfo)
                {
                    result.Add(fieldInfo);
                }
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo GetMethod(this Type type, string name, Type[] typeArguments)
        {
            var members = type.GetTypeInfo().DeclaredMembers;
            foreach (var member in members)
            {
                if (!(member is MethodInfo methodInfo))
                {
                    continue;
                }

                if (!string.Equals(member.Name, name, StringComparison.Ordinal))
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
            var members = type.GetTypeInfo().DeclaredMembers;
            MethodInfo found = null;
            foreach (var member in members)
            {
                if (!(member is MethodInfo methodInfo))
                {
                    continue;
                }

                if (!string.Equals(member.Name, name, StringComparison.Ordinal))
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
            var members = type.GetTypeInfo().DeclaredMembers;
            var result = new List<MethodInfo>();
            foreach (var member in members)
            {
                if (member is MethodInfo methodInfo)
                {
                    result.Add(methodInfo);
                }
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo[] GetProperties(this Type type)
        {
            var members = type.GetTypeInfo().DeclaredMembers;
            var result = new List<PropertyInfo>();
            foreach (var member in members)
            {
                if (member is PropertyInfo propertyInfo)
                {
                    result.Add(propertyInfo);
                }
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo GetProperty(this Type type, string name, Type[] typeArguments)
        {
            var members = type.GetTypeInfo().DeclaredMembers;
            foreach (var member in members)
            {
                if (!(member is PropertyInfo propertyInfo))
                {
                    continue;
                }

                if (!string.Equals(member.Name, name, StringComparison.Ordinal))
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
            var members = type.GetTypeInfo().DeclaredMembers;
            PropertyInfo found = null;
            foreach (var member in members)
            {
                if (!(member is PropertyInfo propertyInfo))
                {
                    continue;
                }

                if (!string.Equals(member.Name, name, StringComparison.Ordinal))
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

#elif TARGETS_NETSTANDARD

using System.Runtime.CompilerServices;

namespace System.Reflection
{
    public static class TypeExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConstructorInfo GetConstructor(this Type type, Type[] typeArguments)
        {
            return type.GetTypeInfo().GetConstructor(typeArguments);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConstructorInfo[] GetConstructors(this Type type)
        {
            return type.GetTypeInfo().GetConstructors();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static FieldInfo[] GetFields(this Type type)
        {
            return type.GetTypeInfo().GetFields();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo GetMethod(this Type type, string name, Type[] typeArguments)
        {
            return type.GetTypeInfo().GetMethod(name, typeArguments);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetMethod(name);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo[] GetMethods(this Type type)
        {
            return type.GetTypeInfo().GetMethods();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().GetProperties();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo GetProperty(this Type type, string name, Type[] typeArguments)
        {
            return type.GetTypeInfo().GetProperty(name, typeArguments);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetProperty(name);
        }
    }
}

#endif