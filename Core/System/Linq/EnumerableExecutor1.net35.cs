#if NET20 || NET30 || NET35

using System.Linq.Expressions;

namespace System.Linq
{
    public class EnumerableExecutor<T> : EnumerableExecutor
    {
        public EnumerableExecutor(Expression expression)
        {
            //Empty
        }
    }
}

#endif