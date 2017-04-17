#if FAT

using System;
using System.IO;
using System.Reflection;
using System.Threading;

#if !NETCOREAPP1_1
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace Theraot.Core
{
    public static class CloneHelper<T>
    {
        public static ICloner<T> GetCloner()
        {
            var type = typeof(T);
            return GetStructCloner(type)
                   ?? GetGenericCloner(type)
#if !NETCOREAPP1_1
                   ?? GetObjectCloner(type)
#endif
                   ?? GetMockCloner(type)
                   ?? GetConstructorCloner(type)
                   ?? GetDeconstructCloner(type)
#if !NETCOREAPP1_1
                   ?? GetSerializerCloner(type)
#endif
                ;
        }

        private static ICloner<T> GetConstructorCloner(Type type)
        {
            var constructorInfo = type.GetConstructor(new[] { type });
            if (constructorInfo == null)
            {
                return null;
            }
            return ConstructorCloner.GetInstance(constructorInfo);
        }

        private static ICloner<T> GetDeconstructCloner(Type type)
        {
            var pair = TypeHelper.GetConstructorDeconstructPair(type);
            if (pair == null)
            {
                return null;
            }
            return DeconstructCloner.GetInstance(pair.Item1, pair.Item2);
        }

        private static ICloner<T> GetGenericCloner(Type type)
        {
            if (type.IsImplementationOf(typeof(ICloneable<T>)))
            {
                return GenericCloner.GetInstance();
            }
            return null;
        }

        private static ICloner<T> GetMockCloner(Type type)
        {
            var method = type.GetMethod("Clone", Type.EmptyTypes);
            if (method != null && (method.ReturnType == type || method.ReturnType == typeof(object)))
            {
                return MockCloner.GetInstance(method);
            }
            return null;
        }

#if !NETCOREAPP1_1
        private static ICloner<T> GetObjectCloner(Type type)
        {
            if (type.IsImplementationOf(typeof(ICloneable)))
            {
                return Cloner.GetInstance();
            }
            return null;
        }

        private static ICloner<T> GetSerializerCloner(Type type)
        {
            if (type.IsSerializable)
            {
                return SerializerCloner.GetInstance();
            }
            return null;
        }
#endif

        private static ICloner<T> GetStructCloner(Type type)
        {
            if (type.IsValueTypeRecursive())
            {
                return StructCloner.GetInstance();
            }
            return null;
        }

#if !NETCOREAPP1_1
        private class Cloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new Cloner(); // We need a cloner per T type.

            private Cloner()
            {
                //Empty
            }

            public static ICloner<T> GetInstance()
            {
                return _instance;
            }

            public T Clone(T target)
            {
                // Only called with ICloneable target
                // No need to check for null - let it throw
                return (T)(target as ICloneable).Clone();
            }
        }
#endif

        private class ConstructorCloner : ICloner<T>
        {
            private static ICloner<T> _instance; // We need a cloner per T type.

            private readonly ConstructorInfo _constructor;

            private ConstructorCloner(ConstructorInfo constructor)
            {
                _constructor = constructor;
            }

            public static ICloner<T> GetInstance(ConstructorInfo constructor)
            {
                var found = _instance;
                if (found == null)
                {
                    var created = new ConstructorCloner(constructor);
                    found = Interlocked.CompareExchange(ref _instance, created, null);
                    if (found == null)
                    {
                        return created;
                    }
                }
                return found;
            }

            public T Clone(T target)
            {
                return (T)_constructor.Invoke(new object[] { target });
            }
        }

        private class DeconstructCloner : ICloner<T>
        {
            private static ICloner<T> _instance; // We need a cloner per T type.

            private readonly ConstructorInfo _constructor;
            private readonly MethodInfo _deconstruct;

            private DeconstructCloner(MethodInfo deconstruct, ConstructorInfo constructor)
            {
                _deconstruct = deconstruct;
                _constructor = constructor;
            }

            public static ICloner<T> GetInstance(MethodInfo deconstruct, ConstructorInfo constructor)
            {
                var found = _instance;
                if (found == null)
                {
                    var created = new DeconstructCloner(deconstruct, constructor);
                    found = Interlocked.CompareExchange(ref _instance, created, null);
                    if (found == null)
                    {
                        return created;
                    }
                }
                return found;
            }

            public T Clone(T target)
            {
                var parameters = new object[] { _deconstruct.GetParameters().Length };
                _deconstruct.Invoke(target, parameters);
                return (T)_constructor.Invoke(parameters);
            }
        }

        private class GenericCloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new GenericCloner(); // We need a cloner per T type.

            private GenericCloner()
            {
                //Empty
            }

            public static ICloner<T> GetInstance()
            {
                return _instance;
            }

            public T Clone(T target)
            {
                // Only called with ICloneable<T> target
                // No need to check for null - let it throw
                return (target as ICloneable<T>).Clone();
            }
        }

        private class MockCloner : ICloner<T>
        {
            private static ICloner<T> _instance; // We need a cloner per T type.

            private readonly MethodInfo _method;

            private MockCloner(MethodInfo method)
            {
                _method = method;
            }

            public static ICloner<T> GetInstance(MethodInfo method)
            {
                var found = _instance;
                if (found == null)
                {
                    var created = new MockCloner(method);
                    found = Interlocked.CompareExchange(ref _instance, created, null);
                    if (found == null)
                    {
                        return created;
                    }
                }
                return found;
            }

            public T Clone(T target)
            {
                return (T)_method.Invoke(target, TypeHelper.EmptyObjects);
            }
        }

#if !NETCOREAPP1_1
        private class SerializerCloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new SerializerCloner(); // We need a cloner per T type.

            private SerializerCloner()
            {
                //Empty
            }

            public static ICloner<T> GetInstance()
            {
                return _instance;
            }

            public T Clone(T target)
            {
                var formatter = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, target);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
        }
#endif

        private class StructCloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new StructCloner(); // We need a cloner per T type.

            private StructCloner()
            {
                //Empty
            }

            public static ICloner<T> GetInstance()
            {
                return _instance;
            }

            public T Clone(T target)
            {
                return target;
            }
        }
    }
}

#endif