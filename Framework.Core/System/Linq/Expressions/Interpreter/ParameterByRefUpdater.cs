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

    internal sealed class ParameterByRefUpdater : ByRefUpdater
    {
        private readonly LocalVariable _parameter;

        public ParameterByRefUpdater(LocalVariable parameter, int argumentIndex)
            : base(argumentIndex)
        {
            _parameter = parameter;
        }

        public override void Update(InterpretedFrame frame, object? value)
        {
            if (_parameter.InClosure)
            {
                var box = frame.Closure![_parameter.Index];
                box.Value = value;
            }
            else if (_parameter.IsBoxed)
            {
                var box = (IStrongBox)frame.Data[_parameter.Index]!;
                box.Value = value;
            }
            else
            {
                frame.Data[_parameter.Index] = value;
            }
        }
    }
}

#endif