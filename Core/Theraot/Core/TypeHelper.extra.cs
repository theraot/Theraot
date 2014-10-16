#if FAT

using System;
using System.Collections.Generic;
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
            else
            {
                var types = assembly.GetTypes();
                int index = 0;
                return new ProgressiveLookup<string, Type>
                (
                    EnumerableHelper.Create
                    (
                        () =>
                        {
                            index++;
                            return index < types.Length;
                        },
                        () => new KeyValuePair<string, Type>(types[index].Namespace, types[index])
                    )
                );
            }
        }
    }
}

#endif