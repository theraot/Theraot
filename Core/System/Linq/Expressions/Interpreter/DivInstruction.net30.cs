#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class DivInstruction : Instruction
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
            get { return "Div"; }
        }

        private DivInstruction()
        {
        }

        internal sealed class DivInt32 : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject((Int32)l / (Int32)r);
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class DivInt16 : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Int16)((Int16)l / (Int16)r);
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class DivInt64 : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Int64)l / (Int64)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class DivUInt16 : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = (UInt16)((UInt16)l / (UInt16)r);
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class DivUInt32 : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = (UInt32)l / (UInt32)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class DivUInt64 : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = (UInt64)l / (UInt64)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class DivSingle : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Single)l / (Single)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class DivDouble : DivInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Double)l / (Double)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new DivInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new DivInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new DivInt64());

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new DivUInt16());

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new DivUInt32());

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new DivUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new DivSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new DivDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Div", type);
            }
        }

        public override string ToString()
        {
            return "Div()";
        }
    }

    internal abstract class ModuloInstruction : Instruction
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
            get { return "Modulo"; }
        }

        private ModuloInstruction()
        {
        }

        internal sealed class ModuloInt32 : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject((Int32)l % (Int32)r);
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class ModuloInt16 : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Int16)((Int16)l % (Int16)r);
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class ModuloInt64 : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Int64)l % (Int64)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class ModuloUInt16 : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = (UInt16)((UInt16)l % (UInt16)r);
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class ModuloUInt32 : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = (UInt32)l % (UInt32)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class ModuloUInt64 : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = (UInt64)l % (UInt64)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class ModuloSingle : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Single)l % (Single)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        internal sealed class ModuloDouble : ModuloInstruction
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
                    frame.Data[frame.StackIndex - 2] = (Double)l % (Double)r;
                }
                frame.StackIndex--;
                return 1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new ModuloInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new ModuloInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new ModuloInt64());

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new ModuloUInt16());

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new ModuloUInt32());

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new ModuloUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new ModuloSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new ModuloDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Modulo", type);
            }
        }

        public override string ToString()
        {
            return "Modulo()";
        }
    }
}

#endif