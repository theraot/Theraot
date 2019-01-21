// Needed for Workaround

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Theraot.Threading;
using Theraot.Threading.Needles;

namespace Theraot.Collections.ThreadSafe
{
    [DebuggerNonUserCode]
    [DebuggerDisplay("Count={Count}")]
    public class WeakCollection<T, TNeedle> : ICollection<T>
        where T : class
        where TNeedle : WeakNeedle<T>, new()
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly SafeCollection<TNeedle> _wrapped;
        private WeakNeedle<EventHandler> _eventHandler;

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

        public bool AutoRemoveDeadItems
        {
            get => _eventHandler.IsAlive;

            set
            {
                if (value == _eventHandler.IsAlive)
                {
                    return;
                }

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

        public int Count => _wrapped.Count;

        bool ICollection<T>.IsReadOnly => false;

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
                item.Free();
            }
        }

        public bool Contains(T item)
        {
            var check = Check(item);
            return _wrapped.Where(check).Any();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(Count, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var pair in _wrapped)
            {
                if (pair.TryGetValue(out var result))
                {
                    yield return result;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(T item)
        {
            var check = Check(item);
            foreach (var removed in _wrapped.RemoveWhereEnumerable(check))
            {
                removed.Free();
                return true;
            }

            return false;
        }

        ~WeakCollection()
        {
            UnRegisterForAutoRemoveDeadItemsExtracted();
        }

        public bool Contains(Predicate<T> itemCheck)
        {
            if (itemCheck == null)
            {
                throw new ArgumentNullException(nameof(itemCheck));
            }

            var check = Check(itemCheck);
            return _wrapped.Where(check).Any();
        }

        public int RemoveDeadItems()
        {
            return _wrapped.RemoveWhere(input => !input.IsAlive);
        }

        public int RemoveWhere(Predicate<T> itemCheck)
        {
            var check = Check(itemCheck);
            return _wrapped.RemoveWhere(check);
        }

        public IEnumerable<T> RemoveWhereEnumerable(Predicate<T> itemCheck)
        {
            var check = Check(itemCheck);
            foreach (var removed in _wrapped.RemoveWhereEnumerable(check))
            {
                if (removed.TryGetValue(out var value))
                {
                    yield return value;
                }

                removed.Free();
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
                if (removed.TryGetValue(out var value))
                {
                    yield return value;
                }

                removed.Free();
            }
        }

        private static Predicate<TNeedle> Check(Predicate<T> itemCheck)
        {
            return input =>
            {
                if (input.TryGetValue(out var value))
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
                if (input.TryGetValue(out var value))
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
            var eventHandler = _eventHandler.Value;
            if (eventHandler == null)
            {
                eventHandler = GarbageCollected;
                _eventHandler = new WeakNeedle<EventHandler>(eventHandler);
                result = true;
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
            if (_eventHandler.Retrieve(out var eventHandler))
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