#if LESSTHAN_NET35

#pragma warning disable CA1305 // Specify IFormatProvider

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Interpreter
{
    internal static class ConvertHelper
    {
        public static int ToInt32NoNull(object val)
        {
            return val == null
                ? throw new InvalidOperationException()
                : Convert.ToInt32(val, null);
        }
    }

    internal sealed class ArrayLengthInstruction : Instruction
    {
        public static readonly ArrayLengthInstruction Instance = new();

        private ArrayLengthInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "ArrayLength";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var obj = frame.Pop()!;
            frame.Push(((Array)obj).Length);
            return 1;
        }
    }

    internal sealed class GetArrayItemInstruction : Instruction
    {
        internal static readonly GetArrayItemInstruction Instance = new();

        private GetArrayItemInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "GetArrayItem";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var index = ConvertHelper.ToInt32NoNull(frame.Pop()!);
            var array = (Array)frame.Pop()!;
            frame.Push(array.GetValue(index));
            return 1;
        }
    }

    internal sealed class NewArrayBoundsInstruction : Instruction
    {
        private readonly Type _elementType;

        internal NewArrayBoundsInstruction(Type elementType, int rank)
        {
            _elementType = elementType;
            ConsumedStack = rank;
        }

        public override int ConsumedStack { get; }
        public override string InstructionName => "NewArrayBounds";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var lengths = new int[ConsumedStack];
            for (var i = ConsumedStack - 1; i >= 0; i--)
            {
                var length = ConvertHelper.ToInt32NoNull(frame.Pop()!);

                if (length < 0)
                {
                    // to make behavior aligned with array creation emitted by C# compiler
                    throw new OverflowException();
                }

                lengths[i] = length;
            }

            var array = Array.CreateInstance(_elementType, lengths);
            frame.Push(array);
            return 1;
        }
    }

    internal sealed class NewArrayInitInstruction : Instruction
    {
        private readonly Type _elementType;

        internal NewArrayInitInstruction(Type elementType, int elementCount)
        {
            _elementType = elementType;
            ConsumedStack = elementCount;
        }

        public override int ConsumedStack { get; }
        public override string InstructionName => "NewArrayInit";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var array = Array.CreateInstance(_elementType, ConsumedStack);
            for (var i = ConsumedStack - 1; i >= 0; i--)
            {
                array.SetValue(frame.Pop(), i);
            }

            frame.Push(array);
            return 1;
        }
    }

    internal sealed class NewArrayInstruction : Instruction
    {
        private readonly Type _elementType;

        internal NewArrayInstruction(Type elementType)
        {
            _elementType = elementType;
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "NewArray";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var length = ConvertHelper.ToInt32NoNull(frame.Pop()!);
            // To make behavior aligned with array creation emitted by C# compiler if length is less than
            // zero we try to use it to create an array, which will throw an OverflowException with the
            // correct localized error message.
            frame.Push(length < 0 ? new int[length] : Array.CreateInstance(_elementType, length));
            return 1;
        }
    }

    internal sealed class SetArrayItemInstruction : Instruction
    {
        internal static readonly SetArrayItemInstruction Instance = new();

        private SetArrayItemInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 3;
        public override string InstructionName => "SetArrayItem";

        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();
            var index = ConvertHelper.ToInt32NoNull(frame.Pop()!);
            var array = (Array)frame.Pop()!;
            array.SetValue(value, index);
            return 1;
        }
    }
}

#endif