#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class DefaultValueInstruction : Instruction
    {
        private readonly Type _type;

        internal DefaultValueInstruction(Type type)
        {
            Debug.Assert(type.IsValueType);
            Debug.Assert(!type.IsNullable());
            _type = type;
        }

        public override string InstructionName => "DefaultValue";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            frame.Push(Activator.CreateInstance(_type));
            return 1;
        }

        public override string ToString()
        {
            return "DefaultValue " + _type;
        }
    }
}

#endif