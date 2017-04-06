#if NET20 || NET30

using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
    [Obsolete("do not use this type", true)]
    public partial class ExecutionScope
    {
        //These fields are accessed via Reflection
        public object[] Globals;

        public object[] Locals;

        public ExecutionScope Parent;
    }

    public partial class ExecutionScope
    {
        public Delegate CreateDelegate(int indexLambda, object[] locals)
        {
            GC.KeepAlive(indexLambda);
            GC.KeepAlive(locals);
            throw new NotSupportedException();
        }

        public object[] CreateHoistedLocals()
        {
            throw new NotSupportedException();
        }

        public Expression IsolateExpression(Expression expression, object[] locals)
        {
            GC.KeepAlive(expression);
            GC.KeepAlive(locals);
            throw new NotSupportedException();
        }
    }
}

#endif