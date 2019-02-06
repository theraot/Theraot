#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Globalization;
using System.Reflection.Emit;
using Theraot.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        // Add key to a new or existing bucket
        private static void AddToBuckets(List<List<SwitchLabel>> buckets, SwitchLabel key)
        {
            if (buckets.Count > 0)
            {
                var last = buckets[buckets.Count - 1];
                if (FitsInBucket(last, key.Key, 1))
                {
                    last.Add(key);
                    // we might be able to merge now
                    MergeBuckets(buckets);
                    return;
                }
            }

            // else create a new bucket
            buckets.Add(new List<SwitchLabel> {key});
        }

        // Determines if the type is an integer we can switch on.
        private static bool CanOptimizeSwitchType(Type valueType)
        {
            // enums & char are allowed
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;

                default:
                    return false;
            }
        }

        private static decimal ConvertSwitchValue(object value)
        {
            if (value is char c)
            {
                return c;
            }

            return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }

        private static bool FitsInBucket(List<SwitchLabel> buckets, decimal key, int count)
        {
            Debug.Assert(key > buckets[buckets.Count - 1].Key);
            var jumpTableSlots = key - buckets[0].Key + 1;
            if (jumpTableSlots > int.MaxValue)
            {
                return false;
            }

            // density must be > 50%
            return (buckets.Count + count) * 2 > jumpTableSlots;
        }

        private static Type GetTestValueType(SwitchExpression node)
        {
            if (node.Comparison == null)
            {
                // If we have no comparison, all right side types must be the
                // same.
                return node.Cases[0].TestValues[0].Type;
            }

            // Otherwise, get the type from the method.
            var result = node.Comparison.GetParameters()[1].ParameterType.GetNonRefTypeInternal();
            if (node.IsLifted)
            {
                result = result.GetNullable();
            }

            return result;
        }

        private static bool HasVariables(object node)
        {
            if (node is BlockExpression block)
            {
                return block.Variables.Count > 0;
            }

            return ((CatchBlock)node).Variable != null;
        }

        private static void MergeBuckets(List<List<SwitchLabel>> buckets)
        {
            while (buckets.Count > 1)
            {
                var first = buckets[buckets.Count - 2];
                var second = buckets[buckets.Count - 1];

                if (!FitsInBucket(first, second[second.Count - 1].Key, second.Count))
                {
                    return;
                }

                // Merge them
                first.AddRange(second);
                buckets.RemoveAt(buckets.Count - 1);
            }
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

            throw new InvalidOperationException("Rethrow statement is valid only inside a Catch block.");
        }

        private void CheckTry()
        {
            // Try inside a filter is not verifiable
            for (var j = _labelBlock; j != null; j = j.Parent)
            {
                if (j.Kind == LabelScopeKind.Filter)
                {
                    throw new InvalidOperationException("Try expression is not allowed inside a filter body.");
                }
            }
        }

        private void DefineSwitchCaseLabel(SwitchCase @case, out Label label, out bool isGoto)
        {
            // if it's a goto with no value
            if (@case.Body is GotoExpression jump && jump.Value == null)
            {
                // Reference the label from the switch. This will cause us to
                // analyze the jump target and determine if it is safe.
                var jumpInfo = ReferenceLabel(jump.Target);

                // If we have are allowed to emit the "branch" opcode, then we
                // can jump directly there from the switch's jump table.
                // (Otherwise, we need to emit the goto later as a "leave".)
                if (jumpInfo.CanBranch)
                {
                    label = jumpInfo.Label;
                    isGoto = true;
                    return;
                }
            }

            // otherwise, just define a new label
            label = IL.DefineLabel();
            isGoto = false;
        }

        private void Emit(BlockExpression node, CompilationFlags flags)
        {
            var count = node.ExpressionCount;

            if (count == 0)
            {
                return;
            }

            EnterScope(node);

            var emitAs = flags & CompilationFlags.EmitAsTypeMask;

            var tailCall = flags & CompilationFlags.EmitAsTailCallMask;
            for (var index = 0; index < count - 1; index++)
            {
                var e = node.GetExpression(index);
                var next = node.GetExpression(index + 1);

                var tailCallFlag = tailCall != CompilationFlags.EmitAsNoTail
                    ? next is GotoExpression g && (g.Value == null || !Significant(g.Value)) && ReferenceLabel(g.Target).CanReturn
                        ? CompilationFlags.EmitAsTail
                        : CompilationFlags.EmitAsMiddle
                    : CompilationFlags.EmitAsNoTail;

                flags = UpdateEmitAsTailCallFlag(flags, tailCallFlag);
                EmitExpressionAsVoid(e, flags);
            }

            // if the type of Block it means this is not a Comma
            // so we will force the last expression to emit as void.
            // We don't need EmitAsType flag anymore, should only pass
            // the EmitTailCall field in flags to emitting the last expression.
            if (emitAs == CompilationFlags.EmitAsVoidType || node.Type == typeof(void))
            {
                EmitExpressionAsVoid(node.GetExpression(count - 1), tailCall);
            }
            else
            {
                EmitExpressionAsType(node.GetExpression(count - 1), node.Type, tailCall);
            }

            ExitScope(node);
        }

        private void EmitBlockExpression(Expression expr, CompilationFlags flags)
        {
            // emit body
            Emit((BlockExpression)expr, UpdateEmitAsTypeFlag(flags, CompilationFlags.EmitAsDefaultType));
        }

        private void EmitCatchStart(CatchBlock cb)
        {
            if (cb.Filter == null)
            {
                EmitSaveExceptionOrPop(cb);
                return;
            }

            // emit filter block. Filter blocks are untyped so we need to do
            // the type check ourselves.
            var endFilter = IL.DefineLabel();
            var rightType = IL.DefineLabel();

            // skip if it's not our exception type, but save
            // the exception if it is so it's available to the
            // filter
            IL.Emit(OpCodes.Isinst, cb.Test);
            IL.Emit(OpCodes.Dup);
            IL.Emit(OpCodes.Brtrue, rightType);
            IL.Emit(OpCodes.Pop);
            IL.Emit(OpCodes.Ldc_I4_0);
            IL.Emit(OpCodes.Br, endFilter);

            // it's our type, save it and emit the filter.
            IL.MarkLabel(rightType);
            EmitSaveExceptionOrPop(cb);
            PushLabelBlock(LabelScopeKind.Filter);
            EmitExpression(cb.Filter);
            PopLabelBlock(LabelScopeKind.Filter);

            // begin the catch, clear the exception, we've
            // already saved it
            IL.MarkLabel(endFilter);
            IL.BeginCatchBlock(null);
            IL.Emit(OpCodes.Pop);
        }

        private void EmitDefaultExpression(Expression expr)
        {
            var node = (DefaultExpression)expr;
            if (node.Type != typeof(void))
            {
                // emit default(T)
                IL.EmitDefault(node.Type, this);
            }
        }

        private void EmitLoopExpression(Expression expr)
        {
            var node = (LoopExpression)expr;

            PushLabelBlock(LabelScopeKind.Statement);
            var breakTarget = DefineLabel(node.BreakLabel);
            var continueTarget = DefineLabel(node.ContinueLabel);

            continueTarget.MarkWithEmptyStack();

            EmitExpressionAsVoid(node.Body);

            IL.Emit(OpCodes.Br, continueTarget.Label);

            PopLabelBlock(LabelScopeKind.Statement);

            breakTarget.MarkWithEmptyStack();
        }

        private void EmitSaveExceptionOrPop(CatchBlock cb)
        {
            if (cb.Variable != null)
            {
                // If the variable is present, store the exception
                // in the variable.
                _scope.EmitSet(cb.Variable);
            }
            else
            {
                // Otherwise, pop it off the stack.
                IL.Emit(OpCodes.Pop);
            }
        }

        private void EmitSwitchBucket(SwitchInfo info, List<SwitchLabel> bucket)
        {
            // No need for switch if we only have one value
            if (bucket.Count == 1)
            {
                IL.Emit(OpCodes.Ldloc, info.Value);
                EmitConstant(bucket[0].Constant);
                IL.Emit(OpCodes.Beq, bucket[0].Label);
                return;
            }

            //
            // If we're switching off of Int64/UInt64, we need more guards here
            // because we'll have to narrow the switch value to an Int32, and
            // we can't do that unless the value is in the right range.
            //
            Label? after = null;
            if (info.Is64BitSwitch)
            {
                after = IL.DefineLabel();
                IL.Emit(OpCodes.Ldloc, info.Value);
                EmitConstant(bucket.Last().Constant);
                IL.Emit(info.IsUnsigned ? OpCodes.Bgt_Un : OpCodes.Bgt, after.Value);
                IL.Emit(OpCodes.Ldloc, info.Value);
                EmitConstant(bucket[0].Constant);
                IL.Emit(info.IsUnsigned ? OpCodes.Blt_Un : OpCodes.Blt, after.Value);
            }

            IL.Emit(OpCodes.Ldloc, info.Value);

            // Normalize key
            var key = bucket[0].Key;
            if (key != 0)
            {
                EmitConstant(bucket[0].Constant);
                IL.Emit(OpCodes.Sub);
            }

            if (info.Is64BitSwitch)
            {
                IL.Emit(OpCodes.Conv_I4);
            }

            // Collect labels
            var len = (int)(bucket[bucket.Count - 1].Key - bucket[0].Key + 1);
            var jmpLabels = new Label[len];

            // Initialize all labels to the default
            var slot = 0;
            foreach (var label in bucket)
            {
                while (key++ != label.Key)
                {
                    jmpLabels[slot++] = info.Default;
                }

                jmpLabels[slot++] = label.Label;
            }

            // check we used all keys and filled all slots
            Debug.Assert(key == bucket[bucket.Count - 1].Key + 1);
            Debug.Assert(slot == jmpLabels.Length);

            // Finally, emit the switch instruction
            IL.Emit(OpCodes.Switch, jmpLabels);

            if (info.Is64BitSwitch && after.HasValue)
            {
                IL.MarkLabel(after.Value);
            }
        }

        private void EmitSwitchBuckets(SwitchInfo info, List<List<SwitchLabel>> buckets, int first, int last)
        {
            for (;;)
            {
                if (first == last)
                {
                    EmitSwitchBucket(info, buckets[first]);
                    return;
                }

                // Split the buckets into two groups, and use an if test to find
                // the right bucket. This ensures we'll only need O(lg(B)) tests
                // where B is the number of buckets
                var mid = (int)(((long)first + last + 1) / 2);

                if (first == mid - 1)
                {
                    EmitSwitchBucket(info, buckets[first]);
                }
                else
                {
                    // If the first half contains more than one, we need to emit an
                    // explicit guard
                    var secondHalf = IL.DefineLabel();
                    IL.Emit(OpCodes.Ldloc, info.Value);
                    EmitConstant(buckets[mid - 1].Last().Constant);
                    IL.Emit(info.IsUnsigned ? OpCodes.Bgt_Un : OpCodes.Bgt, secondHalf);
                    EmitSwitchBuckets(info, buckets, first, mid - 1);
                    IL.MarkLabel(secondHalf);
                }

                first = mid;
            }
        }

        private void EmitSwitchCases(SwitchExpression node, Label[] labels, bool[] isGoto, Label @default, Label end, CompilationFlags flags)
        {
            // Jump to default (to handle the fallthrough case)
            IL.Emit(OpCodes.Br, @default);

            // Emit the cases
            for (int i = 0, n = node.Cases.Count; i < n; i++)
            {
                // If the body is a goto, we already emitted an optimized
                // branch directly to it. No need to emit anything else.
                if (isGoto[i])
                {
                    continue;
                }

                IL.MarkLabel(labels[i]);
                EmitExpressionAsType(node.Cases[i].Body, node.Type, flags);

                // Last case doesn't need branch
                if (node.DefaultBody == null && i >= n - 1)
                {
                    continue;
                }

                if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail)
                {
                    //The switch case is at the tail of the lambda so
                    //it is safe to emit a Ret.
                    IL.Emit(OpCodes.Ret);
                }
                else
                {
                    IL.Emit(OpCodes.Br, end);
                }
            }

            // Default value
            if (node.DefaultBody != null)
            {
                IL.MarkLabel(@default);
                EmitExpressionAsType(node.DefaultBody, node.Type, flags);
            }

            IL.MarkLabel(end);
        }

        private void EmitSwitchExpression(Expression expr, CompilationFlags flags)
        {
            var node = (SwitchExpression)expr;

            if (node.Cases.Count == 0)
            {
                // Emit the switch value in case it has side-effects, but as void
                // since the value is ignored.
                EmitExpressionAsVoid(node.SwitchValue);

                // Now if there is a default body, it happens unconditionally.
                if (node.DefaultBody != null)
                {
                    EmitExpressionAsType(node.DefaultBody, node.Type, flags);
                }
                else
                {
                    // If there are no cases and no default then the type must be void.
                    // Assert that earlier validation caught any exceptions to that.
                    Debug.Assert(node.Type == typeof(void));
                }

                return;
            }

            // Try to emit it as an IL switch. Works for integer types.
            if (TryEmitSwitchInstruction(node, flags))
            {
                return;
            }

            // Try to emit as a hashtable lookup. Works for strings.
            if (TryEmitHashtableSwitch(node, flags))
            {
                return;
            }

            //
            // Fall back to a series of tests. We need to IL gen instead of
            // transform the tree to avoid stack overflow on a big switch.
            //

            var switchValue = Expression.Parameter(node.SwitchValue.Type, "switchValue");
            var testValue = Expression.Parameter(GetTestValueType(node), "testValue");
            _scope.AddLocal(this, switchValue);
            _scope.AddLocal(this, testValue);

            EmitExpression(node.SwitchValue);
            _scope.EmitSet(switchValue);

            // Emit tests
            var labels = new Label[node.Cases.Count];
            var isGoto = new bool[node.Cases.Count];
            for (int i = 0, n = node.Cases.Count; i < n; i++)
            {
                DefineSwitchCaseLabel(node.Cases[i], out labels[i], out isGoto[i]);
                foreach (var test in node.Cases[i].TestValues)
                {
                    // Pull the test out into a temp so it runs on the same
                    // stack as the switch. This simplifies spilling.
                    EmitExpression(test);
                    _scope.EmitSet(testValue);
                    Debug.Assert(testValue.Type.IsReferenceAssignableFromInternal(test.Type));
                    EmitExpressionAndBranch(true, Expression.Equal(switchValue, testValue, false, node.Comparison), labels[i]);
                }
            }

            // Define labels
            var end = IL.DefineLabel();
            var @default = node.DefaultBody == null ? end : IL.DefineLabel();

            // Emit the case and default bodies
            EmitSwitchCases(node, labels, isGoto, @default, end, flags);
        }

        private void EmitTryExpression(Expression expr)
        {
            var node = (TryExpression)expr;

            CheckTry();

            //******************************************************************
            // 1. ENTERING TRY
            //******************************************************************

            PushLabelBlock(LabelScopeKind.Try);
            IL.BeginExceptionBlock();

            //******************************************************************
            // 2. Emit the try statement body
            //******************************************************************

            EmitExpression(node.Body);

            var tryType = node.Type;
            LocalBuilder value = null;
            if (tryType != typeof(void))
            {
                //store the value of the try body
                value = GetLocal(tryType);
                IL.Emit(OpCodes.Stloc, value);
            }
            //******************************************************************
            // 3. Emit the catch blocks
            //******************************************************************

            foreach (var cb in node.Handlers)
            {
                PushLabelBlock(LabelScopeKind.Catch);

                // Begin the strongly typed exception block
                if (cb.Filter == null)
                {
                    IL.BeginCatchBlock(cb.Test);
                }
                else
                {
                    IL.BeginExceptFilterBlock();
                }

                EnterScope(cb);

                EmitCatchStart(cb);

                //
                // Emit the catch block body
                //
                EmitExpression(cb.Body);
                if (tryType != typeof(void))
                {
                    //store the value of the catch block body
                    // ReSharper disable once AssignNullToNotNullAttribute
                    IL.Emit(OpCodes.Stloc, value);
                }

                ExitScope(cb);

                PopLabelBlock(LabelScopeKind.Catch);
            }

            //******************************************************************
            // 4. Emit the finally block
            //******************************************************************

            if (node.Finally != null || node.Fault != null)
            {
                PushLabelBlock(LabelScopeKind.Finally);

                if (node.Finally != null)
                {
                    IL.BeginFinallyBlock();
                }
                else
                {
                    IL.BeginFaultBlock();
                }

                // Emit the body
                EmitExpressionAsVoid(node.Finally ?? node.Fault);

                IL.EndExceptionBlock();
                PopLabelBlock(LabelScopeKind.Finally);
            }
            else
            {
                IL.EndExceptionBlock();
            }

            if (tryType != typeof(void))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                IL.Emit(OpCodes.Ldloc, value);
                FreeLocal(value);
            }

            PopLabelBlock(LabelScopeKind.Try);
        }

        private void EnterScope(object node)
        {
            if (!HasVariables(node) || _scope.MergedScopes?.Contains(node as BlockExpression) == true)
            {
                return;
            }

            if (!_tree.Scopes.TryGetValue(node, out var scope))
            {
                //
                // Very often, we want to compile nodes as reductions
                // rather than as IL, but usually they need to allocate
                // some IL locals. To support this, we allow emitting a
                // BlockExpression that was not bound by VariableBinder.
                // This works as long as the variables are only used
                // locally -- i.e. not closed over.
                //
                // User-created blocks will never hit this case; only our
                // internally reduced nodes will.
                //
                scope = new CompilerScope(node, false) {NeedsClosure = _scope.NeedsClosure};
            }

            _scope = scope.Enter(this, _scope);
            Debug.Assert(_scope.Node == node);
        }

        private void ExitScope(object node)
        {
            if (_scope.Node == node)
            {
                _scope = _scope.Exit();
            }
        }

        private bool TryEmitHashtableSwitch(SwitchExpression node, CompilationFlags flags)
        {
            // If we have a comparison other than string equality, bail
            if (node.Comparison != CachedReflectionInfo.StringOpEqualityStringString && node.Comparison != CachedReflectionInfo.StringEqualsStringString)
            {
                return false;
            }

            // All test values must be constant.
            var tests = 0;
            foreach (var c in node.Cases)
            {
                foreach (var t in c.TestValues)
                {
                    if (!(t is ConstantExpression))
                    {
                        return false;
                    }

                    tests++;
                }
            }

            // Must have >= 7 labels for it to be worth it.
            if (tests < 7)
            {
                return false;
            }

            // If we're in a DynamicMethod, we could just build the dictionary
            // immediately. But that would cause the two code paths to be more
            // different than they really need to be.
            var initializers = new List<ElementInit>(tests);
            var cases = new ArrayBuilder<SwitchCase>(node.Cases.Count);

            var nullCase = -1;
            var add = CachedReflectionInfo.DictionaryOfStringInt32AddStringInt32;
            for (int i = 0, n = node.Cases.Count; i < n; i++)
            {
                foreach (var expression in node.Cases[i].TestValues)
                {
                    var t = (ConstantExpression)expression;
                    if (t.Value != null)
                    {
                        initializers.Add(Expression.ElementInit(add, ReadOnlyCollectionEx.Create<Expression>(t, Utils.Constant(i))));
                    }
                    else
                    {
                        nullCase = i;
                    }
                }

                cases.UncheckedAdd(Expression.SwitchCase(node.Cases[i].Body, ReadOnlyCollectionEx.Create<Expression>(Utils.Constant(i))));
            }

            // Create the field to hold the lazily initialized dictionary
            var dictField = CreateLazyInitializedField<Dictionary<string, int>>("dictionarySwitch");

            // If we happen to initialize it twice (multithreaded case), it's
            // not the end of the world. The C# compiler does better here by
            // emitting a volatile access to the field.
            Expression dictInit = Expression.Condition
            (
                Expression.Equal(dictField, Expression.Constant(null, dictField.Type)),
                Expression.Assign
                (
                    dictField,
                    Expression.ListInit
                    (
                        Expression.New
                        (
                            CachedReflectionInfo.DictionaryOfStringInt32CtorInt32,
                            ReadOnlyCollectionEx.Create<Expression>
                            (
                                Utils.Constant(initializers.Count)
                            )
                        ),
                        initializers
                    )
                ),
                dictField
            );

            //
            // Create a tree like:
            //
            // switchValue = switchValueExpression;
            // if (switchValue == null) {
            //     switchIndex = nullCase;
            // } else {
            //     if (_dictField == null) {
            //         _dictField = new Dictionary<string, int>(count) { { ... }, ... };
            //     }
            //     if (!_dictField.TryGetValue(switchValue, out switchIndex)) {
            //         switchIndex = -1;
            //     }
            // }
            // switch (switchIndex) {
            //     case 0: ...
            //     case 1: ...
            //     ...
            //     default:
            // }
            //
            var switchValue = Expression.Variable(typeof(string), "switchValue");
            var switchIndex = Expression.Variable(typeof(int), "switchIndex");
            var reduced = Expression.Block
            (
                ReadOnlyCollectionEx.Create(switchIndex, switchValue),
                ReadOnlyCollectionEx.Create<Expression>
                (
                    Expression.Assign(switchValue, node.SwitchValue),
                    Expression.IfThenElse
                    (
                        Expression.Equal(switchValue, Expression.Constant(null, typeof(string))),
                        Expression.Assign(switchIndex, Utils.Constant(nullCase)),
                        Expression.IfThenElse
                        (
                            Expression.Call(dictInit, "TryGetValue", null, switchValue, switchIndex),
                            Utils.Empty,
                            Expression.Assign(switchIndex, Utils.Constant(-1))
                        )
                    ),
                    Expression.Switch(node.Type, switchIndex, node.DefaultBody, null, cases.ToArray())
                )
            );

            EmitExpression(reduced, flags);
            return true;
        }

        // Tries to emit switch as a jmp table
        private bool TryEmitSwitchInstruction(SwitchExpression node, CompilationFlags flags)
        {
            // If we have a comparison, bail
            if (node.Comparison != null)
            {
                return false;
            }

            // Make sure the switch value type and the right side type
            // are types we can optimize
            var type = node.SwitchValue.Type;
            if (!CanOptimizeSwitchType(type) || !TypeUtils.AreEquivalent(type, node.Cases[0].TestValues[0].Type))
            {
                return false;
            }

            // Make sure all test values are constant, or we can't emit the
            // jump table.
            if (!node.Cases.All(c => c.TestValues.All(t => t is ConstantExpression)))
            {
                return false;
            }

            //
            // We can emit the optimized switch, let's do it.
            //

            // Build target labels, collect keys.
            var labels = new Label[node.Cases.Count];
            var isGoto = new bool[node.Cases.Count];

            var uniqueKeys = new HashSet<decimal>();
            var keys = new List<SwitchLabel>();
            for (var i = 0; i < node.Cases.Count; i++)
            {
                DefineSwitchCaseLabel(node.Cases[i], out labels[i], out isGoto[i]);

                foreach (var expression in node.Cases[i].TestValues)
                {
                    var test = (ConstantExpression)expression;
                    // Guaranteed to work thanks to CanOptimizeSwitchType.
                    //
                    // Use decimal because it can hold Int64 or UInt64 without
                    // precision loss or signed/unsigned conversions.
                    var key = ConvertSwitchValue(test.Value);

                    // Only add each key once. If it appears twice, it's
                    // allowed, but can't be reached.
                    if (uniqueKeys.Add(key))
                    {
                        keys.Add(new SwitchLabel(key, test.Value, labels[i]));
                    }
                }
            }

            // Sort the keys, and group them into buckets.
            keys.Sort((x, y) => Math.Sign(x.Key - y.Key));
            var buckets = new List<List<SwitchLabel>>();
            foreach (var key in keys)
            {
                AddToBuckets(buckets, key);
            }

            // Emit the switchValue
            var value = GetLocal(node.SwitchValue.Type);
            EmitExpression(node.SwitchValue);
            IL.Emit(OpCodes.Stloc, value);

            // Create end label, and default label if needed
            var end = IL.DefineLabel();
            var @default = node.DefaultBody == null ? end : IL.DefineLabel();

            // Emit the switch
            var info = new SwitchInfo(node, value, @default);
            EmitSwitchBuckets(info, buckets, 0, buckets.Count - 1);

            // Emit the case bodies and default
            EmitSwitchCases(node, labels, isGoto, @default, end, flags);

            FreeLocal(value);
            return true;
        }

        private sealed class SwitchInfo
        {
            internal readonly Label Default;
            internal readonly bool Is64BitSwitch;
            internal readonly bool IsUnsigned;
            internal readonly LocalBuilder Value;

            internal SwitchInfo(SwitchExpression node, LocalBuilder value, Label @default)
            {
                Value = value;
                Default = @default;
                var type = node.SwitchValue.Type;
                IsUnsigned = type.IsUnsigned();
                var code = Type.GetTypeCode(type);
                Is64BitSwitch = code == TypeCode.UInt64 || code == TypeCode.Int64;
            }
        }

        private sealed class SwitchLabel
        {
            // Boxed version of Key, preserving the original type.
            internal readonly object Constant;

            internal readonly decimal Key;
            internal readonly Label Label;

            internal SwitchLabel(decimal key, object constant, Label label)
            {
                Key = key;
                Constant = constant;
                Label = label;
            }
        }
    }
}

#endif