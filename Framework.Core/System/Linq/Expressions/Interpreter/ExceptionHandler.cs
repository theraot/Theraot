#if LESSTHAN_NET35

#pragma warning disable CC0021 // Use nameof

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Linq.Expressions.Interpreter
{
    internal sealed class ExceptionHandler
    {
        public readonly ExceptionFilter? Filter;
        public readonly int HandlerEndIndex;
        public readonly int HandlerStartIndex;
        public readonly int LabelIndex;
        private readonly Type _exceptionType;

        internal ExceptionHandler(int labelIndex, int handlerStartIndex, int handlerEndIndex, Type exceptionType, ExceptionFilter? filter)
        {
            LabelIndex = labelIndex;
            _exceptionType = exceptionType;
            HandlerStartIndex = handlerStartIndex;
            HandlerEndIndex = handlerEndIndex;
            Filter = filter;
        }

        public bool Matches(Type exceptionType)
        {
            return _exceptionType.IsAssignableFrom(exceptionType);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "catch({0}) [{1}->{2}]", _exceptionType.Name, HandlerStartIndex, HandlerEndIndex);
        }
    }
}

#endif