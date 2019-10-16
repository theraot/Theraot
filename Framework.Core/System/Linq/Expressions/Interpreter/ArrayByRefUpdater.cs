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

        public override void Update(InterpretedFrame frame, object? value)
        {
            var index = frame.Data[_index.Index]!;
            ((Array)frame.Data[_array.Index]!).SetValue(value, (int)index);
        }
    }
}

#endif