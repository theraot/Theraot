#if LESSTHAN_NET35
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Theraot.Core;

namespace System.Linq.Expressions.Compiler
{
    /// <summary>
    ///     This type tracks "runtime" constants--live objects that appear in
    ///     ConstantExpression nodes and must be bound to the delegate.
    /// </summary>
    internal sealed class BoundConstants
    {
        /// <summary>
        ///     IL locals for storing frequently used constants
        /// </summary>
        private readonly Dictionary<TypedConstant, LocalBuilder> _cache = new Dictionary<TypedConstant, LocalBuilder>();

        /// <summary>
        ///     The index of each constant in the constant array
        /// </summary>
        private readonly Dictionary<object, int> _indexes = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Instance);

        /// <summary>
        ///     Each constant referenced within this lambda, and how often it was referenced
        /// </summary>
        private readonly Dictionary<TypedConstant, int> _references = new Dictionary<TypedConstant, int>();

        /// <summary>
        ///     The list of constants in the order they appear in the constant array
        /// </summary>
        private readonly List<object> _values = new List<object>();

        internal int Count => _values.Count;

        internal void AddReference(object value, Type type)
        {
            if (_indexes.TryAdd(value, _values.Count))
            {
                _values.Add(value);
            }

            var key = new TypedConstant(value, type);
            _references.TryGetValue(key, out var count);
            _references[key] = count + 1;
        }

        internal void EmitCacheConstants(LambdaCompiler lc)
        {
            var count = 0;
            foreach (var reference in _references)
            {
                if (!lc.CanEmitBoundConstants)
                {
                    throw new InvalidOperationException($"CompileToMethod cannot compile constant '{reference.Key.Value}' because it is a non-trivial value, such as a live object. Instead, create an expression tree that can construct this value.");
                }

                if (ShouldCache(reference.Value))
                {
                    count++;
                }
            }

            if (count == 0)
            {
                return;
            }

            EmitConstantsArray(lc);

            // The same lambda can be in multiple places in the tree, so we
            // need to clear any locals from last time.
            _cache.Clear();

            foreach (var reference in _references)
            {
                if (!ShouldCache(reference.Value))
                {
                    continue;
                }

                if (--count > 0)
                {
                    // Dup array to keep it on the stack
                    lc.IL.Emit(OpCodes.Dup);
                }

                var local = lc.IL.DeclareLocal(reference.Key.Type);
                EmitConstantFromArray(lc, reference.Key.Value, local.LocalType);
                lc.IL.Emit(OpCodes.Stloc, local);
                _cache.Add(reference.Key, local);
            }
        }

        internal void EmitConstant(LambdaCompiler lc, object value, Type type)
        {
            Debug.Assert(!ILGen.CanEmitConstant(value, type));

            if (!lc.CanEmitBoundConstants)
            {
                throw new InvalidOperationException($"CompileToMethod cannot compile constant '{value}' because it is a non-trivial value, such as a live object. Instead, create an expression tree that can construct this value.");
            }

            if (_cache.TryGetValue(new TypedConstant(value, type), out var local))
            {
                lc.IL.Emit(OpCodes.Ldloc, local);
                return;
            }

            EmitConstantsArray(lc);
            EmitConstantFromArray(lc, value, type);
        }

        internal object[] ToArray()
        {
            return _values.ToArray();
        }

        private static void EmitConstantsArray(LambdaCompiler lc)
        {
            Debug.Assert(lc.CanEmitBoundConstants); // this should've been checked already

            lc.EmitClosureArgument();
            lc.IL.Emit(OpCodes.Ldfld, CachedReflectionInfo.ClosureConstants);
        }

        private static bool ShouldCache(int refCount)
        {
            // This caching is too aggressive in the face of conditionals and
            // switch. Also, it is too conservative for variables used inside
            // of loops.
            return refCount > 2;
        }

        private void EmitConstantFromArray(LambdaCompiler lc, object value, Type type)
        {
            if (!_indexes.TryGetValue(value, out var index))
            {
                _indexes.Add(value, index = _values.Count);
                _values.Add(value);
            }

            lc.IL.EmitPrimitive(index);
            lc.IL.Emit(OpCodes.Ldelem_Ref);
            if (type.IsValueType)
            {
                lc.IL.Emit(OpCodes.Unbox_Any, type);
            }
            else if (type != typeof(object))
            {
                lc.IL.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        ///     Constants can emit themselves as different types
        ///     For caching purposes, we need to treat each distinct Type as a
        ///     separate thing to cache. (If we have to cast it on the way out, it
        ///     ends up using a JIT temp and defeats the purpose of caching the
        ///     value in a local)
        /// </summary>
        private readonly struct TypedConstant : IEquatable<TypedConstant>
        {
            internal readonly Type Type;
            internal readonly object Value;

            internal TypedConstant(object value, Type type)
            {
                Value = value;
                Type = type;
            }

            public bool Equals(TypedConstant other)
            {
                return Value == other.Value && Type == other.Type;
            }

            public override bool Equals(object obj)
            {
                return obj is TypedConstant constant && Equals(constant);
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(Value) ^ Type.GetHashCode();
            }
        }
    }
}

#endif