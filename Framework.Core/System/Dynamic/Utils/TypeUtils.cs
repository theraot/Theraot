#if NET20 || NET30 || NET35 || NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    internal static class TypeUtils
    {
        public static bool AreEquivalent(Type t1, Type t2) => t1 != null && t1 == t2;

        public static void ValidateType(Type type, string paramName) => ValidateType(type, paramName, false, false);

        public static void ValidateType(Type type, string paramName, bool allowByRef, bool allowPointer)
        {
            if (ValidateType(type, paramName, -1))
            {
                if (!allowByRef && type.IsByRef)
                {
                    throw Error.TypeMustNotBeByRef(paramName);
                }

                if (!allowPointer && type.IsPointer)
                {
                    throw Error.TypeMustNotBePointer(paramName);
                }
            }
        }

        public static bool ValidateType(Type type, string paramName, int index)
        {
            if (type == typeof(void))
            {
                return false; // Caller can skip further checks.
            }

            if (type.ContainsGenericParameters)
            {
                throw type.IsGenericTypeDefinition
                    ? Error.TypeIsGeneric(type, paramName, index)
                    : Error.TypeContainsGenericParameters(type, paramName, index);
            }

            return true;
        }
    }
}

#endif