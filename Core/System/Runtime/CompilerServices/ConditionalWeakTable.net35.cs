#if NET35

using System.Runtime.InteropServices;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

// Note: the class ConditionalWeakTable is meant to be a weak key dictionary (not a weak value dictionary)
//
// FROM MSDN:
//
// The ConditionalWeakTable<TKey, TValue> class differs from other collection objects in its management of the object lifetime of keys stored in the collection.
// Ordinarily, when an object is stored in a collection, its lifetime lasts until it is removed (and there are no additional references to the object)
// or until the collection object itself is destroyed.
// 
// However, in the ConditionalWeakTable<TKey, TValue> class, adding a key/value pair to the table does not ensure that the key will persist,
// even if it can be reached directly from a value stored in the table (for example, if the table contains one key, A, with a value V1, and a second key, B, with a value P2 that contains a reference to A).
// Instead, ConditionalWeakTable<TKey, TValue> automatically removes the key/value entry as soon as no other references to a key exist outside the table. The example provides an illustration.
//                                             ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//
// The above means that we keep a weak reference to the key but an strong reference to the value
// This is the kind of dictionary that Theraot.Collections.ThreadSafe.WeakDictionary<TKey, TValue, TNeedle> is
// Since microsoft's is failing tests, I'll replace their implementation with WeakDictionary<TKey, TValue, TNeedle>...
// Because at least I know how to maintain WeakDictionary<TKey, TValue, TNeedle> also...
// Having WeakDictionary<TKey, TValue, TNeedle> and ConditionalWeakTable<TKey, TValue> is kind of redundant.
// In fact this was one of the intentions behind creating WeakDictionary<TKey, TValue, TNeedle> in the first place

namespace System.Runtime.CompilerServices
{
    [ComVisible(false)]
    public sealed class ConditionalWeakTable<TKey, TValue>
    where TKey : class
        where TValue : class
    {
        private WeakDictionary<TKey, TValue, WeakNeedle<TKey>> _wrapped;

        public ConditionalWeakTable()
        {
            _wrapped = new WeakDictionary<TKey, TValue, WeakNeedle<TKey>>();
        }

        /// <summary>
        /// Represents a method that creates a non-default value to add as part of a key/value pair to a ConditionalWeakTable<TKey, TValue> object. 
        /// </summary>
        /// <param name="key">The key that belongs to the value to create.</param>
        /// <returns>An instance of a reference type that represents the value to attach to the specified key.</returns>
        public delegate TValue CreateValueCallback(TKey key);

        /// <summary>
        /// Adds a key to the table.
        /// </summary>
        /// <param name="key">The key to add. key represents the object to which the property is attached.</param>
        /// <param name="value">The key's property value.</param>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            _wrapped.AddNew(key, value);
        }

        /// <summary>
        /// Atomically searches for a specified key in the table and returns the corresponding value. If the key does not exist in the table, the method invokes the default constructor of the class that represents the table's value to create a value that is bound to the specified key. 
        /// </summary>
        /// <param name="key">The key to search for. key represents the object to which the property is attached.</param>
        /// <returns>The value that corresponds to key, if key already exists in the table; otherwise, a new value created by the default constructor of the class defined by the TValue generic type parameter.</returns>
        public TValue GetOrCreateValue(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return PrivateGetValue(key, k => Activator.CreateInstance<TValue>());
        }

        /// <summary>
        /// Atomically searches for a specified key in the table and returns the corresponding value.If the key does not exist in the table, the method invokes a callback method to create a value that is bound to the specified key.
        /// </summary>
        /// <param name="key">The key to search for. key represents the object to which the property is attached.</param>
        /// <param name="createValueCallback">A delegate to a method that can create a value for the given key. It has a single parameter of type TKey, and returns a value of type TValue.</param>
        /// <returns>The value attached to key, if key already exists in the table; otherwise, the new value returned by the createValueCallback delegate.</returns>
        public TValue GetValue(TKey key, CreateValueCallback createValueCallback)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (createValueCallback == null)
            {
                throw new ArgumentNullException("createValueCallback");
            }
            return _wrapped.GetOrAdd(key, input => createValueCallback(input));
        }

        private TValue PrivateGetValue(TKey key, Func<TKey, TValue> createValueCallback)
        {
            return _wrapped.GetOrAdd(key, createValueCallback);
        }

        /// <summary>
        /// Removes a key and its value from the table.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>
        ///    <c>true</c> if the key is found and removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return _wrapped.Remove(key);
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key that represents an object with an attached property.</param>
        /// <param name="value">When this method returns, contains the attached property value. If key is not found, value contains the default value.</param>
        /// <returns>
        ///    <c>true</c> if the key is found; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return _wrapped.TryGetValue(key, out value);
        }
    }
}

#endif