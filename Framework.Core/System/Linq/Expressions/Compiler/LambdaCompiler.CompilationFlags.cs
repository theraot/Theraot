#if LESSTHAN_NET35

#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
#pragma warning disable RCS1154 // Sort enum members.
#pragma warning disable RCS1157 // Composite enum value contains undefined flag.
#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        [Flags]
        internal enum CompilationFlags
        {
            EmitExpressionStart = 0x0001,
            EmitNoExpressionStart = 0x0002,
            EmitAsDefaultType = 0x0010,
            EmitAsVoidType = 0x0020,
            EmitAsTail = 0x0100, // at the tail position of a lambda, tail call can be safely emitted
            EmitAsMiddle = 0x0200, // in the middle of a lambda, tail call can be emitted if it is in a return
            EmitAsNoTail = 0x0400, // neither at the tail or in a return, or tail call is not turned on, no tail call is emitted

            EmitExpressionStartMask = 0x000f,
            EmitAsTypeMask = 0x00f0,
            EmitAsTailCallMask = 0x0f00
        }
    }
}

#endif