#if LESSTHAN_NET35

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CC0091 // Use static method
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable AssignNullToNotNullAttribute

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using Theraot;
using Theraot.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        internal void EmitExpression(Expression node)
        {
            EmitExpression(node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitExpressionStart);
        }

        private static Type GetMemberType(MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo memberFieldInfo:
                    return memberFieldInfo.FieldType;

                case PropertyInfo memberPropertyInfo:
                    return memberPropertyInfo.PropertyType;

                default:
                    Debug.Assert(member is FieldInfo || member is PropertyInfo);
                    throw new ArgumentException(string.Empty, nameof(member));
            }
        }

        private static bool MethodHasByRefParameter(MethodInfo mi)
        {
            return mi.GetParameters().Any(pi => pi.IsByRefParameterInternal());
        }

        private static CompilationFlags UpdateEmitAsTailCallFlag(CompilationFlags flags, CompilationFlags newValue)
        {
            Debug.Assert(newValue == CompilationFlags.EmitAsTail || newValue == CompilationFlags.EmitAsMiddle || newValue == CompilationFlags.EmitAsNoTail);
            var oldValue = flags & CompilationFlags.EmitAsTailCallMask;
            return (flags ^ oldValue) | newValue;
        }

        private static CompilationFlags UpdateEmitAsTypeFlag(CompilationFlags flags, CompilationFlags newValue)
        {
            Debug.Assert(newValue == CompilationFlags.EmitAsDefaultType || newValue == CompilationFlags.EmitAsVoidType);
            var oldValue = flags & CompilationFlags.EmitAsTypeMask;
            return (flags ^ oldValue) | newValue;
        }

        private static CompilationFlags UpdateEmitExpressionStartFlag(CompilationFlags flags, CompilationFlags newValue)
        {
            Debug.Assert(newValue == CompilationFlags.EmitExpressionStart || newValue == CompilationFlags.EmitNoExpressionStart);
            var oldValue = flags & CompilationFlags.EmitExpressionStartMask;
            return (flags ^ oldValue) | newValue;
        }

        private static bool UseVirtual(MethodInfo mi)
        {
            // There are two factors: is the method static, virtual or non-virtual instance?
            // And is the object ref or value?
            // The cases are:
            //
            // static, ref:     call
            // static, value:   call
            // virtual, ref:    callvirt
            // virtual, value:  call -- e.g. double.ToString must be a non-virtual call to be verifiable.
            // instance, ref:   callvirt -- this looks wrong, but is verifiable and gives us a free null check.
            // instance, value: call
            //
            // We never need to generate a non-virtual call to a virtual method on a reference type because
            // expression trees do not support "base.Foo()" style calling.
            //
            // We could do an optimization here for the case where we know that the object is a non-null
            // reference type and the method is a non-virtual instance method.  For example, if we had
            // (new Foo()).Bar() for instance method Bar we don't need the null check so we could do a
            // call rather than a callvirt.  However that seems like it would not be a very big win for
            // most dynamically generated code scenarios, so let's not do that for now.

            if (mi.IsStatic)
            {
                return false;
            }

            return mi.DeclaringType?.IsValueType == false;
        }

        private List<WriteBack>? EmitArguments(MethodBase method, IArgumentProvider args, int skipParameters = 0)
        {
            var pis = method.GetParameters();
            Debug.Assert(args.ArgumentCount + skipParameters == pis.Length);

            List<WriteBack>? writeBacks = null;
            for (int i = skipParameters, n = pis.Length; i < n; i++)
            {
                var parameter = pis[i];
                var argument = args.GetArgument(i - skipParameters);
                var type = parameter.ParameterType;

                if (type.IsByRef)
                {
                    type = type.GetElementType();

                    var wb = EmitAddressWriteBack(argument, type);
                    if (wb != null)
                    {
                        (writeBacks ??= new List<WriteBack>()).Add(wb);
                    }
                }
                else
                {
                    EmitExpression(argument);
                }
            }

            return writeBacks;
        }

        private void EmitAssign(AssignBinaryExpression node, CompilationFlags emitAs)
        {
            switch (node.Left.NodeType)
            {
                case ExpressionType.Index:
                    EmitIndexAssignment(node, emitAs);
                    return;

                case ExpressionType.MemberAccess:
                    EmitMemberAssignment(node, emitAs);
                    return;

                case ExpressionType.Parameter:
                    EmitVariableAssignment(node, emitAs);
                    return;

                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private void EmitAssignBinaryExpression(Expression expr)
        {
            EmitAssign((AssignBinaryExpression)expr, CompilationFlags.EmitAsDefaultType);
        }

        private void EmitBinding(MemberBinding binding, Type objectType)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    EmitMemberAssignment((MemberAssignment)binding, objectType);
                    break;

                case MemberBindingType.ListBinding:
                    EmitMemberListBinding((MemberListBinding)binding);
                    break;

                case MemberBindingType.MemberBinding:
                    EmitMemberMemberBinding((MemberMemberBinding)binding);
                    break;

                default:
                    break;
            }
        }

        private void EmitCall(Type? objectType, MethodInfo method)
        {
            if (method.CallingConvention == CallingConventions.VarArgs)
            {
                throw new InvalidOperationException($"Unexpected VarArgs call to method '{method}'");
            }

            var callOp = UseVirtual(method) ? OpCodes.Callvirt : OpCodes.Call;
            if (callOp == OpCodes.Callvirt && objectType?.IsValueType == true)
            {
                IL.Emit(OpCodes.Constrained, objectType);
            }

            IL.Emit(callOp, method);
        }

        private void EmitConstant(object? value)
        {
            EmitConstant(value, value == null ? typeof(object) : value.GetType());
        }

        private void EmitConstant(object? value, Type type)
        {
            // Try to emit the constant directly into IL
            if (!IL.TryEmitConstant(value, type, this))
            {
                _boundConstants.EmitConstant(this, value, type);
            }
        }

        private void EmitConstantExpression(Expression expr)
        {
            var node = (ConstantExpression)expr;

            EmitConstant(node.Value, node.Type);
        }

        private void EmitDebugInfoExpression(Expression expr)
        {
            No.Op(expr);
        }

        private void EmitDynamicExpression(Expression expr)
        {
            if (!(_method is DynamicMethod))
            {
                throw new NotSupportedException("Dynamic expressions are not supported by CompileToMethod. Instead, create an expression tree that uses System.Runtime.CompilerServices.CallSite.");
            }

            var node = (IDynamicExpression)expr;

            var site = node.CreateCallSite();
            var siteType = site.GetType();

            var invoke = node.DelegateType.GetInvokeMethod();

            // site.Target.Invoke(site, args)
            EmitConstant(site, siteType);

            // Emit the temp as type CallSite so we get more reuse
            IL.Emit(OpCodes.Dup);
            var siteTemp = GetLocal(siteType);
            IL.Emit(OpCodes.Stloc, siteTemp);
            IL.Emit(OpCodes.Ldfld, siteType.GetField("Target"));
            IL.Emit(OpCodes.Ldloc, siteTemp);
            FreeLocal(siteTemp);

            var wb = EmitArguments(invoke, node, 1);
            IL.Emit(OpCodes.Callvirt, invoke);
            EmitWriteBack(wb);
        }

        private void EmitExpressionAsType(Expression node, Type type, CompilationFlags flags)
        {
            if (type == typeof(void))
            {
                EmitExpressionAsVoid(node, flags);
            }
            else
            {
                // if the node is emitted as a different type, CastClass IL is emitted at the end,
                // should not emit with tail calls.
                if (!TypeUtils.AreEquivalent(node.Type, type))
                {
                    EmitExpression(node);
                    Debug.Assert(type.IsReferenceAssignableFromInternal(node.Type));
                    IL.Emit(OpCodes.Castclass, type);
                }
                else
                {
                    // emit the node with the flags and emit expression start
                    EmitExpression(node, UpdateEmitExpressionStartFlag(flags, CompilationFlags.EmitExpressionStart));
                }
            }
        }

        private void EmitExpressionAsVoid(Expression node, CompilationFlags flags = CompilationFlags.EmitAsNoTail)
        {
            var labelScopeChangeInfo = GetLabelScopeChangeInfo(true, _labelBlock, node);
            if (labelScopeChangeInfo.HasValue)
            {
                _labelBlock = new LabelScopeInfo(labelScopeChangeInfo.Value.parent, labelScopeChangeInfo.Value.kind);
                DefineBlockLabels(labelScopeChangeInfo.Value.nodes);
            }

            switch (node.NodeType)
            {
                case ExpressionType.Assign:
                    EmitAssign((AssignBinaryExpression)node, CompilationFlags.EmitAsVoidType);
                    break;

                case ExpressionType.Block:
                    Emit((BlockExpression)node, UpdateEmitAsTypeFlag(flags, CompilationFlags.EmitAsVoidType));
                    break;

                case ExpressionType.Throw:
                    EmitThrow((UnaryExpression)node, CompilationFlags.EmitAsVoidType);
                    break;

                case ExpressionType.Goto:
                    EmitGotoExpression(node, UpdateEmitAsTypeFlag(flags, CompilationFlags.EmitAsVoidType));
                    break;

                case ExpressionType.Constant:
                case ExpressionType.Default:
                case ExpressionType.Parameter:
                    // no-op
                    break;

                default:
                    if (node.Type == typeof(void))
                    {
                        EmitExpression(node, UpdateEmitExpressionStartFlag(flags, CompilationFlags.EmitNoExpressionStart));
                    }
                    else
                    {
                        EmitExpression(node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
                        IL.Emit(OpCodes.Pop);
                    }

                    break;
            }

            if (labelScopeChangeInfo.HasValue)
            {
                _labelBlock = labelScopeChangeInfo.Value.parent;
            }
        }

        private void EmitGetArrayElement(Type arrayType)
        {
            if (arrayType.IsSafeArray())
            {
                // For one dimensional arrays, emit load
                IL.EmitLoadElement(arrayType.GetElementType());
            }
            else
            {
                // Multidimensional arrays, call get
                IL.Emit(OpCodes.Call, arrayType.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance));
            }
        }

        private void EmitGetIndexCall(IndexExpression node, Type? objectType)
        {
            if (node.Indexer != null)
            {
                // For indexed properties, just call the getter
                var method = node.Indexer.GetGetMethod(true);
                EmitCall(objectType, method);
            }
            else
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException(nameof(objectType));
                }
                EmitGetArrayElement(objectType);
            }
        }

        private void EmitIndexAssignment(AssignBinaryExpression node, CompilationFlags flags)
        {
            Debug.Assert(!node.IsByRef);

            var index = (IndexExpression)node.Left;

            var emitAs = flags & CompilationFlags.EmitAsTypeMask;

            // Emit instance, if calling an instance method
            Type? objectType = null;
            if (index.Object != null)
            {
                EmitInstance(index.Object, out objectType);
            }

            // Emit indexes. We don't allow byref args, so no need to worry
            // about writebacks or EmitAddress
            for (int i = 0, n = index.ArgumentCount; i < n; i++)
            {
                var arg = index.GetArgument(i);
                EmitExpression(arg);
            }

            // Emit value
            EmitExpression(node.Right);

            // Save the expression value, if needed
            LocalBuilder? temp = null;
            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                IL.Emit(OpCodes.Dup);
                IL.Emit(OpCodes.Stloc, temp = GetLocal(node.Type));
            }

            EmitSetIndexCall(index, objectType);

            // Restore the value
            if (temp == null)
            {
                return;
            }

            IL.Emit(OpCodes.Ldloc, temp);
            FreeLocal(temp);
        }

        private void EmitIndexExpression(Expression expr)
        {
            var node = (IndexExpression)expr;

            // Emit instance, if calling an instance method
            Type? objectType = null;
            if (node.Object != null)
            {
                EmitInstance(node.Object, out objectType);
            }

            // Emit indexes. We don't allow byref args, so no need to worry
            // about writebacks or EmitAddress
            for (int i = 0, n = node.ArgumentCount; i < n; i++)
            {
                var arg = node.GetArgument(i);
                EmitExpression(arg);
            }

            EmitGetIndexCall(node, objectType);
        }

        private void EmitInlinedInvoke(InvocationExpression invoke, CompilationFlags flags)
        {
            var lambda = invoke.LambdaOperand;

            // This is tricky: we need to emit the arguments outside of the
            // scope, but set them inside the scope. Fortunately, using the IL
            // stack it is entirely doable.

            // 1. Emit invoke arguments
            var wb = EmitArguments(lambda.Type.GetInvokeMethod(), invoke);

            // 2. Create the nested LambdaCompiler
            var inner = new LambdaCompiler(this, lambda, invoke);

            // 3. Emit the body
            // if the inlined lambda is the last expression of the whole lambda,
            // tail call can be applied.
            if (wb != null)
            {
                Debug.Assert(wb.Count > 0);
                flags = UpdateEmitAsTailCallFlag(flags, CompilationFlags.EmitAsNoTail);
            }

            inner.EmitLambdaBody(_scope, true, flags);

            // 4. Emit writebacks if needed
            EmitWriteBack(wb);
        }

        private void EmitInstance(Expression instance, [NotNull] out Type type)
        {
            type = instance.Type;

            // NB: Instance can be a ByRef type due to stack spilling introducing ref locals for
            //     accessing an instance of a value type. In that case, we don't have to take the
            //     address of the instance anymore; we just load the ref local.

            if (type.IsByRef)
            {
                type = type.GetElementType();

                Debug.Assert(instance.NodeType == ExpressionType.Parameter);

                EmitExpression(instance);
            }
            else if (type.IsValueType)
            {
                EmitAddress(instance, type);
            }
            else
            {
                EmitExpression(instance);
            }
        }

        private void EmitInvocationExpression(Expression expr, CompilationFlags flags)
        {
            var node = (InvocationExpression)expr;

            // Optimization: inline code for literal lambda's directly
            //
            // This is worth it because otherwise we end up with an extra call
            // to DynamicMethod.CreateDelegate, which is expensive.
            //
            if (node.LambdaOperand != null)
            {
                EmitInlinedInvoke(node, flags);
                return;
            }

            expr = node.Expression;
            Debug.Assert(!typeof(LambdaExpression).IsAssignableFrom(expr.Type));
            EmitMethodCall(expr, expr.Type.GetInvokeMethod(), node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitExpressionStart);
        }

        private void EmitLambdaExpression(Expression expr)
        {
            var node = (LambdaExpression)expr;
            EmitDelegateConstruction(node);
        }

        private void EmitLift(ExpressionType nodeType, Type resultType, MethodCallExpression mc, ParameterExpression[] paramList, Expression[] argList)
        {
            Debug.Assert(TypeUtils.AreEquivalent(resultType.GetNonNullable(), mc.Type.GetNonNullable()));

            switch (nodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    {
                        if (TypeUtils.AreEquivalent(resultType, mc.Type.GetNullable()))
                        {
                            goto default;
                        }

                        var exit = IL.DefineLabel();
                        var exitAllNull = IL.DefineLabel();
                        var exitAnyNull = IL.DefineLabel();

                        var anyNull = GetLocal(typeof(bool));
                        var allNull = GetLocal(typeof(bool));
                        IL.Emit(OpCodes.Ldc_I4_0);
                        IL.Emit(OpCodes.Stloc, anyNull);
                        IL.Emit(OpCodes.Ldc_I4_1);
                        IL.Emit(OpCodes.Stloc, allNull);

                        for (int i = 0, n = paramList.Length; i < n; i++)
                        {
                            var v = paramList[i];
                            var arg = argList[i];
                            _scope.AddLocal(this, v);
                            if (arg.Type.IsNullable())
                            {
                                EmitAddress(arg, arg.Type);
                                IL.Emit(OpCodes.Dup);
                                IL.EmitHasValue(arg.Type);
                                IL.Emit(OpCodes.Ldc_I4_0);
                                IL.Emit(OpCodes.Ceq);
                                IL.Emit(OpCodes.Dup);
                                IL.Emit(OpCodes.Ldloc, anyNull);
                                IL.Emit(OpCodes.Or);
                                IL.Emit(OpCodes.Stloc, anyNull);
                                IL.Emit(OpCodes.Ldloc, allNull);
                                IL.Emit(OpCodes.And);
                                IL.Emit(OpCodes.Stloc, allNull);
                                IL.EmitGetValueOrDefault(arg.Type);
                            }
                            else
                            {
                                EmitExpression(arg);
                                if (!arg.Type.IsValueType)
                                {
                                    IL.Emit(OpCodes.Dup);
                                    IL.Emit(OpCodes.Ldnull);
                                    IL.Emit(OpCodes.Ceq);
                                    IL.Emit(OpCodes.Dup);
                                    IL.Emit(OpCodes.Ldloc, anyNull);
                                    IL.Emit(OpCodes.Or);
                                    IL.Emit(OpCodes.Stloc, anyNull);
                                    IL.Emit(OpCodes.Ldloc, allNull);
                                    IL.Emit(OpCodes.And);
                                    IL.Emit(OpCodes.Stloc, allNull);
                                }
                                else
                                {
                                    IL.Emit(OpCodes.Ldc_I4_0);
                                    IL.Emit(OpCodes.Stloc, allNull);
                                }
                            }

                            _scope.EmitSet(v);
                        }

                        IL.Emit(OpCodes.Ldloc, allNull);
                        IL.Emit(OpCodes.Brtrue, exitAllNull);
                        IL.Emit(OpCodes.Ldloc, anyNull);
                        IL.Emit(OpCodes.Brtrue, exitAnyNull);

                        EmitMethodCallExpression(mc);
                        if (resultType.IsNullable() && !TypeUtils.AreEquivalent(resultType, mc.Type))
                        {
                            var ci = resultType.GetConstructor(new[] { mc.Type });
                            IL.Emit(OpCodes.Newobj, ci);
                        }

                        IL.Emit(OpCodes.Br_S, exit);

                        IL.MarkLabel(exitAllNull);
                        IL.EmitPrimitive(nodeType == ExpressionType.Equal);
                        IL.Emit(OpCodes.Br_S, exit);

                        IL.MarkLabel(exitAnyNull);
                        IL.EmitPrimitive(nodeType == ExpressionType.NotEqual);

                        IL.MarkLabel(exit);
                        FreeLocal(anyNull);
                        FreeLocal(allNull);
                        return;
                    }
                default:
                    {
                        var exit = IL.DefineLabel();
                        var exitNull = IL.DefineLabel();
                        var anyNull = GetLocal(typeof(bool));
                        for (int i = 0, n = paramList.Length; i < n; i++)
                        {
                            var v = paramList[i];
                            var arg = argList[i];
                            if (arg.Type.IsNullable())
                            {
                                _scope.AddLocal(this, v);
                                EmitAddress(arg, arg.Type);
                                IL.Emit(OpCodes.Dup);
                                IL.EmitHasValue(arg.Type);
                                IL.Emit(OpCodes.Ldc_I4_0);
                                IL.Emit(OpCodes.Ceq);
                                IL.Emit(OpCodes.Stloc, anyNull);
                                IL.EmitGetValueOrDefault(arg.Type);
                                _scope.EmitSet(v);
                            }
                            else
                            {
                                _scope.AddLocal(this, v);
                                EmitExpression(arg);
                                if (!arg.Type.IsValueType)
                                {
                                    IL.Emit(OpCodes.Dup);
                                    IL.Emit(OpCodes.Ldnull);
                                    IL.Emit(OpCodes.Ceq);
                                    IL.Emit(OpCodes.Stloc, anyNull);
                                }

                                _scope.EmitSet(v);
                            }

                            IL.Emit(OpCodes.Ldloc, anyNull);
                            IL.Emit(OpCodes.Brtrue, exitNull);
                        }

                        EmitMethodCallExpression(mc);
                        if (resultType.IsNullable() && !TypeUtils.AreEquivalent(resultType, mc.Type))
                        {
                            var ci = resultType.GetConstructor(new[] { mc.Type });
                            IL.Emit(OpCodes.Newobj, ci);
                        }

                        IL.Emit(OpCodes.Br_S, exit);
                        IL.MarkLabel(exitNull);
                        if (TypeUtils.AreEquivalent(resultType, mc.Type.GetNullable()))
                        {
                            if (resultType.IsValueType)
                            {
                                var result = GetLocal(resultType);
                                IL.Emit(OpCodes.Ldloca, result);
                                IL.Emit(OpCodes.Initobj, resultType);
                                IL.Emit(OpCodes.Ldloc, result);
                                FreeLocal(result);
                            }
                            else
                            {
                                IL.Emit(OpCodes.Ldnull);
                            }
                        }
                        else
                        {
                            Debug.Assert
                            (
                                nodeType == ExpressionType.LessThan
                                || nodeType == ExpressionType.LessThanOrEqual
                                || nodeType == ExpressionType.GreaterThan
                                || nodeType == ExpressionType.GreaterThanOrEqual
                            );

                            IL.Emit(OpCodes.Ldc_I4_0);
                        }

                        IL.MarkLabel(exit);
                        FreeLocal(anyNull);
                        return;
                    }
            }
        }

        private void EmitListInit(ListInitExpression init)
        {
            EmitExpression(init.NewExpression);
            LocalBuilder? loc = null;
            if (init.NewExpression.Type.IsValueType)
            {
                loc = GetLocal(init.NewExpression.Type);
                IL.Emit(OpCodes.Stloc, loc);
                IL.Emit(OpCodes.Ldloca, loc);
            }

            EmitListInit(init.Initializers, loc == null, init.NewExpression.Type);
            if (loc == null)
            {
                return;
            }

            IL.Emit(OpCodes.Ldloc, loc);
            FreeLocal(loc);
        }

        // This method assumes that the list instance is on the stack and is expected, based on "keepOnStack" flag
        // to either leave the list instance on the stack, or pop it.
        private void EmitListInit(ReadOnlyCollection<ElementInit> initializers, bool keepOnStack, Type objectType)
        {
            var n = initializers.Count;

            if (n == 0)
            {
                // If there are no initializers and instance is not to be kept on the stack, we must pop explicitly.
                if (!keepOnStack)
                {
                    IL.Emit(OpCodes.Pop);
                }
            }
            else
            {
                for (var i = 0; i < n; i++)
                {
                    if (keepOnStack || i < n - 1)
                    {
                        IL.Emit(OpCodes.Dup);
                    }

                    EmitMethodCall(initializers[i].AddMethod, initializers[i], objectType);

                    // Some add methods, ArrayList.Add for example, return non-void
                    if (initializers[i].AddMethod.ReturnType != typeof(void))
                    {
                        IL.Emit(OpCodes.Pop);
                    }
                }
            }
        }

        private void EmitListInitExpression(Expression expr)
        {
            EmitListInit((ListInitExpression)expr);
        }

        private void EmitMemberAssignment(AssignBinaryExpression node, CompilationFlags flags)
        {
            Debug.Assert(!node.IsByRef);

            var lvalue = (MemberExpression)node.Left;
            var member = lvalue.Member;

            // emit "this", if any
            Type? objectType = null;
            if (lvalue.Expression != null)
            {
                EmitInstance(lvalue.Expression, out objectType);
            }

            // emit value
            EmitExpression(node.Right);

            LocalBuilder? temp = null;

            var emitAs = flags & CompilationFlags.EmitAsTypeMask;
            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                // save the value so we can return it
                IL.Emit(OpCodes.Dup);
                temp = GetLocal(node.Type);
                IL.Emit(OpCodes.Stloc, temp);
            }

            if (member is FieldInfo info)
            {
                IL.EmitFieldSet(info);
            }
            else
            {
                // MemberExpression.Member can only be a FieldInfo or a PropertyInfo
                Debug.Assert(member is PropertyInfo);
                var prop = (PropertyInfo)member;
                EmitCall(objectType, prop.GetSetMethod(true));
            }

            if (temp == null)
            {
                return;
            }

            IL.Emit(OpCodes.Ldloc, temp);
            FreeLocal(temp);
        }

        private void EmitMemberAssignment(MemberAssignment binding, Type objectType)
        {
            EmitExpression(binding.Expression);
            if (binding.Member is FieldInfo fi)
            {
                IL.Emit(OpCodes.Stfld, fi);
            }
            else
            {
                if (!(binding.Member is PropertyInfo propertyInfo))
                {
                    throw new ArgumentException(string.Empty, nameof(binding));
                }

                EmitCall(objectType, propertyInfo.GetSetMethod(true));
            }
        }

        private void EmitMemberExpression(Expression expr)
        {
            var node = (MemberExpression)expr;

            // emit "this", if any
            Type? instanceType = null;
            if (node.Expression != null)
            {
                EmitInstance(node.Expression, out instanceType);
            }

            EmitMemberGet(node.Member, instanceType);
        }

        // assumes instance is already on the stack
        private void EmitMemberGet(MemberInfo member, Type? objectType)
        {
            if (member is FieldInfo fi)
            {
                if (fi.IsLiteral)
                {
                    EmitConstant(fi.GetRawConstantValue(), fi.FieldType);
                }
                else
                {
                    IL.EmitFieldGet(fi);
                }
            }
            else
            {
                // MemberExpression.Member or MemberBinding.Member can only be a FieldInfo or a PropertyInfo
                Debug.Assert(member is PropertyInfo);
                var prop = (PropertyInfo)member;
                EmitCall(objectType, prop.GetGetMethod(true));
            }
        }

        private void EmitMemberInit(MemberInitExpression init)
        {
            EmitExpression(init.NewExpression);
            LocalBuilder? loc = null;
            if (init.NewExpression.Type.IsValueType && init.Bindings.Count > 0)
            {
                loc = GetLocal(init.NewExpression.Type);
                IL.Emit(OpCodes.Stloc, loc);
                IL.Emit(OpCodes.Ldloca, loc);
            }

            EmitMemberInit(init.Bindings, loc == null, init.NewExpression.Type);
            if (loc == null)
            {
                return;
            }

            IL.Emit(OpCodes.Ldloc, loc);
            FreeLocal(loc);
        }

        // This method assumes that the instance is on the stack and is expected, based on "keepOnStack" flag
        // to either leave the instance on the stack, or pop it.
        private void EmitMemberInit(ReadOnlyCollection<MemberBinding> bindings, bool keepOnStack, Type objectType)
        {
            var n = bindings.Count;
            if (n == 0)
            {
                // If there are no initializers and instance is not to be kept on the stack, we must pop explicitly.
                if (!keepOnStack)
                {
                    IL.Emit(OpCodes.Pop);
                }
            }
            else
            {
                for (var i = 0; i < n; i++)
                {
                    if (keepOnStack || i < n - 1)
                    {
                        IL.Emit(OpCodes.Dup);
                    }

                    EmitBinding(bindings[i], objectType);
                }
            }
        }

        private void EmitMemberInitExpression(Expression expr)
        {
            EmitMemberInit((MemberInitExpression)expr);
        }

        private void EmitMemberListBinding(MemberListBinding binding)
        {
            var type = GetMemberType(binding.Member);
            if (binding.Member is PropertyInfo && type.IsValueType)
            {
                throw new InvalidOperationException($"Cannot auto initialize elements of value type through property '{binding.Member}', use assignment instead");
            }

            if (type.IsValueType)
            {
                EmitMemberAddress(binding.Member, binding.Member.DeclaringType);
            }
            else
            {
                EmitMemberGet(binding.Member, binding.Member.DeclaringType);
            }

            EmitListInit(binding.Initializers, false, type);
        }

        private void EmitMemberMemberBinding(MemberMemberBinding binding)
        {
            var type = GetMemberType(binding.Member);
            if (binding.Member is PropertyInfo && type.IsValueType)
            {
                throw new InvalidOperationException($"Cannot auto initialize members of value type through property '{binding.Member}', use assignment instead");
            }

            if (type.IsValueType)
            {
                EmitMemberAddress(binding.Member, binding.Member.DeclaringType);
            }
            else
            {
                EmitMemberGet(binding.Member, binding.Member.DeclaringType);
            }

            EmitMemberInit(binding.Bindings, false, type);
        }

        private void EmitMethodCall(Expression? obj, MethodInfo method, IArgumentProvider methodCallExpr, CompilationFlags flags = CompilationFlags.EmitAsNoTail)
        {
            // Emit instance, if calling an instance method
            Type? objectType = null;
            if (!method.IsStatic)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(nameof(obj));
                }
                EmitInstance(obj, out objectType);
            }

            // if the obj has a value type, its address is passed to the method call so we cannot destroy the
            // stack by emitting a tail call
            if (obj?.Type.IsValueType == true)
            {
                EmitMethodCall(method, methodCallExpr, objectType);
            }
            else
            {
                EmitMethodCall(method, methodCallExpr, objectType, flags);
            }
        }

        private void EmitMethodCall(MethodInfo mi, IArgumentProvider args, Type? objectType, CompilationFlags flags = CompilationFlags.EmitAsNoTail)
        {
            // Emit arguments
            var wb = EmitArguments(mi, args);

            // Emit the actual call
            var callOp = UseVirtual(mi) ? OpCodes.Callvirt : OpCodes.Call;
            if (callOp == OpCodes.Callvirt && objectType?.IsValueType == true)
            {
                // This automatically boxes value types if necessary.
                IL.Emit(OpCodes.Constrained, objectType);
            }

            // The method call can be a tail call if
            // 1) the method call is the last instruction before Ret
            // 2) the method does not have any ByRef parameters, refer to ECMA-335 Partition III Section 2.4.
            //    "Verification requires that no managed pointers are passed to the method being called, since
            //    it does not track pointers into the current frame."
            if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail && !MethodHasByRefParameter(mi))
            {
                IL.Emit(OpCodes.Tailcall);
            }

            if (mi.CallingConvention == CallingConventions.VarArgs)
            {
                var methodParametersTypes = mi.GetParameterTypes();
                var count = args.ArgumentCount;
                var types = new Type[count];
                for (var i = 0; i < count; i++)
                {
                    types[i] = args.GetArgument(i)?.Type ?? methodParametersTypes[i];
                }

                IL.EmitCall(callOp, mi, types);
            }
            else
            {
                IL.Emit(callOp, mi);
            }

            // Emit writebacks for properties passed as "ref" arguments
            EmitWriteBack(wb);
        }

        private void EmitMethodCallExpression(Expression expr, CompilationFlags flags = CompilationFlags.EmitAsNoTail)
        {
            var node = (MethodCallExpression)expr;

            EmitMethodCall(node.Object, node.Method, node, flags);
        }

        private void EmitNewArrayExpression(Expression expr)
        {
            var node = (NewArrayExpression)expr;

            var expressions = node.Expressions;
            var n = expressions.Count;

            if (node.NodeType == ExpressionType.NewArrayInit)
            {
                var elementType = node.Type.GetElementType();

                IL.EmitArray(elementType, n);

                for (var i = 0; i < n; i++)
                {
                    IL.Emit(OpCodes.Dup);
                    IL.EmitPrimitive(i);
                    EmitExpression(expressions[i]);
                    IL.EmitStoreElement(elementType);
                }
            }
            else
            {
                for (var i = 0; i < n; i++)
                {
                    var x = expressions[i];
                    EmitExpression(x);
                    IL.EmitConvertToType(x.Type, typeof(int), true, this);
                }

                IL.EmitArray(node.Type);
            }
        }

        private void EmitNewExpression(Expression expr)
        {
            var node = (NewExpression)expr;

            if (node.Constructor != null)
            {
                if (node.Constructor.DeclaringType?.IsAbstract == true)
                {
                    throw new InvalidOperationException("Can't compile a NewExpression with a constructor declared on an abstract class");
                }

                var wb = EmitArguments(node.Constructor, node);
                IL.Emit(OpCodes.Newobj, node.Constructor);
                EmitWriteBack(wb);
            }
            else
            {
                Debug.Assert(node.ArgumentCount == 0, "Node with arguments must have a constructor.");
                Debug.Assert(node.Type.IsValueType, "Only value type may have constructor not set.");
                var temp = GetLocal(node.Type);
                IL.Emit(OpCodes.Ldloca, temp);
                IL.Emit(OpCodes.Initobj, node.Type);
                IL.Emit(OpCodes.Ldloc, temp);
                FreeLocal(temp);
            }
        }

        private void EmitParameterExpression(Expression expr)
        {
            var node = (ParameterExpression)expr;
            _scope.EmitGet(node);
            if (node.IsByRef)
            {
                IL.EmitLoadValueIndirect(node.Type);
            }
        }

        private void EmitRuntimeVariablesExpression(Expression expr)
        {
            var node = (RuntimeVariablesExpression)expr;
            _scope.EmitVariableAccess(this, node.Variables);
        }

        private void EmitSetArrayElement(Type arrayType)
        {
            if (arrayType.IsSafeArray())
            {
                // For one dimensional arrays, emit store
                IL.EmitStoreElement(arrayType.GetElementType());
            }
            else
            {
                // Multidimensional arrays, call set
                IL.Emit(OpCodes.Call, arrayType.GetMethod("Set", BindingFlags.Public | BindingFlags.Instance));
            }
        }

        private void EmitSetIndexCall(IndexExpression node, Type? objectType)
        {
            if (node.Indexer != null)
            {
                // For indexed properties, just call the setter
                var method = node.Indexer.GetSetMethod(true);
                EmitCall(objectType, method);
            }
            else
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException(nameof(objectType));
                }
                EmitSetArrayElement(objectType);
            }
        }

        private void EmitTypeBinaryExpression(Expression expr)
        {
            var node = (TypeBinaryExpression)expr;

            if (node.NodeType == ExpressionType.TypeEqual)
            {
                EmitExpression(node.ReduceTypeEqual());
                return;
            }

            var type = node.Expression.Type;

            // Try to determine the result statically
            var result = ConstantCheck.AnalyzeTypeIs(node);

            switch (result)
            {
                case AnalyzeTypeIsResult.KnownTrue:
                case AnalyzeTypeIsResult.KnownFalse:
                    // Result is known statically, so just emit the expression for
                    // its side effects and return the result
                    EmitExpressionAsVoid(node.Expression);
                    IL.EmitPrimitive(result == AnalyzeTypeIsResult.KnownTrue);
                    return;

                case AnalyzeTypeIsResult.KnownAssignable when type.IsNullable():
                    EmitAddress(node.Expression, type);
                    IL.EmitHasValue(type);
                    return;

                case AnalyzeTypeIsResult.KnownAssignable:
                    Debug.Assert(!type.IsValueType);
                    EmitExpression(node.Expression);
                    IL.Emit(OpCodes.Ldnull);
                    IL.Emit(OpCodes.Cgt_Un);
                    return;

                default:
                    break;
            }

            Debug.Assert(result == AnalyzeTypeIsResult.Unknown);

            // Emit a full runtime "isinst" check
            EmitExpression(node.Expression);
            if (type.IsValueType)
            {
                IL.Emit(OpCodes.Box, type);
            }

            IL.Emit(OpCodes.Isinst, node.TypeOperand);
            IL.Emit(OpCodes.Ldnull);
            IL.Emit(OpCodes.Cgt_Un);
        }

        private void EmitVariableAssignment(AssignBinaryExpression node, CompilationFlags flags)
        {
            var variable = (ParameterExpression)node.Left;
            var emitAs = flags & CompilationFlags.EmitAsTypeMask;

            if (node.IsByRef)
            {
                EmitAddress(node.Right, node.Right.Type);
            }
            else
            {
                EmitExpression(node.Right);
            }

            if (emitAs != CompilationFlags.EmitAsVoidType)
            {
                IL.Emit(OpCodes.Dup);
            }

            if (variable.IsByRef)
            {
                // Note: the stloc/ldloc pattern is a bit suboptimal, but it
                // saves us from having to spill stack when assigning to a
                // byref parameter. We already make this same trade-off for
                // hoisted variables, see ElementStorage.EmitStore

                var value = GetLocal(variable.Type);
                IL.Emit(OpCodes.Stloc, value);
                _scope.EmitGet(variable);
                IL.Emit(OpCodes.Ldloc, value);
                FreeLocal(value);
                IL.EmitStoreValueIndirect(variable.Type);
            }
            else
            {
                _scope.EmitSet(variable);
            }
        }

        private void EmitWriteBack(List<WriteBack>? writeBacks)
        {
            if (writeBacks == null)
            {
                return;
            }

            foreach (var wb in writeBacks)
            {
                wb(this);
            }
        }
    }
}

#endif