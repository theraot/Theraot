#if NET20 || NET30

using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
    public sealed class Expression<TDelegate> : LambdaExpression
    {
        internal Expression(Expression body, ReadOnlyCollection<ParameterExpression> parameters)
            : base(typeof(TDelegate), body, parameters)
        {
            //Empty
        }

        public new TDelegate Compile()
        {
            return (TDelegate)(object)base.Compile();
        }
    }
}

#endif