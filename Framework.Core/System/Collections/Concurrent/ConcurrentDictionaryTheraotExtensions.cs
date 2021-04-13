// ReSharper disable RedundantUsingDirective

#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable IDE0005 // Using directive is unnecessary.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Collections.Concurrent
{
    public static class ConcurrentDictionaryTheraotExtensions
    {
#if (GREATERTHAN_NET35 && LESSTHAN_NET472) || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD21

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key</param>
        /// <param name="factoryArgument">An argument value to pass into <paramref name="valueFactory"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The value for the key.  This will be either the existing value for the key if the
        /// key is already in the dictionary, or the new value for the key as returned by valueFactory
        /// if the key was not in the dictionary.</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static TValue GetOrAdd<TKey, TValue, TArg>(
            this ConcurrentDictionary<TKey, TValue> concurrentDictionary,
            TKey key,
            Func<TKey, TArg, TValue> valueFactory,
            TArg factoryArgument) where TKey : notnull
        {
            if (concurrentDictionary is null)
            {
                throw new NullReferenceException();
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (valueFactory is null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            return concurrentDictionary.GetOrAdd(key, elementKey => valueFactory(elementKey, factoryArgument));
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key does not already
        /// exist, or updates a key/value pair in the <see cref="ConcurrentDictionary{TKey,TValue}"/> if the key
        /// already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key
        /// based on the key's existing value</param>
        /// <param name="factoryArgument">An argument to pass into <paramref name="addValueFactory"/> and <paramref name="updateValueFactory"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="addValueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="updateValueFactory"/> is a null reference
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="OverflowException">The dictionary contains too many
        /// elements.</exception>
        /// <returns>The new value for the key.  This will be either be the result of addValueFactory (if the key was
        /// absent) or the result of updateValueFactory (if the key was present).</returns>
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static TValue AddOrUpdate<TKey, TValue, TArg>(
            this ConcurrentDictionary<TKey, TValue> concurrentDictionary,
            TKey key,
            Func<TKey, TArg, TValue> addValueFactory,
            Func<TKey, TValue, TArg, TValue> updateValueFactory,
            TArg factoryArgument)
        {
            if (concurrentDictionary is null)
            {
                throw new NullReferenceException();
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (addValueFactory is null)
            {
                throw new ArgumentNullException(nameof(addValueFactory));
            }

            if (updateValueFactory is null)
            {
                throw new ArgumentNullException(nameof(updateValueFactory));
            }

            return concurrentDictionary.AddOrUpdate(
                key,
                elementKey => addValueFactory(elementKey, factoryArgument),
                (elementKey, elementValue) => updateValueFactory(elementKey, elementValue, factoryArgument));
        }

#endif

#if GREATERTHAN_NET35 || GREATERTHAN_NETSTANDARD10 || NETCOREAPP1_0 || GREATERTHAN_NETCOREAPP10

        /// <summary>Removes a key and value from the dictionary.</summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey,TValue}"/> representing the key and value to remove.</param>
        /// <returns>
        /// true if the key and value represented by <paramref name="item"/> are successfully
        /// found and removed; otherwise, false.
        /// </returns>
        /// <remarks>
        /// Both the specifed key and value must match the entry in the dictionary for it to be removed.
        /// The key is compared using the dictionary's comparer (or the default comparer for <typeparamref name="TKey"/>
        /// if no comparer was provided to the dictionary when it was constructed).  The value is compared using the
        /// default comparer for <typeparamref name="TValue"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <see cref="KeyValuePair{TKey, TValue}.Key"/> property of <paramref name="item"/> is a null reference.
        /// </exception>
        public static bool TryRemove<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> concurrentDictionary,
            KeyValuePair<TKey, TValue> item)
        {
            if (concurrentDictionary is null)
            {
                throw new NullReferenceException();
            }

            if (item.Key is null)
            {
                throw new ArgumentNullException(nameof(item), "ItemKeyIsNull");
            }

            return ((ICollection<KeyValuePair<TKey, TValue>>)concurrentDictionary).Remove(item);
        }

#endif
    }
}