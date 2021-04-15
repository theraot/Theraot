#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Interpreter
{
    internal readonly struct InterpretedFrameInfo
    {
        private readonly DebugInfo? _debugInfo;

        private readonly string? _methodName;

        public InterpretedFrameInfo(string? methodName, DebugInfo? info)
        {
            _methodName = methodName;
            _debugInfo = info;
        }

        public override string ToString()
        {
            var methodName = _methodName ?? string.Empty;
            return _debugInfo != null
                ? $"{methodName}: {_debugInfo}"
                : methodName;
        }
    }
}

#endif