using System;
using System.Collections.Generic;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    internal static class TransactionExtensions
    {
        public static T Read<T>(this Func<T> source)
        {
            var transaction = Transaction._currentTransaction;
            if (transaction == null)
            {
                return source.Invoke();
            }
            else
            {
                ITransactionResource resource;
                if (transaction.TryGetResource(source, out resource))
                {
                    return (resource as ITransactionNeedle<T>).Value;
                }
                else
                {
                    var value = source.Invoke();
                    transaction.SetResource(source, new TransactionNeedle<T>(value, source, null, null));
                    return value;
                }
            }
        }

        public static T Read<T>(this Func<T> source, IEqualityComparer<T> comparer)
        {
            var transaction = Transaction._currentTransaction;
            if (transaction == null)
            {
                return source.Invoke();
            }
            else
            {
                ITransactionResource resource;
                if (transaction.TryGetResource(source, out resource))
                {
                    return (resource as ITransactionNeedle<T>).Value;
                }
                else
                {
                    var value = source.Invoke();
                    transaction.SetResource(source, new TransactionNeedle<T>(value, source, null, comparer));
                    return value;
                }
            }
        }

        public static void Write<T>(Action<T> target, T value)
        {
            var transaction = Transaction._currentTransaction;
            if (transaction == null)
            {
                target.Invoke(value);
            }
            else
            {
                ITransactionResource resource;
                if (transaction.TryGetResource(target, out resource))
                {
                    (resource as ITransactionNeedle<T>).Value = value;
                }
                else
                {
                    transaction.SetResource(target, new TransactionNeedle<T>(value, null, target, null));
                }
            }
        }
    }
}