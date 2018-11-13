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
        public static Instruction Bool, INT64, INT32, INT16, Uint64, Uint32, Uint16, Byte, Sbyte;

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
                    frame.Push(~(long)value);
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
                    frame.Push(~(int)value);
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
                    frame.Push((short)~(short)value);
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
                    frame.Push(~(ulong)value);
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
                    frame.Push(~(uint)value);
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
                    frame.Push((ushort)~(ushort)value);
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
                    frame.Push((object)(byte)~(byte)value);
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
                    frame.Push((object)(sbyte)~(sbyte)value);
                }
                return +1;
            }
        }

        public static Instruction Create(Type t)
        {
            switch (t.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Boolean:
                    return Bool ?? (Bool = new BoolNot());

                case TypeCode.Int64:
                    return INT64 ?? (INT64 = new Int64Not());

                case TypeCode.Int32:
                    return INT32 ?? (INT32 = new Int32Not());

                case TypeCode.Int16:
                    return INT16 ?? (INT16 = new Int16Not());

                case TypeCode.UInt64:
                    return Uint64 ?? (Uint64 = new UInt64Not());

                case TypeCode.UInt32:
                    return Uint32 ?? (Uint32 = new UInt32Not());

                case TypeCode.UInt16:
                    return Uint16 ?? (Uint16 = new UInt16Not());

                case TypeCode.Byte:
                    return Byte ?? (Byte = new ByteNot());

                case TypeCode.SByte:
                    return Sbyte ?? (Sbyte = new SByteNot());

                default:
                    throw new InvalidOperationException("Not for " + t);
            }
        }
    }
}

#endif