﻿#if LESSTHAN_NET35

#pragma warning disable CA1051 // Do not declare visible instance fields

using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
    [Obsolete("do not use this type", true)]
    public partial class ExecutionScope
    {
        //These fields are accessed via Reflection
        public object[]? Globals;

        public object[]? Locals;

        public ExecutionScope? Parent;
    }

    public partial class ExecutionScope
    {
        public Delegate CreateDelegate(int indexLambda, object[] locals)
        {
            // Should not be static
            _ = indexLambda;
            _ = locals;
            throw new NotSupportedException();
        }

        public object[] CreateHoistedLocals()
        {
            // Should not be static
            throw new NotSupportedException();
        }

        public Expression IsolateExpression(Expression expression, object[] locals)
        {
            // Should not be static
            _ = expression;
            _ = locals;
            throw new NotSupportedException();
        }
    }
}

#endif