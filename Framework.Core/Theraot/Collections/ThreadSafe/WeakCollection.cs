// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Linq;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public class WeakCollection<T, TNeedle> : ICollection<T>
        where T : class
        where TNeedle : WeakNeedle<T>, new()
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly SafeCollection<TNeedle> _wrapped;
        private StructNeedle<WeakNeedle<EventHandler>> _eventHandler;

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
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _wrapped = new SafeCollection<TNeedle>();
            if (autoRemoveDeadItems)
            {
                RegisterForAutoRemoveDeadItemsExtracted();
            }
            else
            {
                GC.SuppressFinalize(this);
            }
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
            var needle = new TNeedle
            {
                Value = item
            };
            _wrapped.Add(needle);
        }

        public void Clear()
        {
            var displaced = _wrapped.ClearEnumerable();
            foreach (var item in displaced)
            {
                item.Dispose();
            }
        }

        public bool Contains(T item)
        {
            Predicate<TNeedle> check = Check(item);
            return _wrapped.Where(check).Any();
        }

        public bool Contains(Predicate<T> itemCheck)
        {
            if (itemCheck == null)
            {
                throw new ArgumentNullException(nameof(itemCheck));
            }
            Predicate<TNeedle> check = Check(itemCheck);
            return _wrapped.Where(check).Any();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
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
                if (pair.TryGetValue(out T result))
                {
                    yield return result;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(T item)
        {
            Predicate<TNeedle> check = Check(item);
            foreach (var removed in _wrapped.RemoveWhereEnumerable(check))
            {
                removed.Dispose();
                return true;
            }
            return false;
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhere(input => !input.IsAlive);
        }

        public int RemoveWhere(Predicate<T> itemCheck)
        {
            Predicate<TNeedle> check = Check(itemCheck);
            return _wrapped.RemoveWhere(check);
        }

        public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> itemCheck)
        {
            Predicate<TNeedle> check = Check(itemCheck);
            foreach (var removed in _wrapped.RemoveWhereEnumerable(check))
            {
                if (removed.TryGetValue(out T value))
                {
                    yield return value;
                }
                removed.Dispose();
            }
        }

        protected void Add(TNeedle needle)
        {
            _wrapped.Add(needle);
        }

        protected bool Contains(Predicate<TNeedle> needleCheck)
        {
            return _wrapped.Where(needleCheck).Any();
        }

        protected IEnumerable<TNeedle> GetNeedleEnumerable()
        {
            return _wrapped;
        }

        protected IEnumerable<T> RemoveWhereEnumerable(Predicate<TNeedle> needleCheck)
        {
            foreach (var removed in _wrapped.RemoveWhereEnumerable(needleCheck))
            {
                if (removed.TryGetValue(out T value))
                {
                    yield return value;
                }
                removed.Dispose();
            }
        }

        private static Predicate<TNeedle> Check(Predicate<T> itemCheck)
        {
            return input =>
            {
                if (input.TryGetValue(out T value))
                {
                    return itemCheck(value);
                }
                return false;
            };
        }

        private Predicate<TNeedle> Check(T item)
        {
            return input =>
            {
                if (input.TryGetValue(out T value))
                {
                    return _comparer.Equals(item, value);
                }
                return false;
            };
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
            var result = false;
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
            if (_eventHandler.Value.Retrieve(out EventHandler eventHandler))
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