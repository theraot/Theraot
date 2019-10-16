#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Theraot.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal sealed class AssemblyGen
    {
        private static AssemblyGen? _assembly;
        private readonly ModuleBuilder _myModule;
        private int _index;

        private AssemblyGen()
        {
            var name = new AssemblyName("Snippets");

            var thisDomain = Thread.GetDomain();
            var myAssembly = thisDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            _myModule = myAssembly.DefineDynamicModule(name.Name);
        }

        private static AssemblyGen Assembly
        {
            get
            {
                return TypeHelper.LazyCreate(ref _assembly, () => new AssemblyGen());
            }
        }

        internal static TypeBuilder DefineDelegateType(string name)
        {
            return Assembly.DefineType
            (
                name,
                typeof(MulticastDelegate),
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass
            );
        }

        private TypeBuilder DefineType(string name, Type parent, TypeAttributes attr)
        {
            ContractUtils.RequiresNotNull(name, nameof(name));
            ContractUtils.RequiresNotNull(parent, nameof(parent));

            var sb = new StringBuilder(name);

            var index = Interlocked.Increment(ref _index);
            sb.Append("$");
            sb.Append(index);

            // An unhandled Exception: System.Runtime.InteropServices.COMException (0x80131130): Record not found on lookup.
            // is thrown if there is any of the characters []*&+,\ in the type name and a method defined on the type is called.
            sb.Replace('+', '_').Replace('[', '_').Replace(']', '_').Replace('*', '_').Replace('&', '_').Replace(',', '_').Replace('\\', '_');

            name = sb.ToString();

            return _myModule.DefineType(name, attr, parent);
        }
    }
}

#endif