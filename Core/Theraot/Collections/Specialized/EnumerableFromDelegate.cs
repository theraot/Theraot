using System;
using System.Collections;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections.Specialized
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public class EnumerableFromDelegate<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerator<T>> _getEnumerator;

        public EnumerableFromDelegate(Func<IEnumerator> getEnumerator)
        {
            _getEnumerator = getEnumerator.ChainConvertion(ConvertEnumerator);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _getEnumerator.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _getEnumerator.Invoke();
        }

        private static IEnumerator<T> ConvertEnumerator(IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                return null;
            }
            else if (enumerator is IEnumerator<T>)
            {
                return enumerator as IEnumerator<T>;
            }
            else
            {
                return ConvertEnumeratorExtracted(enumerator);
            }
        }

        private static IEnumerator<T> ConvertEnumeratorExtracted(IEnumerator enumerator)
        {
            try
            {
                while (enumerator.MoveNext())
                {
                    object element = enumerator.Current;
                    if (element is T)
                    {
                        yield return (T)element;
                    }
                }
                yield break;
            }
            finally
            {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}