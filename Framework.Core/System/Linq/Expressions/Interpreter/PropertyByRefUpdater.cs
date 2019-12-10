#if LESSTHAN_NET35

#pragma warning disable CC0021 // Use nameof

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using System.Reflection;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class PropertyByRefUpdater : ByRefUpdater
    {
        private readonly LocalDefinition? _object;
        private readonly PropertyInfo _property;

        public PropertyByRefUpdater(LocalDefinition? obj, PropertyInfo property, int argumentIndex)
            : base(argumentIndex)
        {
            _object = obj;
            _property = property;
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

            try
            {
                _property.SetValue(obj, value);
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