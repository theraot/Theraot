#if NET20

namespace System.Collections.Specialized
{
    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        private readonly NotifyCollectionChangedAction _action;
        private int _newIndex = -1;
        private IList _newItems;
        private int _oldIndex = -1;
        private IList _oldItems;

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            _action = action;
            if (action != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException("This constructor can only be used with the Reset action.", "action");
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
            : this(action, changedItems, -1)
        {
            //Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
            : this(action, changedItem, -1)
        {
            //Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            : this(action, newItems, oldItems, -1)
        {
            //Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            _action = action;
            if (action == NotifyCollectionChangedAction.Add || action == NotifyCollectionChangedAction.Remove)
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                if (startingIndex < -1)
                {
                    throw new ArgumentException("The value of startingIndex must be -1 or greater.", "startingIndex");
                }
                if (action == NotifyCollectionChangedAction.Add)
                {
                    InitializeAdd(changedItems, startingIndex);
                }
                else
                {
                    InitializeRemove(changedItems, startingIndex);
                }
            }
            else if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException("This constructor can only be used with the Reset action if changedItems is null", "changedItems");
                }
                if (startingIndex != -1)
                {
                    throw new ArgumentException("This constructor can only be used with the Reset action if startingIndex is -1", "startingIndex");
                }
            }
            else
            {
                throw new ArgumentException("This constructor can only be used with the Reset, Add, or Remove actions.", "action");
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            IList changedItems = new[] { changedItem };
            _action = action;
            if (action == NotifyCollectionChangedAction.Add)
            {
                InitializeAdd(changedItems, index);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                InitializeRemove(changedItems, index);
            }
            else if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException("This constructor can only be used with the Reset action if changedItem is null", "changedItem");
                }
                if (index != -1)
                {
                    throw new ArgumentException("This constructor can only be used with the Reset action if index is -1", "index");
                }
            }
            else
            {
                throw new ArgumentException("This constructor can only be used with the Reset, Add, or Remove actions.", "action");
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
            : this(action, newItem, oldItem, -1)
        {
            //Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            _action = action;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("This constructor can only be used with the Replace action.", "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            _oldItems = oldItems;
            _newItems = newItems;
            _oldIndex = startingIndex;
            _newIndex = startingIndex;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        {
            _action = action;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException("This constructor can only be used with the Move action.", "action");
            }
            if (index < -1)
            {
                throw new ArgumentException("The value of index must be -1 or greater.", "index");
            }
            InitializeMove(changedItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
            : this(action, new[] { changedItem }, index, oldIndex)
        {
            //Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            _action = action;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("This constructor can only be used with the Replace action.", "action");
            }
            InitializeReplace(new[] { newItem }, new[] { oldItem }, index);
        }

        public NotifyCollectionChangedAction Action
        {
            get
            {
                return _action;
            }
        }

        public IList NewItems
        {
            get
            {
                return _newItems;
            }
        }

        public int NewStartingIndex
        {
            get
            {
                return _newIndex;
            }
        }

        public IList OldItems
        {
            get
            {
                return _oldItems;
            }
        }

        public int OldStartingIndex
        {
            get
            {
                return _oldIndex;
            }
        }

        private void InitializeAdd(IList items, int index)
        {
            _newItems = ArrayList.ReadOnly(items);
            _newIndex = index;
        }

        private void InitializeMove(IList changedItems, int newItemIndex, int oldItemIndex)
        {
            InitializeAdd(changedItems, newItemIndex);
            InitializeRemove(changedItems, oldItemIndex);
        }

        private void InitializeRemove(IList items, int index)
        {
            _oldItems = ArrayList.ReadOnly(items);
            _oldIndex = index;
        }

        private void InitializeReplace(IList addedItems, IList removedItems, int index)
        {
            InitializeAdd(addedItems, index);
            InitializeRemove(removedItems, index);
        }
    }
}

#endif