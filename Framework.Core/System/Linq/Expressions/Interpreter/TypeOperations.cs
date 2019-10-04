#if LESSTHAN_NET35

#pragma warning disable CC0021 // Use nameof

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class CastInstruction : Instruction
    {
        private static CastInstruction _boolean, _byte, _char, _dateTime, _decimal, _double, _int16, _int32, _int64, _sByte, _single, _string, _uInt16, _uInt32, _uInt64;

        public override int ConsumedStack => 1;
        public override string InstructionName => "Cast";
        public override int ProducedStack => 1;

        public static Instruction Create(Type t)
        {
            Debug.Assert(!t.IsEnum);
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean: return _boolean ??= new CastInstructionT<bool>();
                case TypeCode.Byte: return _byte ??= new CastInstructionT<byte>();
                case TypeCode.Char: return _char ??= new CastInstructionT<char>();
                case TypeCode.DateTime: return _dateTime ??= new CastInstructionT<DateTime>();
                case TypeCode.Decimal: return _decimal ??= new CastInstructionT<decimal>();
                case TypeCode.Double: return _double ??= new CastInstructionT<double>();
                case TypeCode.Int16: return _int16 ??= new CastInstructionT<short>();
                case TypeCode.Int32: return _int32 ??= new CastInstructionT<int>();
                case TypeCode.Int64: return _int64 ??= new CastInstructionT<long>();
                case TypeCode.SByte: return _sByte ??= new CastInstructionT<sbyte>();
                case TypeCode.Single: return _single ??= new CastInstructionT<float>();
                case TypeCode.String: return _string ??= new CastInstructionT<string>();
                case TypeCode.UInt16: return _uInt16 ??= new CastInstructionT<ushort>();
                case TypeCode.UInt32: return _uInt32 ??= new CastInstructionT<uint>();
                case TypeCode.UInt64: return _uInt64 ??= new CastInstructionT<ulong>();
                default: return CastInstructionNoT.Create(t);
            }
        }
    }

    internal abstract class CastInstructionNoT : CastInstruction
    {
        private readonly Type _t;

        private CastInstructionNoT(Type t)
        {
            _t = t;
        }

        public static new CastInstruction Create(Type t)
        {
            if (t.IsValueType && !t.IsNullable())
            {
                return new Value(t);
            }

            return new Ref(t);
        }

        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();
            if (value != null)
            {
                var valueType = value.GetType();

                if (!valueType.HasReferenceConversionTo(_t) && !valueType.HasIdentityPrimitiveOrNullableConversionTo(_t))
                {
                    throw new InvalidCastException();
                }

                if (!_t.IsAssignableFrom(valueType))
                {
                    throw new InvalidCastException();
                }

                frame.Push(value);
            }
            else
            {
                ConvertNull(frame);
            }

            return 1;
        }

        protected abstract void ConvertNull(InterpretedFrame frame);

        private sealed class Ref : CastInstructionNoT
        {
            public Ref(Type t)
                : base(t)
            {
            }

            protected override void ConvertNull(InterpretedFrame frame)
            {
                frame.Push(null);
            }
        }

        private sealed class Value : CastInstructionNoT
        {
            public Value(Type t)
                : base(t)
            {
            }

            protected override void ConvertNull(InterpretedFrame frame)
            {
#pragma warning disable CA2201 // Do not raise reserved exception types
                throw new NullReferenceException();
#pragma warning restore CA2201 // Do not raise reserved exception types
            }
        }
    }

    internal sealed class CastInstructionT<T> : CastInstruction
    {
        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();
            frame.Push((T)value);
            return 1;
        }
    }

    internal sealed class CastReferenceToEnumInstruction : CastInstruction
    {
        private readonly Type _t;

        public CastReferenceToEnumInstruction(Type t)
        {
            Debug.Assert(t.IsEnum);
            _t = t;
        }

        public override int Run(InterpretedFrame frame)
        {
            var from = frame.Pop();
            Debug.Assert(from != null);

            // If from is neither a T nor a type assignable to T (viz. an T-backed enum)
            // this will cause an InvalidCastException, which is what this operation should
            // throw in this case.

            switch (Type.GetTypeCode(_t))
            {
                case TypeCode.Int32:
                    frame.Push(Enum.ToObject(_t, (int)from));
                    break;

                case TypeCode.Int64:
                    frame.Push(Enum.ToObject(_t, (long)from));
                    break;

                case TypeCode.UInt32:
                    frame.Push(Enum.ToObject(_t, (uint)from));
                    break;

                case TypeCode.UInt64:
                    frame.Push(Enum.ToObject(_t, (ulong)from));
                    break;

                case TypeCode.Byte:
                    frame.Push(Enum.ToObject(_t, (byte)from));
                    break;

                case TypeCode.SByte:
                    frame.Push(Enum.ToObject(_t, (sbyte)from));
                    break;

                case TypeCode.Int16:
                    frame.Push(Enum.ToObject(_t, (short)from));
                    break;

                case TypeCode.UInt16:
                    frame.Push(Enum.ToObject(_t, (ushort)from));
                    break;

                case TypeCode.Char:
                    // Disallowed in C#, but allowed in CIL
                    frame.Push(Enum.ToObject(_t, (char)from));
                    break;

                default:
                    // Only remaining possible type.
                    // Disallowed in C#, but allowed in CIL
                    Debug.Assert(Type.GetTypeCode(_t) == TypeCode.Boolean);
                    frame.Push(Enum.ToObject(_t, (bool)from));
                    break;
            }

            return 1;
        }
    }

    internal sealed class CastToEnumInstruction : CastInstruction
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
            Debug.Assert
            (
                new[]
                {
                    TypeCode.Empty, TypeCode.Int32, TypeCode.SByte, TypeCode.Int16, TypeCode.Int64, TypeCode.UInt32,
                    TypeCode.Byte, TypeCode.UInt16, TypeCode.UInt64, TypeCode.Char, TypeCode.Boolean
                }.Contains(Convert.GetTypeCode(from))
            );
            frame.Push(from == null ? null : Enum.ToObject(_t, from));
            return 1;
        }
    }

    internal sealed class CreateDelegateInstruction : Instruction
    {
        private readonly LightDelegateCreator _creator;

        internal CreateDelegateInstruction(LightDelegateCreator delegateCreator)
        {
            _creator = delegateCreator;
        }

        public override int ConsumedStack => _creator.Interpreter.ClosureSize;
        public override string InstructionName => "CreateDelegate";
        public override int ProducedStack => 1;

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
            return 1;
        }
    }

    internal abstract class NullableMethodCallInstruction : Instruction
    {
        private static NullableMethodCallInstruction _hasValue, _value, _equals, _getHashCode, _getValueOrDefault1, _toString;

        private NullableMethodCallInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "NullableMethod";
        public override int ProducedStack => 1;

        public static Instruction Create(string method, int argCount, MethodInfo mi)
        {
            switch (method)
            {
                case "get_HasValue": return _hasValue ??= new HasValue();
                case "get_Value": return _value ??= new GetValue();
                case "Equals": return _equals ??= new EqualsClass();
                case "GetHashCode": return _getHashCode ??= new GetHashCodeClass();
                case "GetValueOrDefault":
                    return argCount == 0
                        ? new GetValueOrDefault(mi)
                        : _getValueOrDefault1 ??= new GetValueOrDefault1();

                case "ToString": return _toString ??= new ToStringClass();
                default:
                    // System.Nullable doesn't have other instance methods
                    throw ContractUtils.Unreachable;
            }
        }

        public static Instruction CreateGetValue()
        {
            return _value ??= new GetValue();
        }

        private sealed class EqualsClass : NullableMethodCallInstruction
        {
            public override int ConsumedStack => 2;

            public override int Run(InterpretedFrame frame)
            {
                var other = frame.Pop();
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(other == null);
                }
                else if (other == null)
                {
                    frame.Push(Utils.BoxedFalse);
                }
                else
                {
                    frame.Push(obj.Equals(other));
                }

                return 1;
            }
        }

        private sealed class GetHashCodeClass : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                frame.Push(obj?.GetHashCode() ?? 0);
                return 1;
            }
        }

        private sealed class GetValue : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                if (frame.Peek() == null)
                {
                    // Trigger InvalidOperationException with same localized method as if we'd called the Value getter.
                    // ReSharper disable once PossibleInvalidOperationException
                    return (int)default(int?);
                }

                return 1;
            }
        }

        private sealed class GetValueOrDefault : NullableMethodCallInstruction
        {
            private readonly Type _defaultValueType;

            public GetValueOrDefault(MethodInfo mi)
            {
                _defaultValueType = mi.ReturnType;
            }

            public override int Run(InterpretedFrame frame)
            {
                if (frame.Peek() != null)
                {
                    return 1;
                }

                frame.Pop();
                frame.Push(Activator.CreateInstance(_defaultValueType));
                return 1;
            }
        }

        private sealed class GetValueOrDefault1 : NullableMethodCallInstruction
        {
            public override int ConsumedStack => 2;

            public override int Run(InterpretedFrame frame)
            {
                var dflt = frame.Pop();
                var obj = frame.Pop();
                frame.Push(obj ?? dflt);
                return 1;
            }
        }

        private sealed class HasValue : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                frame.Push(obj != null);
                return 1;
            }
        }

        private sealed class ToStringClass : NullableMethodCallInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                frame.Push(obj == null ? "" : obj.ToString());
                return 1;
            }
        }
    }

    internal sealed class QuoteInstruction : Instruction
    {
        private readonly Dictionary<ParameterExpression, LocalVariable> _hoistedVariables;
        private readonly Expression _operand;

        public QuoteInstruction(Expression operand, Dictionary<ParameterExpression, LocalVariable> hoistedVariables)
        {
            _operand = operand;
            _hoistedVariables = hoistedVariables;
        }

        public override string InstructionName => "Quote";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var operand = _operand;
            if (_hoistedVariables != null)
            {
                operand = new ExpressionQuoter(_hoistedVariables, frame).Visit(operand);
            }

            frame.Push(operand);
            return 1;
        }

        // Modifies a quoted Expression instance by changing hoisted variables and
        // parameters into hoisted local references. The variable's StrongBox is
        // burned as a constant, and all hoisted variables/parameters are rewritten
        // as indexing expressions.
        //
        // The behavior of Quote is intended to be like C# and VB expression quoting
        private sealed class ExpressionQuoter : ExpressionVisitor
        {
            private readonly InterpretedFrame _frame;

            // A stack of variables that are defined in nested scopes. We search
            // this first when resolving a variable in case a nested scope shadows
            // one of our variable instances.
            private readonly Stack<HashSet<ParameterExpression>> _shadowedVars = new Stack<HashSet<ParameterExpression>>();

            private readonly Dictionary<ParameterExpression, LocalVariable> _variables;

            internal ExpressionQuoter(Dictionary<ParameterExpression, LocalVariable> hoistedVariables, InterpretedFrame frame)
            {
                _variables = hoistedVariables;
                _frame = frame;
            }

            protected internal override Expression VisitBlock(BlockExpression node)
            {
                if (node.Variables.Count > 0)
                {
                    _shadowedVars.Push(new HashSet<ParameterExpression>(node.Variables));
                }

                var b = ExpressionVisitorUtils.VisitBlockExpressions(this, node);
                if (node.Variables.Count > 0)
                {
                    _shadowedVars.Pop();
                }

                return b == null ? node : node.Rewrite(node.Variables, b);
            }

            protected internal override Expression VisitLambda<T>(Expression<T> node)
            {
                if (node.ParameterCount > 0)
                {
                    var parameters = new HashSet<ParameterExpression>();

                    for (int i = 0, n = node.ParameterCount; i < n; i++)
                    {
                        parameters.Add(node.GetParameter(i));
                    }

                    _shadowedVars.Push(parameters);
                }

                var b = Visit(node.Body);
                if (node.ParameterCount > 0)
                {
                    _shadowedVars.Pop();
                }

                return b == node.Body ? node : node.Rewrite(b, null);
            }

            protected internal override Expression VisitParameter(ParameterExpression node)
            {
                var box = GetBox(node);
                if (box == null)
                {
                    return node;
                }

                return Expression.Convert(Expression.Field(Expression.Constant(box), "Value"), node.Type);
            }

            protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                var count = node.Variables.Count;
                var boxes = new List<IStrongBox>();
                var vars = new List<ParameterExpression>();
                var indexes = new int[count];
                for (var i = 0; i < indexes.Length; i++)
                {
                    var box = GetBox(node.Variables[i]);
                    if (box == null)
                    {
                        indexes[i] = vars.Count;
                        vars.Add(node.Variables[i]);
                    }
                    else
                    {
                        indexes[i] = -1 - boxes.Count;
                        boxes.Add(box);
                    }
                }

                // No variables were rewritten. Just return the original node.
                if (boxes.Count == 0)
                {
                    return node;
                }

                var boxesConst = Expression.Constant(new RuntimeOps.RuntimeVariables(boxes.ToArray()), typeof(IRuntimeVariables));
                // All of them were rewritten. Just return the array as a constant
                if (vars.Count == 0)
                {
                    return boxesConst;
                }

                // Otherwise, we need to return an object that merges them.
                return Expression.Invoke
                (
                    Expression.Constant(new Func<IRuntimeVariables, IRuntimeVariables, int[], IRuntimeVariables>(MergeRuntimeVariables)),
                    Expression.RuntimeVariables(ReadOnlyCollectionEx.Create(vars.ToArray())),
                    boxesConst,
                    Expression.Constant(indexes)
                );
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Variable != null)
                {
                    _shadowedVars.Push(new HashSet<ParameterExpression> { node.Variable });
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

            private static IRuntimeVariables MergeRuntimeVariables(IRuntimeVariables first, IRuntimeVariables second, int[] indexes)
            {
                return new RuntimeOps.MergedRuntimeVariables(first, second, indexes);
            }

            private IStrongBox GetBox(ParameterExpression variable)
            {
                if (!_variables.TryGetValue(variable, out var var))
                {
                    return null;
                }

                if (var.InClosure)
                {
                    return _frame.Closure[var.Index];
                }

                return (IStrongBox)_frame.Data[var.Index];
            }
        }
    }

    internal sealed class TypeAsInstruction : Instruction
    {
        private readonly Type _type;

        internal TypeAsInstruction(Type type)
        {
            _type = type;
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "TypeAs";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var value = frame.Pop();
            frame.Push(_type.IsInstanceOfType(value) ? value : null);
            return 1;
        }

        public override string ToString()
        {
            return "TypeAs " + _type;
        }
    }

    internal sealed class TypeEqualsInstruction : Instruction
    {
        public static readonly TypeEqualsInstruction Instance = new TypeEqualsInstruction();

        private TypeEqualsInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "TypeEquals";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            var type = frame.Pop();
            var obj = frame.Pop();
            frame.Push(ReferenceEquals(obj?.GetType(), type));
            return 1;
        }
    }

    internal sealed class TypeIsInstruction : Instruction
    {
        private readonly Type _type;

        internal TypeIsInstruction(Type type)
        {
            _type = type;
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "TypeIs";
        public override int ProducedStack => 1;

        public override int Run(InterpretedFrame frame)
        {
            frame.Push(_type.IsInstanceOfType(frame.Pop()));
            return 1;
        }

        public override string ToString()
        {
            return "TypeIs " + _type;
        }
    }
}

#endif