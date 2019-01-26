#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class DupInstruction : Instruction
    {
        internal static readonly DupInstruction Instance = new DupInstruction();

        private DupInstruction()
        {
            // Empty
        }

        public override string InstructionName => "Dup";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            frame.Dup();
            return 1;
        }
    }

    internal sealed class LoadCachedObjectInstruction : Instruction
    {
        private readonly uint _index;

        internal LoadCachedObjectInstruction(uint index)
        {
            _index = index;
        }

        public override string InstructionName => "LoadCachedObject";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex++] = frame.Interpreter.Objects[_index];
            return 1;
        }

        public override string ToDebugString(int instructionIndex, object cookie, Func<int, int> labelIndexer, IList<object> objects)
        {
            return string.Format(CultureInfo.InvariantCulture, "LoadCached({0}: {1})", _index, objects[(int)_index]);
        }

        public override string ToString()
        {
            return "LoadCached(" + _index + ")";
        }
    }

    internal sealed class LoadObjectInstruction : Instruction
    {
        private readonly object _value;

        internal LoadObjectInstruction(object value)
        {
            _value = value;
        }

        public override string InstructionName => "LoadObject";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            frame.Data[frame.StackIndex++] = _value;
            return 1;
        }

        public override string ToString()
        {
            return "LoadObject(" + (_value ?? "null") + ")";
        }
    }

    internal sealed class PopInstruction : Instruction
    {
        internal static readonly PopInstruction Instance = new PopInstruction();

        private PopInstruction()
        {
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "Pop";

        public override int Run(InterpretedFrame frame)
        {
            frame.Pop();
            return 1;
        }
    }
}

#endif