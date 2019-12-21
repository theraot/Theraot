#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections;

namespace System.Linq.Expressions.Interpreter
{
    [DebuggerTypeProxy(typeof(DebugView))]
    internal sealed partial class InstructionList
    {
        private const int _cachedObjectCount = 256;
        private const int _localInstrCacheSize = 64;
        private const int _pushIntMaxCachedValue = 100;
        private const int _pushIntMinCachedValue = -100;
        private static readonly RuntimeLabel[] _emptyRuntimeLabels = { new RuntimeLabel(Interpreter.RethrowOnReturn, 0, 0) };
        private static readonly Dictionary<FieldInfo, Instruction> _loadFields = new Dictionary<FieldInfo, Instruction>();
        private readonly List<Instruction> _instructions = new List<Instruction>();

        // list of (instruction index, cookie) sorted by instruction index:
        // Not readonly for debug
        private List<KeyValuePair<int, object?>>? _debugCookies;

        private int _maxContinuationDepth;
        private List<object>? _objects;
        private int _runtimeLabelCount;

        public int Count => _instructions.Count;

        public int CurrentContinuationsDepth { get; private set; }

        public int CurrentStackDepth { get; private set; }

        public int MaxStackDepth { get; private set; }

        public void Emit(Instruction instruction)
        {
            _instructions.Add(instruction);
            UpdateStackDepth(instruction);
        }

        public void EmitAnd(Type type)
        {
            Emit(AndInstruction.Create(type));
        }

        public void EmitArrayLength()
        {
            Emit(ArrayLengthInstruction.Instance);
        }

        public void EmitAssignLocalBoxed(int index)
        {
            Emit(AssignLocalBoxed(index));
        }

        public void EmitBranch(BranchLabel label)
        {
            EmitBranch(new BranchInstruction(), label);
        }

        public void EmitBranch(BranchLabel label, bool hasResult, bool hasValue)
        {
            EmitBranch(new BranchInstruction(hasResult, hasValue), label);
        }

        public void EmitBranchFalse(BranchLabel elseLabel)
        {
            EmitBranch(new BranchFalseInstruction(), elseLabel);
        }

        public void EmitBranchTrue(BranchLabel elseLabel)
        {
            EmitBranch(new BranchTrueInstruction(), elseLabel);
        }

        public void EmitByRefNew(ConstructorInfo constructorInfo, ParameterInfo[] parameters, ByRefUpdater[] updaters)
        {
            Emit(new ByRefNewInstruction(constructorInfo, parameters.Length, updaters));
        }

        public void EmitCall(MethodInfo method)
        {
            EmitCall(method, method.GetParameters());
        }

        public void EmitCall(MethodInfo method, ParameterInfo[] parameters)
        {
            Emit(CallInstruction.Create(method, parameters));
        }

        public void EmitCast(Type toType)
        {
            Emit(CastInstruction.Create(toType));
        }

        public void EmitCastReferenceToEnum(Type toType)
        {
            Debug.Assert(_instructions[_instructions.Count - 1] == NullCheckInstruction.Instance);
            Emit(new CastReferenceToEnumInstruction(toType));
        }

        public void EmitCastToEnum(Type toType)
        {
            Emit(new CastToEnumInstruction(toType));
        }

        public void EmitCoalescingBranch(BranchLabel leftNotNull)
        {
            EmitBranch(new CoalescingBranchInstruction(), leftNotNull);
        }

        public void EmitConvertToUnderlying(TypeCode to, bool isLiftedToNull)
        {
            Emit(new NumericConvertInstruction.ToUnderlying(to, isLiftedToNull));
        }

        public void EmitDecrement(Type type)
        {
            Emit(DecrementInstruction.Create(type));
        }

        public void EmitDefaultValue(Type type)
        {
            Emit(new DefaultValueInstruction(type));
        }

        public void EmitDiv(Type type)
        {
            Emit(DivInstruction.Create(type));
        }

        public void EmitDup()
        {
            Emit(DupInstruction.Instance);
        }

        public void EmitEnterExceptionFilter()
        {
            Emit(EnterExceptionFilterInstruction.Instance);
        }

        public void EmitEnterExceptionHandlerNonVoid()
        {
            Emit(EnterExceptionHandlerInstruction.NonVoid);
        }

        public void EmitEnterExceptionHandlerVoid()
        {
            Emit(EnterExceptionHandlerInstruction.Void);
        }

        public void EmitEnterFault(BranchLabel faultStartLabel)
        {
            Emit(EnterFaultInstruction.Create(EnsureLabelIndex(faultStartLabel)));
        }

        public void EmitEnterFinally(BranchLabel finallyStartLabel)
        {
            Emit(EnterFinallyInstruction.Create(EnsureLabelIndex(finallyStartLabel)));
        }

        public void EmitEnterTryCatch()
        {
            Emit(EnterTryCatchFinallyInstruction.CreateTryCatch());
        }

        public EnterTryFaultInstruction EmitEnterTryFault(BranchLabel tryEnd)
        {
            var instruction = new EnterTryFaultInstruction(EnsureLabelIndex(tryEnd));
            Emit(instruction);
            return instruction;
        }

        public void EmitEnterTryFinally(BranchLabel finallyStartLabel)
        {
            Emit(EnterTryCatchFinallyInstruction.CreateTryFinally(EnsureLabelIndex(finallyStartLabel)));
        }

        public void EmitEqual(Type type, bool liftedToNull = false)
        {
            Emit(EqualInstruction.Create(type, liftedToNull));
        }

        public void EmitExclusiveOr(Type type)
        {
            Emit(ExclusiveOrInstruction.Create(type));
        }

        public void EmitGetArrayItem()
        {
            Emit(GetArrayItemInstruction.Instance);
        }

        public void EmitGoto(BranchLabel label, bool hasResult, bool hasValue, bool labelTargetGetsValue)
        {
            Emit(GotoInstruction.Create(EnsureLabelIndex(label), hasResult, hasValue, labelTargetGetsValue));
        }

        public void EmitGreaterThan(Type type, bool liftedToNull)
        {
            Emit(GreaterThanInstruction.Create(type, liftedToNull));
        }

        public void EmitGreaterThanOrEqual(Type type, bool liftedToNull)
        {
            Emit(GreaterThanOrEqualInstruction.Create(type, liftedToNull));
        }

        public void EmitIncrement(Type type)
        {
            Emit(IncrementInstruction.Create(type));
        }

        public void EmitIntSwitch<T>(Dictionary<T, int> cases)
        {
            Emit(new IntSwitchInstruction<T>(cases));
        }

        public void EmitLeaveExceptionFilter()
        {
            Emit(LeaveExceptionFilterInstruction.Instance);
        }

        public void EmitLeaveExceptionHandler(bool hasValue, BranchLabel tryExpressionEndLabel)
        {
            Emit(LeaveExceptionHandlerInstruction.Create(EnsureLabelIndex(tryExpressionEndLabel), hasValue));
        }

        public void EmitLeaveFault()
        {
            Emit(LeaveFaultInstruction.Instance);
        }

        public void EmitLeaveFinally()
        {
            Emit(LeaveFinallyInstruction.Instance);
        }

        public void EmitLeftShift(Type type)
        {
            Emit(LeftShiftInstruction.Create(type));
        }

        public void EmitLessThan(Type type, bool liftedToNull)
        {
            Emit(LessThanInstruction.Create(type, liftedToNull));
        }

        public void EmitLessThanOrEqual(Type type, bool liftedToNull)
        {
            Emit(LessThanOrEqualInstruction.Create(type, liftedToNull));
        }

        public void EmitLoad(object? value)
        {
            EmitLoad(value, null);
        }

        public void EmitLoadField(FieldInfo field)
        {
            Emit(GetLoadField(field));
        }

        public void EmitLoadLocalBoxed(int index)
        {
            Emit(LoadLocalBoxed(index));
        }

        public void EmitModulo(Type type)
        {
            Emit(ModuloInstruction.Create(type));
        }

        public void EmitNegate(Type type)
        {
            Emit(NegateInstruction.Create(type));
        }

        public void EmitNegateChecked(Type type)
        {
            Emit(NegateCheckedInstruction.Create(type));
        }

        public void EmitNew(ConstructorInfo constructorInfo, ParameterInfo[] parameters)
        {
            Emit(new NewInstruction(constructorInfo, parameters.Length));
        }

        public void EmitNewArray(Type elementType)
        {
            Emit(new NewArrayInstruction(elementType));
        }

        public void EmitNewArrayBounds(Type elementType, int rank)
        {
            Emit(new NewArrayBoundsInstruction(elementType, rank));
        }

        public void EmitNewArrayInit(Type elementType, int elementCount)
        {
            Emit(new NewArrayInitInstruction(elementType, elementCount));
        }

        public void EmitNewRuntimeVariables(int count)
        {
            Emit(new RuntimeVariablesInstruction(count));
        }

        public void EmitNot(Type type)
        {
            Emit(NotInstruction.Create(type));
        }

        public void EmitNotEqual(Type type, bool liftedToNull = false)
        {
            Emit(NotEqualInstruction.Create(type, liftedToNull));
        }

        public void EmitNullableCall(MethodInfo method, ParameterInfo[] parameters)
        {
            Emit(NullableMethodCallInstruction.Create(method.Name, parameters.Length, method));
        }

        public void EmitNumericConvertChecked(TypeCode from, TypeCode to, bool isLiftedToNull)
        {
            Emit(new NumericConvertInstruction.Checked(from, to, isLiftedToNull));
        }

        public void EmitNumericConvertUnchecked(TypeCode from, TypeCode to, bool isLiftedToNull)
        {
            Emit(new NumericConvertInstruction.Unchecked(from, to, isLiftedToNull));
        }

        public void EmitOr(Type type)
        {
            Emit(OrInstruction.Create(type));
        }

        public void EmitPop()
        {
            Emit(PopInstruction.Instance);
        }

        public void EmitRethrow()
        {
            Emit(ThrowInstruction.Rethrow);
        }

        public void EmitRethrowVoid()
        {
            Emit(ThrowInstruction.VoidRethrow);
        }

        public void EmitRightShift(Type type)
        {
            Emit(RightShiftInstruction.Create(type));
        }

        public void EmitSetArrayItem()
        {
            Emit(SetArrayItemInstruction.Instance);
        }

        public void EmitStoreLocalBoxed(int index)
        {
            Emit(StoreLocalBoxed(index));
        }

        public void EmitStoreLocalToClosure(int index)
        {
            EmitAssignLocalToClosure(index);
            EmitPop();
        }

        public void EmitStringSwitch(Dictionary<string, int> cases, StrongBox<int> nullCase)
        {
            Emit(new StringSwitchInstruction(cases, nullCase));
        }

        public void EmitThrow()
        {
            Emit(ThrowInstruction.Throw);
        }

        public void EmitThrowVoid()
        {
            Emit(ThrowInstruction.VoidThrow);
        }

        public void EmitTypeAs(Type type)
        {
            Emit(new TypeAsInstruction(type));
        }

        public void EmitTypeEquals()
        {
            Emit(TypeEqualsInstruction.Instance);
        }

        public void EmitTypeIs(Type type)
        {
            Emit(new TypeIsInstruction(type));
        }

        public void MarkLabel(BranchLabel label)
        {
            label.Mark(this);
        }

        public int MarkRuntimeLabel()
        {
            var handlerLabel = MakeLabel();
            MarkLabel(handlerLabel);
            return EnsureLabelIndex(handlerLabel);
        }

        [Conditional("DEBUG")]
        public void SetDebugCookie(object? cookie)
        {
#if DEBUG
            if (_debugCookies == null)
            {
                _debugCookies = new List<KeyValuePair<int, object?>>();
            }

            Debug.Assert(Count > 0);
            _debugCookies.Add(new KeyValuePair<int, object?>(Count - 1, cookie));
#else
            Theraot.No.Op(cookie);
            _debugCookies = null;
#endif
        }

        public InstructionArray ToArray()
        {
            return new InstructionArray
            (
                MaxStackDepth,
                _maxContinuationDepth,
                _instructions.AsArrayInternal(),
                _objects.AsArrayInternal(),
                BuildRuntimeLabels(),
                _debugCookies
            );
        }

        // "Un-emit" the previous instruction.
        // Useful if the instruction was emitted in the calling method, and covers the more usual case.
        // In particular, calling this after an EmitPush() or EmitDup() costs about the same as adding
        // an EmitPop() to undo it at compile time, and leaves a slightly leaner instruction list.
        public void UnEmit()
        {
            var instruction = _instructions[_instructions.Count - 1];
            _instructions.RemoveAt(_instructions.Count - 1);

            CurrentContinuationsDepth -= instruction.ProducedContinuations;
            CurrentContinuationsDepth += instruction.ConsumedContinuations;
            CurrentStackDepth -= instruction.ProducedStack;
            CurrentStackDepth += instruction.ConsumedStack;
        }

        internal static Instruction InitImmutableRefBox(int index)
        {
            return new InitializeLocalInstruction.ImmutableRefBox(index);
        }

        internal static Instruction InitReference(int index)
        {
            return new InitializeLocalInstruction.Reference(index);
        }

        internal static Instruction Parameter(int index)
        {
            return new InitializeLocalInstruction.Parameter(index);
        }

        internal static Instruction ParameterBox(int index)
        {
            return new InitializeLocalInstruction.ParameterBox(index);
        }

        internal void EmitCreateDelegate(LightDelegateCreator creator)
        {
            Emit(new CreateDelegateInstruction(creator));
        }

        internal void EmitInitializeParameter(int index)
        {
            Emit(Parameter(index));
        }

        internal void FixupBranch(int branchIndex, int offset)
        {
            _instructions[branchIndex] = ((OffsetInstruction)_instructions[branchIndex]).Fixup(offset);
        }

        internal Instruction GetInstruction(int index)
        {
            return _instructions[index];
        }

        private void EmitBranch(OffsetInstruction instruction, BranchLabel label)
        {
            Emit(instruction);
            label.AddBranch(this, Count - 1);
        }
    }

    internal sealed partial class InstructionList
    {
        public void EmitAdd(Type type, bool @checked)
        {
            Emit(@checked ? AddOvfInstruction.Create(type) : AddInstruction.Create(type));
        }

        public void EmitByRefCall(MethodInfo method, ParameterInfo[] parameters, ByRefUpdater[] byrefArgs)
        {
            Emit(new ByRefMethodInfoCallInstruction(method, method.IsStatic ? parameters.Length : parameters.Length + 1, byrefArgs));
        }

        public void EmitInitializeLocal(int index, Type type)
        {
            var value = ScriptingRuntimeHelpers.GetPrimitiveDefaultValue(type);
            if (value != null)
            {
                Emit(new InitializeLocalInstruction.ImmutableValue(index, value));
            }
            else if (type.IsValueType)
            {
                Emit(new InitializeLocalInstruction.MutableValue(index, type));
            }
            else
            {
                Emit(InitReference(index));
            }
        }

        public void EmitMul(Type type, bool @checked)
        {
            Emit(@checked ? MulOvfInstruction.Create(type) : MulInstruction.Create(type));
        }

        public void EmitStoreField(FieldInfo field)
        {
            if (field.IsStatic)
            {
                Emit(new StoreStaticFieldInstruction(field));
            }
            else
            {
                Emit(new StoreFieldInstruction(field));
            }
        }

        public void EmitSub(Type type, bool @checked)
        {
            Emit(@checked ? SubOvfInstruction.Create(type) : SubInstruction.Create(type));
        }

        internal void SwitchToBoxed(int index, int instructionIndex)
        {
            if (!(_instructions[instructionIndex] is IBoxableInstruction instruction))
            {
                return;
            }

            var newInstruction = instruction.BoxIfIndexMatches(index);
            if (newInstruction != null)
            {
                _instructions[instructionIndex] = newInstruction;
            }
        }

        private static Instruction GetLoadField(FieldInfo field)
        {
            lock (_loadFields)
            {
                if (_loadFields.TryGetValue(field, out var instruction))
                {
                    return instruction;
                }

                instruction = field.IsStatic ? (Instruction)new LoadStaticFieldInstruction(field) : new LoadFieldInstruction(field);
                _loadFields.Add(field, instruction);
                return instruction;
            }
        }

        private int EnsureLabelIndex(BranchLabel label)
        {
            if (label.HasRuntimeLabel)
            {
                return label.LabelIndex;
            }

            label.LabelIndex = _runtimeLabelCount;
            _runtimeLabelCount++;
            return label.LabelIndex;
        }

        private void UpdateStackDepth(Instruction instruction)
        {
            Debug.Assert(instruction.ConsumedStack >= 0 && instruction.ProducedStack >= 0 && instruction.ConsumedContinuations >= 0 && instruction.ProducedContinuations >= 0, "bad instruction " + instruction);

            CurrentStackDepth -= instruction.ConsumedStack;
            Debug.Assert(CurrentStackDepth >= 0, "negative stack depth " + instruction);
            CurrentStackDepth += instruction.ProducedStack;
            if (CurrentStackDepth > MaxStackDepth)
            {
                MaxStackDepth = CurrentStackDepth;
            }

            CurrentContinuationsDepth -= instruction.ConsumedContinuations;
            Debug.Assert(CurrentContinuationsDepth >= 0, "negative continuations " + instruction);
            CurrentContinuationsDepth += instruction.ProducedContinuations;
            if (CurrentContinuationsDepth > _maxContinuationDepth)
            {
                _maxContinuationDepth = CurrentContinuationsDepth;
            }
        }
    }

    internal sealed partial class InstructionList
    {
        private List<BranchLabel>? _labels;

        public BranchLabel MakeLabel()
        {
            if (_labels == null)
            {
                _labels = new List<BranchLabel>();
            }

            var label = new BranchLabel();
            _labels.Add(label);
            return label;
        }

        private RuntimeLabel[] BuildRuntimeLabels()
        {
            if (_runtimeLabelCount == 0)
            {
                return _emptyRuntimeLabels;
            }

            var result = new RuntimeLabel[_runtimeLabelCount + 1];
            foreach (var label in _labels!)
            {
                if (label.HasRuntimeLabel)
                {
                    result[label.LabelIndex] = label.ToRuntimeLabel();
                }
            }

            // "return and rethrow" label:
            result[result.Length - 1] = new RuntimeLabel(Interpreter.RethrowOnReturn, 0, 0);
            return result;
        }
    }
}

#endif