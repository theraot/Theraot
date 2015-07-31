#if NET20 || NET30

using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
    [Obsolete("do not use this type", true)]
    public partial class ExecutionScope
    {
        //These fields are accessed via Reflection
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Microsoft's Design")]
        public object[] Globals;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Microsoft's Design")]
        public object[] Locals;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Microsoft's Design")]
        public ExecutionScope Parent;
    }

    public partial class ExecutionScope
    {
        public Delegate CreateDelegate(int indexLambda, object[] locals)
        {
            throw new NotSupportedException();
        }

        public object[] CreateHoistedLocals()
        {
            throw new NotSupportedException();
        }

        public Expression IsolateExpression(Expression expression, object[] locals)
        {
            throw new NotSupportedException();
        }
    }
}

#endif