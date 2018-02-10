#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class CreateDelegateInstruction : Instruction
    {
        private readonly LightDelegateCreator _creator;

        internal CreateDelegateInstruction(LightDelegateCreator delegateCreator)
        {
            _creator = delegateCreator;
        }

        public override int ConsumedStack
        {
            get { return _creator.Interpreter.ClosureSize; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "CreateDelegate"; }
        }

        public override int Run(InterpretedFrame frame)
        {
            IStrongBox[] closure;
            if (ConsumedStack > 0)
            {
                closure = new IStrongBox[ConsumedStack];
                for (var i = closure.Length - 1; i >= 0; i--)
                {
                    closure[i] = (IStrongBox)frame.Pop();
                }
            }
            else
            {
                closure = null;
            }

            var d = _creator.CreateDelegate(closure);

            frame.Push(d);
            return +1;
        }
    }

    internal sealed class NewInstruction : Instruction
    {
        private readonly ConstructorInfo _constructor;
        private readonly int _argCount;

        public NewInstruction(ConstructorInfo constructor)
        {
            _constructor = constructor;
            _argCount = constructor.GetParameters().Length;
        }

        public override int ConsumedStack
        {
            get { return _argCount; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "New"; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var args = new object[_argCount];
            for (var i = _argCount - 1; i >= 0; i--)
            {
                args[i] = frame.Pop();
            }

            object ret;
            try
            {
                ret = _constructor.Invoke(args);
            }
            catch (TargetInvocationException e)
            {
                ExceptionHelpers.UpdateForRethrow(e.InnerException);
                throw e.InnerException;
            }
            frame.Push(ret);
            return +1;
        }

        public override string ToString()
        {
            return "New " + _constructor.DeclaringType.Name + "(" + _constructor + ")";
        }
    }

    internal class ByRefNewInstruction : Instruction
    {
        private readonly ByRefUpdater[] _byrefArgs;
        private readonly ConstructorInfo _constructor;
        private readonly int _argCount;

        internal ByRefNewInstruction(ConstructorInfo target, ByRefUpdater[] byrefArgs)
        {
            _constructor = target;
            _argCount = target.GetParameters().Length;
            _byrefArgs = byrefArgs;
        }

        public override int ConsumedStack
        {
            get { return _argCount; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "ByRefNew"; }
        }

        public sealed override int Run(InterpretedFrame frame)
        {
            var args = new object[_argCount];
            for (var i = _argCount - 1; i >= 0; i--)
            {
                args[i] = frame.Pop();
            }

            try
            {
                object ret;
                try
                {
                    ret = _constructor.Invoke(args);
                }
                catch (TargetInvocationException e)
                {
                    throw ExceptionHelpers.UpdateForRethrow(e.InnerException);
                }

                frame.Push(ret);
            }
            finally
            {
                if (args != null)
                {
                    foreach (var arg in _byrefArgs)
                    {
                        arg.Update(frame, args[arg.ArgumentIndex]);
                    }
                }
            }

            return 1;
        }
    }

    internal sealed class DefaultValueInstruction : Instruction
    {
        private readonly Type _type;

        internal DefaultValueInstruction(Type type)
        {
            _type = type;
        }

        public override int ConsumedStack
        {
            get { return 0; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "DefaultValue"; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var value = _type.IsValueType ? Activator.CreateInstance(_type) : null;
            frame.Push(value);
            return +1;
        }

        public override string ToString()
        {
            return "New " + _type;
        }
    }

    internal sealed class TypeIsInstruction : Instruction
    {
        private readonly Type _type;

        internal TypeIsInstruction(Type type)
        {
            _type = type;
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
            get { return "TypeIs"; }
        }

        public override int Run(InterpretedFrame frame)
        {
            frame.Push(ScriptingRuntimeHelpers.BooleanToObject(_type.IsInstanceOfType(frame.Pop())));
            return +1;
        }

        public override string ToString()
        {
            return "TypeIs " + _type;
        }
    }

    internal sealed class TypeAsInstruction : Instruction
    {
        private readonly Type _type;

        internal TypeAsInstruction(Type type)
        {
            _type = type;
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
            get { return "TypeAs"; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();
            if (_type.IsInstanceOfType(value))
            {
                frame.Push(value);
            }
            else
            {
                frame.Push(null);
            }
            return +1;
        }

        public override string ToString()
        {
            return "TypeAs " + _type;
        }
    }

    internal sealed class TypeEqualsInstruction : Instruction
    {
        public static readonly TypeEqualsInstruction Instance = new TypeEqualsInstruction();

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
            get { return "TypeEquals"; }
        }

        private TypeEqualsInstruction()
        {
        }

        public override int Run(InterpretedFrame frame)
        {
            var type = frame.Pop();
            var obj = frame.Pop();
            frame.Push(ScriptingRuntimeHelpers.BooleanToObject(obj != null && ReferenceEquals(obj.GetType(), type)));
            return +1;
        }
    }

    internal sealed class NullableTypeEqualsInstruction : Instruction
    {
        public static readonly NullableTypeEqualsInstruction Instance = new NullableTypeEqualsInstruction();

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
            get { return "NullableTypeEquals"; }
        }

        private NullableTypeEqualsInstruction()
        {
        }

        public override int Run(InterpretedFrame frame)
        {
            var type = frame.Pop();
            var obj = frame.Pop();
            frame.Push(ScriptingRuntimeHelpers.BooleanToObject(obj != null && ReferenceEquals(obj.GetType(), type)));
            return +1;
        }
    }

    internal sealed class ArrayLengthInstruction : Instruction
    {
        public static readonly ArrayLengthInstruction Instance = new ArrayLengthInstruction();

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
            get { return "ArrayLength"; }
        }

        private ArrayLengthInstruction()
        {
        }

        public override int Run(InterpretedFrame frame)
        {
            var obj = frame.Pop();
            frame.Push(((Array)obj).Length);
            return +1;
        }
    }

    internal abstract class NegateInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _single, _double;

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
            get { return "Negate"; }
        }

        private NegateInstruction()
        {
        }

        internal sealed class NegateInt32 : NegateInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.Int32ToObject(unchecked(-(int)obj)));
                }
                return +1;
            }
        }

        internal sealed class NegateInt16 : NegateInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)unchecked(-(short)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateInt64 : NegateInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked(-(long)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateUInt16 : NegateInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)unchecked(-(ushort)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateUInt32 : NegateInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((int)unchecked(-(uint)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateSingle : NegateInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(-(float)obj);
                }
                return +1;
            }
        }

        internal sealed class NegateDouble : NegateInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(-(double)obj);
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new NegateInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new NegateInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new NegateInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new NegateUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new NegateUInt32());

                case TypeCode.Single:
                    return _single ?? (_single = new NegateSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new NegateDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Negate", type);
            }
        }

        public override string ToString()
        {
            return "Negate()";
        }
    }

    internal abstract class NegateCheckedInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _single, _double;

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
            get { return "NegateChecked"; }
        }

        private NegateCheckedInstruction()
        {
        }

        internal sealed class NegateCheckedInt32 : NegateCheckedInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.Int32ToObject(checked(-(int)obj)));
                }
                return +1;
            }
        }

        internal sealed class NegateCheckedInt16 : NegateCheckedInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)checked(-(short)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateCheckedInt64 : NegateCheckedInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(checked(-(long)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateCheckedUInt16 : NegateCheckedInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)checked(-(ushort)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateCheckedUInt32 : NegateCheckedInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((int)checked(-(uint)obj));
                }
                return +1;
            }
        }

        internal sealed class NegateCheckedSingle : NegateCheckedInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(-(float)obj);
                }
                return +1;
            }
        }

        internal sealed class NegateCheckedDouble : NegateCheckedInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(-(double)obj);
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new NegateCheckedInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new NegateCheckedInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new NegateCheckedInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new NegateCheckedUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new NegateCheckedUInt32());

                case TypeCode.Single:
                    return _single ?? (_single = new NegateCheckedSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new NegateCheckedDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("NegateChecked", type);
            }
        }

        public override string ToString()
        {
            return "NegateChecked()";
        }
    }

    internal abstract class IncrementInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _single, _double;

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
            get { return "Increment"; }
        }

        private IncrementInstruction()
        {
        }

        internal sealed class IncrementInt32 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.Int32ToObject(unchecked(1 + (int)obj)));
                }
                return +1;
            }
        }

        internal sealed class IncrementInt16 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)unchecked(1 + (short)obj));
                }
                return +1;
            }
        }

        internal sealed class IncrementInt64 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked(1 + (long)obj));
                }
                return +1;
            }
        }

        internal sealed class IncrementUInt16 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)unchecked(1 + (ushort)obj));
                }
                return +1;
            }
        }

        internal sealed class IncrementUInt32 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((int)unchecked(1 + (uint)obj));
                }
                return +1;
            }
        }

        internal sealed class IncrementSingle : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(1 + (float)obj);
                }
                return +1;
            }
        }

        internal sealed class IncrementDouble : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(1 + (double)obj);
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new IncrementInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new IncrementInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new IncrementInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new IncrementUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new IncrementUInt32());

                case TypeCode.Single:
                    return _single ?? (_single = new IncrementSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new IncrementDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Increment", type);
            }
        }

        public override string ToString()
        {
            return "Increment()";
        }
    }

    internal abstract class DecrementInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _single, _double;

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
            get { return "Decrement"; }
        }

        private DecrementInstruction()
        {
        }

        internal sealed class DecrementInt32 : DecrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.Int32ToObject(unchecked((int)obj - 1)));
                }
                return +1;
            }
        }

        internal sealed class DecrementInt16 : DecrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)unchecked((short)obj - 1));
                }
                return +1;
            }
        }

        internal sealed class DecrementInt64 : DecrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked((long)obj - 1));
                }
                return +1;
            }
        }

        internal sealed class DecrementUInt16 : DecrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)unchecked((ushort)obj - 1));
                }
                return +1;
            }
        }

        internal sealed class DecrementUInt32 : DecrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked((uint)obj - 1));
                }
                return +1;
            }
        }

        internal sealed class DecrementSingle : DecrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((float)obj - 1);
                }
                return +1;
            }
        }

        internal sealed class DecrementDouble : DecrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((double)obj - 1);
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new DecrementInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new DecrementInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new DecrementInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new DecrementUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new DecrementUInt32());

                case TypeCode.Single:
                    return _single ?? (_single = new DecrementSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new DecrementDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Decrement", type);
            }
        }

        public override string ToString()
        {
            return "Decrement()";
        }
    }

    internal abstract class LeftShiftInstruction : Instruction
    {
        // Perf: LeftShiftityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64;

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
            get { return "LeftShift"; }
        }

        private LeftShiftInstruction()
        {
        }

        internal sealed class LeftShiftSByte : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((sbyte)(((sbyte)value) << ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class LeftShiftInt16 : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)(((short)value) << ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class LeftShiftInt32 : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((int)value) << ((int)shift));
                }
                return +1;
            }
        }

        internal sealed class LeftShiftInt64 : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((long)value) << ((int)shift));
                }
                return +1;
            }
        }

        internal sealed class LeftShiftByte : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((byte)(((byte)value) << ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class LeftShiftUInt16 : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((ushort)(((ushort)value) << ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class LeftShiftUInt32 : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((uint)value) << ((int)shift));
                }
                return +1;
            }
        }

        internal sealed class LeftShiftUInt64 : LeftShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((ulong)value) << ((int)shift));
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            // Boxed enums can be unboxed as their underlying types:
            switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
            {
                case TypeCode.SByte:
                    return _sByte ?? (_sByte = new LeftShiftSByte());

                case TypeCode.Byte:
                    return _byte ?? (_byte = new LeftShiftByte());

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new LeftShiftInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new LeftShiftInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new LeftShiftInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new LeftShiftUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new LeftShiftUInt32());

                case TypeCode.UInt64:
                    return _uInt64 ?? (_uInt64 = new LeftShiftUInt64());

                default:
                    throw Error.ExpressionNotSupportedForType("LeftShift", type);
            }
        }

        public override string ToString()
        {
            return "LeftShift()";
        }
    }

    internal abstract class RightShiftInstruction : Instruction
    {
        // Perf: RightShiftityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64;

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
            get { return "RightShift"; }
        }

        private RightShiftInstruction()
        {
        }

        internal sealed class RightShiftSByte : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((sbyte)(((sbyte)value) >> ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class RightShiftInt16 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)(((short)value) >> ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class RightShiftInt32 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((int)value) >> ((int)shift));
                }
                return +1;
            }
        }

        internal sealed class RightShiftInt64 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((long)value) >> ((int)shift));
                }
                return +1;
            }
        }

        internal sealed class RightShiftByte : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((byte)(((byte)value) >> ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class RightShiftUInt16 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((ushort)(((ushort)value) >> ((int)shift)));
                }
                return +1;
            }
        }

        internal sealed class RightShiftUInt32 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((uint)value) >> ((int)shift));
                }
                return +1;
            }
        }

        internal sealed class RightShiftUInt64 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(((ulong)value) >> ((int)shift));
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            // Boxed enums can be unboxed as their underlying types:
            switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
            {
                case TypeCode.SByte:
                    return _sByte ?? (_sByte = new RightShiftSByte());

                case TypeCode.Byte:
                    return _byte ?? (_byte = new RightShiftByte());

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new RightShiftInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new RightShiftInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new RightShiftInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new RightShiftUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new RightShiftUInt32());

                case TypeCode.UInt64:
                    return _uInt64 ?? (_uInt64 = new RightShiftUInt64());

                default:
                    throw Error.ExpressionNotSupportedForType("RightShift", type);
            }
        }

        public override string ToString()
        {
            return "RightShift()";
        }
    }

    internal abstract class ExclusiveOrInstruction : Instruction
    {
        // Perf: ExclusiveOrityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _bool;

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
            get { return "ExclusiveOr"; }
        }

        private ExclusiveOrInstruction()
        {
        }

        internal sealed class ExclusiveOrSByte : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((sbyte)(((sbyte)left) ^ ((sbyte)right)));
                return +1;
            }
        }

        internal sealed class ExclusiveOrInt16 : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((short)(((short)left) ^ ((short)right)));
                return +1;
            }
        }

        internal sealed class ExclusiveOrInt32 : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((int)left) ^ ((int)right));
                return +1;
            }
        }

        internal sealed class ExclusiveOrInt64 : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((long)left) ^ ((long)right));
                return +1;
            }
        }

        internal sealed class ExclusiveOrByte : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((byte)(((byte)left) ^ ((byte)right)));
                return +1;
            }
        }

        internal sealed class ExclusiveOrUInt16 : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((ushort)(((ushort)left) ^ ((ushort)right)));
                return +1;
            }
        }

        internal sealed class ExclusiveOrUInt32 : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((uint)left) ^ ((uint)right));
                return +1;
            }
        }

        internal sealed class ExclusiveOrUInt64 : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((ulong)left) ^ ((ulong)right));
                return +1;
            }
        }

        internal sealed class ExclusiveOrBool : ExclusiveOrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((bool)left) ^ ((bool)right));
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            // Boxed enums can be unboxed as their underlying types:
            switch (GetTypeCode(type))
            {
                case TypeCode.SByte:
                    return _sByte ?? (_sByte = new ExclusiveOrSByte());

                case TypeCode.Byte:
                    return _byte ?? (_byte = new ExclusiveOrByte());

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new ExclusiveOrInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new ExclusiveOrInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new ExclusiveOrInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new ExclusiveOrUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new ExclusiveOrUInt32());

                case TypeCode.UInt64:
                    return _uInt64 ?? (_uInt64 = new ExclusiveOrUInt64());

                case TypeCode.Boolean:
                    return _bool ?? (_bool = new ExclusiveOrBool());

                default:
                    throw Error.ExpressionNotSupportedForType("ExclusiveOr", type);
            }
        }

        private static TypeCode GetTypeCode(Type type)
        {
            return (type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode();
        }

        public override string ToString()
        {
            return "ExclusiveOr()";
        }
    }

    internal abstract class OrInstruction : Instruction
    {
        // Perf: OrityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _bool;

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
            get { return "Or"; }
        }

        private OrInstruction()
        {
        }

        internal sealed class OrSByte : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((sbyte)(((sbyte)left) | ((sbyte)right)));
                return +1;
            }
        }

        internal sealed class OrInt16 : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((short)(((short)left) | ((short)right)));
                return +1;
            }
        }

        internal sealed class OrInt32 : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((int)left) | ((int)right));
                return +1;
            }
        }

        internal sealed class OrInt64 : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((long)left) | ((long)right));
                return +1;
            }
        }

        internal sealed class OrByte : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((byte)(((byte)left) | ((byte)right)));
                return +1;
            }
        }

        internal sealed class OrUInt16 : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((ushort)(((ushort)left) | ((ushort)right)));
                return +1;
            }
        }

        internal sealed class OrUInt32 : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((uint)left) | ((uint)right));
                return +1;
            }
        }

        internal sealed class OrUInt64 : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((ulong)left) | ((ulong)right));
                return +1;
            }
        }

        internal sealed class OrBool : OrInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    if (right == null)
                    {
                        frame.Push(null);
                    }
                    else
                    {
                        frame.Push((bool)right ? ScriptingRuntimeHelpers.True : null);
                    }
                    return +1;
                }
                if (right == null)
                {
                    frame.Push((bool)left ? ScriptingRuntimeHelpers.True : null);
                    return +1;
                }
                frame.Push(((bool)left) | ((bool)right));
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            // Boxed enums can be unboxed as their underlying types:
            switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
            {
                case TypeCode.SByte:
                    return _sByte ?? (_sByte = new OrSByte());

                case TypeCode.Byte:
                    return _byte ?? (_byte = new OrByte());

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new OrInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new OrInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new OrInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new OrUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new OrUInt32());

                case TypeCode.UInt64:
                    return _uInt64 ?? (_uInt64 = new OrUInt64());

                case TypeCode.Boolean:
                    return _bool ?? (_bool = new OrBool());

                default:
                    throw Error.ExpressionNotSupportedForType("Or", type);
            }
        }

        public override string ToString()
        {
            return "Or()";
        }
    }

    internal abstract class AndInstruction : Instruction
    {
        // Perf: AndityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _bool;

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
            get { return "And"; }
        }

        private AndInstruction()
        {
        }

        internal sealed class AndSByte : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((sbyte)(((sbyte)left) & ((sbyte)right)));
                return +1;
            }
        }

        internal sealed class AndInt16 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((short)(((short)left) & ((short)right)));
                return +1;
            }
        }

        internal sealed class AndInt32 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((int)left) & ((int)right));
                return +1;
            }
        }

        internal sealed class AndInt64 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((long)left) & ((long)right));
                return +1;
            }
        }

        internal sealed class AndByte : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((byte)(((byte)left) & ((byte)right)));
                return +1;
            }
        }

        internal sealed class AndUInt16 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push((ushort)(((ushort)left) & ((ushort)right)));
                return +1;
            }
        }

        internal sealed class AndUInt32 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((uint)left) & ((uint)right));
                return +1;
            }
        }

        internal sealed class AndUInt64 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return +1;
                }
                frame.Push(((ulong)left) & ((ulong)right));
                return +1;
            }
        }

        internal sealed class AndBool : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    if (right == null)
                    {
                        frame.Push(null);
                    }
                    else
                    {
                        frame.Push((bool)right ? null : ScriptingRuntimeHelpers.False);
                    }
                    return +1;
                }
                if (right == null)
                {
                    frame.Push((bool)left ? null : ScriptingRuntimeHelpers.False);
                    return +1;
                }
                frame.Push(((bool)left) & ((bool)right));
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            // Boxed enums can be unboxed as their underlying types:
            switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
            {
                case TypeCode.SByte:
                    return _sByte ?? (_sByte = new AndSByte());

                case TypeCode.Byte:
                    return _byte ?? (_byte = new AndByte());

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new AndInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new AndInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new AndInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new AndUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new AndUInt32());

                case TypeCode.UInt64:
                    return _uInt64 ?? (_uInt64 = new AndUInt64());

                case TypeCode.Boolean:
                    return _bool ?? (_bool = new AndBool());

                default:
                    throw Error.ExpressionNotSupportedForType("And", type);
            }
        }

        public override string ToString()
        {
            return "And()";
        }
    }

    internal abstract class NullableMethodCallInstruction : Instruction
    {
        private static NullableMethodCallInstruction _hasValue, _value, _equals, _getHashCode, _getValueOrDefault, _getValueOrDefault1, _toString;

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
            get { return "NullableMethod"; }
        }

        private NullableMethodCallInstruction()
        {
        }

        private class HasValue : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                frame.Push(ScriptingRuntimeHelpers.BooleanToObject(obj != null));
                return +1;
            }
        }

        private class GetValue : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    throw new InvalidOperationException();
                }
                frame.Push(obj);
                return +1;
            }
        }

        private class GetValueOrDefault : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                // we're already unboxed in the interpreter, so this is a nop.
                return +1;
            }
        }

        private class GetValueOrDefault1 : NullableMethodCallInstruction
        {
            public override int ConsumedStack
            {
                get { return 2; }
            }

            public override int Run(InterpretedFrame frame)
            {
                var dflt = frame.Pop();
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(dflt);
                }
                else
                {
                    frame.Push(obj);
                }
                return +1;
            }
        }

        private class EqualsClass : NullableMethodCallInstruction
        {
            public override int ConsumedStack
            {
                get { return 2; }
            }

            public override int Run(InterpretedFrame frame)
            {
                var other = frame.Pop();
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(other == null));
                }
                else if (other == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(obj.Equals(other)));
                }
                return +1;
            }
        }

        private class ToStringClass : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push("");
                }
                else
                {
                    frame.Push(obj.ToString());
                }
                return +1;
            }
        }

        private class GetHashCodeClass : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.Int32ToObject(0));
                }
                else
                {
                    frame.Push(obj.GetHashCode());
                }
                return +1;
            }
        }

        public static Instruction Create(string method, int argCount)
        {
            switch (method)
            {
                case "get_HasValue":
                    return _hasValue ?? (_hasValue = new HasValue());

                case "get_Value":
                    return _value ?? (_value = new GetValue());

                case "Equals":
                    return _equals ?? (_equals = new EqualsClass());

                case "GetHashCode":
                    return _getHashCode ?? (_getHashCode = new GetHashCodeClass());

                case "GetValueOrDefault":
                    if (argCount == 0)
                    {
                        return _getValueOrDefault ?? (_getValueOrDefault = new GetValueOrDefault());
                    }
                    else
                    {
                        return _getValueOrDefault1 ?? (_getValueOrDefault1 = new GetValueOrDefault1());
                    }
                case "ToString":
                    return _toString ?? (_toString = new ToStringClass());

                default:
                    // System.Nullable doesn't have other instance methods
                    throw Assert.Unreachable;
            }
        }
    }

    internal abstract class CastInstruction : Instruction
    {
        private static CastInstruction _boolean, _byte, _char, _dateTime, _decimal, _double, _int16, _int32, _int64, _sByte, _single, _string, _uInt16, _uInt32, _uInt64;

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
            get { return "Cast"; }
        }

        private class CastInstructionT<T> : CastInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value != null)
                {
                    frame.Push((T)value);
                }
                else
                {
                    frame.Push(null);
                }
                return +1;
            }
        }

        private class CastInstructionNoT : CastInstruction
        {
            private readonly Type _t;

            public CastInstructionNoT(Type t)
            {
                _t = t;
            }

            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value != null)
                {
                    if (!TypeHelper.HasReferenceConversion(value.GetType(), _t) &&
                        !TypeHelper.HasIdentityPrimitiveOrNullableConversion(value.GetType(), _t))
                    {
                        throw new InvalidCastException();
                    }
                    frame.Push(value);
                }
                else
                {
                    frame.Push(null);
                }
                return +1;
            }
        }

        public static Instruction Create(Type t)
        {
            if (!t.IsEnum)
            {
                switch (t.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return _boolean ?? (_boolean = new CastInstructionT<bool>());

                    case TypeCode.Byte:
                        return _byte ?? (_byte = new CastInstructionT<byte>());

                    case TypeCode.Char:
                        return _char ?? (_char = new CastInstructionT<char>());

                    case TypeCode.DateTime:
                        return _dateTime ?? (_dateTime = new CastInstructionT<DateTime>());

                    case TypeCode.Decimal:
                        return _decimal ?? (_decimal = new CastInstructionT<decimal>());

                    case TypeCode.Double:
                        return _double ?? (_double = new CastInstructionT<double>());

                    case TypeCode.Int16:
                        return _int16 ?? (_int16 = new CastInstructionT<short>());

                    case TypeCode.Int32:
                        return _int32 ?? (_int32 = new CastInstructionT<int>());

                    case TypeCode.Int64:
                        return _int64 ?? (_int64 = new CastInstructionT<long>());

                    case TypeCode.SByte:
                        return _sByte ?? (_sByte = new CastInstructionT<sbyte>());

                    case TypeCode.Single:
                        return _single ?? (_single = new CastInstructionT<float>());

                    case TypeCode.String:
                        return _string ?? (_string = new CastInstructionT<string>());

                    case TypeCode.UInt16:
                        return _uInt16 ?? (_uInt16 = new CastInstructionT<ushort>());

                    case TypeCode.UInt32:
                        return _uInt32 ?? (_uInt32 = new CastInstructionT<uint>());

                    case TypeCode.UInt64:
                        return _uInt64 ?? (_uInt64 = new CastInstructionT<ulong>());
                }
            }

            return new CastInstructionNoT(t);
        }
    }

    internal class CastToEnumInstruction : CastInstruction
    {
        private readonly Type _t;

        public CastToEnumInstruction(Type t)
        {
            Debug.Assert(t.IsEnum);
            _t = t;
        }

        public override int Run(InterpretedFrame frame)
        {
            var from = frame.Pop();
            var to = from == null ? null : Enum.ToObject(_t, from);
            frame.Push(to);

            return +1;
        }
    }

    internal class NullCheckInstruction : Instruction
    {
        private readonly int _stackOffset;
        private const int _cacheSize = 12;
        private static readonly NullCheckInstruction[] _cache = new NullCheckInstruction[_cacheSize];

        public override int ConsumedStack
        {
            get { return 0; }
        }

        public override int ProducedStack
        {
            get { return 0; }
        }

        public override string InstructionName
        {
            get { return "NullCheck"; }
        }

        private NullCheckInstruction(int stackOffset)
        {
            _stackOffset = stackOffset;
        }

        public static Instruction Create(int stackOffset)
        {
            if (stackOffset < _cacheSize)
            {
                return _cache[stackOffset] ?? (_cache[stackOffset] = new NullCheckInstruction(stackOffset));
            }

            return new NullCheckInstruction(stackOffset);
        }

        public override int Run(InterpretedFrame frame)
        {
            if (frame.Data[frame.StackIndex - 1 - _stackOffset] == null)
            {
                throw new NullReferenceException();
            }
            return +1;
        }
    }

#if DEBUG

    internal class LogInstruction : Instruction
    {
        private readonly string _message; // TODO never used

        public LogInstruction(string message)
        {
            _message = message;
        }

        public override string InstructionName
        {
            get { return "Log"; }
        }

        public override int Run(InterpretedFrame frame)
        {
            //Console.WriteLine(_message);
            return +1;
        }
    }

#endif

    internal class QuoteInstruction : Instruction
    {
        private readonly Expression _operand;
        private readonly Dictionary<ParameterExpression, LocalVariable> _hoistedVariables;

        public QuoteInstruction(Expression operand, Dictionary<ParameterExpression, LocalVariable> hoistedVariables)
        {
            _operand = operand;
            _hoistedVariables = hoistedVariables;
        }

        public override int ConsumedStack
        {
            get { return 0; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "Quote"; }
        }

        public override int Run(InterpretedFrame frame)
        {
            var operand = _operand;
            if (_hoistedVariables != null)
            {
                operand = new ExpressionQuoter(_hoistedVariables, frame).Visit(operand);
            }
            frame.Push(operand);
            return +1;
        }

        // Modifies a quoted Expression instance by changing hoisted variables and
        // parameters into hoisted local references. The variable's StrongBox is
        // burned as a constant, and all hoisted variables/parameters are rewritten
        // as indexing expressions.
        //
        // The behavior of Quote is indended to be like C# and VB expression quoting
        private sealed class ExpressionQuoter : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, LocalVariable> _variables;
            private readonly InterpretedFrame _frame;

            // A stack of variables that are defined in nested scopes. We search
            // this first when resolving a variable in case a nested scope shadows
            // one of our variable instances.
            private readonly Stack<Set<ParameterExpression>> _shadowedVars = new Stack<Set<ParameterExpression>>();

            internal ExpressionQuoter(Dictionary<ParameterExpression, LocalVariable> hoistedVariables, InterpretedFrame frame)
            {
                _variables = hoistedVariables;
                _frame = frame;
            }

            protected internal override Expression VisitLambda(LambdaExpression node)
            {
                _shadowedVars.Push(new Set<ParameterExpression>(node.Parameters));
                var b = Visit(node.Body);
                _shadowedVars.Pop();
                if (b == node.Body)
                {
                    return node;
                }
                return node.Update(b, node.Parameters);
            }

            protected internal override Expression VisitBlock(BlockExpression node)
            {
                if (node.Variables.Count > 0)
                {
                    _shadowedVars.Push(new Set<ParameterExpression>(node.Variables));
                }
                var b = Visit(node.Expressions);
                if (node.Variables.Count > 0)
                {
                    _shadowedVars.Pop();
                }
                if (b == node.Expressions)
                {
                    return node;
                }
                return Expression.Block(node.Variables, b);
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Variable != null)
                {
                    _shadowedVars.Push(new Set<ParameterExpression>(new[] { node.Variable }));
                }
                var b = Visit(node.Body);
                var f = Visit(node.Filter);
                if (node.Variable != null)
                {
                    _shadowedVars.Pop();
                }
                if (b == node.Body && f == node.Filter)
                {
                    return node;
                }
                return Expression.MakeCatchBlock(node.Test, node.Variable, b, f);
            }

            protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                var count = node.Variables.Count;
                var boxes = new List<IStrongBox>();
                var vars = new List<ParameterExpression>();
                var indexes = new int[count];
                for (var i = 0; i < count; i++)
                {
                    LocalVariable var;
                    if (_variables.TryGetValue(node.Variables[i], out var))
                    {
                        indexes[i] = -1 - boxes.Count;
                        if (var.InClosure)
                        {
                            boxes.Add(_frame.Closure[var.Index]);
                        }
                        else
                        {
                            boxes.Add((IStrongBox)_frame.Data[var.Index]);
                        }
                    }
                    else
                    {
                        indexes[i] = vars.Count;
                        vars.Add(node.Variables[i]);
                    }
                }

                // No variables were rewritten. Just return the original node
                if (boxes.Count == 0)
                {
                    return node;
                }

                var boxesConst = Expression.Constant(new RuntimeVariables(boxes.ToArray()), typeof(IRuntimeVariables));
                // All of them were rewritten. Just return the array as a constant
                if (vars.Count == 0)
                {
                    return boxesConst;
                }

                // Otherwise, we need to return an object that merges them
                return Expression.Invoke(
                    Expression.Constant(new Func<IRuntimeVariables, IRuntimeVariables, int[], IRuntimeVariables>(MergeRuntimeVariables)),
                    Expression.RuntimeVariables(new TrueReadOnlyCollection<ParameterExpression>(vars.ToArray())),
                    boxesConst,
                    Expression.Constant(indexes)
                );
            }

            private static IRuntimeVariables MergeRuntimeVariables(IRuntimeVariables first, IRuntimeVariables second, int[] indexes)
            {
                return new MergedRuntimeVariables(first, second, indexes);
            }

            protected internal override Expression VisitParameter(ParameterExpression node)
            {
                LocalVariable var;
                if (_variables.TryGetValue(node, out var))
                {
                    return Expression.Convert(
                        Expression.Field(
                            var.LoadFromArray(
                                Expression.Constant(_frame.Data),
                                Expression.Constant(_frame.Closure)
                            ),
                            "Value"
                        ),
                        node.Type
                    );
                }
                return node;
            }

            private sealed class RuntimeVariables : IRuntimeVariables
            {
                private readonly IStrongBox[] _boxes;

                internal RuntimeVariables(IStrongBox[] boxes)
                {
                    _boxes = boxes;
                }

                int IRuntimeVariables.Count
                {
                    get { return _boxes.Length; }
                }

                object IRuntimeVariables.this[int index]
                {
                    get { return _boxes[index].Value; }
                    set { _boxes[index].Value = value; }
                }
            }

            /// <summary>
            /// Provides a list of variables, supporing read/write of the values
            /// Exposed via RuntimeVariablesExpression
            /// </summary>
            private sealed class MergedRuntimeVariables : IRuntimeVariables
            {
                private readonly IRuntimeVariables _first;
                private readonly IRuntimeVariables _second;

                // For reach item, the index into the first or second list
                // Positive values mean the first array, negative means the second
                private readonly int[] _indexes;

                internal MergedRuntimeVariables(IRuntimeVariables first, IRuntimeVariables second, int[] indexes)
                {
                    _first = first;
                    _second = second;
                    _indexes = indexes;
                }

                public int Count
                {
                    get { return _indexes.Length; }
                }

                public object this[int index]
                {
                    get
                    {
                        index = _indexes[index];
                        return (index >= 0) ? _first[index] : _second[-1 - index];
                    }
                    set
                    {
                        index = _indexes[index];
                        if (index >= 0)
                        {
                            _first[index] = value;
                        }
                        else
                        {
                            _second[-1 - index] = value;
                        }
                    }
                }
            }
        }
    }
}

#endif