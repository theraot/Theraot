#if LESSTHAN_NET35
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        private void AddressOf(LabelScopeInfo labelBlock, BinaryExpression node, Type type)
        {
            Debug.Assert(node.NodeType == ExpressionType.ArrayIndex && node.Method == null);

            if (TypeUtils.AreEquivalent(type, node.Type))
            {
                EmitExpression(labelBlock, node.Left);
                EmitExpression(labelBlock, node.Right);
                Debug.Assert(node.Right.Type == typeof(int));
                IL.Emit(OpCodes.Ldelema, node.Type);
            }
            else
            {
                EmitExpressionAddress(labelBlock, node, type);
            }
        }

        private void AddressOf(LabelScopeInfo labelBlock, ParameterExpression node, Type type)
        {
            if (TypeUtils.AreEquivalent(type, node.Type))
            {
                if (node.IsByRef)
                {
                    _scope.EmitGet(node);
                }
                else
                {
                    _scope.EmitAddressOf(node);
                }
            }
            else
            {
                // NB: Stack spilling can generate ref locals.

                if (node.Type.IsByRef && node.Type.GetElementType() == type)
                {
                    EmitExpression(labelBlock, node);
                }
                else
                {
                    EmitExpressionAddress(labelBlock, node, type);
                }
            }
        }

        private void AddressOf(LabelScopeInfo labelBlock, MemberExpression node, Type type)
        {
            if (TypeUtils.AreEquivalent(type, node.Type))
            {
                // emit "this", if any
                Type? objectType = null;
                if (node.Expression != null)
                {
                    EmitInstance(labelBlock, node.Expression, out objectType);
                }

                EmitMemberAddress(node.Member, objectType);
            }
            else
            {
                EmitExpressionAddress(labelBlock, node, type);
            }
        }

        private void AddressOf(LabelScopeInfo labelBlock, MethodCallExpression node, Type type)
        {
            // An array index of a multi-dimensional array is represented by a call to Array.Get,
            // rather than having its own array-access node. This means that when we are trying to
            // get the address of a member of a multi-dimensional array, we'll be trying to
            // get the address of a Get method, and it will fail to do so. Instead, detect
            // this situation and replace it with a call to the Address method.
            if
            (
                node.Object != null
                && !node.Method.IsStatic
                && node.Object.Type.IsArray
                && node.Method == node.Object.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance)
            )
            {
                var mi = node.Object.Type.GetMethod("Address", BindingFlags.Public | BindingFlags.Instance);

                EmitMethodCall(labelBlock, node.Object, mi, node);
            }
            else
            {
                EmitExpressionAddress(labelBlock, node, type);
            }
        }

        private void AddressOf(LabelScopeInfo labelBlock, IndexExpression node, Type type)
        {
            if (!TypeUtils.AreEquivalent(type, node.Type) || node.Indexer != null)
            {
                EmitExpressionAddress(labelBlock, node, type);
                return;
            }

            if (node.Object == null)
            {
                throw new ArgumentException(string.Empty, nameof(node));
            }

            if (node.ArgumentCount == 1)
            {
                EmitExpression(labelBlock, node.Object);
                EmitExpression(labelBlock, node.GetArgument(0));
                IL.Emit(OpCodes.Ldelema, node.Type);
            }
            else
            {
                var address = node.Object.Type.GetMethod("Address", BindingFlags.Public | BindingFlags.Instance);
                EmitMethodCall(labelBlock, node.Object, address, node);
            }
        }

        private void AddressOf(LabelScopeInfo labelBlock, UnaryExpression node, Type type)
        {
            Debug.Assert(node.NodeType == ExpressionType.Unbox);
            Debug.Assert(type.IsValueType);

            // Unbox leaves a pointer to the boxed value on the stack
            EmitExpression(labelBlock, node.Operand);
            IL.Emit(OpCodes.Unbox, type);
        }

        private WriteBack? AddressOfWriteBack(LabelScopeInfo labelBlock, MemberExpression node)
        {
            if (!(node.Member is PropertyInfo property) || !property.CanWrite)
            {
                return null;
            }

            return AddressOfWriteBackCore(labelBlock, node); // avoids closure allocation
        }

        private WriteBack? AddressOfWriteBack(LabelScopeInfo labelBlock, IndexExpression node)
        {
            return node.Indexer?.CanWrite != true ? null : AddressOfWriteBackCore(labelBlock, node);
        }

        private WriteBack AddressOfWriteBackCore(LabelScopeInfo labelBlock, MemberExpression node)
        {
            // emit instance, if any
            LocalBuilder? instanceLocal = null;
            Type? instanceType = null;
            if (node.Expression != null)
            {
                EmitInstance(labelBlock, node.Expression, out instanceType);

                // store in local
                IL.Emit(OpCodes.Dup);
                IL.Emit(OpCodes.Stloc, instanceLocal = GetInstanceLocal(instanceType));
            }

            var pi = (PropertyInfo)node.Member;

            // emit the get
            EmitCall(instanceType, pi.GetGetMethod(true));

            // emit the address of the value
            var valueLocal = GetLocal(node.Type);
            IL.Emit(OpCodes.Stloc, valueLocal);
            IL.Emit(OpCodes.Ldloca, valueLocal);

            // Set the property after the method call
            // don't re-evaluate anything
            return @this =>
            {
                if (instanceLocal != null)
                {
                    @this.IL.Emit(OpCodes.Ldloc, instanceLocal);
                    @this.FreeLocal(instanceLocal);
                }

                @this.IL.Emit(OpCodes.Ldloc, valueLocal);
                @this.FreeLocal(valueLocal);
                @this.EmitCall(instanceLocal?.LocalType, pi.GetSetMethod(true));
            };
        }

        private WriteBack AddressOfWriteBackCore(LabelScopeInfo labelBlock, IndexExpression node)
        {
            // emit instance, if any
            LocalBuilder? instanceLocal = null;
            Type? instanceType = null;
            if (node.Object != null)
            {
                EmitInstance(labelBlock, node.Object, out instanceType);

                // store in local
                IL.Emit(OpCodes.Dup);
                IL.Emit(OpCodes.Stloc, instanceLocal = GetInstanceLocal(instanceType));
            }

            // Emit indexes. We don't allow byref args, so no need to worry
            // about write-backs or EmitAddress
            var n = node.ArgumentCount;
            var args = new LocalBuilder[n];
            for (var i = 0; i < n; i++)
            {
                var arg = node.GetArgument(i);
                EmitExpression(labelBlock, arg);

                var argLocal = GetLocal(arg.Type);
                IL.Emit(OpCodes.Dup);
                IL.Emit(OpCodes.Stloc, argLocal);
                args[i] = argLocal;
            }

            // emit the get
            EmitGetIndexCall(node, instanceType);

            // emit the address of the value
            var valueLocal = GetLocal(node.Type);
            IL.Emit(OpCodes.Stloc, valueLocal);
            IL.Emit(OpCodes.Ldloca, valueLocal);

            // Set the property after the method call
            // don't re-evaluate anything
            return @this =>
            {
                if (instanceLocal != null)
                {
                    @this.IL.Emit(OpCodes.Ldloc, instanceLocal);
                    @this.FreeLocal(instanceLocal);
                }
                foreach (LocalBuilder arg in args)
                {
                    @this.IL.Emit(OpCodes.Ldloc, arg);
                    @this.FreeLocal(arg);
                }
                @this.IL.Emit(OpCodes.Ldloc, valueLocal);
                @this.FreeLocal(valueLocal);

                @this.EmitSetIndexCall(node, instanceLocal?.LocalType);
            };
        }

        // We don't want "ref" parameters to modify values of expressions
        // except where it would in IL: locals, args, fields, and array elements
        // (Unbox is an exception, it's intended to emit a ref to the original
        // boxed value)
        private void EmitAddress(LabelScopeInfo labelBlock, Expression node, Type type, CompilationFlags flags = CompilationFlags.EmitExpressionStart)
        {
            var emitStart = (flags & CompilationFlags.EmitExpressionStartMask) == CompilationFlags.EmitExpressionStart;
            var labelScopeChangeInfo = GetLabelScopeChangeInfo(emitStart, labelBlock, node);
            if (labelScopeChangeInfo.HasValue)
            {
                labelBlock = new LabelScopeInfo(labelScopeChangeInfo.Value.parent, labelScopeChangeInfo.Value.kind);
                DefineBlockLabels(labelBlock, labelScopeChangeInfo.Value.nodes);
            }

            switch (node.NodeType)
            {
                default:
                    EmitExpressionAddress(labelBlock, node, type);
                    break;

                case ExpressionType.ArrayIndex:
                    AddressOf(labelBlock, (BinaryExpression)node, type);
                    break;

                case ExpressionType.Parameter:
                    AddressOf(labelBlock, (ParameterExpression)node, type);
                    break;

                case ExpressionType.MemberAccess:
                    AddressOf(labelBlock, (MemberExpression)node, type);
                    break;

                case ExpressionType.Unbox:
                    AddressOf(labelBlock, (UnaryExpression)node, type);
                    break;

                case ExpressionType.Call:
                    AddressOf(labelBlock, (MethodCallExpression)node, type);
                    break;

                case ExpressionType.Index:
                    AddressOf(labelBlock, (IndexExpression)node, type);
                    break;
            }

            if (labelScopeChangeInfo.HasValue)
            {
                labelBlock = labelScopeChangeInfo.Value.parent;
            }
        }

        // Emits the address of the expression, returning the write back if necessary
        //
        // For properties, we want to write back into the property if it's
        // passed byref.
        private WriteBack? EmitAddressWriteBack(LabelScopeInfo labelBlock, Expression node, Type type)
        {
            var labelScopeChangeInfo = GetLabelScopeChangeInfo(true, labelBlock, node);
            if (labelScopeChangeInfo.HasValue)
            {
                labelBlock = new LabelScopeInfo(labelScopeChangeInfo.Value.parent, labelScopeChangeInfo.Value.kind);
                DefineBlockLabels(labelBlock, labelScopeChangeInfo.Value.nodes);
            }

            WriteBack? result = null;
            if (TypeUtils.AreEquivalent(type, node.Type))
            {
                switch (node.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        result = AddressOfWriteBack(labelBlock, (MemberExpression)node);
                        break;

                    case ExpressionType.Index:
                        result = AddressOfWriteBack(labelBlock, (IndexExpression)node);
                        break;

                    default:
                        break;
                }
            }

            if (result == null)
            {
                EmitAddress(labelBlock, node, type, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
            }

            if (labelScopeChangeInfo.HasValue)
            {
                labelBlock = labelScopeChangeInfo.Value.parent;
            }

            return result;
        }

        private void EmitExpressionAddress(LabelScopeInfo labelBlock, Expression node, Type type)
        {
            Debug.Assert(type.IsReferenceAssignableFromInternal(node.Type));
            EmitExpression(labelBlock, node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
            var tmp = GetLocal(type);
            IL.Emit(OpCodes.Stloc, tmp);
            IL.Emit(OpCodes.Ldloca, tmp);
        }

        // assumes the instance is already on the stack
        private void EmitMemberAddress(MemberInfo member, Type? objectType)
        {
            // Verifiable code may not take the address of an init-only field.
            // If we are asked to do so then get the value out of the field, stuff it
            // into a local of the same type, and then take the address of the local.
            // Typically this is what we want to do anyway; if we are saying
            // Foo.bar.ToString() for a static value-typed field bar then we don't need
            // the address of field bar to do the call.  The address of a local which
            // has the same value as bar is sufficient.

            // The C# compiler will not compile a lambda expression tree
            // which writes to the address of an init-only field. But one could
            // probably use the expression tree API to build such an expression.
            // (When compiled, such an expression would fail silently.)  It might
            // be worth it to add checking to the expression tree API to ensure
            // that it is illegal to attempt to write to an init-only field,
            // the same way that it is illegal to write to a read-only property.
            // The same goes for literal fields.
            if (member is FieldInfo field && !field.IsLiteral && !field.IsInitOnly)
            {
                IL.EmitFieldAddress(field);
                return;
            }

            EmitMemberGet(member, objectType);
            var temp = GetLocal(GetMemberType(member));
            IL.Emit(OpCodes.Stloc, temp);
            IL.Emit(OpCodes.Ldloca, temp);
        }

        private LocalBuilder GetInstanceLocal(Type type)
        {
            var instanceLocalType = type.IsValueType ? type.MakeByRefType() : type;
            return GetLocal(instanceLocalType);
        }
    }
}

#endif