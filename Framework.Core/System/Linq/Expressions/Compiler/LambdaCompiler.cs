#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Theraot.Collections.Specialized;
using Theraot.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal interface ILocalCache
    {
        void FreeLocal(LocalBuilder local);

        LocalBuilder GetLocal(Type type);
    }

    /// <summary>
    /// LambdaCompiler is responsible for compiling individual lambda (LambdaExpression). The complete tree may
    /// contain multiple lambdas, the Compiler class is responsible for compiling the whole tree, individual
    /// lambdas are then compiled by the LambdaCompiler.
    /// </summary>
    internal sealed partial class LambdaCompiler : ILocalCache
    {
        // Runtime constants bound to the delegate
        private readonly BoundConstants _boundConstants;

        // Free list of locals, so we reuse them rather than creating new ones
        private readonly KeyedStack<Type, LocalBuilder> _freeLocals = new KeyedStack<Type, LocalBuilder>();

        // True if the method's first argument is of type Closure
        private readonly bool _hasClosureArgument;

        // Mapping of labels used for "long" jumps (jumping out and into blocks)
        private readonly Dictionary<LabelTarget, LabelInfo> _labelInfo = new Dictionary<LabelTarget, LabelInfo>();

        // The lambda we are compiling
        private readonly LambdaExpression _lambda;

        private readonly MethodInfo _method;

        // Information on the entire lambda tree currently being compiled
        private readonly AnalyzedTree _tree;

        // The TypeBuilder backing this method, if any
        private readonly TypeBuilder _typeBuilder;

        // Currently active LabelTargets and their mapping to IL labels
        private LabelScopeInfo _labelBlock = new LabelScopeInfo(null, LabelScopeKind.Lambda);

        // The currently active variable scope
        private CompilerScope _scope;

        /// <summary>
        /// Creates a lambda compiler that will compile to a dynamic method
        /// </summary>
        private LambdaCompiler(AnalyzedTree tree, LambdaExpression lambda)
        {
            var parameterTypes = GetParameterTypes(lambda, typeof(Closure));

            var method = new DynamicMethod(lambda.Name ?? "lambda_method", lambda.ReturnType, parameterTypes, true);

            _tree = tree;
            _lambda = lambda;
            _method = method;

            IL = method.GetILGenerator();

            _hasClosureArgument = true;

            // These are populated by AnalyzeTree/VariableBinder
            _scope = tree.Scopes[lambda];
            _boundConstants = tree.Constants[lambda];

            InitializeMethod();
        }

        /// <summary>
        /// Creates a lambda compiler that will compile into the provided MethodBuilder
        /// </summary>
        private LambdaCompiler(AnalyzedTree tree, LambdaExpression lambda, MethodBuilder method)
        {
            var scope = tree.Scopes[lambda];
            var hasClosureArgument = scope.NeedsClosure;

            var paramTypes = GetParameterTypes(lambda, hasClosureArgument ? typeof(Closure) : null);

            method.SetReturnType(lambda.ReturnType);
            method.SetParameters(paramTypes);
            var parameters = lambda.Parameters;
            // parameters are index from 1, with closure argument we need to skip the first arg
            var startIndex = hasClosureArgument ? 2 : 1;
            for (int i = 0, n = parameters.Count; i < n; i++)
            {
                method.DefineParameter(i + startIndex, ParameterAttributes.None, parameters[i].Name);
            }

            _tree = tree;
            _lambda = lambda;
            _typeBuilder = (TypeBuilder)method.DeclaringType;
            _method = method;
            _hasClosureArgument = hasClosureArgument;

            IL = method.GetILGenerator();

            // These are populated by AnalyzeTree/VariableBinder
            _scope = scope;
            _boundConstants = tree.Constants[lambda];

            InitializeMethod();
        }

        /// <summary>
        /// Creates a lambda compiler for an inlined lambda
        /// </summary>
        private LambdaCompiler(
            LambdaCompiler parent,
            LambdaExpression lambda,
            InvocationExpression invocation)
        {
            _tree = parent._tree;
            _lambda = lambda;
            _method = parent._method;
            IL = parent.IL;
            _hasClosureArgument = parent._hasClosureArgument;
            _typeBuilder = parent._typeBuilder;
            // inlined scopes are associated with invocation, not with the lambda
            _scope = _tree.Scopes[invocation];
            _boundConstants = parent._boundConstants;
        }

        private delegate void WriteBack(LambdaCompiler compiler);

        internal bool CanEmitBoundConstants => _method is DynamicMethod;

        internal ILGenerator IL { get; }

        internal IParameterProvider Parameters => _lambda;

        public void FreeLocal(LocalBuilder local)
        {
            Debug.Assert(local != null);
            _freeLocals.Add(local.LocalType, local);
        }

        public LocalBuilder GetLocal(Type type)
        {
            if (_freeLocals.TryTake(type, out var builder))
            {
                return builder;
            }
            return IL.DeclareLocal(type);
        }

        internal void EmitClosureArgument()
        {
            Debug.Assert(_hasClosureArgument, "must have a Closure argument");
            Debug.Assert(_method.IsStatic, "must be a static method");
            IL.EmitLoadArg(0);
        }

        internal void EmitLambdaArgument(int index)
        {
            IL.EmitLoadArg(GetLambdaArgument(index));
        }

        internal int GetLambdaArgument(int index)
        {
            return index + (_hasClosureArgument ? 1 : 0) + (_method.IsStatic ? 0 : 1);
        }

        private static AnalyzedTree AnalyzeLambda(ref LambdaExpression lambda)
        {
            // Spill the stack for any exception handling blocks or other
            // constructs which require entering with an empty stack
            lambda = StackSpiller.AnalyzeLambda(lambda);

            // Bind any variable references in this lambda
            return VariableBinder.Bind(lambda);
        }

        private Delegate CreateDelegate()
        {
            Debug.Assert(_method is DynamicMethod);

            return _method.CreateDelegate(_lambda.Type, new Closure(_boundConstants.ToArray(), null));
        }

        private MemberExpression CreateLazyInitializedField<T>(string name)
        {
            return _method is DynamicMethod
                ? Expression.Field(Expression.Constant(new StrongBox<T>(default)), "Value")
                : Expression.Field(null, CreateStaticField(name, typeof(T)));
        }

        private FieldBuilder CreateStaticField(string name, Type type)
        {
            // We are emitting into a third party type. We don't want name
            // conflicts, so choose a long name that is unlikely to conflict.
            // Naming scheme chosen here is similar to what the C# compiler
            // uses.
            return _typeBuilder.DefineField("<ExpressionCompilerImplementationDetails>{" + Threading.Interlocked.Increment(ref _counter) + "}" + name, type, FieldAttributes.Static | FieldAttributes.Private);
        }

        private void InitializeMethod()
        {
            // See if we can find a return label, so we can emit better IL
            AddReturnLabel(_lambda);
            _boundConstants.EmitCacheConstants(this);
        }

        #region Compiler entry points

        /// <summary>
        /// Compiler entry point
        /// </summary>
        /// <param name="lambda">LambdaExpression to compile.</param>
        /// <returns>The compiled delegate.</returns>
        internal static Delegate Compile(LambdaExpression lambda)
        {
            lambda.ValidateArgumentCount();

            // 1. Bind lambda
            var tree = AnalyzeLambda(ref lambda);

            // 2. Create lambda compiler
            var c = new LambdaCompiler(tree, lambda);

            // 3. Emit
            c.EmitLambdaBody();

            // 4. Return the delegate.
            return c.CreateDelegate();
        }

        internal static void Compile(LambdaExpression lambda, MethodBuilder method)
        {
            // 1. Bind lambda
            var tree = AnalyzeLambda(ref lambda);

            // 2. Create lambda compiler
            var c = new LambdaCompiler(tree, lambda, method);

            // 3. Emit
            c.EmitLambdaBody();
        }

        #endregion Compiler entry points
    }
}

#endif