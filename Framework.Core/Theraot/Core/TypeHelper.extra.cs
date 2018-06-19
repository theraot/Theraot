#if FAT

using System;
using System.Linq;
using System.Reflection;
using Theraot.Collections;

namespace Theraot.Core
{
    public static partial class TypeHelper
    {
        public static Tuple<MethodInfo, ConstructorInfo> GetConstructorDeconstructPair(Type type)
        {
            var deconstructs = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var deconstruct in deconstructs)
            {
                // If the method is not called Deconstruct, skip it
                if (deconstruct.Name != "Deconstruct")
                {
                    continue;
                }
                // If the method returns something, skip it
                if (deconstruct.ReturnType != typeof(void))
                {
                    continue;
                }
                var methodParameters = deconstruct.GetParameters();
                // Filter out all constructors that don't have the same number of parameters
                var next = new ConstructorInfo[constructors.Length];
                var nextIndex = 0;
                for (int index = 0; index < constructors.Length; index++)
                {
                    var constructorParameters = constructors[index].GetParameters();
                    if (constructorParameters.Length == methodParameters.Length)
                    {
                        next[nextIndex] = constructors[index];
                        nextIndex++;
                    }
                }
                // If no constructor matches the parameters, skip it
                if (nextIndex == 0)
                {
                    continue;
                }
                for (int parameterIndex = 0; parameterIndex < methodParameters.Length; parameterIndex++)
                {
                    // If no constructor matches the parameters, skip it
                    if (nextIndex == 0)
                    {
                        goto skip;
                    }
                    var candidates = next;
                    next = new ConstructorInfo[nextIndex];
                    nextIndex = 0;
                    // If the method has parameters that are not out, skip it
                    var methodParameter = methodParameters[parameterIndex];
                    if (!methodParameter.IsOut)
                    {
                        goto skip;
                    }
                    for (int index = 0; index < candidates.Length && candidates[index] != null; index++)
                    {
                        var constructorParameters = candidates[index].GetParameters();
                        var constructorParameter = constructorParameters[parameterIndex];
                        var constructorParameterType = constructorParameter.ParameterType;
                        if (!constructorParameterType.IsByRef && constructorParameterType.MakeByRefType() == methodParameter.ParameterType)
                        {
                            next[nextIndex] = constructors[index];
                            nextIndex++;
                        }
                    }
                }
                if (nextIndex > 0)
                {
                    return new Tuple<MethodInfo, ConstructorInfo>(deconstruct, next[0]);
                }
                skip:
                ;
            }
            return null;
        }

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