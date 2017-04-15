#if FAT

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Theraot.Core
{
    public static class CloneHelper<T>
    {
        public static ICloner<T> GetCloner()
        {
            var type = typeof(T);
            if (type.IsValueTypeRecursive())
            {
                return StructCloner.Instance;
            }
            if (type.IsImplementationOf(typeof(ICloneable<T>)))
            {
                return GenericCloner.Instance;
            }
            if (type.IsImplementationOf(typeof(ICloneable)))
            {
                return Cloner.Instance;
            }
            if (type.IsSerializable)
            {
                return SerializerCloner.Instance;
            }
            return null;
        }

        private class Cloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new Cloner(); // We need a cloner per T type.

            private Cloner()
            {
                //Empty
            }

            public static ICloner<T> Instance
            {
                get
                {
                    return _instance;
                }
            }

            public T Clone(T target)
            {
                // Only called with ICloneable target
                // No need to check for null - let it throw
                return (T)(target as ICloneable).Clone();
            }
        }

        private class GenericCloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new GenericCloner(); // We need a cloner per T type.

            private GenericCloner()
            {
                //Empty
            }

            public static ICloner<T> Instance
            {
                get
                {
                    return _instance;
                }
            }

            public T Clone(T target)
            {
                // Only called with ICloneable<T> target
                // No need to check for null - let it throw
                return (target as ICloneable<T>).Clone();
            }
        }

        private class SerializerCloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new SerializerCloner(); // We need a cloner per T type.

            private SerializerCloner()
            {
                //Empty
            }

            public static ICloner<T> Instance
            {
                get
                {
                    return _instance;
                }
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

        private class StructCloner : ICloner<T>
        {
            private static readonly ICloner<T> _instance = new StructCloner(); // We need a cloner per T type.

            private StructCloner()
            {
                //Empty
            }

            public static ICloner<T> Instance
            {
                get
                {
                    return _instance;
                }
            }

            public T Clone(T target)
            {
                return target;
            }
        }
    }
}

#endif