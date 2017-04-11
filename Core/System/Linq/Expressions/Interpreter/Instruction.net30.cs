#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal interface IInstructionProvider
    {
        void AddInstructions(LightCompiler compiler);
    }

    internal abstract class Instruction
    {
        public const int UnknownInstrIndex = int.MaxValue;

        public virtual int ConsumedStack
        {
            get { return 0; }
        }

        public virtual int ProducedStack
        {
            get { return 0; }
        }

        public virtual int ConsumedContinuations
        {
            get { return 0; }
        }

        public virtual int ProducedContinuations
        {
            get { return 0; }
        }

        public int StackBalance
        {
            get { return ProducedStack - ConsumedStack; }
        }

        public int ContinuationsBalance
        {
            get { return ProducedContinuations - ConsumedContinuations; }
        }

        public abstract int Run(InterpretedFrame frame);

        public virtual string InstructionName
        {
            get { return "<Unknown>"; }
        }

        public override string ToString()
        {
            return InstructionName + "()";
        }

        public virtual string ToDebugString(int instructionIndex, object cookie, Func<int, int> labelIndexer, IList<object> objects)
        {
            return ToString();
        }

        public virtual object GetDebugCookie(LightCompiler compiler)
        {
            return null;
        }
    }

    internal abstract class NotInstruction : Instruction
    {
        public static Instruction _bool, _int64, _int32, _int16, _uint64, _uint32, _uint16, _byte, _sbyte;

        private NotInstruction()
        {
        }

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "Not"; }
        }

        private class BoolNot : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((bool)value ? ScriptingRuntimeHelpers.False : ScriptingRuntimeHelpers.True);
                }
                return +1;
            }
        }

        private class Int64Not : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(Int64)value);
                }
                return +1;
            }
        }

        private class Int32Not : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(Int32)value);
                }
                return +1;
            }
        }

        private class Int16Not : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((Int16)(~(Int16)value));
                }
                return +1;
            }
        }

        private class UInt64Not : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(UInt64)value);
                }
                return +1;
            }
        }

        private class UInt32Not : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(UInt32)value);
                }
                return +1;
            }
        }

        private class UInt16Not : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((UInt16)(~(UInt16)value));
                }
                return +1;
            }
        }

        private class ByteNot : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((object)(Byte)(~(Byte)value));
                }
                return +1;
            }
        }

        private class SByteNot : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((object)(SByte)(~(SByte)value));
                }
                return +1;
            }
        }

        public static Instruction Create(Type t)
        {
            switch (t.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Boolean:
                    return _bool ?? (_bool = new BoolNot());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new Int64Not());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new Int32Not());

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new Int16Not());

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new UInt64Not());

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new UInt32Not());

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new UInt16Not());

                case TypeCode.Byte:
                    return _byte ?? (_byte = new ByteNot());

                case TypeCode.SByte:
                    return _sbyte ?? (_sbyte = new SByteNot());

                default:
                    throw new InvalidOperationException("Not for " + t);
            }
        }
    }
}

#endif