#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Enables instruction counting and displaying stats at process exit.
// #define STATS

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections.ThreadSafe;

namespace System.Linq.Expressions.Interpreter
{
    [DebuggerTypeProxy(typeof(DebugView))]
    internal /*readonly*/ struct InstructionArray
    {
        // list of (instruction index, cookie) sorted by instruction index:
        internal readonly KeyValuePair<int, object>[] DebugCookies;

        internal readonly Instruction[] Instructions;
        internal readonly RuntimeLabel[] Labels;
        internal readonly int MaxContinuationDepth;
        internal readonly int MaxStackDepth;
        internal readonly object[] Objects;

        internal InstructionArray(int maxStackDepth, int maxContinuationDepth, Instruction[] instructions,
            object[] objects, RuntimeLabel[] labels, IEnumerable<KeyValuePair<int, object>> debugCookies)
        {
            MaxStackDepth = maxStackDepth;
            MaxContinuationDepth = maxContinuationDepth;
            Instructions = instructions;
            DebugCookies = Theraot.Collections.Extensions.AsArrayInternal(debugCookies);
            Objects = objects;
            Labels = labels;
        }

        internal sealed class DebugView
        {
            private readonly InstructionArray _array;

            public DebugView(InstructionArray array)
            {
                ContractUtils.RequiresNotNull(array, nameof(array));
                _array = array;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public InstructionList.DebugView.InstructionView[]/*!*/ A0 => GetInstructionViews(true);

            public InstructionList.DebugView.InstructionView[] GetInstructionViews(bool includeDebugCookies = false)
            {
                return InstructionList.DebugView.GetInstructionViews(
                    _array.Instructions,
                    _array.Objects,
                    index => _array.Labels[index].Index,
                    includeDebugCookies ? _array.DebugCookies : null
                );
            }
        }
    }

    [DebuggerTypeProxy(typeof(DebugView))]
    internal sealed class InstructionList
    {
#if STATS
        private static Dictionary<string, int> _executedInstructions = new Dictionary<string, int>();
        private static Dictionary<string, Dictionary<object, bool>> _instances = new Dictionary<string, Dictionary<object, bool>>();

        static InstructionList()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((_, __) =>
            {
                PerfTrack.DumpHistogram(_executedInstructions);
                Console.WriteLine("-- Total executed: {0}", _executedInstructions.Values.Aggregate(0, (sum, value) => sum + value));
                Console.WriteLine("-----");

                var referenced = new Dictionary<string, int>();
                int total = 0;
                foreach (var entry in _instances)
                {
                    referenced[entry.Key] = entry.Value.Count;
                    total += entry.Value.Count;
                }

                PerfTrack.DumpHistogram(referenced);
                Console.WriteLine("-- Total referenced: {0}", total);
                Console.WriteLine("-----");
            });
        }
#endif

        private const int _cachedObjectCount = 256;

        private const int _localInstrCacheSize = 64;
        private const int _pushIntMaxCachedValue = 100;
        private const int _pushIntMinCachedValue = -100;

        private static Instruction[] _assignLocal;

        private static Instruction[] _assignLocalBoxed;

        private static Instruction[] _assignLocalToClosure;

        private static readonly RuntimeLabel[] _emptyRuntimeLabels = { new RuntimeLabel(Interpreter.RethrowOnReturn, 0, 0) };
        private static Instruction _false;
        private static Instruction[] _ints;

        private static readonly Dictionary<FieldInfo, Instruction> _loadFields = new Dictionary<FieldInfo, Instruction>();

        private static Instruction[] _loadLocal;

        private static Instruction[] _loadLocalBoxed;

        private static Instruction[] _loadLocalFromClosure;

        private static Instruction[] _loadLocalFromClosureBoxed;
        private static Instruction[] _loadObjectCached;
        private static Instruction _null;

        private static Instruction[] _storeLocal;

        private static Instruction[] _storeLocalBoxed;
        private static Instruction _true;

        // list of (instruction index, cookie) sorted by instruction index:
        // Not readonly for debug
        private List<KeyValuePair<int, object>> _debugCookies;
        private readonly List<Instruction> _instructions = new List<Instruction>();

        private List<BranchLabel> _labels;
        private int _maxContinuationDepth;
        private List<object> _objects;
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

        public void EmitAdd(Type type, bool @checked)
        {
            Emit(@checked ? AddOvfInstruction.Create(type) : AddInstruction.Create(type));
        }

        public void EmitAnd(Type type)
        {
            Emit(AndInstruction.Create(type));
        }

        public void EmitArrayLength()
        {
            Emit(ArrayLengthInstruction.Instance);
        }

        public void EmitAssignLocal(int index)
        {
            if (_assignLocal == null)
            {
                _assignLocal = new Instruction[_localInstrCacheSize];
            }

            if (index < _assignLocal.Length)
            {
                Emit(_assignLocal[index] ?? (_assignLocal[index] = new AssignLocalInstruction(index)));
            }
            else
            {
                Emit(new AssignLocalInstruction(index));
            }
        }

        public void EmitAssignLocalBoxed(int index)
        {
            Emit(AssignLocalBoxed(index));
        }

        public void EmitAssignLocalToClosure(int index)
        {
            if (_assignLocalToClosure == null)
            {
                _assignLocalToClosure = new Instruction[_localInstrCacheSize];
            }

            if (index < _assignLocalToClosure.Length)
            {
                Emit(_assignLocalToClosure[index] ?? (_assignLocalToClosure[index] = new AssignLocalToClosureInstruction(index)));
            }
            else
            {
                Emit(new AssignLocalToClosureInstruction(index));
            }
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

        public void EmitByRefCall(MethodInfo method, ParameterInfo[] parameters, ByRefUpdater[] byrefArgs)
        {
            Emit(new ByRefMethodInfoCallInstruction(method, method.IsStatic ? parameters.Length : parameters.Length + 1, byrefArgs));
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

        public void EmitLoad(object value)
        {
            EmitLoad(value, null);
        }

        public void EmitLoad(bool value)
        {
            if (value)
            {
                Emit(_true ?? (_true = new LoadObjectInstruction(Utils.BoxedTrue)));
            }
            else
            {
                Emit(_false ?? (_false = new LoadObjectInstruction(Utils.BoxedFalse)));
            }
        }

        public void EmitLoad(object value, Type type)
        {
            if (value == null)
            {
                Emit(_null ?? (_null = new LoadObjectInstruction(null)));
                return;
            }

            if (type?.IsValueType != false)
            {
                switch (value)
                {
                    case bool b:
                        EmitLoad(b);
                        return;
                    case int i when i >= _pushIntMinCachedValue && i <= _pushIntMaxCachedValue:
                        if (_ints == null)
                        {
                            _ints = new Instruction[_pushIntMaxCachedValue - _pushIntMinCachedValue + 1];
                        }

                        i -= _pushIntMinCachedValue;
                        Emit(_ints[i] ?? (_ints[i] = new LoadObjectInstruction(i)));
                        return;

                    default:
                        break;
                }
            }

            if (_objects == null)
            {
                _objects = new List<object>();
                if (_loadObjectCached == null)
                {
                    _loadObjectCached = new Instruction[_cachedObjectCount];
                }
            }

            if (_objects.Count < _loadObjectCached.Length)
            {
                var index = (uint)_objects.Count;
                _objects.Add(value);
                Emit(_loadObjectCached[index] ?? (_loadObjectCached[index] = new LoadCachedObjectInstruction(index)));
            }
            else
            {
                Emit(new LoadObjectInstruction(value));
            }
        }

        public void EmitLoadField(FieldInfo field)
        {
            Emit(GetLoadField(field));
        }

        public void EmitLoadLocal(int index)
        {
            if (_loadLocal == null)
            {
                _loadLocal = new Instruction[_localInstrCacheSize];
            }

            if (index < _loadLocal.Length)
            {
                Emit(_loadLocal[index] ?? (_loadLocal[index] = new LoadLocalInstruction(index)));
            }
            else
            {
                Emit(new LoadLocalInstruction(index));
            }
        }

        public void EmitLoadLocalBoxed(int index)
        {
            Emit(LoadLocalBoxed(index));
        }

        public void EmitLoadLocalFromClosure(int index)
        {
            if (_loadLocalFromClosure == null)
            {
                _loadLocalFromClosure = new Instruction[_localInstrCacheSize];
            }

            if (index < _loadLocalFromClosure.Length)
            {
                Emit(_loadLocalFromClosure[index] ?? (_loadLocalFromClosure[index] = new LoadLocalFromClosureInstruction(index)));
            }
            else
            {
                Emit(new LoadLocalFromClosureInstruction(index));
            }
        }

        public void EmitLoadLocalFromClosureBoxed(int index)
        {
            if (_loadLocalFromClosureBoxed == null)
            {
                _loadLocalFromClosureBoxed = new Instruction[_localInstrCacheSize];
            }

            if (index < _loadLocalFromClosureBoxed.Length)
            {
                Emit(_loadLocalFromClosureBoxed[index] ?? (_loadLocalFromClosureBoxed[index] = new LoadLocalFromClosureBoxedInstruction(index)));
            }
            else
            {
                Emit(new LoadLocalFromClosureBoxedInstruction(index));
            }
        }

        public void EmitModulo(Type type)
        {
            Emit(ModuloInstruction.Create(type));
        }

        public void EmitMul(Type type, bool @checked)
        {
            Emit(@checked ? MulOvfInstruction.Create(type) : MulInstruction.Create(type));
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

        public void EmitStoreLocal(int index)
        {
            if (_storeLocal == null)
            {
                _storeLocal = new Instruction[_localInstrCacheSize];
            }

            if (index < _storeLocal.Length)
            {
                Emit(_storeLocal[index] ?? (_storeLocal[index] = new StoreLocalInstruction(index)));
            }
            else
            {
                Emit(new StoreLocalInstruction(index));
            }
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

        public void EmitSub(Type type, bool @checked)
        {
            Emit(@checked ? SubOvfInstruction.Create(type) : SubInstruction.Create(type));
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
        public void SetDebugCookie(object cookie)
        {
#if DEBUG
            if (_debugCookies == null)
            {
                _debugCookies = new List<KeyValuePair<int, object>>();
            }

            Debug.Assert(Count > 0);
            _debugCookies.Add(new KeyValuePair<int, object>(Count - 1, cookie));
#else
            Theraot.No.Op(cookie);
            _debugCookies = null;
#endif
        }

        public InstructionArray ToArray()
        {
#if STATS
            lock (_executedInstructions)
            {
                _instructions.ForEach((instr) =>
                {
                    int value = 0;
                    var name = instr.GetType().Name;
                    _executedInstructions.TryGetValue(name, out value);
                    _executedInstructions[name] = value + 1;

                    Dictionary<object, bool> dict;
                    if (!_instances.TryGetValue(name, out dict))
                    {
                        _instances[name] = dict = new Dictionary<object, bool>();
                    }
                    dict[instr] = true;
                });
            }
#endif
            return new InstructionArray(
                MaxStackDepth,
                _maxContinuationDepth,
                Theraot.Collections.Extensions.AsArrayInternal(_instructions),
                _objects == null ? null : Theraot.Collections.Extensions.AsArrayInternal(_objects),
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

        internal static Instruction AssignLocalBoxed(int index)
        {
            if (_assignLocalBoxed == null)
            {
                _assignLocalBoxed = new Instruction[_localInstrCacheSize];
            }

            if (index < _assignLocalBoxed.Length)
            {
                return _assignLocalBoxed[index] ?? (_assignLocalBoxed[index] = new AssignLocalBoxedInstruction(index));
            }

            return new AssignLocalBoxedInstruction(index);
        }

        internal static Instruction InitImmutableRefBox(int index)
        {
            return new InitializeLocalInstruction.ImmutableRefBox(index);
        }

        internal static Instruction InitReference(int index)
        {
            return new InitializeLocalInstruction.Reference(index);
        }

        internal static Instruction LoadLocalBoxed(int index)
        {
            if (_loadLocalBoxed == null)
            {
                _loadLocalBoxed = new Instruction[_localInstrCacheSize];
            }

            if (index < _loadLocalBoxed.Length)
            {
                return _loadLocalBoxed[index] ?? (_loadLocalBoxed[index] = new LoadLocalBoxedInstruction(index));
            }

            return new LoadLocalBoxedInstruction(index);
        }

        internal static Instruction Parameter(int index)
        {
            return new InitializeLocalInstruction.Parameter(index);
        }

        internal static Instruction ParameterBox(int index)
        {
            return new InitializeLocalInstruction.ParameterBox(index);
        }

        internal static Instruction StoreLocalBoxed(int index)
        {
            if (_storeLocalBoxed == null)
            {
                _storeLocalBoxed = new Instruction[_localInstrCacheSize];
            }

            if (index < _storeLocalBoxed.Length)
            {
                return _storeLocalBoxed[index] ?? (_storeLocalBoxed[index] = new StoreLocalBoxedInstruction(index));
            }

            return new StoreLocalBoxedInstruction(index);
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

        internal Instruction GetInstruction(int index) => _instructions[index];

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

        private RuntimeLabel[] BuildRuntimeLabels()
        {
            if (_runtimeLabelCount == 0)
            {
                return _emptyRuntimeLabels;
            }

            var result = new RuntimeLabel[_runtimeLabelCount + 1];
            foreach (var label in _labels)
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

        private void EmitBranch(OffsetInstruction instruction, BranchLabel label)
        {
            Emit(instruction);
            label.AddBranch(this, Count - 1);
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

        internal sealed class DebugView
        {
            private readonly InstructionList _list;

            public DebugView(InstructionList list)
            {
                ContractUtils.RequiresNotNull(list, nameof(list));
                _list = list;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public InstructionView[]/*!*/ A0 => GetInstructionViews(true);

            public InstructionView[] GetInstructionViews(bool includeDebugCookies = false)
            {
                return GetInstructionViews(
                        _list._instructions,
                        _list._objects,
                        index => _list._labels[index].TargetIndex,
                        includeDebugCookies ? _list._debugCookies : null
                    );
            }

            internal static InstructionView[] GetInstructionViews(IList<Instruction> instructions, IList<object> objects,
                Func<int, int> labelIndexer, IList<KeyValuePair<int, object>> debugCookies)
            {
                var result = new List<InstructionView>();
                var stackDepth = 0;
                var continuationsDepth = 0;

                using
                (
                    var cookieEnumerator =
                    (
                        debugCookies ??
                        ArrayReservoir<KeyValuePair<int, object>>.EmptyArray
                    )
                    .GetEnumerator()
                )
                {
                    var hasCookie = cookieEnumerator.MoveNext();

                    for (int i = 0, n = instructions.Count; i < n; i++)
                    {
                        var instruction = instructions[i];

                        object cookie = null;
                        while (hasCookie && cookieEnumerator.Current.Key == i)
                        {
                            cookie = cookieEnumerator.Current.Value;
                            hasCookie = cookieEnumerator.MoveNext();
                        }

                        var stackDiff = instruction.StackBalance;
                        var contDiff = instruction.ContinuationsBalance;
                        var name = instruction.ToDebugString(i, cookie, labelIndexer, objects);
                        result.Add(new InstructionView(instruction, name, i, stackDepth, continuationsDepth));

                        stackDepth += stackDiff;
                        continuationsDepth += contDiff;
                    }

                    return Theraot.Collections.Extensions.AsArrayInternal(result);
                }
            }

            [DebuggerDisplay("{GetValue(),nq}", Name = "{GetName(),nq}", Type = "{GetDisplayType(), nq}")]
            internal /*readonly*/ struct InstructionView
            {
                private readonly int _continuationsDepth;
                private readonly int _index;
                private readonly Instruction _instruction;
                private readonly string _name;
                private readonly int _stackDepth;

                public InstructionView(Instruction instruction, string name, int index, int stackDepth, int continuationsDepth)
                {
                    _instruction = instruction;
                    _name = name;
                    _index = index;
                    _stackDepth = stackDepth;
                    _continuationsDepth = continuationsDepth;
                }

                internal string GetDisplayType()
                {
                    return _instruction.ContinuationsBalance + "/" + _instruction.StackBalance;
                }

                internal string GetName()
                {
                    return _index +
                        (_continuationsDepth == 0 ? "" : " C(" + _continuationsDepth + ")") +
                        (_stackDepth == 0 ? "" : " S(" + _stackDepth + ")");
                }

                internal string GetValue()
                {
                    return _name;
                }
            }
        }
    }
}

#endif