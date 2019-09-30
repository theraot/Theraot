#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Linq.Expressions.Compiler
{
    /// <summary>
    ///     Dynamic Language Runtime Compiler.
    ///     This part compiles lambdas.
    /// </summary>
    internal partial class LambdaCompiler
    {
        private static int _counter;

        internal void EmitConstantArray<T>(T[] array)
        {
            // Emit as runtime constant if possible
            // if not, emit into IL
            if (_method is DynamicMethod)
            {
                EmitConstant(array, typeof(T[]));
            }
            else if (_typeBuilder != null)
            {
                // store into field in our type builder, we will initialize
                // the value only once.
                var fb = CreateStaticField("ConstantArray", typeof(T[]));
                var l = IL.DefineLabel();
                IL.Emit(OpCodes.Ldsfld, fb);
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Bne_Un, l);
                IL.EmitArray(array, this);
                IL.Emit(OpCodes.Stsfld, fb);
                IL.MarkLabel(l);
                IL.Emit(OpCodes.Ldsfld, fb);
            }
            else
            {
                IL.EmitArray(array, this);
            }
        }

        private static Type[] GetParameterTypes(LambdaExpression lambda, Type? firstType)
        {
            var count = lambda.ParameterCount;

            Type[] result;
            int i;

            if (firstType != null)
            {
                result = new Type[count + 1];
                result[0] = firstType;
                i = 1;
            }
            else
            {
                result = new Type[count];
                i = 0;
            }

            for (var j = 0; j < count; j++, i++)
            {
                var p = lambda.GetParameter(j);
                result[i] = p.IsByRef ? p.Type.MakeByRefType() : p.Type;
            }

            return result;
        }

        private static string GetUniqueMethodName()
        {
            return "<ExpressionCompilerImplementationDetails>{" + Interlocked.Increment(ref _counter) + "}lambda_method";
        }

        private void EmitClosureCreation(LambdaCompiler inner)
        {
            var closure = inner._scope.NeedsClosure;
            var boundConstants = inner._boundConstants.Count > 0;

            if (!closure && !boundConstants)
            {
                IL.EmitNull();
                return;
            }

            // new Closure(constantPool, currentHoistedLocals)
            if (boundConstants)
            {
                _boundConstants.EmitConstant(this, inner._boundConstants.ToArray(), typeof(object[]));
            }
            else
            {
                IL.EmitNull();
            }

            if (closure)
            {
                _scope.EmitGet(_scope.NearestHoistedLocals!.SelfVariable);
            }
            else
            {
                IL.EmitNull();
            }

            IL.EmitNew(CachedReflectionInfo.ClosureObjectArrayObjectArray);
        }

        private void EmitDelegateConstruction(LambdaCompiler inner)
        {
            var delegateType = inner._lambda.Type;
            if (inner._method is DynamicMethod dynamicMethod)
            {
                // dynamicMethod.CreateDelegate(delegateType, closure)
                _boundConstants.EmitConstant(this, dynamicMethod, typeof(DynamicMethod));
                IL.EmitType(delegateType);
                EmitClosureCreation(inner);
                // ReSharper disable once AssignNullToNotNullAttribute
                IL.Emit(OpCodes.Callvirt, typeof(DynamicMethod).GetMethod(nameof(DynamicMethod.CreateDelegate), new[] { typeof(Type), typeof(object) }));
                IL.Emit(OpCodes.Castclass, delegateType);
            }
            else
            {
                // new DelegateType(closure)
                EmitClosureCreation(inner);
                IL.Emit(OpCodes.Ldftn, inner._method);
                IL.Emit(OpCodes.Newobj, (ConstructorInfo)delegateType.GetMember(".ctor")[0]);
            }
        }

        /// <summary>
        ///     Emits a delegate to the method generated for the LambdaExpression.
        ///     May end up creating a wrapper to match the requested delegate type.
        /// </summary>
        /// <param name="lambda">Lambda for which to generate a delegate</param>
        private void EmitDelegateConstruction(LambdaExpression lambda)
        {
            // 1. Create the new compiler
            LambdaCompiler impl;
            if (_method is DynamicMethod)
            {
                impl = new LambdaCompiler(_tree, lambda);
            }
            else
            {
                // When the lambda does not have a name or the name is empty, generate a unique name for it.
                var name = string.IsNullOrEmpty(lambda.Name) ? GetUniqueMethodName() : lambda.Name;
                var mb = _typeBuilder!.DefineMethod(name, MethodAttributes.Private | MethodAttributes.Static);
                impl = new LambdaCompiler(_tree, lambda, mb);
            }

            // 2. emit the lambda
            // Since additional ILs are always emitted after the lambda's body, should not emit with tail call optimization.
            impl.EmitLambdaBody(_scope, false, CompilationFlags.EmitAsNoTail);

            // 3. emit the delegate creation in the outer lambda
            EmitDelegateConstruction(impl);
        }

        private void EmitLambdaBody()
        {
            // The lambda body is the "last" expression of the lambda
            var tailCallFlag = _lambda.TailCall ? CompilationFlags.EmitAsTail : CompilationFlags.EmitAsNoTail;
            EmitLambdaBody(null, false, tailCallFlag);
        }

        /// <summary>
        ///     Emits the lambda body. If inlined, the parameters should already be
        ///     pushed onto the IL stack.
        /// </summary>
        /// <param name="parent">The parent scope.</param>
        /// <param name="inlined">true if the lambda is inlined; false otherwise.</param>
        /// <param name="flags">
        ///     The enum to specify if the lambda is compiled with the tail call optimization.
        /// </param>
        private void EmitLambdaBody(CompilerScope? parent, bool inlined, CompilationFlags flags)
        {
            _scope.Enter(this, parent);

            if (inlined)
            {
                // The arguments were already pushed onto the IL stack.
                // Store them into locals, popping in reverse order.
                //
                // If any arguments were ByRef, the address is on the stack and
                // we'll be storing it into the variable, which has a ref type.
                for (var i = _lambda.ParameterCount - 1; i >= 0; i--)
                {
                    _scope.EmitSet(_lambda.GetParameter(i));
                }
            }

            // Need to emit the expression start for the lambda body
            flags = UpdateEmitExpressionStartFlag(flags, CompilationFlags.EmitExpressionStart);
            if (_lambda.ReturnType == typeof(void))
            {
                EmitExpressionAsVoid(_lambda.Body, flags);
            }
            else
            {
                EmitExpression(_lambda.Body, flags);
            }

            // Return must be the last instruction in a CLI method.
            // But if we're inlining the lambda, we want to leave the return
            // value on the IL stack.
            if (!inlined)
            {
                IL.Emit(OpCodes.Ret);
            }

            _scope.Exit();

            // Validate labels
            Debug.Assert(_labelBlock.Parent == null && _labelBlock.Kind == LabelScopeKind.Lambda);
            foreach (var label in _labelInfo.Values)
            {
                label.ValidateFinish();
            }
        }
    }
}

#endif