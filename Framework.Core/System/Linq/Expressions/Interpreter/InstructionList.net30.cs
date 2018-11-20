#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Enables instruction counting and displaying stats at process exit.
// TODO update from corefx

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter
{
    [DebuggerTypeProxy(typeof(DebugView))]
    internal /*readonly*/ struct InstructionArray
    {
        // list of (instruction index, cookie) sorted by instruction index:
        internal readonly List<KeyValuePair<int, object>> DebugCookies;

        internal readonly Instruction[] Instructions;
        internal readonly RuntimeLabel[] Labels;
        internal readonly int MaxContinuationDepth;
        internal readonly int MaxStackDepth;
        internal readonly object[] Objects;

        internal InstructionArray(int maxStackDepth, int maxContinuationDepth, Instruction[] instructions,
            object[] objects, RuntimeLabel[] labels, List<KeyValuePair<int, object>> debugCookies)
        {
            MaxStackDepth = maxStackDepth;
            MaxContinuationDepth = maxContinuationDepth;
            Instructions = instructions;
            DebugCookies = debugCookies;
            Objects = objects;
            Labels = labels;
        }

        internal int Length
        {
            get { return Instructions.Length; }
        }

        #region Debug View

        internal sealed class DebugView
        {
            private readonly InstructionArray _array;

            public DebugView(InstructionArray array)
            {
                _array = array;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public InstructionList.DebugView.InstructionView[]/*!*/ A0
            {
                get
                {
                    var labels = _array.Labels;
                    return InstructionList.DebugView.GetInstructionViews(
                        _array.Instructions,
                        _array.Objects,
                        (index) => labels[index].Index,
                        _array.DebugCookies
                    );
                }
            }
        }

        #endregion Debug View
    }

    [DebuggerTypeProxy(typeof(DebugView))]
    internal sealed class InstructionList
    {
        private readonly List<Instruction> _instructions = new List<Instruction>();
        private int _currentContinuationsDepth;
        private int _currentStackDepth;

        // list of (instruction index, cookie) sorted by instruction index:
        private List<KeyValuePair<int, object>> _debugCookies;

        private List<BranchLabel> _labels;
        private int _maxContinuationDepth;
        private int _maxStackDepth;
        private List<object> _objects;
        private int _runtimeLabelCount;

        #region Debug View

        internal sealed class DebugView
        {
            private readonly InstructionList _list;

            public DebugView(InstructionList list)
            {
                _list = list;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public InstructionView[]/*!*/ A0
            {
                get
                {
                    var labels = _list._labels;
                    return GetInstructionViews(
                        _list._instructions,
                        _list._objects,
                        (index) => labels[index].TargetIndex,
                        _list._debugCookies
                    );
                }
            }

            internal static InstructionView[] GetInstructionViews(IList<Instruction> instructions, IList<object> objects,
                Func<int, int> labelIndexer, IList<KeyValuePair<int, object>> debugCookies)
            {
                var result = new List<InstructionView>();
                var stackDepth = 0;
                var continuationsDepth = 0;

                using (var cookieEnumerator = (debugCookies ?? new KeyValuePair<int, object>[0]).GetEnumerator())
                {
                    var hasCookie = cookieEnumerator.MoveNext();

                    for (var i = 0; i < instructions.Count; i++)
                    {
                        object cookie = null;
                        while (hasCookie && cookieEnumerator.Current.Key == i)
                        {
                            cookie = cookieEnumerator.Current.Value;
                            hasCookie = cookieEnumerator.MoveNext();
                        }

                        var stackDiff = instructions[i].StackBalance;
                        var contDiff = instructions[i].ContinuationsBalance;
                        var name = instructions[i].ToDebugString(i, cookie, labelIndexer, objects);
                        result.Add(new InstructionView(instructions[i], name, i, stackDepth, continuationsDepth));

                        stackDepth += stackDiff;
                        continuationsDepth += contDiff;
                    }

                    return result.ToArray();
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
                    return $"{_instruction.ContinuationsBalance}/{_instruction.StackBalance}";
                }

                internal string GetName()
                {
                    return _index +
                        (_continuationsDepth == 0 ? "" : $" C({_continuationsDepth})") +
                        (_stackDepth == 0 ? "" : $" S({_stackDepth})");
                }

                internal string GetValue()
                {
                    return _name;
                }
            }
        }

        #endregion Debug View

        #region Core Emit Ops

        public int Count
        {
            get { return _instructions.Count; }
        }

        public int CurrentContinuationsDepth
        {
            get { return _currentContinuationsDepth; }
        }

        public int CurrentStackDepth
        {
            get { return _currentStackDepth; }
        }

        public int MaxStackDepth
        {
            get { return _maxStackDepth; }
        }

        public void Emit(Instruction instruction)
        {
            _instructions.Add(instruction);
            UpdateStackDepth(instruction);
        }

        [Conditional("DEBUG")]
        public void SetDebugCookie(object cookie)
        {
            if (_debugCookies == null)
            {
                _debugCookies = new List<KeyValuePair<int, object>>();
            }

            Debug.Assert(Count > 0);
            _debugCookies.Add(new KeyValuePair<int, object>(Count - 1, cookie));
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
                _maxStackDepth,
                _maxContinuationDepth,
                _instructions.ToArray(),
                (_objects != null) ? _objects.ToArray() : null,
                BuildRuntimeLabels(),
                _debugCookies
            );
        }

        internal Instruction GetInstruction(int index)
        {
            return _instructions[index];
        }

        private void UpdateStackDepth(Instruction instruction)
        {
            Debug.Assert(instruction.ConsumedStack >= 0 && instruction.ProducedStack >= 0 &&
                instruction.ConsumedContinuations >= 0 && instruction.ProducedContinuations >= 0, "bad instruction " + instruction);

            _currentStackDepth -= instruction.ConsumedStack;
            Debug.Assert(_currentStackDepth >= 0, "negative stack depth " + instruction);
            _currentStackDepth += instruction.ProducedStack;
            if (_currentStackDepth > _maxStackDepth)
            {
                _maxStackDepth = _currentStackDepth;
            }

            _currentContinuationsDepth -= instruction.ConsumedContinuations;
            Debug.Assert(_currentContinuationsDepth >= 0, "negative continuations " + instruction);
            _currentContinuationsDepth += instruction.ProducedContinuations;
            if (_currentContinuationsDepth > _maxContinuationDepth)
            {
                _maxContinuationDepth = _currentContinuationsDepth;
            }
        }

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

        #endregion Core Emit Ops

        #region Stack Operations

        private const int _cachedObjectCount = 256;
        private const int _pushIntMaxCachedValue = 100;
        private const int _pushIntMinCachedValue = -100;
        private static Instruction _false;
        private static Instruction[] _ints;
        private static Instruction[] _loadObjectCached;
        private static Instruction _null;
        private static Instruction _true;

        public void EmitDup()
        {
            Emit(DupInstruction.Instance);
        }

        public void EmitLoad(object value)
        {
            EmitLoad(value, null);
        }

        public void EmitLoad(bool value)
        {
            if (value)
            {
                Emit(_true ?? (_true = new LoadObjectInstruction(value)));
            }
            else
            {
                Emit(_false ?? (_false = new LoadObjectInstruction(value)));
            }
        }

        public void EmitLoad(object value, Type type)
        {
            if (value == null)
            {
                Emit(_null ?? (_null = new LoadObjectInstruction(null)));
                return;
            }

            if (type == null || type.IsValueType)
            {
                if (value is bool)
                {
                    EmitLoad((bool)value);
                    return;
                }

                if (value is int)
                {
                    var i = (int)value;
                    if (i >= _pushIntMinCachedValue && i <= _pushIntMaxCachedValue)
                    {
                        if (_ints == null)
                        {
                            _ints = new Instruction[_pushIntMaxCachedValue - _pushIntMinCachedValue + 1];
                        }
                        i -= _pushIntMinCachedValue;
                        Emit(_ints[i] ?? (_ints[i] = new LoadObjectInstruction(value)));
                        return;
                    }
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

        public void EmitPop()
        {
            Emit(PopInstruction.Instance);
        }

        #endregion Stack Operations

        #region Locals

        private const int _localInstrCacheSize = 64;

        private static Instruction[] _assignLocal;

        private static Instruction[] _assignLocalBoxed;

        private static Instruction[] _assignLocalToClosure;

        private static Instruction[] _loadLocal;

        private static Instruction[] _loadLocalBoxed;

        private static Instruction[] _loadLocalFromClosure;

        private static Instruction[] _loadLocalFromClosureBoxed;

        private static Instruction[] _storeLocal;

        private static Instruction[] _storeLocalBoxed;

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

        public void EmitNewRuntimeVariables(int count)
        {
            Emit(new RuntimeVariablesInstruction(count));
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

        internal static Instruction Parameter(int index, Type parameterType)
        {
            return new InitializeLocalInstruction.Parameter(index, parameterType);
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

        internal void EmitInitializeParameter(int index, Type parameterType)
        {
            Emit(Parameter(index, parameterType));
        }

        internal void SwitchToBoxed(int index, int instructionIndex)
        {
            if (_instructions[instructionIndex] is IBoxableInstruction instruction)
            {
                var newInstruction = instruction.BoxIfIndexMatches(index);
                if (newInstruction != null)
                {
                    _instructions[instructionIndex] = newInstruction;
                }
            }
        }

        #endregion Locals

        #region Array Operations

        public void EmitGetArrayItem()
        {
            Emit(GetArrayItemInstruction.Instruction);
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

        public void EmitSetArrayItem()
        {
            Emit(new SetArrayItemInstruction());
        }

        #endregion Array Operations

        #region Arithmetic Operations

        public void EmitAdd(Type type, bool @checked)
        {
            if (@checked)
            {
                Emit(AddOvfInstruction.Create(type));
            }
            else
            {
                Emit(AddInstruction.Create(type));
            }
        }

        public void EmitDiv(Type type)
        {
            Emit(DivInstruction.Create(type));
        }

        public void EmitModulo(Type type)
        {
            Emit(ModuloInstruction.Create(type));
        }

        public void EmitMul(Type type, bool @checked)
        {
            if (@checked)
            {
                Emit(MulOvfInstruction.Create(type));
            }
            else
            {
                Emit(MulInstruction.Create(type));
            }
        }

        public void EmitSub(Type type, bool @checked)
        {
            if (@checked)
            {
                Emit(SubOvfInstruction.Create(type));
            }
            else
            {
                Emit(SubInstruction.Create(type));
            }
        }

        #endregion Arithmetic Operations

        #region Comparisons

        public void EmitAnd(Type type)
        {
            Emit(AndInstruction.Create(type));
        }

        public void EmitEqual(Type type)
        {
            Emit(EqualInstruction.Create(type, false));
        }

        public void EmitEqual(Type type, bool liftedToNull)
        {
            Emit(EqualInstruction.Create(type, liftedToNull));
        }

        public void EmitExclusiveOr(Type type)
        {
            Emit(ExclusiveOrInstruction.Create(type));
        }

        public void EmitGreaterThan(Type type, bool liftedToNull)
        {
            Emit(GreaterThanInstruction.Create(type, liftedToNull));
        }

        public void EmitGreaterThanOrEqual(Type type, bool liftedToNull)
        {
            Emit(GreaterThanOrEqualInstruction.Create(type, liftedToNull));
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

        public void EmitNotEqual(Type type)
        {
            Emit(NotEqualInstruction.Create(type, false));
        }

        public void EmitNotEqual(Type type, bool liftedToNull)
        {
            Emit(NotEqualInstruction.Create(type, liftedToNull));
        }

        public void EmitOr(Type type)
        {
            Emit(OrInstruction.Create(type));
        }

        public void EmitRightShift(Type type)
        {
            Emit(RightShiftInstruction.Create(type));
        }

        #endregion Comparisons

        #region Conversions

        public void EmitCast(Type toType)
        {
            Emit(CastInstruction.Create(toType));
        }

        public void EmitCastToEnum(Type toType)
        {
            Emit(new CastToEnumInstruction(toType));
        }

        public void EmitNumericConvertChecked(TypeCode from, TypeCode to, bool isLiftedToNull)
        {
            Emit(new NumericConvertInstruction.Checked(from, to, isLiftedToNull));
        }

        public void EmitNumericConvertUnchecked(TypeCode from, TypeCode to, bool isLiftedToNull)
        {
            Emit(new NumericConvertInstruction.Unchecked(from, to, isLiftedToNull));
        }

        #endregion Conversions

        #region Boolean Operators

        public void EmitNot(Type type)
        {
            Emit(NotInstruction.Create(type));
        }

        #endregion Boolean Operators

        #region Types

        public void EmitArrayLength()
        {
            Emit(ArrayLengthInstruction.Instance);
        }

        public void EmitByRefNew(ConstructorInfo constructorInfo, ByRefUpdater[] updaters)
        {
            Emit(new ByRefNewInstruction(constructorInfo, updaters));
        }

        public void EmitDecrement(Type type)
        {
            Emit(DecrementInstruction.Create(type));
        }

        public void EmitDefaultValue(Type type)
        {
            Emit(new DefaultValueInstruction(type));
        }

        public void EmitIncrement(Type type)
        {
            Emit(IncrementInstruction.Create(type));
        }

        public void EmitNegate(Type type)
        {
            Emit(NegateInstruction.Create(type));
        }

        public void EmitNegateChecked(Type type)
        {
            Emit(NegateCheckedInstruction.Create(type));
        }

        public void EmitNew(ConstructorInfo constructorInfo)
        {
            Emit(new NewInstruction(constructorInfo));
        }

        public void EmitNullableTypeEquals()
        {
            Emit(NullableTypeEqualsInstruction.Instance);
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

        internal void EmitCreateDelegate(LightDelegateCreator creator)
        {
            Emit(new CreateDelegateInstruction(creator));
        }

        #endregion Types

        #region Fields and Methods

        private static readonly Dictionary<FieldInfo, Instruction> _loadFields = new Dictionary<FieldInfo, Instruction>();

        public void EmitByRefCall(MethodInfo method, ParameterInfo[] parameters, ByRefUpdater[] byrefArgs)
        {
            Emit(new ByRefMethodInfoCallInstruction(method, method.IsStatic ? parameters.Length : parameters.Length + 1, byrefArgs));
        }

        public void EmitCall(MethodInfo method)
        {
            EmitCall(method, method.GetParameters());
        }

        public void EmitCall(MethodInfo method, ParameterInfo[] parameters)
        {
            Emit(CallInstruction.Create(method, parameters));
        }

        public void EmitLoadField(FieldInfo field)
        {
            Emit(GetLoadField(field));
        }

        public void EmitNullableCall(MethodInfo method, ParameterInfo[] parameters)
        {
            Emit(NullableMethodCallInstruction.Create(method.Name, parameters.Length));
        }

        public void EmitNullCheck(int stackOffset)
        {
            Emit(NullCheckInstruction.Create(stackOffset));
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

        private static Instruction GetLoadField(FieldInfo field)
        {
            lock (_loadFields)
            {
                if (!_loadFields.TryGetValue(field, out Instruction instruction))
                {
                    if (field.IsStatic)
                    {
                        instruction = new LoadStaticFieldInstruction(field);
                    }
                    else
                    {
                        instruction = new LoadFieldInstruction(field);
                    }
                    _loadFields.Add(field, instruction);
                }
                return instruction;
            }
        }

        #endregion Fields and Methods

        #region Control Flow

        private static readonly RuntimeLabel[] _emptyRuntimeLabels = { new RuntimeLabel(Interpreter.RethrowOnReturn, 0, 0) };

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

        public void EmitCoalescingBranch(BranchLabel leftNotNull)
        {
            EmitBranch(new CoalescingBranchInstruction(), leftNotNull);
        }

        public void EmitEnterExceptionHandlerNonVoid()
        {
            Emit(EnterExceptionHandlerInstruction.NonVoid);
        }

        public void EmitEnterExceptionHandlerVoid()
        {
            Emit(EnterExceptionHandlerInstruction.Void);
        }

        public void EmitEnterFinally(BranchLabel finallyStartLabel)
        {
            Emit(EnterFinallyInstruction.Create(EnsureLabelIndex(finallyStartLabel)));
        }

        public void EmitEnterTryCatch()
        {
            Emit(EnterTryCatchFinallyInstruction.CreateTryCatch());
        }

        public void EmitEnterTryFinally(BranchLabel finallyStartLabel)
        {
            Emit(EnterTryCatchFinallyInstruction.CreateTryFinally(EnsureLabelIndex(finallyStartLabel)));
        }

        public void EmitGoto(BranchLabel label, bool hasResult, bool hasValue, bool labelTargetGetsValue)
        {
            Emit(GotoInstruction.Create(EnsureLabelIndex(label), hasResult, hasValue, labelTargetGetsValue));
        }

        public void EmitIntSwitch<T>(Dictionary<T, int> cases)
        {
            Emit(new IntSwitchInstruction<T>(cases));
        }

        public void EmitLeaveExceptionHandler(bool hasValue, BranchLabel tryExpressionEndLabel)
        {
            Emit(LeaveExceptionHandlerInstruction.Create(EnsureLabelIndex(tryExpressionEndLabel), hasValue));
        }

        public void EmitLeaveFault(bool hasValue)
        {
            Emit(hasValue ? LeaveFaultInstruction.NonVoid : LeaveFaultInstruction.Void);
        }

        public void EmitLeaveFinally()
        {
            Emit(LeaveFinallyInstruction.Instance);
        }

        public void EmitRethrow()
        {
            Emit(ThrowInstruction.Rethrow);
        }

        public void EmitRethrowVoid()
        {
            Emit(ThrowInstruction.VoidRethrow);
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

        internal void FixupBranch(int branchIndex, int offset)
        {
            _instructions[branchIndex] = ((OffsetInstruction)_instructions[branchIndex]).Fixup(offset);
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

        #endregion Control Flow
    }
}

#endif