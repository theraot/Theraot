#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Linq;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static Type[] GetTypes(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new NullReferenceException(nameof(assembly));
            }

            return assembly.DefinedTypes.Select(typeInfo => typeInfo.AsType()).ToArray();
        }
    }
}

#endif