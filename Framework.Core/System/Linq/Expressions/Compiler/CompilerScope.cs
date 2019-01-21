#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Theraot.Collections;
using static System.Linq.Expressions.CachedReflectionInfo;

namespace System.Linq.Expressions.Compiler
{
    internal enum VariableStorageKind
    {
        Local,
        Hoisted
    }

    internal static class ParameterProviderExtensions
    {
        public static bool Contains(this IParameterProvider provider, ParameterExpression parameter)
        {
            return provider.IndexOf(parameter) >= 0;
        }

        public static int IndexOf(this IParameterProvider provider, ParameterExpression parameter)
        {
            for (int i = 0, n = provider.ParameterCount; i < n; i++)
            {
                if (provider.GetParameter(i) == parameter)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// <para>
    /// CompilerScope is the data structure which the Compiler keeps information
    /// related to compiling scopes. It stores the following information:
    ///   1. Parent relationship (for resolving variables)
    ///   2. Information about hoisted variables
    ///   3. Information for resolving closures
    /// </para>
    /// <para>
    /// Instances are produced by VariableBinder, which does a tree walk
    /// looking for scope nodes: LambdaExpression, BlockExpression, and CatchBlock.
    /// </para>
    /// </summary>
    internal sealed partial class CompilerScope
    {
        /// <summary>
        /// Variables defined in this scope, and whether they're hoisted or not
        /// Populated by VariableBinder
        /// </summary>
        internal readonly Dictionary<ParameterExpression, VariableStorageKind> Definitions;

        /// <summary>
        /// True if this node corresponds to an IL method.
        /// Can only be true if the Node is a LambdaExpression.
        /// But inlined lambdas will have it set to false.
        /// </summary>
        internal readonly bool IsMethod;

        /// <summary>
        /// <para>Scopes whose variables were merged into this one</para>
        /// <para>Created lazily as we create hundreds of compiler scopes w/o merging scopes when compiling rules.</para>
        /// </summary>
        internal HashSet<BlockExpression> MergedScopes;

        /// <summary>
        /// Does this scope (or any inner scope) close over variables from any
        /// parent scope?
        /// Populated by VariableBinder
        /// </summary>
        internal bool NeedsClosure;

        /// <summary>
        /// The expression node for this scope
        /// Can be LambdaExpression, BlockExpression, or CatchBlock
        /// </summary>
        internal readonly object Node;

        /// <summary>
        /// Each variable referenced within this scope, and how often it was referenced
        /// Populated by VariableBinder
        /// </summary>
        internal Dictionary<ParameterExpression, int> ReferenceCount;

        /// <summary>
        /// The closed over hoisted locals
        /// </summary>
        private HoistedLocals _closureHoistedLocals;

        /// <summary>
        /// The scope's hoisted locals, if any.
        /// Provides storage for variables that are referenced from nested lambdas
        /// </summary>
        private HoistedLocals _hoistedLocals;

        /// <summary>
        /// Mutable dictionary that maps non-hoisted variables to either local
        /// slots or argument slots
        /// </summary>
        private readonly Dictionary<ParameterExpression, Storage> _locals = new Dictionary<ParameterExpression, Storage>();

        /// <summary>
        /// parent scope, if any
        /// </summary>
        private CompilerScope _parent;

        internal CompilerScope(object node, bool isMethod)
        {
            Node = node;
            IsMethod = isMethod;
            var variables = GetVariables(node);

            Definitions = new Dictionary<ParameterExpression, VariableStorageKind>(variables.Length);
            foreach (var v in variables)
            {
                Definitions.Add(v, VariableStorageKind.Local);
            }
        }

        /// <summary>
        /// This scope's hoisted locals, or the closed over locals, if any
        /// Equivalent to: _hoistedLocals ?? _closureHoistedLocals
        /// </summary>
        internal HoistedLocals NearestHoistedLocals => _hoistedLocals ?? _closureHoistedLocals;

        private string CurrentLambdaName
        {
            get
            {
                var s = this;
                while (s != null)
                {
                    if (s.Node is LambdaExpression lambda)
                    {
                        return lambda.Name;
                    }
                    s = s._parent;
                }
                throw ContractUtils.Unreachable;
            }
        }

        internal void AddLocal(LambdaCompiler gen, ParameterExpression variable)
        {
            _locals.Add(variable, new LocalStorage(gen, variable));
        }

        internal void EmitAddressOf(ParameterExpression variable)
        {
            ResolveVariable(variable).EmitAddress();
        }

        internal void EmitGet(ParameterExpression variable)
        {
            ResolveVariable(variable).EmitLoad();
        }

        internal void EmitSet(ParameterExpression variable)
        {
            ResolveVariable(variable).EmitStore();
        }

        internal void EmitVariableAccess(LambdaCompiler lc, ReadOnlyCollection<ParameterExpression> vars)
        {
            if (NearestHoistedLocals != null && vars.Count > 0)
            {
                // Find what array each variable is on & its index
                var indexes = new ArrayBuilder<long>(vars.Count);

                foreach (var variable in vars)
                {
                    // For each variable, find what array it's defined on
                    ulong parents = 0;
                    var locals = NearestHoistedLocals;
                    while (!locals.Indexes.ContainsKey(variable))
                    {
                        parents++;
                        locals = locals.Parent;
                        Debug.Assert(locals != null);
                    }

                    // combine the number of parents we walked, with the
                    // real index of variable to get the index to emit.
                    var index = (parents << 32) | (uint)locals.Indexes[variable];

                    indexes.UncheckedAdd((long)index);
                }

                EmitGet(NearestHoistedLocals.SelfVariable);
                lc.EmitConstantArray(indexes.ToArray());
                lc.IL.Emit(OpCodes.Call, RuntimeOpsCreateRuntimeVariablesObjectArrayInt64Array);
            }
            else
            {
                // No visible variables
                lc.IL.Emit(OpCodes.Call, RuntimeOpsCreateRuntimeVariables);
            }
        }

        internal CompilerScope Enter(LambdaCompiler lc, CompilerScope parent)
        {
            SetParent(lc, parent);

            AllocateLocals(lc);

            if (IsMethod && _closureHoistedLocals != null)
            {
                EmitClosureAccess(lc, _closureHoistedLocals);
            }

            EmitNewHoistedLocals(lc);

            if (IsMethod)
            {
                EmitCachedVariables();
            }

            return this;
        }

        /// <summary>
        /// Frees unnamed locals, clears state associated with this compiler
        /// </summary>
        internal CompilerScope Exit()
        {
            // free scope's variables
            if (!IsMethod)
            {
                foreach (var storage in _locals.Values)
                {
                    storage.FreeLocal();
                }
            }

            // Clear state that is associated with this parent
            // (because the scope can be reused in another context)
            var parent = _parent;
            _parent = null;
            _hoistedLocals = null;
            _closureHoistedLocals = null;
            _locals.Clear();

            return parent;
        }

        private static ParameterExpression[] GetVariables(object scope)
        {
            if (scope is LambdaExpression lambda)
            {
                return Theraot.Collections.Extensions.AsArrayInternal(new ParameterList(lambda));
            }
            if (scope is BlockExpression block)
            {
                return Theraot.Collections.Extensions.AsArrayInternal(block.Variables);
            }
            return new[] { ((CatchBlock)scope).Variable };
        }

        // Allocates slots for IL locals or IL arguments
        private void AllocateLocals(LambdaCompiler lc)
        {
            foreach (var v in GetVariables())
            {
                if (Definitions[v] == VariableStorageKind.Local)
                {
                    //
                    // If v is in lc.Parameters, it is a parameter.
                    // Otherwise, it is a local variable.
                    //
                    // Also, for inlined lambdas we'll create a local, which
                    // is possibly a byref local if the parameter is byref.
                    //
                    var s = IsMethod && lc.Parameters.Contains(v) ? (Storage)new ArgumentStorage(lc, v) : new LocalStorage(lc, v);
                    _locals.Add(v, s);
                }
            }
        }

        private void CacheBoxToLocal(LambdaCompiler lc, ParameterExpression v)
        {
            Debug.Assert(ShouldCache(v) && !_locals.ContainsKey(v));
            var local = new LocalBoxStorage(lc, v);
            local.EmitStoreBox();
            _locals.Add(v, local);
        }

        // If hoisted variables are referenced "enough", we cache the
        // StrongBox<T> in an IL local, which saves an array index and a cast
        // when we go to look it up later
        private void EmitCachedVariables()
        {
            if (ReferenceCount == null)
            {
                return;
            }

            foreach (var refCount in ReferenceCount)
            {
                if (ShouldCache(refCount.Key, refCount.Value) && ResolveVariable(refCount.Key) is ElementBoxStorage storage)
                {
                    storage.EmitLoadBox();
                    CacheBoxToLocal(storage.Compiler, refCount.Key);
                }
            }
        }

        // Creates IL locals for accessing closures
        private void EmitClosureAccess(LambdaCompiler lc, HoistedLocals locals)
        {
            if (locals == null)
            {
                return;
            }

            EmitClosureToVariable(lc, locals);

            while ((locals = locals.Parent) != null)
            {
                var v = locals.SelfVariable;
                var local = new LocalStorage(lc, v);
                local.EmitStore(ResolveVariable(v));
                _locals.Add(v, local);
            }
        }

        private void EmitClosureToVariable(LambdaCompiler lc, HoistedLocals locals)
        {
            lc.EmitClosureArgument();
            lc.IL.Emit(OpCodes.Ldfld, ClosureLocals);
            AddLocal(lc, locals.SelfVariable);
            EmitSet(locals.SelfVariable);
        }

        // Emits creation of the hoisted local storage
        private void EmitNewHoistedLocals(LambdaCompiler lc)
        {
            if (_hoistedLocals == null)
            {
                return;
            }

            // create the array
            lc.IL.EmitPrimitive(_hoistedLocals.Variables.Count);
            lc.IL.Emit(OpCodes.Newarr, typeof(object));

            // initialize all elements
            var i = 0;
            foreach (var v in _hoistedLocals.Variables)
            {
                // array[i] = new StrongBox<T>(...);
                lc.IL.Emit(OpCodes.Dup);
                lc.IL.EmitPrimitive(i++);
                var boxType = typeof(StrongBox<>).MakeGenericType(v.Type);

                int index;
                if (IsMethod && (index = lc.Parameters.IndexOf(v)) >= 0)
                {
                    // array[i] = new StrongBox<T>(argument);
                    lc.EmitLambdaArgument(index);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    lc.IL.Emit(OpCodes.Newobj, boxType.GetConstructor(new[] { v.Type }));
                }
                else if (v == _hoistedLocals.ParentVariable)
                {
                    // array[i] = new StrongBox<T>(closure.Locals);
                    ResolveVariable(v, _closureHoistedLocals).EmitLoad();
                    // ReSharper disable once AssignNullToNotNullAttribute
                    lc.IL.Emit(OpCodes.Newobj, boxType.GetConstructor(new[] { v.Type }));
                }
                else
                {
                    // array[i] = new StrongBox<T>();
                    // ReSharper disable once AssignNullToNotNullAttribute
                    lc.IL.Emit(OpCodes.Newobj, boxType.GetConstructor(Type.EmptyTypes));
                }
                // if we want to cache this into a local, do it now
                if (ShouldCache(v))
                {
                    lc.IL.Emit(OpCodes.Dup);
                    CacheBoxToLocal(lc, v);
                }
                lc.IL.Emit(OpCodes.Stelem_Ref);
            }

            // store it
            EmitSet(_hoistedLocals.SelfVariable);
        }

        private IEnumerable<ParameterExpression> GetVariables() =>
            MergedScopes == null ? GetVariables(Node) : GetVariablesIncludingMerged();

        private IEnumerable<ParameterExpression> GetVariablesIncludingMerged()
        {
            foreach (var param in GetVariables(Node))
            {
                yield return param;
            }

            foreach (var scope in MergedScopes)
            {
                foreach (var param in scope.Variables)
                {
                    yield return param;
                }
            }
        }

        private Storage ResolveVariable(ParameterExpression variable)
        {
            return ResolveVariable(variable, NearestHoistedLocals);
        }

        private Storage ResolveVariable(ParameterExpression variable, HoistedLocals hoistedLocals)
        {
            // Search IL locals and arguments, but only in this lambda
            for (var s = this; s != null; s = s._parent)
            {
                if (s._locals.TryGetValue(variable, out var storage))
                {
                    return storage;
                }

                // if this is a lambda, we're done
                if (s.IsMethod)
                {
                    break;
                }
            }

            // search hoisted locals
            for (var h = hoistedLocals; h != null; h = h.Parent)
            {
                if (h.Indexes.TryGetValue(variable, out var index))
                {
                    return new ElementBoxStorage(
                        ResolveVariable(h.SelfVariable, hoistedLocals),
                        index,
                        variable
                    );
                }
            }

            //
            // If this is an unbound variable in the lambda, the error will be
            // thrown from VariableBinder. So an error here is generally caused
            // by an internal error, e.g. a scope was created but it bypassed
            // VariableBinder.
            //
            throw new InvalidOperationException($"variable '{variable.Name}' of type '{variable.Type}' referenced from scope '{CurrentLambdaName}', but it is not defined");
        }

        private void SetParent(LambdaCompiler lc, CompilerScope parent)
        {
            Debug.Assert(_parent == null && parent != this);
            _parent = parent;

            if (NeedsClosure && _parent != null)
            {
                _closureHoistedLocals = _parent.NearestHoistedLocals;
            }

            var hoistedVars = GetVariables().Where(p => Definitions[p] == VariableStorageKind.Hoisted).ToReadOnlyCollection();

            if (hoistedVars.Count > 0)
            {
                _hoistedLocals = new HoistedLocals(_closureHoistedLocals, hoistedVars);
                AddLocal(lc, _hoistedLocals.SelfVariable);
            }
        }

        private bool ShouldCache(ParameterExpression v, int refCount)
        {
            // This caching is too aggressive in the face of conditionals and
            // switch. Also, it is too conservative for variables used inside
            // of loops.
            return refCount > 2 && !_locals.ContainsKey(v);
        }

        private bool ShouldCache(ParameterExpression v)
        {
            if (ReferenceCount == null)
            {
                return false;
            }
            return ReferenceCount.TryGetValue(v, out var refCount) && ShouldCache(v, refCount);
        }
    }

    internal sealed class ParameterList : IReadOnlyList<ParameterExpression>
    {
        private readonly IParameterProvider _provider;

        public ParameterList(IParameterProvider provider)
        {
            _provider = provider;
        }

        public int Count => _provider.ParameterCount;

        public ParameterExpression this[int index] => _provider.GetParameter(index);

        public IEnumerator<ParameterExpression> GetEnumerator()
        {
            for (int i = 0, n = _provider.ParameterCount; i < n; i++)
            {
                yield return _provider.GetParameter(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

#endif