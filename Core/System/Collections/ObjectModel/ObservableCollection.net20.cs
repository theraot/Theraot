#if NET20

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Theraot.Threading;

namespace System.Collections.ObjectModel
{
    [Serializable]
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        // Using TrackingThreadLocal instead of NoTrackingThreadLocal or ThreadLocal to avoid not managed resources
        // This field is disposable and will not be disposed
        private readonly TrackingThreadLocal<int> _entryCheck;

        public ObservableCollection()
            : base(new List<T>())
        {
            _entryCheck = new TrackingThreadLocal<int>();
        }

        public ObservableCollection(IEnumerable<T> collection)
            : base(new List<T>(collection))
        {
            _entryCheck = new TrackingThreadLocal<int>();
        }

        public ObservableCollection(List<T> list)
            : base(new List<T>(list))
        {
            _entryCheck = new TrackingThreadLocal<int>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected IDisposable BlockReentrancy()
        {
            return DisposableAkin.Create(() => _entryCheck.Value--);
        }

        protected void CheckReentrancy()
        {
            int value;
            if (_entryCheck.TryGetValue(out value) && value > 0)
            {
                throw new InvalidOperationException("Reentry");
            }
        }

        protected override void ClearItems()
        {
            CheckReentrancy();
            base.ClearItems();
            InvokePropertyChanged("Count");
            InvokePropertyChanged("Item[]");
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();
            base.InsertItem(index, item);
            InvokePropertyChanged("Count");
            InvokePropertyChanged("Item[]");
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            CheckReentrancy();
            // While it is tempting to use monitor here, this class is not really meant to be thread-safe
            // Also, let it fail
            var item = base[index];
            base.RemoveItem(index);
            InvokePropertyChanged("Count");
            InvokePropertyChanged("Item[]");
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();
            // While it is tempting to use monitor here, this class is not really meant to be thread-safe
            // Also, let it fail
            T item = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);
            InvokePropertyChanged("Item[]");
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        protected override void SetItem(int index, T item)
        {
            CheckReentrancy();
            // While it is tempting to use monitor here, this class is not really meant to be thread-safe
            // Also, let it fail
            T oldItem = base[index];
            base.SetItem(index, item);
            InvokePropertyChanged("Item[]");
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
        }

        private void InvokeCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            var collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                try
                {
                    _entryCheck.Value++;
                    collectionChanged.Invoke(this, eventArgs);
                }
                finally
                {
                    _entryCheck.Value--;
                }
            }
        }
        private void InvokePropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                try
                {
                    _entryCheck.Value++;
                    propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                finally
                {
                    _entryCheck.Value--;
                }
            }
        }
    }
}

#endif