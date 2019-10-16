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

        public override void Update(InterpretedFrame frame, object? value)
        {
            var obj = _object == null ? null : frame.Data[_object.GetValueOrDefault().Index];
            _field.SetValue(obj, value);
        }
    }
}

#endif