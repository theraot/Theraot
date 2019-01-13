#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections.ThreadSafe;
using Theraot.Reflection;
using static System.Linq.Expressions.CachedReflectionInfo;

using AstUtils = System.Linq.Expressions.Utils;

namespace System.Linq.Expressions.Interpreter
{
    internal /*readonly*/ struct InterpretedFrameInfo
    {
        private readonly DebugInfo _debugInfo;

        private readonly string _methodName;

        public InterpretedFrameInfo(string methodName, DebugInfo info)
        {
            _methodName = methodName;
            _debugInfo = info;
        }

        public override string ToString() => _debugInfo != null ? _methodName + ": " + _debugInfo : _methodName;
    }

    internal sealed class ArrayByRefUpdater : ByRefUpdater
    {
        private readonly LocalDefinition _array, _index;

        public ArrayByRefUpdater(LocalDefinition array, LocalDefinition index, int argumentIndex)
            : base(argumentIndex)
        {
            _array = array;
            _index = index;
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            locals.UndefineLocal(_array, instructions.Count);
            locals.UndefineLocal(_index, instructions.Count);
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            var index = frame.Data[_index.Index];
            ((Array)frame.Data[_array.Index]).SetValue(value, (int)index);
        }
    }

    internal abstract class ByRefUpdater
    {
        public readonly int ArgumentIndex;

        protected ByRefUpdater(int argumentIndex)
        {
            ArgumentIndex = argumentIndex;
        }

        public virtual void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
        }

        public abstract void Update(InterpretedFrame frame, object value);
    }

    internal sealed class DebugInfo
    {
        public string FileName;
        public int Index;
        public bool IsClear;
        public int StartLine, EndLine;
        private static readonly DebugInfoComparer _debugComparer = new DebugInfoComparer();

        public static DebugInfo GetMatchingDebugInfo(DebugInfo[] debugInfos, int index)
        {
            //Create a faked DebugInfo to do the search
            var d = new DebugInfo { Index = index };

            //to find the closest debug info before the current index

            var i = Array.BinarySearch(debugInfos, d, _debugComparer);
            if (i < 0)
            {
                //~i is the index for the first bigger element
                //if there is no bigger element, ~i is the length of the array
                i = ~i;
                if (i == 0)
                {
                    return null;
                }
                //return the last one that is smaller
                i = i - 1;
            }

            return debugInfos[i];
        }

        public override string ToString()
        {
            if (IsClear)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}: clear", Index);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}: [{1}-{2}] '{3}'", Index, StartLine, EndLine, FileName);
        }

        private class DebugInfoComparer : IComparer<DebugInfo>
        {
            //We allow comparison between int and DebugInfo here
            int IComparer<DebugInfo>.Compare(DebugInfo x, DebugInfo y)
            {
                var d1Index = x.Index;
                var d2Index = y.Index;
                if (d1Index > d2Index)
                {
                    return 1;
                }

                if (d1Index == d2Index)
                {
                    return 0;
                }

                return -1;
            }
        }
    }

    internal sealed class ExceptionFilter
    {
        public readonly int EndIndex;
        public readonly int LabelIndex;
        public readonly int StartIndex;

        internal ExceptionFilter(int labelIndex, int start, int end)
        {
            LabelIndex = labelIndex;
            StartIndex = start;
            EndIndex = end;
        }
    }

    internal sealed class ExceptionHandler
    {
        public readonly ExceptionFilter Filter;
        public readonly int HandlerEndIndex;
        public readonly int HandlerStartIndex;
        public readonly int LabelIndex;
        private readonly Type _exceptionType;

        internal ExceptionHandler(int labelIndex, int handlerStartIndex, int handlerEndIndex, Type exceptionType, ExceptionFilter filter)
        {
            Debug.Assert(exceptionType != null);
            LabelIndex = labelIndex;
            _exceptionType = exceptionType;
            HandlerStartIndex = handlerStartIndex;
            HandlerEndIndex = handlerEndIndex;
            Filter = filter;
        }

        public bool Matches(Type exceptionType) => _exceptionType.IsAssignableFrom(exceptionType);

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "catch({0}) [{1}->{2}]", _exceptionType.Name, HandlerStartIndex, HandlerEndIndex);
    }

    internal sealed class FieldByRefUpdater : ByRefUpdater
    {
        private readonly FieldInfo _field;
        private readonly LocalDefinition? _object;

        public FieldByRefUpdater(LocalDefinition? obj, FieldInfo field, int argumentIndex)
            : base(argumentIndex)
        {
            _object = obj;
            _field = field;
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            if (_object != null)
            {
                locals.UndefineLocal(_object.GetValueOrDefault(), instructions.Count);
            }
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            var obj = _object == null ? null : frame.Data[_object.GetValueOrDefault().Index];
            _field.SetValue(obj, value);
        }
    }

    internal sealed class IndexMethodByRefUpdater : ByRefUpdater
    {
        private readonly LocalDefinition[] _args;
        private readonly MethodInfo _indexer;
        private readonly LocalDefinition? _obj;

        public IndexMethodByRefUpdater(LocalDefinition? obj, LocalDefinition[] args, MethodInfo indexer, int argumentIndex)
            : base(argumentIndex)
        {
            _obj = obj;
            _args = args;
            _indexer = indexer;
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            if (_obj != null)
            {
                locals.UndefineLocal(_obj.GetValueOrDefault(), instructions.Count);
            }
            foreach (var arg in _args)
            {
                locals.UndefineLocal(arg, instructions.Count);
            }
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            var args = new object[_args.Length + 1];
            for (var i = 0; i < args.Length - 1; i++)
            {
                args[i] = frame.Data[_args[i].Index];
            }
            args[args.Length - 1] = value;

            var instance = _obj == null ? null : frame.Data[_obj.GetValueOrDefault().Index];

            try
            {
                _indexer.Invoke(instance, args);
            }
            catch (TargetInvocationException e)
            {
                ExceptionHelpers.UnwrapAndRethrow(e);
                throw ContractUtils.Unreachable;
            }
        }
    }

    internal sealed class LightCompiler
    {
        private static readonly LocalDefinition[] _emptyLocals = ArrayReservoir<LocalDefinition>.EmptyArray;
        private readonly List<DebugInfo> _debugInfos = new List<DebugInfo>();
        private readonly Stack<ParameterExpression> _exceptionForRethrowStack = new Stack<ParameterExpression>();
        private readonly StackGuard _guard = new StackGuard();
        private readonly LocalVariables _locals = new LocalVariables();
        private readonly LightCompiler _parent;
        private readonly HybridReferenceDictionary<LabelTarget, LabelInfo> _treeLabels = new HybridReferenceDictionary<LabelTarget, LabelInfo>();
        private LabelScopeInfo _labelBlock = new LabelScopeInfo(null, LabelScopeKind.Lambda);

        public LightCompiler()
        {
            Instructions = new InstructionList();
        }

        private LightCompiler(LightCompiler parent)
            : this()
        {
            _parent = parent;
        }

        public InstructionList Instructions { get; }

        public LightDelegateCreator CompileTop(LambdaExpression node)
        {
            node.ValidateArgumentCount();

            //Console.WriteLine(node.DebugView);
            for (int i = 0, n = node.ParameterCount; i < n; i++)
            {
                var p = node.GetParameter(i);
                var local = _locals.DefineLocal(p, 0);
                Instructions.EmitInitializeParameter(local.Index);
            }

            Compile(node.Body);

            // pop the result of the last expression:
            if (node.Body.Type != typeof(void) && node.ReturnType == typeof(void))
            {
                Instructions.EmitPop();
            }

            Debug.Assert(Instructions.CurrentStackDepth == (node.ReturnType != typeof(void) ? 1 : 0));

            return new LightDelegateCreator(MakeInterpreter(node.Name), node);
        }

        private static Type GetMemberType(MemberInfo member)
        {
            if (member is FieldInfo fi)
            {
                return fi.FieldType;
            }

            if (member is PropertyInfo pi)
            {
                return pi.PropertyType;
            }

            throw new InvalidOperationException("MemberNotFieldOrProperty");
        }

#if DEBUG

        private static bool IsNullComparison(Expression left, Expression right)
        {
            return IsNullConstant(left)
                ? !IsNullConstant(right) && right.Type.IsNullable()
                : IsNullConstant(right) && left.Type.IsNullable();
        }

        private static bool IsNullConstant(Expression e)
        {
            return e is ConstantExpression c && c.Value == null;
        }

#endif

        private static bool ShouldWritebackNode(Expression node)
        {
            if (node.Type.IsValueType)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Parameter:
                    case ExpressionType.Call:
                    case ExpressionType.ArrayIndex:
                        return true;

                    case ExpressionType.Index:
                        return ((IndexExpression)node).Object.Type.IsArray;

                    case ExpressionType.MemberAccess:
                        return ((MemberExpression)node).Member is FieldInfo;
                        // ExpressionType.Unbox does have the behaviour writeback is used to simulate, but
                        // it doesn't need explicit writeback to produce it, so include it in the default
                        // false cases.
                }
            }
            return false;
        }

        private void CheckRethrow()
        {
            // Rethrow is only valid inside a catch.
            for (var j = _labelBlock; j != null; j = j.Parent)
            {
                if (j.Kind == LabelScopeKind.Catch)
                {
                    return;
                }

                if (j.Kind == LabelScopeKind.Finally)
                {
                    // Rethrow from inside finally is not verifiable
                    break;
                }
            }
            throw Error.RethrowRequiresCatch();
        }

        private void Compile(Expression expr, bool asVoid)
        {
            if (asVoid)
            {
                CompileAsVoid(expr);
            }
            else
            {
                Compile(expr);
            }
        }

        private void Compile(Expression expr)
        {
            var pushLabelBlock = TryPushLabelBlock(expr);
            CompileNoLabelPush(expr);
            if (pushLabelBlock)
            {
                PopLabelBlock(_labelBlock.Kind);
            }
        }

        /// <summary>
        /// Emits the address of the specified node.
        /// </summary>
        private ByRefUpdater CompileAddress(Expression node, int index)
        {
            if (index != -1 || ShouldWritebackNode(node))
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Parameter:
                        LoadLocalNoValueTypeCopy((ParameterExpression)node);

                        return new ParameterByRefUpdater(ResolveLocal((ParameterExpression)node), index);

                    case ExpressionType.ArrayIndex:
                        var array = (BinaryExpression)node;

                        return CompileArrayIndexAddress(array.Left, array.Right, index);

                    case ExpressionType.Index:
                        var indexNode = (IndexExpression)node;
                        if (/*!TypeUtils.AreEquivalent(type, node.Type) || */indexNode.Indexer != null)
                        {
                            LocalDefinition? objTmp = null;
                            if (indexNode.Object != null)
                            {
                                objTmp = _locals.DefineLocal(Expression.Parameter(indexNode.Object.Type), Instructions.Count);
                                EmitThisForMethodCall(indexNode.Object);
                                Instructions.EmitDup();
                                Instructions.EmitStoreLocal(objTmp.GetValueOrDefault().Index);
                            }

                            var count = indexNode.ArgumentCount;
                            var indexLocals = new LocalDefinition[count];
                            for (var i = 0; i < count; i++)
                            {
                                var arg = indexNode.GetArgument(i);
                                Compile(arg);

                                var argTmp = _locals.DefineLocal(Expression.Parameter(arg.Type), Instructions.Count);
                                Instructions.EmitDup();
                                Instructions.EmitStoreLocal(argTmp.Index);

                                indexLocals[i] = argTmp;
                            }

                            EmitIndexGet(indexNode);

                            return new IndexMethodByRefUpdater(objTmp, indexLocals, indexNode.Indexer.GetSetMethod(), index);
                        }
                        else if (indexNode.ArgumentCount == 1)
                        {
                            return CompileArrayIndexAddress(indexNode.Object, indexNode.GetArgument(0), index);
                        }
                        else
                        {
                            return CompileMultiDimArrayAccess(indexNode.Object, indexNode, index);
                        }
                    case ExpressionType.MemberAccess:
                        var member = (MemberExpression)node;

                        LocalDefinition? memberTemp = null;
                        if (member.Expression != null)
                        {
                            memberTemp = _locals.DefineLocal(Expression.Parameter(member.Expression.Type, "member"), Instructions.Count);
                            EmitThisForMethodCall(member.Expression);
                            Instructions.EmitDup();
                            Instructions.EmitStoreLocal(memberTemp.GetValueOrDefault().Index);
                        }

                        if (member.Member is FieldInfo field)
                        {
                            Instructions.EmitLoadField(field);
                            if (!field.IsLiteral && !field.IsInitOnly)
                            {
                                return new FieldByRefUpdater(memberTemp, field, index);
                            }
                            return null;
                        }
                        Debug.Assert(member.Member is PropertyInfo);
                        var property = (PropertyInfo)member.Member;
                        Instructions.EmitCall(property.GetGetMethod(nonPublic: true));
                        if (property.CanWrite)
                        {
                            return new PropertyByRefUpdater(memberTemp, property, index);
                        }
                        return null;

                    case ExpressionType.Call:
                        // An array index of a multi-dimensional array is represented by a call to Array.Get,
                        // rather than having its own array-access node. This means that when we are trying to
                        // get the address of a member of a multi-dimensional array, we'll be trying to
                        // get the address of a Get method, and it will fail to do so. Instead, detect
                        // this situation and replace it with a call to the Address method.
                        var call = (MethodCallExpression)node;
                        if (!call.Method.IsStatic &&
                            call.Object.Type.IsArray &&
                            call.Method == call.Object.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance))
                        {
                            return CompileMultiDimArrayAccess(
                                call.Object,
                                call,
                                index
                            );
                        }
                        break;
                }
            }
            // Includes Unbox case as it doesn't need explicit writeback.
            Compile(node);
            return null;
        }

        private void CompileAndAlsoBinaryExpression(Expression expr)
        {
            CompileLogicalBinaryExpression((BinaryExpression)expr, andAlso: true);
        }

        private void CompileArithmetic(ExpressionType nodeType, Expression left, Expression right)
        {
            Debug.Assert(left.Type == right.Type && left.Type.IsArithmetic());
            Compile(left);
            Compile(right);
            switch (nodeType)
            {
                case ExpressionType.Add: Instructions.EmitAdd(left.Type, @checked: false); break;
                case ExpressionType.AddChecked: Instructions.EmitAdd(left.Type, @checked: true); break;
                case ExpressionType.Subtract: Instructions.EmitSub(left.Type, @checked: false); break;
                case ExpressionType.SubtractChecked: Instructions.EmitSub(left.Type, @checked: true); break;
                case ExpressionType.Multiply: Instructions.EmitMul(left.Type, @checked: false); break;
                case ExpressionType.MultiplyChecked: Instructions.EmitMul(left.Type, @checked: true); break;
                case ExpressionType.Divide: Instructions.EmitDiv(left.Type); break;
                case ExpressionType.Modulo: Instructions.EmitModulo(left.Type); break;
                default: throw ContractUtils.Unreachable;
            }
        }

        private ByRefUpdater CompileArrayIndexAddress(Expression array, Expression index, int argumentIndex)
        {
            var left = _locals.DefineLocal(Expression.Parameter(array.Type, nameof(array)), Instructions.Count);
            var right = _locals.DefineLocal(Expression.Parameter(index.Type, nameof(index)), Instructions.Count);
            Compile(array);
            Instructions.EmitStoreLocal(left.Index);
            Compile(index);
            Instructions.EmitStoreLocal(right.Index);

            Instructions.EmitLoadLocal(left.Index);
            Instructions.EmitLoadLocal(right.Index);
            Instructions.EmitGetArrayItem();

            return new ArrayByRefUpdater(left, right, argumentIndex);
        }

        private void CompileAssignBinaryExpression(Expression expr, bool asVoid)
        {
            var node = (BinaryExpression)expr;

            switch (node.Left.NodeType)
            {
                case ExpressionType.Index:
                    CompileIndexAssignment(node, asVoid);
                    break;

                case ExpressionType.MemberAccess:
                    CompileMemberAssignment(node, asVoid);
                    break;

                case ExpressionType.Parameter:
                case ExpressionType.Extension:
                    CompileVariableAssignment(node, asVoid);
                    break;

                default:
                    throw Error.InvalidLvalue(node.Left.NodeType);
            }
        }

        private void CompileAsVoid(Expression expr)
        {
            var pushLabelBlock = TryPushLabelBlock(expr);
            var startingStackDepth = Instructions.CurrentStackDepth;
            switch (expr.NodeType)
            {
                case ExpressionType.Assign:
                    CompileAssignBinaryExpression(expr, asVoid: true);
                    break;

                case ExpressionType.Block:
                    CompileBlockExpression(expr, asVoid: true);
                    break;

                case ExpressionType.Throw:
                    CompileThrowUnaryExpression(expr, asVoid: true);
                    break;

                case ExpressionType.Constant:
                case ExpressionType.Default:
                case ExpressionType.Parameter:
                    // no-op
                    break;

                default:
                    CompileNoLabelPush(expr);
                    if (expr.Type != typeof(void))
                    {
                        Instructions.EmitPop();
                    }
                    break;
            }
            Debug.Assert(Instructions.CurrentStackDepth == startingStackDepth);
            if (pushLabelBlock)
            {
                PopLabelBlock(_labelBlock.Kind);
            }
        }

        private void CompileBinaryExpression(Expression expr)
        {
            var node = (BinaryExpression)expr;

            if (node.Method != null)
            {
                if (node.IsLifted)
                {
                    // lifting: we need to do the null checks for nullable types and reference types.  If the value
                    // is null we return null, or false for a comparison unless it's not equal, in which case we return
                    // true.

                    // INCOMPAT: The DLR binder short circuits on comparisons other than equal and not equal,
                    // but C# doesn't.
                    var end = Instructions.MakeLabel();

                    var leftTemp = _locals.DefineLocal(Expression.Parameter(node.Left.Type), Instructions.Count);
                    Compile(node.Left);
                    Instructions.EmitStoreLocal(leftTemp.Index);

                    var rightTemp = _locals.DefineLocal(Expression.Parameter(node.Right.Type), Instructions.Count);
                    Compile(node.Right);
                    Instructions.EmitStoreLocal(rightTemp.Index);

                    switch (node.NodeType)
                    {
                        case ExpressionType.Equal:
                        case ExpressionType.NotEqual:
                            /* generating (equal/not equal):
                                * if(left == null) {
                                *      right == null/right != null
                                * }else if(right == null) {
                                *      False/True
                                * }else{
                                *      op_Equality(left, right)/op_Inequality(left, right)
                                * }
                                */
                            if (node.IsLiftedToNull)
                            {
                                goto default;
                            }

                            var testRight = Instructions.MakeLabel();
                            var callMethod = Instructions.MakeLabel();

                            Instructions.EmitLoadLocal(leftTemp.Index);
                            Instructions.EmitLoad(null, typeof(object));
                            Instructions.EmitEqual(typeof(object));
                            Instructions.EmitBranchFalse(testRight);

                            // left is null
                            Instructions.EmitLoadLocal(rightTemp.Index);
                            Instructions.EmitLoad(null, typeof(object));
                            if (node.NodeType == ExpressionType.Equal)
                            {
                                Instructions.EmitEqual(typeof(object));
                            }
                            else
                            {
                                Instructions.EmitNotEqual(typeof(object));
                            }
                            Instructions.EmitBranch(end, hasResult: false, hasValue: true);

                            Instructions.MarkLabel(testRight);

                            // left is not null, check right
                            Instructions.EmitLoadLocal(rightTemp.Index);
                            Instructions.EmitLoad(null, typeof(object));
                            Instructions.EmitEqual(typeof(object));
                            Instructions.EmitBranchFalse(callMethod);

                            // right null, left not, false
                            // right null, left not, true
                            Instructions.EmitLoad(
                                node.NodeType == ExpressionType.Equal ? AstUtils.BoxedFalse : AstUtils.BoxedTrue,
                                typeof(bool));
                            Instructions.EmitBranch(end, hasResult: false, hasValue: true);

                            // both are not null
                            Instructions.MarkLabel(callMethod);
                            Instructions.EmitLoadLocal(leftTemp.Index);
                            Instructions.EmitLoadLocal(rightTemp.Index);
                            Instructions.EmitCall(node.Method);
                            break;

                        default:
                            var loadDefault = Instructions.MakeLabel();

                            if (node.Left.Type.CanBeNull())
                            {
                                Instructions.EmitLoadLocal(leftTemp.Index);
                                Instructions.EmitLoad(null, typeof(object));
                                Instructions.EmitEqual(typeof(object));
                                Instructions.EmitBranchTrue(loadDefault);
                            }

                            if (node.Right.Type.CanBeNull())
                            {
                                Instructions.EmitLoadLocal(rightTemp.Index);
                                Instructions.EmitLoad(null, typeof(object));
                                Instructions.EmitEqual(typeof(object));
                                Instructions.EmitBranchTrue(loadDefault);
                            }

                            Instructions.EmitLoadLocal(leftTemp.Index);
                            Instructions.EmitLoadLocal(rightTemp.Index);
                            Instructions.EmitCall(node.Method);
                            Instructions.EmitBranch(end, hasResult: false, hasValue: true);

                            Instructions.MarkLabel(loadDefault);
                            switch (node.NodeType)
                            {
                                case ExpressionType.LessThan:
                                case ExpressionType.LessThanOrEqual:
                                case ExpressionType.GreaterThan:
                                case ExpressionType.GreaterThanOrEqual:
                                    if (node.IsLiftedToNull)
                                    {
                                        goto default;
                                    }
                                    Instructions.EmitLoad(AstUtils.BoxedFalse, typeof(object));
                                    break;

                                default:
                                    Instructions.EmitLoad(null, typeof(object));
                                    break;
                            }
                            break;
                    }

                    Instructions.MarkLabel(end);

                    _locals.UndefineLocal(leftTemp, Instructions.Count);
                    _locals.UndefineLocal(rightTemp, Instructions.Count);
                }
                else
                {
                    Compile(node.Left);
                    Compile(node.Right);
                    Instructions.EmitCall(node.Method);
                }
            }
            else
            {
                switch (node.NodeType)
                {
                    case ExpressionType.ArrayIndex:
                        Debug.Assert(node.Right.Type == typeof(int));
                        Compile(node.Left);
                        Compile(node.Right);
                        Instructions.EmitGetArrayItem();
                        return;

                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                        CompileArithmetic(node.NodeType, node.Left, node.Right);
                        return;

                    case ExpressionType.ExclusiveOr:
                        Compile(node.Left);
                        Compile(node.Right);
                        Instructions.EmitExclusiveOr(node.Left.Type);
                        break;

                    case ExpressionType.Or:
                        Compile(node.Left);
                        Compile(node.Right);
                        Instructions.EmitOr(node.Left.Type);
                        break;

                    case ExpressionType.And:
                        Compile(node.Left);
                        Compile(node.Right);
                        Instructions.EmitAnd(node.Left.Type);
                        break;

                    case ExpressionType.Equal:
                        CompileEqual(node.Left, node.Right, node.IsLiftedToNull);
                        return;

                    case ExpressionType.NotEqual:
                        CompileNotEqual(node.Left, node.Right, node.IsLiftedToNull);
                        return;

                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                        CompileComparison(node);
                        return;

                    case ExpressionType.LeftShift:
                        Compile(node.Left);
                        Compile(node.Right);
                        Instructions.EmitLeftShift(node.Left.Type);
                        break;

                    case ExpressionType.RightShift:
                        Compile(node.Left);
                        Compile(node.Right);
                        Instructions.EmitRightShift(node.Left.Type);
                        break;

                    default:
                        throw new PlatformNotSupportedException(SR.Format(SR.UnsupportedExpressionType, node.NodeType));
                }
            }
        }

        private void CompileBlockEnd(LocalDefinition[] locals)
        {
            foreach (var local in locals)
            {
                _locals.UndefineLocal(local, Instructions.Count);
            }
        }

        private void CompileBlockExpression(Expression expr, bool asVoid)
        {
            var node = (BlockExpression)expr;

            if (node.ExpressionCount != 0)
            {
                var end = CompileBlockStart(node);

                var lastExpression = node.Expressions[node.Expressions.Count - 1];
                Compile(lastExpression, asVoid);
                CompileBlockEnd(end);
            }
        }

        private LocalDefinition[] CompileBlockStart(BlockExpression node)
        {
            var start = Instructions.Count;

            LocalDefinition[] locals;
            var variables = node.Variables;
            if (variables.Count != 0)
            {
                // TODO: basic flow analysis so we don't have to initialize all
                // variables.
                locals = new LocalDefinition[variables.Count];
                var localCnt = 0;
                foreach (var variable in variables)
                {
                    var local = _locals.DefineLocal(variable, start);
                    locals[localCnt++] = local;

                    Instructions.EmitInitializeLocal(local.Index, variable.Type);
                    Instructions.SetDebugCookie(variable.Name);
                }
            }
            else
            {
                locals = _emptyLocals;
            }

            for (var i = 0; i < node.Expressions.Count - 1; i++)
            {
                CompileAsVoid(node.Expressions[i]);
            }
            return locals;
        }

        private void CompileCoalesceBinaryExpression(Expression expr)
        {
            var node = (BinaryExpression)expr;

            var hasConversion = node.Conversion != null;
            var hasImplicitConversion = false;
            if (!hasConversion && node.Left.Type.IsNullable())
            {
                // reference types don't need additional conversions (the interpreter operates on Object
                // anyway); non-nullable value types can't occur on the left side; all that's left is
                // nullable value types with implicit (numeric) conversions which are allowed by Coalesce
                // factory methods

                var typeToCompare = node.Left.Type;
                if (!node.Type.IsNullable())
                {
                    typeToCompare = typeToCompare.GetNonNullable();
                }

                if (!TypeUtils.AreEquivalent(node.Type, typeToCompare))
                {
                    hasImplicitConversion = true;
                    hasConversion = true;
                }
            }

            var leftNotNull = Instructions.MakeLabel();
            BranchLabel end = null;

            Compile(node.Left);
            Instructions.EmitCoalescingBranch(leftNotNull);
            Instructions.EmitPop();
            Compile(node.Right);

            if (hasConversion)
            {
                // skip over conversion on RHS
                end = Instructions.MakeLabel();
                Instructions.EmitBranch(end);
            }
            else if (node.Right.Type.IsValueType && !TypeUtils.AreEquivalent(node.Type, node.Right.Type))
            {
                // The right hand side may need to be widened to either the left hand side's type
                // if the right hand side is nullable, or the left hand side's underlying type otherwise
                CompileConvertToType(node.Right.Type, node.Type, isChecked: true, isLiftedToNull: node.Type.IsNullable());
            }

            Instructions.MarkLabel(leftNotNull);

            if (node.Conversion != null)
            {
                var temp = Expression.Parameter(node.Left.Type, "temp");
                var local = _locals.DefineLocal(temp, Instructions.Count);
                Instructions.EmitStoreLocal(local.Index);

                CompileMethodCallExpression(
                    Expression.Call(node.Conversion, node.Conversion.Type.GetInvokeMethod(), new Expression[] { temp })
                );

                _locals.UndefineLocal(local, Instructions.Count);
            }
            else if (hasImplicitConversion)
            {
                var nnLeftType = node.Left.Type.GetNonNullable();
                CompileConvertToType(nnLeftType, node.Type, isChecked: true, isLiftedToNull: false);
            }

            if (hasConversion)
            {
                Instructions.MarkLabel(end);
            }
        }

        private void CompileComparison(BinaryExpression node)
        {
            var left = node.Left;
            var right = node.Right;
            Debug.Assert(left.Type == right.Type && left.Type.IsNumeric());

            Compile(left);
            Compile(right);

            switch (node.NodeType)
            {
                case ExpressionType.LessThan: Instructions.EmitLessThan(left.Type, node.IsLiftedToNull); break;
                case ExpressionType.LessThanOrEqual: Instructions.EmitLessThanOrEqual(left.Type, node.IsLiftedToNull); break;
                case ExpressionType.GreaterThan: Instructions.EmitGreaterThan(left.Type, node.IsLiftedToNull); break;
                case ExpressionType.GreaterThanOrEqual: Instructions.EmitGreaterThanOrEqual(left.Type, node.IsLiftedToNull); break;
                default: throw ContractUtils.Unreachable;
            }
        }

        private void CompileConditionalExpression(Expression expr, bool asVoid)
        {
            var node = (ConditionalExpression)expr;
            Compile(node.Test);

            if (node.IfTrue == AstUtils.Empty)
            {
                var endOfFalse = Instructions.MakeLabel();
                Instructions.EmitBranchTrue(endOfFalse);
                Compile(node.IfFalse, asVoid);
                Instructions.MarkLabel(endOfFalse);
            }
            else
            {
                var endOfTrue = Instructions.MakeLabel();
                Instructions.EmitBranchFalse(endOfTrue);
                Compile(node.IfTrue, asVoid);

                if (node.IfFalse != AstUtils.Empty)
                {
                    var endOfFalse = Instructions.MakeLabel();
                    Instructions.EmitBranch(endOfFalse, false, !asVoid);
                    Instructions.MarkLabel(endOfTrue);
                    Compile(node.IfFalse, asVoid);
                    Instructions.MarkLabel(endOfFalse);
                }
                else
                {
                    Instructions.MarkLabel(endOfTrue);
                }
            }
        }

        private void CompileConstantExpression(Expression expr)
        {
            var node = (ConstantExpression)expr;
            Instructions.EmitLoad(node.Value, node.Type);
        }

        private void CompileConvertToType(Type typeFrom, Type typeTo, bool isChecked, bool isLiftedToNull)
        {
            Debug.Assert(typeFrom != typeof(void) && typeTo != typeof(void));

            if (typeTo == typeFrom)
            {
                return;
            }

            if (typeFrom.IsValueType &&
                typeTo.IsNullable() &&
                typeTo.GetNonNullable() == typeFrom)
            {
                // VT -> vt?, no conversion necessary
                return;
            }

            if (typeTo.IsValueType &&
                typeFrom.IsNullable() &&
                typeFrom.GetNonNullable() == typeTo)
            {
                // VT? -> vt, call get_Value
                Instructions.Emit(NullableMethodCallInstruction.CreateGetValue());
                return;
            }

            var nonNullableFrom = typeFrom.GetNonNullable();
            var nonNullableTo = typeTo.GetNonNullable();

            // use numeric conversions for both numeric types and enums
            if ((nonNullableFrom.IsNumericOrBool() || nonNullableFrom.IsEnum)
                 && (nonNullableTo.IsNumericOrBool() || nonNullableTo.IsEnum || nonNullableTo == typeof(decimal)))
            {
                Type enumTypeTo = null;

                if (nonNullableFrom.IsEnum)
                {
                    nonNullableFrom = Enum.GetUnderlyingType(nonNullableFrom);
                }
                if (nonNullableTo.IsEnum)
                {
                    enumTypeTo = nonNullableTo;
                    nonNullableTo = Enum.GetUnderlyingType(nonNullableTo);
                }

                var from = nonNullableFrom.GetTypeCode();
                var to = nonNullableTo.GetTypeCode();

                if (from == to)
                {
                    if (enumTypeTo != null)
                    {
                        // If casting between enums of the same underlying type or to enum from the underlying
                        // type, there's no need for the numeric conversion, so just include a null-check if
                        // appropriate.
                        if (typeFrom.IsNullable() && !typeTo.IsNullable())
                        {
                            Instructions.Emit(NullableMethodCallInstruction.CreateGetValue());
                        }
                    }
                    else
                    {
                        // Casting to the underlying check still needs a numeric conversion to force the type
                        // change that EmitCastToEnum provides for enums, but needs only one cast. Checked can
                        // also never throw, so always be unchecked.
                        Instructions.EmitConvertToUnderlying(to, isLiftedToNull);
                    }
                }
                else
                {
                    if (isChecked)
                    {
                        Instructions.EmitNumericConvertChecked(from, to, isLiftedToNull);
                    }
                    else
                    {
                        Instructions.EmitNumericConvertUnchecked(from, to, isLiftedToNull);
                    }
                }

                if (enumTypeTo != null)
                {
                    // Convert from underlying to the enum
                    Instructions.EmitCastToEnum(enumTypeTo);
                }

                return;
            }

            if (typeTo.IsEnum)
            {
                Instructions.Emit(NullCheckInstruction.Instance);
                Instructions.EmitCastReferenceToEnum(typeTo);
                return;
            }

            if (typeTo == typeof(object) || typeTo.IsAssignableFrom(typeFrom))
            {
                // Conversions to a super-class or implemented interfaces are no-op.
                return;
            }

            // A conversion to a non-implemented interface or an unrelated class, etc. should fail.
            Instructions.EmitCast(typeTo);
        }

        private void CompileConvertUnaryExpression(Expression expr)
        {
            var node = (UnaryExpression)expr;
            if (node.Method != null)
            {
                var end = Instructions.MakeLabel();
                var loadDefault = Instructions.MakeLabel();
                var method = node.Method;
                var parameters = method.GetParameters();
                Debug.Assert(parameters.Length == 1);
                var parameter = parameters[0];
                var operand = node.Operand;
                var operandType = operand.Type;
                var opTemp = _locals.DefineLocal(Expression.Parameter(operandType), Instructions.Count);
                ByRefUpdater updater = null;
                var parameterType = parameter.ParameterType;
                if (parameterType.IsByRef)
                {
                    if (node.IsLifted)
                    {
                        Compile(node.Operand);
                    }
                    else
                    {
                        updater = CompileAddress(node.Operand, 0);
                        parameterType = parameterType.GetElementType();
                    }
                }
                else
                {
                    Compile(node.Operand);
                }

                Instructions.EmitStoreLocal(opTemp.Index);

                if (!operandType.IsValueType || operandType.IsNullable() && node.IsLiftedToNull)
                {
                    Instructions.EmitLoadLocal(opTemp.Index);
                    Instructions.EmitLoad(null, typeof(object));
                    Instructions.EmitEqual(typeof(object));
                    Instructions.EmitBranchTrue(loadDefault);
                }

                Instructions.EmitLoadLocal(opTemp.Index);
                if (operandType.IsNullable() && parameterType == operandType.GetNonNullable())
                {
                    Instructions.Emit(NullableMethodCallInstruction.CreateGetValue());
                }

                if (updater == null)
                {
                    Instructions.EmitCall(method);
                }
                else
                {
                    Instructions.EmitByRefCall(method, parameters, new[] { updater });
                    updater.UndefineTemps(Instructions, _locals);
                }

                Instructions.EmitBranch(end, hasResult: false, hasValue: true);

                Instructions.MarkLabel(loadDefault);
                Instructions.EmitLoad(null, typeof(object));

                Instructions.MarkLabel(end);

                _locals.UndefineLocal(opTemp, Instructions.Count);
            }
            else if (node.Type == typeof(void))
            {
                CompileAsVoid(node.Operand);
            }
            else
            {
                Compile(node.Operand);
                CompileConvertToType(node.Operand.Type, node.Type, node.NodeType == ExpressionType.ConvertChecked, node.IsLiftedToNull);
            }
        }

        private void CompileDebugInfoExpression(Expression expr)
        {
            var node = (DebugInfoExpression)expr;
            var start = Instructions.Count;
            var info = new DebugInfo
            {
                Index = start,
                FileName = node.Document.FileName,
                StartLine = node.StartLine,
                EndLine = node.EndLine,
                IsClear = node.IsClear
            };
            _debugInfos.Add(info);
        }

        private void CompileDefaultExpression(Expression expr)
        {
            CompileDefaultExpression(expr.Type);
        }

        private void CompileDefaultExpression(Type type)
        {
            if (type != typeof(void))
            {
                if (type.CanBeNull())
                {
                    Instructions.EmitLoad(value: null);
                }
                else
                {
                    var value = ScriptingRuntimeHelpers.GetPrimitiveDefaultValue(type);
                    if (value != null)
                    {
                        Instructions.EmitLoad(value);
                    }
                    else
                    {
                        Instructions.EmitDefaultValue(type);
                    }
                }
            }
        }

        private void CompileEqual(Expression left, Expression right, bool liftedToNull)
        {
#if DEBUG
            Debug.Assert(IsNullComparison(left, right) || left.Type == right.Type || !left.Type.IsValueType && !right.Type.IsValueType);
#endif
            Compile(left);
            Compile(right);
            Instructions.EmitEqual(left.Type, liftedToNull);
        }

        private void CompileGetBoxedVariable(ParameterExpression variable)
        {
            var local = ResolveLocal(variable);

            if (local.InClosure)
            {
                Instructions.EmitLoadLocalFromClosureBoxed(local.Index);
            }
            else
            {
                Debug.Assert(local.IsBoxed);
                Instructions.EmitLoadLocal(local.Index);
            }

            Instructions.SetDebugCookie(variable.Name);
        }

        private void CompileGetVariable(ParameterExpression variable)
        {
            LoadLocalNoValueTypeCopy(variable);

            Instructions.SetDebugCookie(variable.Name);

            EmitCopyValueType(variable.Type);
        }

        private void CompileGotoExpression(Expression expr)
        {
            var node = (GotoExpression)expr;
            var labelInfo = ReferenceLabel(node.Target);

            if (node.Value != null)
            {
                Compile(node.Value);
            }

            Instructions.EmitGoto(labelInfo.GetLabel(this),
                node.Type != typeof(void),
                node.Value != null && node.Value.Type != typeof(void),
                node.Target.Type != typeof(void));
        }

        private void CompileIndexAssignment(BinaryExpression node, bool asVoid)
        {
            var index = (IndexExpression)node.Left;

            // instance:
            if (index.Object != null)
            {
                EmitThisForMethodCall(index.Object);
            }

            // indexes, byref args not allowed.
            for (int i = 0, n = index.ArgumentCount; i < n; i++)
            {
                Compile(index.GetArgument(i));
            }

            // value:
            Compile(node.Right);
            LocalDefinition local = default;
            if (!asVoid)
            {
                local = _locals.DefineLocal(Expression.Parameter(node.Right.Type), Instructions.Count);
                Instructions.EmitAssignLocal(local.Index);
            }

            if (index.Indexer != null)
            {
                Instructions.EmitCall(index.Indexer.GetSetMethod(nonPublic: true));
            }
            else if (index.ArgumentCount != 1)
            {
                Instructions.EmitCall(index.Object.Type.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                Instructions.EmitSetArrayItem();
            }

            if (!asVoid)
            {
                Instructions.EmitLoadLocal(local.Index);
                _locals.UndefineLocal(local, Instructions.Count);
            }
        }

        private void CompileIndexExpression(Expression expr)
        {
            var index = (IndexExpression)expr;

            // instance:
            if (index.Object != null)
            {
                EmitThisForMethodCall(index.Object);
            }

            // indexes, byref args not allowed.
            for (int i = 0, n = index.ArgumentCount; i < n; i++)
            {
                Compile(index.GetArgument(i));
            }

            EmitIndexGet(index);
        }

        private void CompileIntSwitchExpression<T>(SwitchExpression node)
        {
            var end = DefineLabel(node: null);
            var hasValue = node.Type != typeof(void);

            Compile(node.SwitchValue);
            var caseDict = new Dictionary<T, int>();
            var switchIndex = Instructions.Count;
            Instructions.EmitIntSwitch(caseDict);

            if (node.DefaultBody != null)
            {
                Compile(node.DefaultBody, !hasValue);
            }
            else
            {
                Debug.Assert(!hasValue);
            }
            Instructions.EmitBranch(end.GetLabel(this), false, hasValue);

            for (var i = 0; i < node.Cases.Count; i++)
            {
                var switchCase = node.Cases[i];

                var caseOffset = Instructions.Count - switchIndex;
                foreach (ConstantExpression testValue in switchCase.TestValues)
                {
                    var key = (T)testValue.Value;
                    caseDict.TryAdd(key, caseOffset);
                }

                Compile(switchCase.Body, !hasValue);

                if (i < node.Cases.Count - 1)
                {
                    Instructions.EmitBranch(end.GetLabel(this), false, hasValue);
                }
            }

            Instructions.MarkLabel(end.GetLabel(this));
        }

        private void CompileInvocationExpression(Expression expr)
        {
            var node = (InvocationExpression)expr;

            if (typeof(LambdaExpression).IsAssignableFrom(node.Expression.Type))
            {
                var compMethod = node.Expression.Type.GetMethod("Compile", ArrayReservoir<Type>.EmptyArray);
                CompileMethodCallExpression(
                    Expression.Call(
                        node.Expression,
                        compMethod
                    ),
                    compMethod.ReturnType.GetInvokeMethod(),
                    node
                );
            }
            else
            {
                CompileMethodCallExpression(
                    node.Expression, node.Expression.Type.GetInvokeMethod(), node
                );
            }
        }

        private void CompileLabelExpression(Expression expr)
        {
            var node = (LabelExpression)expr;

            // If we're an immediate child of a block, our label will already
            // be defined. If not, we need to define our own block so this
            // label isn't exposed except to its own child expression.
            LabelInfo label = null;

            if (_labelBlock.Kind == LabelScopeKind.Block)
            {
                _labelBlock.TryGetLabelInfo(node.Target, out label);

                // We're in a block but didn't find our label, try switch
                if (label == null && _labelBlock.Parent.Kind == LabelScopeKind.Switch)
                {
                    _labelBlock.Parent.TryGetLabelInfo(node.Target, out label);
                }

                // if we're in a switch or block, we should've found the label
                Debug.Assert(label != null);
            }

            if (label == null)
            {
                label = DefineLabel(node.Target);
            }

            if (node.DefaultValue != null)
            {
                if (node.Target.Type == typeof(void))
                {
                    CompileAsVoid(node.DefaultValue);
                }
                else
                {
                    Compile(node.DefaultValue);
                }
            }

            Instructions.MarkLabel(label.GetLabel(this));
        }

        private void CompileLambdaExpression(Expression expr)
        {
            var node = (LambdaExpression)expr;
            var compiler = new LightCompiler(this);
            var creator = compiler.CompileTop(node);

            if (compiler._locals.ClosureVariables != null)
            {
                foreach (var variable in compiler._locals.ClosureVariables.Keys)
                {
                    EnsureAvailableForClosure(variable);
                    CompileGetBoxedVariable(variable);
                }
            }
            Instructions.EmitCreateDelegate(creator);
        }

        private void CompileLiftedLogicalBinaryExpression(BinaryExpression node, bool andAlso)
        {
            var computeRight = Instructions.MakeLabel();
            var returnFalse = Instructions.MakeLabel();
            var returnNull = Instructions.MakeLabel();
            var returnValue = Instructions.MakeLabel();
            var result = _locals.DefineLocal(Expression.Parameter(node.Left.Type), Instructions.Count);
            var leftTemp = _locals.DefineLocal(Expression.Parameter(node.Left.Type), Instructions.Count);

            Compile(node.Left);
            Instructions.EmitStoreLocal(leftTemp.Index);

            Instructions.EmitLoadLocal(leftTemp.Index);
            Instructions.EmitLoad(null, typeof(object));
            Instructions.EmitEqual(typeof(object));

            Instructions.EmitBranchTrue(computeRight);

            Instructions.EmitLoadLocal(leftTemp.Index);

            if (andAlso)
            {
                Instructions.EmitBranchFalse(returnFalse);
            }
            else
            {
                Instructions.EmitBranchTrue(returnFalse);
            }

            // compute right
            Instructions.MarkLabel(computeRight);
            var rightTemp = _locals.DefineLocal(Expression.Parameter(node.Right.Type), Instructions.Count);
            Compile(node.Right);
            Instructions.EmitStoreLocal(rightTemp.Index);

            Instructions.EmitLoadLocal(rightTemp.Index);
            Instructions.EmitLoad(null, typeof(object));
            Instructions.EmitEqual(typeof(object));
            Instructions.EmitBranchTrue(returnNull);

            Instructions.EmitLoadLocal(rightTemp.Index);
            if (andAlso)
            {
                Instructions.EmitBranchFalse(returnFalse);
            }
            else
            {
                Instructions.EmitBranchTrue(returnFalse);
            }

            // check left for null again
            Instructions.EmitLoadLocal(leftTemp.Index);
            Instructions.EmitLoad(null, typeof(object));
            Instructions.EmitEqual(typeof(object));
            Instructions.EmitBranchTrue(returnNull);

            // return true
            Instructions.EmitLoad(andAlso ? AstUtils.BoxedTrue : AstUtils.BoxedFalse, typeof(object));
            Instructions.EmitStoreLocal(result.Index);
            Instructions.EmitBranch(returnValue);

            // return false
            Instructions.MarkLabel(returnFalse);
            Instructions.EmitLoad(andAlso ? AstUtils.BoxedFalse : AstUtils.BoxedTrue, typeof(object));
            Instructions.EmitStoreLocal(result.Index);
            Instructions.EmitBranch(returnValue);

            // return null
            Instructions.MarkLabel(returnNull);
            Instructions.EmitLoad(null, typeof(object));
            Instructions.EmitStoreLocal(result.Index);

            Instructions.MarkLabel(returnValue);
            Instructions.EmitLoadLocal(result.Index);

            _locals.UndefineLocal(leftTemp, Instructions.Count);
            _locals.UndefineLocal(rightTemp, Instructions.Count);
            _locals.UndefineLocal(result, Instructions.Count);
        }

        private void CompileListInit(ReadOnlyCollection<ElementInit> initializers)
        {
            foreach (var initializer in initializers)
            {
                Instructions.EmitDup();
                foreach (var arg in initializer.Arguments)
                {
                    Compile(arg);
                }
                var add = initializer.AddMethod;
                Instructions.EmitCall(add);
                if (add.ReturnType != typeof(void))
                {
                    Instructions.EmitPop();
                }
            }
        }

        private void CompileListInitExpression(Expression expr)
        {
            var node = (ListInitExpression)expr;
            EmitThisForMethodCall(node.NewExpression);
            var initializers = node.Initializers;
            CompileListInit(initializers);
        }

        private void CompileLogicalBinaryExpression(BinaryExpression b, bool andAlso)
        {
            if (b.Method != null && !b.IsLiftedLogical)
            {
                CompileMethodLogicalBinaryExpression(b, andAlso);
            }
            else if (b.Left.Type == typeof(bool?))
            {
                CompileLiftedLogicalBinaryExpression(b, andAlso);
            }
            else if (b.IsLiftedLogical)
            {
                Compile(b.ReduceUserDefinedLifted());
            }
            else
            {
                CompileUnliftedLogicalBinaryExpression(b, andAlso);
            }
        }

        private void CompileLoopExpression(Expression expr)
        {
            var node = (LoopExpression)expr;

            PushLabelBlock(LabelScopeKind.Statement);
            var breakLabel = DefineLabel(node.BreakLabel);
            var continueLabel = DefineLabel(node.ContinueLabel);

            Instructions.MarkLabel(continueLabel.GetLabel(this));

            // emit loop body:
            CompileAsVoid(node.Body);

            // emit loop branch:
            Instructions.EmitBranch(continueLabel.GetLabel(this), node.Type != typeof(void), hasValue: false);

            Instructions.MarkLabel(breakLabel.GetLabel(this));

            PopLabelBlock(LabelScopeKind.Statement);
        }

        private void CompileMember(Expression from, MemberInfo member, bool forBinding)
        {
            if (member is FieldInfo fi)
            {
                if (fi.IsLiteral)
                {
                    Debug.Assert(!forBinding);
                    Instructions.EmitLoad(fi.GetValue(obj: null), fi.FieldType);
                }
                else if (fi.IsStatic)
                {
                    if (forBinding)
                    {
                        throw Error.InvalidProgram();
                    }

                    if (fi.IsInitOnly)
                    {
                        Instructions.EmitLoad(fi.GetValue(obj: null), fi.FieldType);
                    }
                    else
                    {
                        Instructions.EmitLoadField(fi);
                    }
                }
                else
                {
                    if (from != null)
                    {
                        EmitThisForMethodCall(from);
                    }

                    Instructions.EmitLoadField(fi);
                }
            }
            else
            {
                // MemberExpression can use either FieldInfo or PropertyInfo - other types derived from MemberInfo are not permitted
                var pi = (PropertyInfo)member;
                if (pi != null)
                {
                    var method = pi.GetGetMethod(nonPublic: true);
                    if (forBinding && method.IsStatic)
                    {
                        throw Error.InvalidProgram();
                    }

                    if (from != null)
                    {
                        EmitThisForMethodCall(from);
                    }

                    if (!method.IsStatic && from != null && from.Type.IsNullable())
                    {
                        // reflection doesn't let us call methods on Nullable<T> when the value
                        // is null...  so we get to special case those methods!
                        Instructions.EmitNullableCall(method, ArrayReservoir<ParameterInfo>.EmptyArray);
                    }
                    else
                    {
                        Instructions.EmitCall(method);
                    }
                }
            }
        }

        private void CompileMemberAssignment(BinaryExpression node, bool asVoid)
        {
            var member = (MemberExpression)node.Left;
            var expr = member.Expression;
            if (expr != null)
            {
                EmitThisForMethodCall(expr);
            }

            CompileMemberAssignment(asVoid, member.Member, node.Right, forBinding: false);
        }

        private void CompileMemberAssignment(bool asVoid, MemberInfo refMember, Expression value, bool forBinding)
        {
            if (refMember is PropertyInfo pi)
            {
                var method = pi.GetSetMethod(nonPublic: true);
                if (forBinding && method.IsStatic)
                {
                    throw Error.InvalidProgram();
                }

                EmitThisForMethodCall(value);

                var start = Instructions.Count;
                if (!asVoid)
                {
                    var local = _locals.DefineLocal(Expression.Parameter(value.Type), start);
                    Instructions.EmitAssignLocal(local.Index);
                    Instructions.EmitCall(method);
                    Instructions.EmitLoadLocal(local.Index);
                    _locals.UndefineLocal(local, Instructions.Count);
                }
                else
                {
                    Instructions.EmitCall(method);
                }
            }
            else
            {
                // other types inherited from MemberInfo (EventInfo\MethodBase\Type) cannot be used in MemberAssignment
                var fi = (FieldInfo)refMember;
                Debug.Assert(fi != null);
                if (fi.IsLiteral)
                {
                    throw Error.NotSupported();
                }

                if (forBinding && fi.IsStatic)
                {
                    Instructions.UnEmit(); // Undo having pushed the instance to the stack.
                }

                EmitThisForMethodCall(value);

                var start = Instructions.Count;
                if (!asVoid)
                {
                    var local = _locals.DefineLocal(Expression.Parameter(value.Type), start);
                    Instructions.EmitAssignLocal(local.Index);
                    Instructions.EmitStoreField(fi);
                    Instructions.EmitLoadLocal(local.Index);
                    _locals.UndefineLocal(local, Instructions.Count);
                }
                else
                {
                    Instructions.EmitStoreField(fi);
                }
            }
        }

        private void CompileMemberExpression(Expression expr)
        {
            var node = (MemberExpression)expr;

            CompileMember(node.Expression, node.Member, forBinding: false);
        }

        private void CompileMemberInit(ReadOnlyCollection<MemberBinding> bindings)
        {
            foreach (var binding in bindings)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        Instructions.EmitDup();
                        CompileMemberAssignment(
                            true,
                            ((MemberAssignment)binding).Member,
                            ((MemberAssignment)binding).Expression,
                            forBinding: true
                        );
                        break;

                    case MemberBindingType.ListBinding:
                        var memberList = (MemberListBinding)binding;
                        Instructions.EmitDup();
                        CompileMember(null, memberList.Member, forBinding: true);
                        CompileListInit(memberList.Initializers);
                        Instructions.EmitPop();
                        break;

                    case MemberBindingType.MemberBinding:
                        var memberMember = (MemberMemberBinding)binding;
                        Instructions.EmitDup();
                        var type = GetMemberType(memberMember.Member);
                        if (memberMember.Member is PropertyInfo && type.IsValueType)
                        {
                            throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(memberMember.Bindings);
                        }

                        CompileMember(null, memberMember.Member, forBinding: true);
                        CompileMemberInit(memberMember.Bindings);
                        Instructions.EmitPop();
                        break;
                }
            }
        }

        private void CompileMemberInitExpression(Expression expr)
        {
            var node = (MemberInitExpression)expr;
            EmitThisForMethodCall(node.NewExpression);
            CompileMemberInit(node.Bindings);
        }

        private void CompileMethodCallExpression(Expression expr)
        {
            var node = (MethodCallExpression)expr;
            CompileMethodCallExpression(node.Object, node.Method, node);
        }

        private void CompileMethodCallExpression(Expression @object, MethodInfo method, IArgumentProvider arguments)
        {
            var parameters = method.GetParameters();

            // TODO: Support pass by reference.
            List<ByRefUpdater> updaters = null;
            if (!method.IsStatic)
            {
                var updater = CompileAddress(@object, -1);
                if (updater != null)
                {
                    updaters = new List<ByRefUpdater> { updater };
                }
            }

            Debug.Assert(parameters.Length == arguments.ArgumentCount);

            for (int i = 0, n = arguments.ArgumentCount; i < n; i++)
            {
                var arg = arguments.GetArgument(i);

                // byref calls leave out values on the stack, we use a callback
                // to emit the code which processes each value left on the stack.
                if (parameters[i].ParameterType.IsByRef)
                {
                    var updater = CompileAddress(arg, i);
                    if (updater != null)
                    {
                        if (updaters == null)
                        {
                            updaters = new List<ByRefUpdater>();
                        }

                        updaters.Add(updater);
                    }
                }
                else
                {
                    Compile(arg);
                }
            }

            if (!method.IsStatic &&
                @object.Type.IsNullable())
            {
                // reflection doesn't let us call methods on Nullable<T> when the value
                // is null...  so we get to special case those methods!
                Instructions.EmitNullableCall(method, parameters);
            }
            else
            {
                if (updaters == null)
                {
                    Instructions.EmitCall(method, parameters);
                }
                else
                {
                    Instructions.EmitByRefCall(method, parameters, updaters.ToArray());

                    foreach (var updater in updaters)
                    {
                        updater.UndefineTemps(Instructions, _locals);
                    }
                }
            }
        }

        private void CompileMethodLogicalBinaryExpression(BinaryExpression expr, bool andAlso)
        {
            var labEnd = Instructions.MakeLabel();
            Compile(expr.Left);
            Instructions.EmitDup();

            var opTrue = TypeUtils.GetBooleanOperator(expr.Method.DeclaringType, andAlso ? "op_False" : "op_True");
            Debug.Assert(opTrue != null, "factory should check that the method exists");
            Instructions.EmitCall(opTrue);
            Instructions.EmitBranchTrue(labEnd);

            Compile(expr.Right);

            Debug.Assert(expr.Method.IsStatic);
            Instructions.EmitCall(expr.Method);

            Instructions.MarkLabel(labEnd);
        }

        private ByRefUpdater CompileMultiDimArrayAccess(Expression array, IArgumentProvider arguments, int index)
        {
            Compile(array);
            var objTmp = _locals.DefineLocal(Expression.Parameter(array.Type), Instructions.Count);
            Instructions.EmitDup();
            Instructions.EmitStoreLocal(objTmp.Index);

            var count = arguments.ArgumentCount;
            var indexLocals = new LocalDefinition[count];
            for (var i = 0; i < count; i++)
            {
                var arg = arguments.GetArgument(i);
                Compile(arg);

                var argTmp = _locals.DefineLocal(Expression.Parameter(arg.Type), Instructions.Count);
                Instructions.EmitDup();
                Instructions.EmitStoreLocal(argTmp.Index);

                indexLocals[i] = argTmp;
            }

            Instructions.EmitCall(array.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance));

            return new IndexMethodByRefUpdater(objTmp, indexLocals, array.Type.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance), index);
        }

        private void CompileNewArrayExpression(Expression expr)
        {
            var node = (NewArrayExpression)expr;

            foreach (var arg in node.Expressions)
            {
                Compile(arg);
            }

            var elementType = node.Type.GetElementType();
            var rank = node.Expressions.Count;

            if (node.NodeType == ExpressionType.NewArrayInit)
            {
                Instructions.EmitNewArrayInit(elementType, rank);
            }
            else
            {
                Debug.Assert(node.NodeType == ExpressionType.NewArrayBounds);
                if (rank == 1)
                {
                    Instructions.EmitNewArray(elementType);
                }
                else
                {
                    Instructions.EmitNewArrayBounds(elementType, rank);
                }
            }
        }

        private void CompileNewExpression(Expression expr)
        {
            var node = (NewExpression)expr;

            if (node.Constructor != null)
            {
                if (node.Constructor.DeclaringType.IsAbstract)
                {
                    throw Error.NonAbstractConstructorRequired();
                }

                var parameters = node.Constructor.GetParameters();
                List<ByRefUpdater> updaters = null;

                for (var i = 0; i < parameters.Length; i++)
                {
                    var arg = node.GetArgument(i);

                    if (parameters[i].ParameterType.IsByRef)
                    {
                        var updater = CompileAddress(arg, i);
                        if (updater != null)
                        {
                            if (updaters == null)
                            {
                                updaters = new List<ByRefUpdater>();
                            }
                            updaters.Add(updater);
                        }
                    }
                    else
                    {
                        Compile(arg);
                    }
                }

                if (updaters != null)
                {
                    Instructions.EmitByRefNew(node.Constructor, parameters, updaters.ToArray());
                }
                else
                {
                    Instructions.EmitNew(node.Constructor, parameters);
                }
            }
            else
            {
                var type = node.Type;
                Debug.Assert(type.IsValueType);
                if (type.IsNullable())
                {
                    Instructions.EmitLoad(value: null);
                }
                else
                {
                    Instructions.EmitDefaultValue(type);
                }
            }
        }

        private void CompileNoLabelPush(Expression expr)
        {
            // When compiling deep trees, we run the risk of triggering a terminating StackOverflowException,
            // so we use the StackGuard utility here to probe for sufficient stack and continue the work on
            // another thread when we run out of stack space.
            if (!_guard.TryEnterOnCurrentStack())
            {
                _guard.RunOnEmptyStack((@this, e) => @this.CompileNoLabelPush(e), this, expr);
                return;
            }

            var startingStackDepth = Instructions.CurrentStackDepth;
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked: CompileBinaryExpression(expr); break;
                case ExpressionType.AndAlso: CompileAndAlsoBinaryExpression(expr); break;
                case ExpressionType.OrElse: CompileOrElseBinaryExpression(expr); break;
                case ExpressionType.Coalesce: CompileCoalesceBinaryExpression(expr); break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Decrement:
                case ExpressionType.Increment:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.OnesComplement:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus: CompileUnaryExpression(expr); break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked: CompileConvertUnaryExpression(expr); break;
                case ExpressionType.Quote: CompileQuoteUnaryExpression(expr); break;
                case ExpressionType.Throw: CompileThrowUnaryExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.Unbox: CompileUnboxUnaryExpression(expr); break;
                case ExpressionType.Call: CompileMethodCallExpression(expr); break;
                case ExpressionType.Conditional: CompileConditionalExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.Constant: CompileConstantExpression(expr); break;
                case ExpressionType.Invoke: CompileInvocationExpression(expr); break;
                case ExpressionType.Lambda: CompileLambdaExpression(expr); break;
                case ExpressionType.ListInit: CompileListInitExpression(expr); break;
                case ExpressionType.MemberAccess: CompileMemberExpression(expr); break;
                case ExpressionType.MemberInit: CompileMemberInitExpression(expr); break;
                case ExpressionType.New: CompileNewExpression(expr); break;
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds: CompileNewArrayExpression(expr); break;
                case ExpressionType.Parameter: CompileParameterExpression(expr); break;
                case ExpressionType.TypeIs: CompileTypeIsExpression(expr); break;
                case ExpressionType.TypeEqual: CompileTypeEqualExpression(expr); break;
                case ExpressionType.Assign: CompileAssignBinaryExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.Block: CompileBlockExpression(expr, expr.Type == typeof(void)); break;
                case ExpressionType.DebugInfo: CompileDebugInfoExpression(expr); break;
                case ExpressionType.Default: CompileDefaultExpression(expr); break;
                case ExpressionType.Goto: CompileGotoExpression(expr); break;
                case ExpressionType.Index: CompileIndexExpression(expr); break;
                case ExpressionType.Label: CompileLabelExpression(expr); break;
                case ExpressionType.RuntimeVariables: CompileRuntimeVariablesExpression(expr); break;
                case ExpressionType.Loop: CompileLoopExpression(expr); break;
                case ExpressionType.Switch: CompileSwitchExpression(expr); break;
                case ExpressionType.Try: CompileTryExpression(expr); break;
                default:
                    Compile(expr.ReduceAndCheck());
                    break;
            }
            Debug.Assert(Instructions.CurrentStackDepth == startingStackDepth + (expr.Type == typeof(void) ? 0 : 1),
                $"{Instructions.CurrentStackDepth} vs {startingStackDepth + (expr.Type == typeof(void) ? 0 : 1)} for {expr.NodeType}");
        }

        private void CompileNotEqual(Expression left, Expression right, bool liftedToNull)
        {
#if DEBUG
            Debug.Assert(IsNullComparison(left, right) || left.Type == right.Type || !left.Type.IsValueType && !right.Type.IsValueType);
#endif
            Compile(left);
            Compile(right);
            Instructions.EmitNotEqual(left.Type, liftedToNull);
        }

        private void CompileNotExpression(UnaryExpression node)
        {
            Compile(node.Operand);
            Instructions.EmitNot(node.Operand.Type);
        }

        private void CompileOrElseBinaryExpression(Expression expr)
        {
            CompileLogicalBinaryExpression((BinaryExpression)expr, andAlso: false);
        }

        private void CompileParameterExpression(Expression expr)
        {
            var node = (ParameterExpression)expr;
            CompileGetVariable(node);
        }

        private void CompileQuoteUnaryExpression(Expression expr)
        {
            var unary = (UnaryExpression)expr;

            var visitor = new QuoteVisitor();
            visitor.Visit(unary.Operand);

            var mapping = new Dictionary<ParameterExpression, LocalVariable>();

            foreach (var local in visitor.HoistedParameters)
            {
                EnsureAvailableForClosure(local);
                mapping[local] = ResolveLocal(local);
            }

            Instructions.Emit(new QuoteInstruction(unary.Operand, mapping.Count > 0 ? mapping : null));
        }

        private void CompileRuntimeVariablesExpression(Expression expr)
        {
            // Generates IRuntimeVariables for all requested variables
            var node = (RuntimeVariablesExpression)expr;
            foreach (var variable in node.Variables)
            {
                EnsureAvailableForClosure(variable);
                CompileGetBoxedVariable(variable);
            }

            Instructions.EmitNewRuntimeVariables(node.Variables.Count);
        }

        private void CompileSetVariable(ParameterExpression variable, bool isVoid)
        {
            var local = ResolveLocal(variable);

            if (local.InClosure)
            {
                if (isVoid)
                {
                    Instructions.EmitStoreLocalToClosure(local.Index);
                }
                else
                {
                    Instructions.EmitAssignLocalToClosure(local.Index);
                }
            }
            else if (local.IsBoxed)
            {
                if (isVoid)
                {
                    Instructions.EmitStoreLocalBoxed(local.Index);
                }
                else
                {
                    Instructions.EmitAssignLocalBoxed(local.Index);
                }
            }
            else
            {
                if (isVoid)
                {
                    Instructions.EmitStoreLocal(local.Index);
                }
                else
                {
                    Instructions.EmitAssignLocal(local.Index);
                }
            }

            Instructions.SetDebugCookie(variable.Name);
        }

        private void CompileStringSwitchExpression(SwitchExpression node)
        {
            var end = DefineLabel(node: null);
            var hasValue = node.Type != typeof(void);

            Compile(node.SwitchValue);
            var caseDict = new Dictionary<string, int>();
            var switchIndex = Instructions.Count;
            // by default same as default
            var nullCase = new StrongBox<int>(1);
            Instructions.EmitStringSwitch(caseDict, nullCase);

            if (node.DefaultBody != null)
            {
                Compile(node.DefaultBody, !hasValue);
            }
            else
            {
                Debug.Assert(!hasValue);
            }
            Instructions.EmitBranch(end.GetLabel(this), false, hasValue);

            for (var i = 0; i < node.Cases.Count; i++)
            {
                var switchCase = node.Cases[i];

                var caseOffset = Instructions.Count - switchIndex;
                foreach (ConstantExpression testValue in switchCase.TestValues)
                {
                    var key = (string)testValue.Value;
                    if (key == null)
                    {
                        if (nullCase.Value == 1)
                        {
                            nullCase.Value = caseOffset;
                        }
                    }
                    else
                    {
                        caseDict.TryAdd(key, caseOffset);
                    }
                }

                Compile(switchCase.Body, !hasValue);

                if (i < node.Cases.Count - 1)
                {
                    Instructions.EmitBranch(end.GetLabel(this), false, hasValue);
                }
            }

            Instructions.MarkLabel(end.GetLabel(this));
        }

        private void CompileSwitchExpression(Expression expr)
        {
            var node = (SwitchExpression)expr;

            if (node.Cases.All(c => c.TestValues.All(t => t is ConstantExpression)))
            {
                if (node.Cases.Count == 0)
                {
                    // Emit the switch value in case it has side-effects, but as void
                    // since the value is ignored.
                    CompileAsVoid(node.SwitchValue);

                    // Now if there is a default body, it happens unconditionally.
                    if (node.DefaultBody != null)
                    {
                        Compile(node.DefaultBody);
                    }
                    else
                    {
                        // If there are no cases and no default then the type must be void.
                        // Assert that earlier validation caught any exceptions to that.
                        Debug.Assert(node.Type == typeof(void));
                    }
                    return;
                }

                var switchType = node.SwitchValue.Type.GetTypeCode();

                if (node.Comparison == null)
                {
                    switch (switchType)
                    {
                        case TypeCode.Int32:
                            CompileIntSwitchExpression<int>(node);
                            return;

                        // the following cases are uncommon,
                        // so to avoid numerous unnecessary generic
                        // instantiations of Dictionary<K, V> and related types
                        // in AOT scenarios, we will just use "object" as the key
                        // NOTE: this does not actually result in any
                        //       extra boxing since both keys and values
                        //       are already boxed when we get them
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                        case TypeCode.UInt16:
                        case TypeCode.Int16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Int64:
                            CompileIntSwitchExpression<object>(node);
                            return;
                    }
                }

                if (switchType == TypeCode.String)
                {
                    // If we have a comparison other than string equality, bail
                    var equality = StringOpEqualityStringString;
                    if (equality != null && !equality.IsStatic)
                    {
                        equality = null;
                    }

                    if (Equals(node.Comparison, equality))
                    {
                        CompileStringSwitchExpression(node);
                        return;
                    }
                }
            }

            var temp = _locals.DefineLocal(Expression.Parameter(node.SwitchValue.Type), Instructions.Count);
            Compile(node.SwitchValue);
            Instructions.EmitStoreLocal(temp.Index);

            var doneLabel = Expression.Label(node.Type, "done");

            foreach (var @case in node.Cases)
            {
                foreach (var val in @case.TestValues)
                {
                    //  temp == val ?
                    //          goto(Body) doneLabel:
                    //          {};
                    CompileConditionalExpression(
                        Expression.Condition(
                            Expression.Equal(temp.Parameter, val, false, node.Comparison),
                            Expression.Goto(doneLabel, @case.Body),
                            AstUtils.Empty
                        ),
                        asVoid: true);
                }
            }

            // doneLabel(DefaultBody):
            CompileLabelExpression(Expression.Label(doneLabel, node.DefaultBody));

            _locals.UndefineLocal(temp, Instructions.Count);
        }

        private void CompileThrowUnaryExpression(Expression expr, bool asVoid)
        {
            var node = (UnaryExpression)expr;

            if (node.Operand == null)
            {
                CheckRethrow();

                CompileParameterExpression(_exceptionForRethrowStack.Peek());
                if (asVoid)
                {
                    Instructions.EmitRethrowVoid();
                }
                else
                {
                    Instructions.EmitRethrow();
                }
            }
            else
            {
                Compile(node.Operand);
                if (asVoid)
                {
                    Instructions.EmitThrowVoid();
                }
                else
                {
                    Instructions.EmitThrow();
                }
            }
        }

        private void CompileTryExpression(Expression expr)
        {
            var node = (TryExpression)expr;
            if (node.Fault != null)
            {
                CompileTryFaultExpression(node);
            }
            else
            {
                var end = Instructions.MakeLabel();
                var gotoEnd = Instructions.MakeLabel();
                var tryStart = Instructions.Count;

                BranchLabel startOfFinally = null;
                if (node.Finally != null)
                {
                    startOfFinally = Instructions.MakeLabel();
                    Instructions.EmitEnterTryFinally(startOfFinally);
                }
                else
                {
                    Instructions.EmitEnterTryCatch();
                }

                List<ExceptionHandler> exHandlers = null;
                var enterTryInstr = Instructions.GetInstruction(tryStart) as EnterTryCatchFinallyInstruction;
                Debug.Assert(enterTryInstr != null);

                PushLabelBlock(LabelScopeKind.Try);
                var hasValue = node.Type != typeof(void);

                Compile(node.Body, !hasValue);

                var tryEnd = Instructions.Count;

                // handlers jump here:
                Instructions.MarkLabel(gotoEnd);
                Instructions.EmitGoto(end, hasValue, hasValue, hasValue);

                // keep the result on the stack:
                if (node.Handlers.Count > 0)
                {
                    exHandlers = new List<ExceptionHandler>();
                    foreach (var handler in node.Handlers)
                    {
                        var parameter = handler.Variable ?? Expression.Parameter(handler.Test);

                        var local = _locals.DefineLocal(parameter, Instructions.Count);
                        _exceptionForRethrowStack.Push(parameter);

                        ExceptionFilter filter = null;

                        if (handler.Filter != null)
                        {
                            PushLabelBlock(LabelScopeKind.Filter);

                            Instructions.EmitEnterExceptionFilter();

                            // at this point the stack balance is prepared for the hidden exception variable:
                            var filterLabel = Instructions.MarkRuntimeLabel();
                            var filterStart = Instructions.Count;

                            CompileSetVariable(parameter, isVoid: true);
                            Compile(handler.Filter);
                            CompileGetVariable(parameter);

                            filter = new ExceptionFilter(filterLabel, filterStart, Instructions.Count);

                            // keep the value of the body on the stack:
                            Instructions.EmitLeaveExceptionFilter();

                            PopLabelBlock(LabelScopeKind.Filter);
                        }

                        PushLabelBlock(LabelScopeKind.Catch);

                        // add a stack balancing nop instruction (exception handling pushes the current exception):
                        if (hasValue)
                        {
                            Instructions.EmitEnterExceptionHandlerNonVoid();
                        }
                        else
                        {
                            Instructions.EmitEnterExceptionHandlerVoid();
                        }

                        // at this point the stack balance is prepared for the hidden exception variable:
                        var handlerLabel = Instructions.MarkRuntimeLabel();
                        var handlerStart = Instructions.Count;

                        CompileSetVariable(parameter, isVoid: true);
                        Compile(handler.Body, !hasValue);

                        _exceptionForRethrowStack.Pop();

                        // keep the value of the body on the stack:
                        Instructions.EmitLeaveExceptionHandler(hasValue, gotoEnd);

                        exHandlers.Add(new ExceptionHandler(handlerLabel, handlerStart, Instructions.Count, handler.Test, filter));
                        PopLabelBlock(LabelScopeKind.Catch);

                        _locals.UndefineLocal(local, Instructions.Count);
                    }
                }

                if (node.Finally != null)
                {
                    Debug.Assert(startOfFinally != null);
                    PushLabelBlock(LabelScopeKind.Finally);

                    Instructions.MarkLabel(startOfFinally);
                    Instructions.EmitEnterFinally(startOfFinally);
                    CompileAsVoid(node.Finally);
                    Instructions.EmitLeaveFinally();

                    enterTryInstr.SetTryHandler(
                        new TryCatchFinallyHandler(tryStart, tryEnd, gotoEnd.TargetIndex,
                            startOfFinally.TargetIndex, Instructions.Count,
                            exHandlers?.ToArray()));
                    PopLabelBlock(LabelScopeKind.Finally);
                }
                else
                {
                    Debug.Assert(exHandlers != null);
                    enterTryInstr.SetTryHandler(
                        new TryCatchFinallyHandler(tryStart, tryEnd, gotoEnd.TargetIndex, exHandlers.ToArray()));
                }

                Instructions.MarkLabel(end);

                PopLabelBlock(LabelScopeKind.Try);
            }
        }

        private void CompileTryFaultExpression(TryExpression expr)
        {
            Debug.Assert(expr.Finally == null);
            Debug.Assert(expr.Handlers.Count == 0);

            // Mark where we begin.
            var tryStart = Instructions.Count;
            var end = Instructions.MakeLabel();
            var enterTryInstr = Instructions.EmitEnterTryFault(end);
            Debug.Assert(enterTryInstr == Instructions.GetInstruction(tryStart));

            // Emit the try block.
            PushLabelBlock(LabelScopeKind.Try);
            var hasValue = expr.Type != typeof(void);
            Compile(expr.Body, !hasValue);
            var tryEnd = Instructions.Count;

            // Jump out of the try block to the end of the finally. If we got
            // This far, then the fault block shouldn't be run.
            Instructions.EmitGoto(end, hasValue, hasValue, hasValue);

            // Emit the fault block. The scope kind used is the same as for finally
            // blocks, which matches the Compiler.LambdaCompiler.EmitTryExpression approach.
            PushLabelBlock(LabelScopeKind.Finally);
            var startOfFault = Instructions.MakeLabel();
            Instructions.MarkLabel(startOfFault);
            Instructions.EmitEnterFault(startOfFault);
            CompileAsVoid(expr.Fault);
            Instructions.EmitLeaveFault();
            enterTryInstr.SetTryHandler(new TryFaultHandler(tryStart, tryEnd, startOfFault.TargetIndex, Instructions.Count));
            PopLabelBlock(LabelScopeKind.Finally);
            PopLabelBlock(LabelScopeKind.Try);
            Instructions.MarkLabel(end);
        }

        private void CompileTypeAsExpression(UnaryExpression node)
        {
            Compile(node.Operand);
            Instructions.EmitTypeAs(node.Type);
        }

        private void CompileTypeEqualExpression(Expression expr)
        {
            Debug.Assert(expr.NodeType == ExpressionType.TypeEqual);
            var node = (TypeBinaryExpression)expr;

            Compile(node.Expression);
            if (node.Expression.Type == typeof(void))
            {
                Instructions.EmitLoad(node.TypeOperand == typeof(void), typeof(bool));
            }
            else
            {
                Instructions.EmitLoad(node.TypeOperand.GetNonNullable());
                Instructions.EmitTypeEquals();
            }
        }

        private void CompileTypeIsExpression(Expression expr)
        {
            Debug.Assert(expr.NodeType == ExpressionType.TypeIs);
            var node = (TypeBinaryExpression)expr;

            var result = ConstantCheck.AnalyzeTypeIs(node);

            Compile(node.Expression);

            switch (result)
            {
                case AnalyzeTypeIsResult.KnownTrue:
                case AnalyzeTypeIsResult.KnownFalse:

                    // Result is known statically, so just emit the expression for
                    // its side effects and return the result
                    if (node.Expression.Type != typeof(void))
                    {
                        Instructions.EmitPop();
                    }

                    Instructions.EmitLoad(result == AnalyzeTypeIsResult.KnownTrue);
                    break;

                case AnalyzeTypeIsResult.KnownAssignable:

                    // Either the value is of the type or it is null
                    // so emit test for not-null.
                    Instructions.EmitLoad(null);
                    Instructions.EmitNotEqual(typeof(object));
                    break;

                default:
                    if (node.TypeOperand.IsValueType)
                    {
                        Instructions.EmitLoad(node.TypeOperand.GetNonNullable());
                        Instructions.EmitTypeEquals();
                    }
                    else
                    {
                        Instructions.EmitTypeIs(node.TypeOperand);
                    }
                    break;
            }
        }

        private void CompileUnaryExpression(Expression expr)
        {
            var node = (UnaryExpression)expr;

            if (node.Method != null)
            {
                EmitUnaryMethodCall(node);
            }
            else
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Not:
                    case ExpressionType.OnesComplement:
                        CompileNotExpression(node);
                        break;

                    case ExpressionType.TypeAs:
                        CompileTypeAsExpression(node);
                        break;

                    case ExpressionType.ArrayLength:
                        Compile(node.Operand);
                        Instructions.EmitArrayLength();
                        break;

                    case ExpressionType.NegateChecked:
                        Compile(node.Operand);
                        Instructions.EmitNegateChecked(node.Type);
                        break;

                    case ExpressionType.Negate:
                        Compile(node.Operand);
                        Instructions.EmitNegate(node.Type);
                        break;

                    case ExpressionType.Increment:
                        Compile(node.Operand);
                        Instructions.EmitIncrement(node.Type);
                        break;

                    case ExpressionType.Decrement:
                        Compile(node.Operand);
                        Instructions.EmitDecrement(node.Type);
                        break;

                    case ExpressionType.UnaryPlus:
                        Compile(node.Operand);
                        break;

                    case ExpressionType.IsTrue:
                    case ExpressionType.IsFalse:
                        EmitUnaryBoolCheck(node);
                        break;

                    default:
                        throw new PlatformNotSupportedException(SR.Format(SR.UnsupportedExpressionType, node.NodeType));
                }
            }
        }

        private void CompileUnboxUnaryExpression(Expression expr)
        {
            var node = (UnaryExpression)expr;

            Compile(node.Operand);

            if (node.Type.IsValueType && !node.Type.IsNullable())
            {
                Instructions.Emit(NullCheckInstruction.Instance);
            }
        }

        private void CompileUnliftedLogicalBinaryExpression(BinaryExpression expr, bool andAlso)
        {
            var elseLabel = Instructions.MakeLabel();
            var endLabel = Instructions.MakeLabel();
            Compile(expr.Left);

            if (andAlso)
            {
                Instructions.EmitBranchFalse(elseLabel);
            }
            else
            {
                Instructions.EmitBranchTrue(elseLabel);
            }
            Compile(expr.Right);
            Instructions.EmitBranch(endLabel, hasResult: false, hasValue: true);
            Instructions.MarkLabel(elseLabel);
            Instructions.EmitLoad(!andAlso);
            Instructions.MarkLabel(endLabel);
        }

        private void CompileVariableAssignment(BinaryExpression node, bool asVoid)
        {
            Compile(node.Right);

            var target = (ParameterExpression)node.Left;
            CompileSetVariable(target, asVoid);
        }

        private void DefineBlockLabels(Expression node)
        {
            if (!(node is BlockExpression block))
            {
                return;
            }

            for (int i = 0, n = block.Expressions.Count; i < n; i++)
            {
                var e = block.Expressions[i];

                if (e is LabelExpression label)
                {
                    DefineLabel(label.Target);
                }
            }
        }

        private LabelInfo DefineLabel(LabelTarget node)
        {
            if (node == null)
            {
                return new LabelInfo(null);
            }
            var result = EnsureLabel(node);
            result.Define(_labelBlock);
            return result;
        }

        private void EmitCopyValueType(Type valueType)
        {
            if (MaybeMutableValueType(valueType))
            {
                // loading a value type on the stack has copy semantics unless
                // we are specifically loading the address of the object, so we
                // emit a copy here if we don't know the type is immutable.
                Instructions.Emit(ValueTypeCopyInstruction.Instruction);
            }
        }

        private void EmitIndexGet(IndexExpression index)
        {
            if (index.Indexer != null)
            {
                Instructions.EmitCall(index.Indexer.GetGetMethod(nonPublic: true));
            }
            else if (index.ArgumentCount != 1)
            {
                Instructions.EmitCall(index.Object.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance));
            }
            else
            {
                Instructions.EmitGetArrayItem();
            }
        }

        private void EmitThisForMethodCall(Expression node)
        {
            CompileAddress(node, -1);
        }

        private void EmitUnaryBoolCheck(UnaryExpression node)
        {
            Compile(node.Operand);
            if (node.IsLifted)
            {
                var notNull = Instructions.MakeLabel();
                var computed = Instructions.MakeLabel();

                Instructions.EmitCoalescingBranch(notNull);
                Instructions.EmitBranch(computed);

                Instructions.MarkLabel(notNull);
                Instructions.EmitLoad(node.NodeType == ExpressionType.IsTrue);
                Instructions.EmitEqual(typeof(bool));

                Instructions.MarkLabel(computed);
            }
            else
            {
                Instructions.EmitLoad(node.NodeType == ExpressionType.IsTrue);
                Instructions.EmitEqual(typeof(bool));
            }
        }

        private void EmitUnaryMethodCall(UnaryExpression node)
        {
            Compile(node.Operand);
            if (node.IsLifted)
            {
                var notNull = Instructions.MakeLabel();
                var computed = Instructions.MakeLabel();

                Instructions.EmitCoalescingBranch(notNull);
                Instructions.EmitBranch(computed);

                Instructions.MarkLabel(notNull);
                Instructions.EmitCall(node.Method);

                Instructions.MarkLabel(computed);
            }
            else
            {
                Instructions.EmitCall(node.Method);
            }
        }

        private LocalVariable EnsureAvailableForClosure(ParameterExpression expr)
        {
            if (_locals.TryGetLocalOrClosure(expr, out var local))
            {
                if (!local.InClosure && !local.IsBoxed)
                {
                    _locals.Box(expr, Instructions);
                }
                return local;
            }

            if (_parent != null)
            {
                _parent.EnsureAvailableForClosure(expr);
                return _locals.AddClosureVariable(expr);
            }

            throw new InvalidOperationException("unbound variable: " + expr);
        }

        private LabelInfo EnsureLabel(LabelTarget node)
        {
            if (!_treeLabels.TryGetValue(node, out var result))
            {
                _treeLabels[node] = result = new LabelInfo(node);
            }
            return result;
        }

        private void LoadLocalNoValueTypeCopy(ParameterExpression variable)
        {
            var local = ResolveLocal(variable);

            if (local.InClosure)
            {
                Instructions.EmitLoadLocalFromClosure(local.Index);
            }
            else if (local.IsBoxed)
            {
                Instructions.EmitLoadLocalBoxed(local.Index);
            }
            else
            {
                Instructions.EmitLoadLocal(local.Index);
            }
        }

        private Interpreter MakeInterpreter(string lambdaName)
        {
            var debugInfos = _debugInfos.ToArray();
            foreach (var kvp in _treeLabels)
            {
                kvp.Value.ValidateFinish();
            }
            return new Interpreter(lambdaName, _locals, Instructions.ToArray(), debugInfos);
        }

        private bool MaybeMutableValueType(Type type)
        {
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        private void PopLabelBlock(LabelScopeKind kind)
        {
            Theraot.No.Op(kind);
            Debug.Assert(_labelBlock != null && _labelBlock.Kind == kind);
            _labelBlock = _labelBlock.Parent;
        }

        private void PushLabelBlock(LabelScopeKind type)
        {
            _labelBlock = new LabelScopeInfo(_labelBlock, type);
        }

        private LabelInfo ReferenceLabel(LabelTarget node)
        {
            var result = EnsureLabel(node);
            result.Reference(_labelBlock);
            return result;
        }

        private LocalVariable ResolveLocal(ParameterExpression variable)
        {
            if (!_locals.TryGetLocalOrClosure(variable, out var local))
            {
                local = EnsureAvailableForClosure(variable);
            }
            return local;
        }

#if DEBUG
#endif

        private bool TryPushLabelBlock(Expression node)
        {
            // Anything that is "statement-like" -- e.g. has no associated
            // stack state can be jumped into, with the exception of try-blocks
            // We indicate this by a "Block"
            //
            // Otherwise, we push an "Expression" to indicate that it can't be
            // jumped into
            switch (node.NodeType)
            {
                default:
                    if (_labelBlock.Kind != LabelScopeKind.Expression)
                    {
                        PushLabelBlock(LabelScopeKind.Expression);
                        return true;
                    }
                    return false;

                case ExpressionType.Label:
                    // LabelExpression is a bit special, if it's directly in a
                    // block it becomes associate with the block's scope. Same
                    // thing if it's in a switch case body.
                    if (_labelBlock.Kind == LabelScopeKind.Block)
                    {
                        var label = ((LabelExpression)node).Target;
                        if (_labelBlock.ContainsTarget(label))
                        {
                            return false;
                        }
                        if (_labelBlock.Parent.Kind == LabelScopeKind.Switch &&
                            _labelBlock.Parent.ContainsTarget(label))
                        {
                            return false;
                        }
                    }
                    PushLabelBlock(LabelScopeKind.Statement);
                    return true;

                case ExpressionType.Block:
                    PushLabelBlock(LabelScopeKind.Block);
                    // Labels defined immediately in the block are valid for
                    // the whole block.
                    if (_labelBlock.Parent.Kind != LabelScopeKind.Switch)
                    {
                        DefineBlockLabels(node);
                    }
                    return true;

                case ExpressionType.Switch:
                    PushLabelBlock(LabelScopeKind.Switch);
                    // Define labels inside of the switch cases so they are in
                    // scope for the whole switch. This allows "goto case" and
                    // "goto default" to be considered as local jumps.
                    var @switch = (SwitchExpression)node;
                    foreach (var c in @switch.Cases)
                    {
                        DefineBlockLabels(c.Body);
                    }
                    DefineBlockLabels(@switch.DefaultBody);
                    return true;

                // Remove this when Convert(Void) goes away.
                case ExpressionType.Convert:
                    if (node.Type != typeof(void))
                    {
                        // treat it as an expression
                        goto default;
                    }
                    PushLabelBlock(LabelScopeKind.Statement);
                    return true;

                case ExpressionType.Conditional:
                case ExpressionType.Loop:
                case ExpressionType.Goto:
                    PushLabelBlock(LabelScopeKind.Statement);
                    return true;
            }
        }

        private sealed class QuoteVisitor : ExpressionVisitor
        {
            public readonly HashSet<ParameterExpression> HoistedParameters = new HashSet<ParameterExpression>();
            private readonly Dictionary<ParameterExpression, int> _definedParameters = new Dictionary<ParameterExpression, int>();

            protected internal override Expression VisitBlock(BlockExpression node)
            {
                PushParameters(node.Variables);

                base.VisitBlock(node);

                PopParameters(node.Variables);

                return node;
            }

            protected internal override Expression VisitLambda<T>(Expression<T> node)
            {
                IEnumerable<ParameterExpression> parameters = ArrayReservoir<ParameterExpression>.EmptyArray;

                var count = node.ParameterCount;

                if (count > 0)
                {
                    var parameterList = new List<ParameterExpression>(count);

                    for (var i = 0; i < count; i++)
                    {
                        parameterList.Add(node.GetParameter(i));
                    }

                    parameters = parameterList;
                }

                PushParameters(parameters);

                base.VisitLambda(node);

                PopParameters(parameters);

                return node;
            }

            protected internal override Expression VisitParameter(ParameterExpression node)
            {
                if (!_definedParameters.ContainsKey(node))
                {
                    HoistedParameters.Add(node);
                }
                return node;
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                if (node.Variable != null)
                {
                    PushParameters(new[] { node.Variable });
                }
                Visit(node.Body);
                Visit(node.Filter);
                if (node.Variable != null)
                {
                    PopParameters(new[] { node.Variable });
                }
                return node;
            }

            private void PopParameters(IEnumerable<ParameterExpression> parameters)
            {
                foreach (var param in parameters)
                {
                    var count = _definedParameters[param];
                    if (count == 0)
                    {
                        _definedParameters.Remove(param);
                    }
                    else
                    {
                        _definedParameters[param] = count - 1;
                    }
                }
            }

            private void PushParameters(IEnumerable<ParameterExpression> parameters)
            {
                foreach (var param in parameters)
                {
                    if (_definedParameters.TryGetValue(param, out var count))
                    {
                        _definedParameters[param] = count + 1;
                    }
                    else
                    {
                        _definedParameters[param] = 1;
                    }
                }
            }
        }
    }

    internal sealed class ParameterByRefUpdater : ByRefUpdater
    {
        private readonly LocalVariable _parameter;

        public ParameterByRefUpdater(LocalVariable parameter, int argumentIndex)
            : base(argumentIndex)
        {
            _parameter = parameter;
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            if (_parameter.InClosure)
            {
                var box = frame.Closure[_parameter.Index];
                box.Value = value;
            }
            else if (_parameter.IsBoxed)
            {
                var box = (IStrongBox)frame.Data[_parameter.Index];
                box.Value = value;
            }
            else
            {
                frame.Data[_parameter.Index] = value;
            }
        }
    }

    internal sealed class PropertyByRefUpdater : ByRefUpdater
    {
        private readonly LocalDefinition? _object;
        private readonly PropertyInfo _property;

        public PropertyByRefUpdater(LocalDefinition? obj, PropertyInfo property, int argumentIndex)
            : base(argumentIndex)
        {
            _object = obj;
            _property = property;
        }

        public override void UndefineTemps(InstructionList instructions, LocalVariables locals)
        {
            if (_object != null)
            {
                locals.UndefineLocal(_object.GetValueOrDefault(), instructions.Count);
            }
        }

        public override void Update(InterpretedFrame frame, object value)
        {
            var obj = _object == null ? null : frame.Data[_object.GetValueOrDefault().Index];

            try
            {
                _property.SetValue(obj, value);
            }
            catch (TargetInvocationException e)
            {
                ExceptionHelpers.UnwrapAndRethrow(e);
                throw ContractUtils.Unreachable;
            }
        }
    }

    /// <summary>
    /// The re-throw instruction will throw this exception
    /// </summary>
    internal sealed class RethrowException : Exception
    {
        public RethrowException()
        {
            // Empty
        }

        public RethrowException(string message)
            : base(message)
        {
            // Empty
        }

        public RethrowException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }
    }

    internal sealed class TryCatchFinallyHandler
    {
        internal readonly int FinallyEndIndex;
        internal readonly int FinallyStartIndex;
        internal readonly int GotoEndTargetIndex;
        internal readonly int TryEndIndex;
        internal readonly int TryStartIndex;

        /// <summary>
        /// No finally block
        /// </summary>
        internal TryCatchFinallyHandler(int tryStart, int tryEnd, int gotoEndTargetIndex, ExceptionHandler[] handlers)
            : this(tryStart, tryEnd, gotoEndTargetIndex, Instruction.UnknownInstrIndex, Instruction.UnknownInstrIndex, handlers)
        {
            Debug.Assert(handlers != null, "catch blocks should exist");
        }

        /// <summary>
        /// Generic constructor
        /// </summary>
        internal TryCatchFinallyHandler(int tryStart, int tryEnd, int gotoEndLabelIndex, int finallyStart, int finallyEnd, ExceptionHandler[] handlers)
        {
            TryStartIndex = tryStart;
            TryEndIndex = tryEnd;
            FinallyStartIndex = finallyStart;
            FinallyEndIndex = finallyEnd;
            GotoEndTargetIndex = gotoEndLabelIndex;
            Handlers = handlers;
        }

        internal ExceptionHandler[] Handlers { get; }

        internal bool IsCatchBlockExist => Handlers != null;

        internal bool IsFinallyBlockExist
        {
            get
            {
                Debug.Assert(FinallyStartIndex != Instruction.UnknownInstrIndex == (FinallyEndIndex != Instruction.UnknownInstrIndex));
                return FinallyStartIndex != Instruction.UnknownInstrIndex;
            }
        }

        internal bool HasHandler(InterpretedFrame frame, Exception exception, out ExceptionHandler handler, out object unwrappedException)
        {
#if DEBUG
            if (exception is RethrowException)
            {
                // Unreachable.
                // Want to assert that this case isn't hit, but an assertion failure here will be eaten because
                // we are in an exception filter. Therefore return true here and assert in the catch block.
                handler = null;
                unwrappedException = exception;
                return true;
            }
#endif
            frame.SaveTraceToException(exception);

            if (IsCatchBlockExist)
            {
                unwrappedException = exception is RuntimeWrappedException rwe ? rwe.WrappedException : exception;
                var exceptionType = unwrappedException.GetType();
                foreach (var candidate in Handlers)
                {
                    if (candidate.Matches(exceptionType) && (candidate.Filter == null || FilterPasses(frame, ref unwrappedException, candidate.Filter)))
                    {
                        handler = candidate;
                        return true;
                    }
                }
            }
            else
            {
                unwrappedException = null;
            }

            handler = null;
            return false;
        }

        private static bool FilterPasses(InterpretedFrame frame, ref object exception, ExceptionFilter filter)
        {
            var interpreter = frame.Interpreter;
            var instructions = interpreter.Instructions.Instructions;
            var stackIndex = frame.StackIndex;
            var frameIndex = frame.InstructionIndex;
            try
            {
                var index = interpreter.Labels[filter.LabelIndex].Index;
                frame.InstructionIndex = index;
                frame.Push(exception);
                while (index >= filter.StartIndex && index < filter.EndIndex)
                {
                    index += instructions[index].Run(frame);
                    frame.InstructionIndex = index;
                }

                // Exception is stored in a local at start of the filter, and loaded from it at the end, so it is now
                // on the top of the stack. It may have been assigned to in the course of the filter running.
                // If this is the handler that will be executed, then if the filter has assigned to the exception variable
                // that change should be visible to the handler. Otherwise, it should not, so we write it back only on true.
                var exceptionLocal = frame.Pop();
                if ((bool)frame.Pop())
                {
                    exception = exceptionLocal;
                    // Stack and instruction indices will be overwritten in the catch block anyway, so no need to restore.
                    return true;
                }
            }
            catch
            {
                // Silently eating exceptions and returning false matches the CLR behavior.
            }

            frame.StackIndex = stackIndex;
            frame.InstructionIndex = frameIndex;
            return false;
        }
    }

    internal sealed class TryFaultHandler
    {
        internal readonly int FinallyEndIndex;
        internal readonly int FinallyStartIndex;
        internal readonly int TryEndIndex;
        internal readonly int TryStartIndex;

        internal TryFaultHandler(int tryStart, int tryEnd, int finallyStart, int finallyEnd)
        {
            TryStartIndex = tryStart;
            TryEndIndex = tryEnd;
            FinallyStartIndex = finallyStart;
            FinallyEndIndex = finallyEnd;
        }
    }
}

#endif