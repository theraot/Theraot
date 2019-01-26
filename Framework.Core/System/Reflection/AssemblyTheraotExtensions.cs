#if LESSTHAN_NETSTANDARD15
using System.Linq;

namespace System.Reflection
{
    public static class AssemblyTheraotExtensions
    {
        public static Type[] GetTypes(this Assembly assembly)
        {
            return assembly.DefinedTypes.Select(typeInfo => typeInfo.AsType()).ToArray();
        }
    }
}

#endif