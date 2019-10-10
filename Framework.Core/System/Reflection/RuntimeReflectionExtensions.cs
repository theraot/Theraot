#if LESSTHAN_NET45

using System.Collections.Generic;

namespace System.Reflection
{
    public static class RuntimeReflectionExtensions
    {
        public static MethodInfo GetMethodInfo(this Delegate del)
        {
            if (del == null)
            {
                throw new ArgumentNullException(nameof(del));
            }
            return del.Method;
        }

        public static MethodInfo GetRuntimeBaseDefinition(this MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            return method.GetBaseDefinition();
        }

        public static EventInfo GetRuntimeEvent(this Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetEvent(name);
        }

        public static IEnumerable<EventInfo> GetRuntimeEvents(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetEvents();
        }

        public static FieldInfo GetRuntimeField(this Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetField(name);
        }

        public static IEnumerable<FieldInfo> GetRuntimeFields(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetFields();
        }

        public static InterfaceMapping GetRuntimeInterfaceMap(this Type typeInfo, Type interfaceType)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }
            return typeInfo.GetInterfaceMap(interfaceType);
        }

        public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] parameters)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetMethod(name, parameters);
        }

        public static IEnumerable<MethodInfo> GetRuntimeMethods(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetMethods();
        }

        public static IEnumerable<PropertyInfo> GetRuntimeProperties(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetProperties();
        }

        public static PropertyInfo GetRuntimeProperty(this Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetProperty(name);
        }
    }
}

#endif