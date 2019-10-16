#if LESSTHAN_NET35

#pragma warning disable CC0021 // Use nameof

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot;
using Theraot.Core;
using Theraot.Reflection;
using AstUtils = System.Linq.Expressions.Utils;

namespace System.Linq.Expressions.Interpreter
{
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

        public override void Update(InterpretedFrame frame, object? value)
        {
            var args = new object?[_args.Length + 1];
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
}

#endif