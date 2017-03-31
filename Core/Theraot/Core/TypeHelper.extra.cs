#if FAT

using System;
using System.Linq;
using System.Reflection;
using Theraot.Collections;

namespace Theraot.Core
{
    public static partial class TypeHelper
    {
        public static ILookup<string, Type> GetNamespaces(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            return ProgressiveLookup<string, Type>.Create(assembly.GetTypes(), type => type.Namespace);
        }
    }
}

#endif