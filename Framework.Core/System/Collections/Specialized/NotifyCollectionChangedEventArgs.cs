#if LESSTHAN_NET40

namespace System.Collections.Specialized
{
    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            Action = action;
            if (action != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException("This constructor can only be used with the Reset action.", nameof(action));
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
            : this(action, changedItems, -1)
        {
            // Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
            : this(action, changedItem, -1)
        {
            // Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            : this(action, newItems, oldItems, -1)
        {
            // Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            Action = action;
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    if (changedItems == null)
                    {
                        throw new ArgumentNullException(nameof(changedItems));
                    }

                    if (startingIndex < -1)
                    {
                        throw new ArgumentException("The value of startingIndex must be -1 or greater.", nameof(startingIndex));
                    }

                    if (action == NotifyCollectionChangedAction.Add)
                    {
                        InitializeAdd(changedItems, startingIndex);
                    }
                    else
                    {
                        InitializeRemove(changedItems, startingIndex);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset when changedItems != null:
                    throw new ArgumentException("This constructor can only be used with the Reset action if changedItems is null", nameof(changedItems));
                case NotifyCollectionChangedAction.Reset when startingIndex != -1:
                    throw new ArgumentException("This constructor can only be used with the Reset action if startingIndex is -1", nameof(startingIndex));
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentException("This constructor can only be used with the Reset, Add, or Remove actions.", nameof(action));
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            IList changedItems = new[] {changedItem};
            Action = action;
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    InitializeAdd(changedItems, index);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    InitializeRemove(changedItems, index);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (changedItem != null)
                    {
                        throw new ArgumentException("This constructor can only be used with the Reset action if changedItem is null", nameof(changedItem));
                    }

                    if (index != -1)
                    {
                        throw new ArgumentException("This constructor can only be used with the Reset action if index is -1", nameof(index));
                    }

                    break;

                default:
                    throw new ArgumentException("This constructor can only be used with the Reset, Add, or Remove actions.", nameof(action));
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
            : this(action, newItem, oldItem, -1)
        {
            // Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            Action = action;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("This constructor can only be used with the Replace action.", nameof(action));
            }

            OldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            NewItems = newItems ?? throw new ArgumentNullException(nameof(newItems));
            OldStartingIndex = startingIndex;
            NewStartingIndex = startingIndex;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        {
            Action = action;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException("This constructor can only be used with the Move action.", nameof(action));
            }

            if (index < -1)
            {
                throw new ArgumentException("The value of index must be -1 or greater.", nameof(index));
            }

            InitializeMove(changedItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
            : this(action, new[] {changedItem}, index, oldIndex)
        {
            // Empty
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            Action = action;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException("This constructor can only be used with the Replace action.", nameof(action));
            }

            InitializeReplace(new[] {newItem}, new[] {oldItem}, index);
        }

        public NotifyCollectionChangedAction Action { get; }

        public IList NewItems { get; private set; }

        public int NewStartingIndex { get; private set; } = -1;

        public IList OldItems { get; private set; }

        public int OldStartingIndex { get; private set; } = -1;

        private void InitializeAdd(IList items, int index)
        {
            NewItems = ArrayList.ReadOnly(items);
            NewStartingIndex = index;
        }

        private void InitializeMove(IList changedItems, int newItemIndex, int oldItemIndex)
        {
            InitializeAdd(changedItems, newItemIndex);
            InitializeRemove(changedItems, oldItemIndex);
        }

        private void InitializeRemove(IList items, int index)
        {
            OldItems = ArrayList.ReadOnly(items);
            OldStartingIndex = index;
        }

        private void InitializeReplace(IList addedItems, IList removedItems, int index)
        {
            InitializeAdd(addedItems, index);
            InitializeRemove(removedItems, index);
        }
    }
}

#endif