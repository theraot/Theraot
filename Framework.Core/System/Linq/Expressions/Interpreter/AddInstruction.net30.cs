#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class AddInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uint16, _uint32, _uint64, _single, _double;

        public override int ConsumedStack
        {
            get { return 2; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "Add"; }
        }

        private AddInstruction()
        {
        }

        internal sealed class AddInt32 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject(unchecked((int)l + (int)r));
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddInt16 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (short)unchecked((short)l + (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddInt64 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = unchecked((long)l + (long)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddUInt16 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (ushort)unchecked((ushort)l + (ushort)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddUInt32 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = unchecked((uint)l + (uint)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddUInt64 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = unchecked((ulong)l + (ulong)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddSingle : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (float)l + (float)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddDouble : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (double)l + (double)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new AddInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new AddInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new AddInt64());

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new AddUInt16());

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new AddUInt32());

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new AddUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new AddSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new AddDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Add", type);
            }
        }

        public override string ToString()
        {
            return "Add()";
        }
    }

    internal abstract class AddOvfInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uint16, _uint32, _uint64, _single, _double;

        public override int ConsumedStack
        {
            get { return 2; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "AddOvf"; }
        }

        private AddOvfInstruction()
        {
        }

        internal sealed class AddOvfInt32 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject(checked((int)l + (int)r));
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddOvfInt16 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (short)checked((short)l + (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddOvfInt64 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = checked((long)l + (long)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddOvfUInt16 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (ushort)checked((ushort)l + (ushort)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddOvfUInt32 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = checked((uint)l + (uint)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddOvfUInt64 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (ulong)checked((short)l + (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddOvfSingle : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (float)l + (float)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class AddOvfDouble : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (double)l + (double)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new AddOvfInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new AddOvfInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new AddOvfInt64());

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new AddOvfUInt16());

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new AddOvfUInt32());

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new AddOvfUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new AddOvfSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new AddOvfDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("AddOvf", type);
            }
        }

        public override string ToString()
        {
            return "AddOvf()";
        }
    }
}

#endif