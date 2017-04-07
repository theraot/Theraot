// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class WeakCollection<T, TNeedle> : ICollection<T>
        where T : class
        where TNeedle : WeakNeedle<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly SafeDictionary<int, TNeedle> _wrapped;
        private StructNeedle<WeakNeedle<EventHandler>> _eventHandler;
        private int _maxIndex;

        public WeakCollection()
            : this(null, true)
        {
            // Empty
        }

        public WeakCollection(IEqualityComparer<T> comparer)
            : this(comparer, true)
        {
            // Empty
        }

        public WeakCollection(bool autoRemoveDeadItems)
            : this(null, autoRemoveDeadItems)
        {
            // Empty
        }

        public WeakCollection(IEqualityComparer<T> comparer, bool autoRemoveDeadItems)
        {
            _maxIndex = -1;
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _wrapped = new SafeDictionary<int, TNeedle>();
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        public WeakCollection(IEqualityComparer<T> comparer, int initialProbing)
            : this(comparer, true, initialProbing)
        {
            // Empty
        }

        public WeakCollection(bool autoRemoveDeadItems, int initialProbing)
            : this(null, autoRemoveDeadItems, initialProbing)
        {
            // Empty
        }

        public WeakCollection(IEqualityComparer<T> comparer, bool autoRemoveDeadItems, int initialProbing)
        {
            _maxIndex = -1;
#if FAT
            _comparer = comparer ?? EqualityComparer<T>.Default;
#else
            _comparer = comparer ?? EqualityComparer<T>.Default;
#endif
            _wrapped = new SafeDictionary<int, TNeedle>(initialProbing);
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        public WeakCollection(int initialProbing)
            : this(null, true, initialProbing)
        {
            // Empty
        }

        ~WeakCollection()
        {
            UnRegisterForAutoRemoveDeadItemsExtracted();
        }

        public bool AutoRemoveDeadItems
        {
            get { return _eventHandler.IsAlive; }

            set
            {
                if (value)
                {
                    RegisterForAutoRemoveDeadItems();
                }
                else
                {
                    UnRegisterForAutoRemoveDeadItems();
                }
            }
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            var needle = NeedleHelper.CreateNeedle<T, TNeedle>(item);
            _wrapped.Set(Interlocked.Increment(ref _maxIndex), needle);
        }

        public void Clear()
        {
            var displaced = _wrapped.ClearEnumerable();
            foreach (var item in displaced)
            {
                item.Value.Dispose();
            }
        }

        public bool Contains(T item)
        {
            foreach (var input in this)
            {
                if (_comparer.Equals(input, item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(Predicate<T> itemCheck)
        {
            foreach (var input in this)
            {
                if (itemCheck(input))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public bool Equals(T x, T y)
        {
            return _comparer.Equals(x, y);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var pair in _wrapped)
            {
                T result;
                if (pair.Value.TryGetValue(out result))
                {
                    yield return result;
                }
            }
        }

        public bool Remove(T item)
        {
            Predicate<TNeedle> check = input =>
            {
                T value;
                if (input.TryGetValue(out value))
                {
                    return _comparer.Equals(item, value);
                }
                return false;
            };
            foreach (var removed in _wrapped.RemoveWhereValueEnumerable(check))
            {
                removed.Dispose();
                return true;
            }
            return false;
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhere(input => !input.Value.IsAlive);
        }

        public int RemoveWhere(Predicate<T> itemCheck)
        {
            Predicate<TNeedle> check = input =>
            {
                T value;
                if (input.TryGetValue(out value))
                {
                    return itemCheck(value);
                }
                return false;
            };
            return _wrapped.RemoveWhereValue(check);
        }

        public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> itemCheck)
        {
            Predicate<TNeedle> check = input =>
            {
                T value;
                if (input.TryGetValue(out value))
                {
                    return itemCheck(value);
                }
                return false;
            };
            foreach (var removed in _wrapped.RemoveWhereValueEnumerable(check))
            {
                T value;
                if (removed.TryGetValue(out value))
                {
                    yield return value;
                }
                removed.Dispose();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void Add(TNeedle needle)
        {
            _wrapped.Set(Interlocked.Increment(ref _maxIndex), needle);
        }

        protected bool Contains(Predicate<TNeedle> needleCheck)
        {
            foreach (var pair in _wrapped)
            {
                if (needleCheck(pair.Value))
                {
                    return true;
                }
            }
            return false;
        }

        protected IEnumerable<TNeedle> GetNeedleEnumerable()
        {
            foreach (var pair in _wrapped)
            {
                yield return pair.Value;
            }
        }

        protected IEnumerable<T> RemoveWhereEnumerable(Predicate<TNeedle> needleCheck)
        {
            foreach (var removed in _wrapped.RemoveWhereValueEnumerable(needleCheck))
            {
                T value;
                if (removed.TryGetValue(out value))
                {
                    yield return value;
                }
                removed.Dispose();
            }
        }

        private void GarbageCollected(object sender, EventArgs e)
        {
            RemoveDeadItems();
        }

        private void RegisterForAutoRemoveDeadItems()
        {
            if (RegisterForAutoRemoveDeadItemsExtracted())
            {
                GC.ReRegisterForFinalize(this);
            }
        }

        private bool RegisterForAutoRemoveDeadItemsExtracted()
        {
            bool result = false;
            EventHandler eventHandler;
            if (ReferenceEquals(_eventHandler.Value, null))
            {
                eventHandler = GarbageCollected;
                _eventHandler = new WeakNeedle<EventHandler>(eventHandler);
                result = true;
            }
            else
            {
                eventHandler = _eventHandler.Value.Value;
                if (!_eventHandler.IsAlive)
                {
                    eventHandler = GarbageCollected;
                    _eventHandler.Value = eventHandler;
                    result = true;
                }
            }
            GCMonitor.Collected += eventHandler;
            return result;
        }

        private void UnRegisterForAutoRemoveDeadItems()
        {
            if (UnRegisterForAutoRemoveDeadItemsExtracted())
            {
                GC.SuppressFinalize(this);
            }
        }

        private bool UnRegisterForAutoRemoveDeadItemsExtracted()
        {
            EventHandler eventHandler;
            if (_eventHandler.Value.Retrieve(out eventHandler))
            {
                GCMonitor.Collected -= eventHandler;
                _eventHandler.Value = null;
                return true;
            }
            _eventHandler.Value = null;
            return false;
        }
    }
}