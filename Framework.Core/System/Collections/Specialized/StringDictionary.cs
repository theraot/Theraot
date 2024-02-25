#if LESSTHAN_NETSTANDARD20

#pragma warning disable CA1308 // Normalize strings to uppercase

using System.Collections.Generic;

namespace System.Collections.Specialized
{
    [Serializable]
    public class StringDictionary : IEnumerable
    {
        private Dictionary<String, String> _wrapped = new Dictionary<String, String>();
        private object _syncroot = new object();

        public virtual int Count
        {
            get
            {
                return _wrapped.Count;
            }
        }

        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public virtual string this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return _wrapped[key.ToLowerInvariant()];
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                _wrapped[key.ToLowerInvariant()] = value;
            }
        }

        public virtual ICollection Keys
        {
            get
            {
                return _wrapped.Keys;
            }
        }

        public virtual object SyncRoot
        {
            get
            {
                return _syncroot;
            }
        }

        public virtual ICollection Values
        {
            get
            {
                return _wrapped.Values;
            }
        }

        public virtual void Add(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _wrapped.Add(key.ToLowerInvariant(), value);
        }

        internal IDictionary<string, string> AsGenericDictionary()
        {
            return _wrapped;
        }

        public virtual void Clear()
        {
            _wrapped.Clear();
        }

        public virtual bool ContainsKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _wrapped.ContainsKey(key.ToLowerInvariant());
        }

        public virtual bool ContainsValue(string value)
        {
            return _wrapped.ContainsValue(value);
        }

        public virtual void CopyTo(Array array, int index)
        {
            ((ICollection)_wrapped).CopyTo(array, index);
        }

        public virtual IEnumerator GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public virtual void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _wrapped.Remove(key.ToLowerInvariant());
        }
    }
}

#endif