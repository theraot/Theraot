#if LESSTHAN_NETSTANDARD13

#pragma warning disable S3923 // All branches in a conditional structure should not have exactly the same implementation
// ReSharper disable LoopCanBeConvertedToQuery

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Theraot.Reflection;

namespace System.Reflection
{
    public static class TypeExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ConstructorInfo? GetConstructor(this Type type, Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (var constructorInfo in type.GetTypeInfo().DeclaredConstructors)
            {
                var parameters = constructorInfo.GetParameters();
                if (parameters.Length != types.Length)
                {
                    continue;
                }

                if (types.Where((t, index) => parameters[index].GetType() != t).Any())
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
        public static ConstructorInfo[] GetConstructors(this Type type, BindingFlags bindingAttr)
        {
            var result = new List<ConstructorInfo>();
            foreach (var constructorInfo in type.GetTypeInfo().DeclaredConstructors)
            {
                if (constructorInfo.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (constructorInfo.IsStatic)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(constructorInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MemberInfo[] GetDefaultMembers(this Type type)
        {
            var result = new List<MemberInfo>();
            foreach (var member in type.GetTypeInfo().DeclaredMembers)
            {
                if (member.HasAttribute<DefaultMemberAttribute>())
                {
                    result.Add(member);
                }
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static EventInfo GetEvent(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredEvent(name);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static EventInfo? GetEvent(this Type type, string name, BindingFlags bindingAttr)
        {
            EventInfo? found = null;
            foreach (var eventInfo in type.GetTypeInfo().DeclaredEvents)
            {
                if (!string.Equals(eventInfo.Name, name, (bindingAttr & BindingFlags.IgnoreCase) == BindingFlags.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (eventInfo.AddMethod.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (eventInfo.AddMethod.IsStatic)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (found != null)
                {
                    throw new AmbiguousMatchException();
                }

                found = eventInfo;
            }

            return found;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static EventInfo[] GetEvents(this Type type)
        {
            return type.GetTypeInfo().DeclaredEvents.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static EventInfo[] GetEvents(this Type type, BindingFlags bindingAttr)
        {
            var result = new List<EventInfo>();
            foreach (var eventInfo in type.GetTypeInfo().DeclaredEvents)
            {
                if (eventInfo.AddMethod.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (eventInfo.AddMethod.IsStatic)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(eventInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static FieldInfo? GetField(this Type type, string name)
        {
            FieldInfo? found = null;
            foreach (var fieldInfo in type.GetTypeInfo().DeclaredFields)
            {
                if (!string.Equals(fieldInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                if (found != null)
                {
                    throw new AmbiguousMatchException();
                }

                found = fieldInfo;
            }

            return found;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static FieldInfo? GetField(this Type type, string name, BindingFlags bindingAttr)
        {
            FieldInfo? found = null;
            foreach (var fieldInfo in type.GetTypeInfo().DeclaredFields)
            {
                if (!string.Equals(fieldInfo.Name, name, (bindingAttr & BindingFlags.IgnoreCase) == BindingFlags.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (fieldInfo.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (fieldInfo.IsStatic)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (found != null)
                {
                    throw new AmbiguousMatchException();
                }

                found = fieldInfo;
            }

            return found;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static FieldInfo[] GetFields(this Type type)
        {
            return type.GetTypeInfo().DeclaredFields.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static FieldInfo[] GetFields(this Type type, BindingFlags bindingAttr)
        {
            var result = new List<FieldInfo>();
            foreach (var fieldInfo in type.GetTypeInfo().DeclaredFields)
            {
                if (fieldInfo.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (fieldInfo.IsStatic)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(fieldInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MemberInfo[] GetMember(this Type type, string name)
        {
            var result = new List<MemberInfo>();
            foreach (var memberInfo in type.GetTypeInfo().DeclaredMembers)
            {
                if (!string.Equals(memberInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                result.Add(memberInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MemberInfo[] GetMember(this Type type, string name, BindingFlags bindingAttr)
        {
            var result = new List<MemberInfo>();
            foreach (var memberInfo in type.GetTypeInfo().DeclaredMembers)
            {
                if (!string.Equals(memberInfo.Name, name, (bindingAttr & BindingFlags.IgnoreCase) == BindingFlags.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (GetIsPublic(memberInfo))
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (GetIsStatic(memberInfo))
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(memberInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MemberInfo[] GetMembers(this Type type)
        {
            return type.GetTypeInfo().DeclaredMembers.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MemberInfo[] GetMembers(this Type type, BindingFlags bindingAttr)
        {
            var result = new List<MemberInfo>();
            foreach (var memberInfo in type.GetTypeInfo().DeclaredMembers)
            {
                if (GetIsPublic(memberInfo))
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (GetIsStatic(memberInfo))
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(memberInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo? GetMethod(this Type type, string name, Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (var methodInfo in type.GetTypeInfo().DeclaredMethods)
            {
                if (!string.Equals(methodInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                var parameters = methodInfo.GetParameters();
                if (parameters.Length != types.Length)
                {
                    continue;
                }

                if (types.Where((t, index) => parameters[index].GetType() != t).Any())
                {
                    continue;
                }

                return methodInfo;
            }

            return null;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static MethodInfo? GetMethod(this Type type, string name)
        {
            MethodInfo? found = null;
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
        public static MethodInfo? GetMethod(this Type type, string name, BindingFlags bindingAttr)
        {
            MethodInfo? found = null;
            foreach (var methodInfo in type.GetTypeInfo().DeclaredMethods)
            {
                if (!string.Equals(methodInfo.Name, name, (bindingAttr & BindingFlags.IgnoreCase) == BindingFlags.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (methodInfo.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (methodInfo.IsStatic)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
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
        public static MethodInfo[] GetMethods(this Type type, BindingFlags bindingAttr)
        {
            var result = new List<MethodInfo>();
            foreach (var methodInfo in type.GetTypeInfo().DeclaredMethods)
            {
                if (methodInfo.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (methodInfo.IsStatic)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(methodInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Type? GetNestedType(this Type type, string name, BindingFlags bindingAttr)
        {
            Type? found = null;
            foreach (var nestedType in type.GetTypeInfo().DeclaredNestedTypes)
            {
                if (!string.Equals(nestedType.Name, name, (bindingAttr & BindingFlags.IgnoreCase) == BindingFlags.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (nestedType.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (!nestedType.IsAbstract && !nestedType.DeclaredConstructors.Any())
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (found != null)
                {
                    throw new AmbiguousMatchException();
                }

                found = nestedType.AsType();
            }

            return found;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Type[] GetNestedTypes(this Type type, BindingFlags bindingAttr)
        {
            var result = new List<Type>();
            foreach (var nestedType in type.GetTypeInfo().DeclaredNestedTypes)
            {
                if (nestedType.IsPublic)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (!nestedType.IsAbstract && !nestedType.DeclaredConstructors.Any())
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(nestedType.AsType());
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo[] GetProperties(this Type type, BindingFlags bindingAttr)
        {
            var result = new List<PropertyInfo>();
            foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties)
            {
                if (propertyInfo.GetGetMethod()?.IsPublic == true || propertyInfo.GetSetMethod()?.IsPublic == true)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (propertyInfo.GetGetMethod()?.IsStatic == true || propertyInfo.GetSetMethod()?.IsStatic == true)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                result.Add(propertyInfo);
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo? GetProperty(this Type type, string name, Type returnType)
        {
            foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties)
            {
                if (!string.Equals(propertyInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                if (propertyInfo.PropertyType != returnType)
                {
                    continue;
                }

                return propertyInfo;
            }

            return null;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo? GetProperty(this Type type, string name, Type returnType, Type[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties)
            {
                if (!string.Equals(propertyInfo.Name, name, StringComparison.Ordinal))
                {
                    continue;
                }

                if (propertyInfo.PropertyType != returnType)
                {
                    continue;
                }

                var parameters = propertyInfo.GetIndexParameters();
                if (parameters.Length != types.Length)
                {
                    continue;
                }

                if (types.Where((t, index) => parameters[index].GetType() != t).Any())
                {
                    continue;
                }

                return propertyInfo;
            }

            return null;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo? GetProperty(this Type type, string name)
        {
            PropertyInfo? found = null;
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

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static PropertyInfo? GetProperty(this Type type, string name, BindingFlags bindingAttr)
        {
            PropertyInfo? found = null;
            foreach (var propertyInfo in type.GetTypeInfo().DeclaredProperties)
            {
                if (!string.Equals(propertyInfo.Name, name, (bindingAttr & BindingFlags.IgnoreCase) == BindingFlags.Default ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (propertyInfo.GetGetMethod()?.IsPublic == true || propertyInfo.GetSetMethod()?.IsPublic == true)
                {
                    if ((bindingAttr & BindingFlags.Public) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (propertyInfo.GetGetMethod()?.IsStatic == true || propertyInfo.GetSetMethod()?.IsStatic == true)
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }
                else
                {
                    if ((bindingAttr & BindingFlags.Static) == BindingFlags.Default)
                    {
                        continue;
                    }
                }

                if (found != null)
                {
                    throw new AmbiguousMatchException();
                }

                found = propertyInfo;
            }

            return found;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool IsInstanceOfType(this Type type, object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }

            return type.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo());
        }

        private static bool GetIsPublic(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case ConstructorInfo constructorInfo:
                    return constructorInfo.IsPublic;

                case EventInfo eventInfo:
                    return eventInfo.AddMethod.IsPublic;

                case FieldInfo fieldInfo:
                    return fieldInfo.IsPublic;

                case MethodInfo methodInfo:
                    return methodInfo.IsPublic;

                default:
                    return memberInfo is PropertyInfo propertyInfo &&
                            (propertyInfo.GetGetMethod()?.IsPublic == true ||
                             propertyInfo.GetSetMethod()?.IsPublic == true);
            }
        }

        private static bool GetIsStatic(MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case ConstructorInfo constructorInfo:
                    return constructorInfo.IsStatic;

                case EventInfo eventInfo:
                    return eventInfo.AddMethod.IsStatic;

                case FieldInfo fieldInfo:
                    return fieldInfo.IsStatic;

                case MethodInfo methodInfo:
                    return methodInfo.IsStatic;

                default:
                    return memberInfo is PropertyInfo propertyInfo &&
                           (propertyInfo.GetGetMethod()?.IsStatic == true ||
                            propertyInfo.GetSetMethod()?.IsStatic == true);
            }
        }
    }
}

#endif