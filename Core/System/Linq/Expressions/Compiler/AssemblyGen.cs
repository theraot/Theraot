// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace System.Linq.Expressions.Compiler
{
    internal sealed class AssemblyGen
    {
        private static AssemblyGen _assembly;

        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        private int _index;

        private static AssemblyGen Assembly
        {
            get
            {
                if (_assembly == null)
                {
                    Interlocked.CompareExchange(ref _assembly, new AssemblyGen(), null);
                }
                return _assembly;
            }
        }

        private AssemblyGen()
        {
            var name = new AssemblyName("Snippets");

            // mark the assembly transparent so that it works in partial trust:
            var attributes = new[] {
                new CustomAttributeBuilder(typeof(SecurityTransparentAttribute).GetConstructor(Type.EmptyTypes), ArrayReservoir<object>.EmptyArray)
            };

            _assemblyBuilder = AssemblyBuilderEx.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run, attributes);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(name.Name);
        }

        private TypeBuilder DefineType(string name, Type parent, TypeAttributes attr)
        {
            ContractUtils.RequiresNotNull(name, "name");
            ContractUtils.RequiresNotNull(parent, "parent");

            var sb = new StringBuilder(name);

            var index = Interlocked.Increment(ref _index);
            sb.Append("$");
            sb.Append(index);

            // An unhandled Exception: System.Runtime.InteropServices.COMException (0x80131130): Record not found on lookup.
            // is thrown if there is any of the characters []*&+,\ in the type name and a method defined on the type is called.
            sb.Replace('+', '_').Replace('[', '_').Replace(']', '_').Replace('*', '_').Replace('&', '_').Replace(',', '_').Replace('\\', '_');

            name = sb.ToString();

            return _moduleBuilder.DefineType(name, attr, parent);
        }

        internal static TypeBuilder DefineDelegateType(string name)
        {
            return Assembly.DefineType(
                name,
                typeof(MulticastDelegate),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass
            );
        }
    }

    internal static class AssemblyBuilderEx
    {
        public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, CustomAttributeBuilder[] assemblyAttributes)
        {
            return Thread.GetDomain().DefineDynamicAssembly(name, access, assemblyAttributes);
        }
    }
}