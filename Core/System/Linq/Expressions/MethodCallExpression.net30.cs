#if NET20 || NET30

using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Linq.Expressions
{
    public sealed class MethodCallExpression : Expression
    {
        private readonly ReadOnlyCollection<Expression> _arguments;
        private readonly MethodInfo _method;
        private readonly Expression _obj;

        internal MethodCallExpression(MethodInfo method, ReadOnlyCollection<Expression> arguments)
            : base(ExpressionType.Call, method.ReturnType)
        {
            _method = method;
            _arguments = arguments;
        }

        internal MethodCallExpression(Expression obj, MethodInfo method, ReadOnlyCollection<Expression> arguments)
            : base(ExpressionType.Call, method.ReturnType)
        {
            _obj = obj;
            _method = method;
            _arguments = arguments;
        }

        public ReadOnlyCollection<Expression> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }

        public Expression Object
        {
            get
            {
                return _obj;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            emitContext.EmitCall(_obj, _arguments, _method);
        }
    }
}

#endif