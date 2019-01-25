// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    public class EnumerableFromDelegate<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerator<T>> _getEnumerator;

        public EnumerableFromDelegate(Func<IEnumerator> getEnumerator)
        {
            // Specify the type arguments explicitly
            _getEnumerator = getEnumerator.ChainConversion(ConvertEnumerator);

            IEnumerator<T> ConvertEnumerator(IEnumerator enumerator)
            {
                switch (enumerator)
                {
                    case null:
                        return null;
                    case IEnumerator<T> genericEnumerator:
                        return genericEnumerator;
                    default:
                        return ConvertEnumeratorExtracted(enumerator);
                }
            }

            IEnumerator<T> ConvertEnumeratorExtracted(IEnumerator enumerator)
            {
                try
                {
                    while (enumerator.MoveNext())
                    {
                        var element = enumerator.Current;
                        if (element is T variable)
                        {
                            yield return variable;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _getEnumerator.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _getEnumerator.Invoke();
        }
    }
}