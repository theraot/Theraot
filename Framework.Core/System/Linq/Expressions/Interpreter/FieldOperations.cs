#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class FieldInstruction : Instruction
    {
        protected readonly FieldInfo Field;

        protected FieldInstruction(FieldInfo field)
        {
            Assert.NotNull(field);
            Field = field;
        }

        public override string ToString()
        {
            return InstructionName + "(" + Field + ")";
        }
    }

    internal sealed class LoadFieldInstruction : FieldInstruction
    {
        public LoadFieldInstruction(FieldInfo field)
            : base(field)
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "LoadField";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var self = frame.Pop();

            NullCheck(self);
            frame.Push(Field.GetValue(self));
            return 1;
        }
    }

    internal sealed class LoadStaticFieldInstruction : FieldInstruction
    {
        public LoadStaticFieldInstruction(FieldInfo field)
            : base(field)
        {
            Debug.Assert(field.IsStatic);
        }

        public override string InstructionName => "LoadStaticField";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            frame.Push(Field.GetValue(null));
            return 1;
        }
    }

    internal sealed class StoreFieldInstruction : FieldInstruction
    {
        public StoreFieldInstruction(FieldInfo field)
            : base(field)
        {
            Assert.NotNull(field);
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "StoreField";

        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();
            var self = frame.Pop();

            NullCheck(self);
            Field.SetValue(self, value);
            return 1;
        }
    }

    internal sealed class StoreStaticFieldInstruction : FieldInstruction
    {
        public StoreStaticFieldInstruction(FieldInfo field)
            : base(field)
        {
            Debug.Assert(field.IsStatic);
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "StoreStaticField";

        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();
            Field.SetValue(null, value);
            return 1;
        }
    }
}

#endif