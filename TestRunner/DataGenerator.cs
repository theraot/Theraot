using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Reflection;

namespace TestRunner
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DataGeneratorAttribute : Attribute {}

    public static class DataGenerator
    {
        private static readonly Dictionary<Type, Delegate> _dataGenerators = FindAllGenerators();
        private static readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        private static Dictionary<Type, Delegate> FindAllGenerators()
        {
            var result = new Dictionary<Type, Delegate>();
            var dataGeneratorsType = typeof(DataGenerator);
            var assembly = dataGeneratorsType.GetTypeInfo().Assembly;
            var pairs = assembly.GetExportedTypes()
                .SelectMany(t => t.GetTypeInfo().GetMethods())
                .Where(IsGeneratorMethod)
                .Select(GetPair);
            foreach (var pair in pairs)
            {
                var type = pair.Key;
                var @delegate = pair.Value;
                if (@delegate == null)
                {
                    continue;
                }
                result.TryAdd(type, @delegate);
            }
            return result;
        }

        private static KeyValuePair<Type, Delegate> GetPair(MethodInfo methodInfo)
        {
            Delegate @delegate;
            if (methodInfo.IsStatic)
            {
                @delegate = TypeHelper.BuildDelegate(methodInfo, null);
            }
            else
            {
                var declaringType = methodInfo.DeclaringType;
                if (declaringType == null)
                {
                    throw new InvalidOperationException();
                }
                if (!_instances.TryGetValue(declaringType, out var instance))
                {
                    try
                    {
                        instance = Activator.CreateInstance(declaringType);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                    _instances[declaringType] = instance;
                }
                if (instance == null)
                {
                    @delegate = null;
                }
                else
                {
                    @delegate = TypeHelper.BuildDelegate(methodInfo, instance);
                }
            }
            return new KeyValuePair<Type, Delegate>(methodInfo.GetReturnType(), @delegate);
        }

        private static bool IsGeneratorMethod(MethodInfo methodInfo)
        {
            return methodInfo.HasAttribute<DataGeneratorAttribute>() && methodInfo.DeclaringType != null &&
                   methodInfo.GetParameters().Length == 0;
        }

        public static object Get(Type type)
        {
            if (_dataGenerators.TryGetValue(type, out var generator))
            {
                return generator.DynamicInvoke(ArrayReservoir<object>.EmptyArray);
            }
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }

    public static class StringGenerator
    {
        private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        [DataGenerator]
        public static string GenerateString()
        {
            var random = new Random();
            var length = random.Next(1, 16);
            var stringBuilder = new StringBuilder(length);
            for (int index = 0; index < length; index++)
            {
                stringBuilder.Append(_chars[random.Next(0, _chars.Length)]);
            }
            return stringBuilder.ToString();
        }
    }
}
