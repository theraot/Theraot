using System;
using System.Collections.Generic;
using System.Threading;

namespace Theraot.Collections.ThreadSafe
{
    [Serializable]
    internal class Branch : IEnumerable<object>
    {
        private const int INT_Capacity = 1 << INT_OffsetStep;
        private const int INT_OffsetStep = 4;

        private static readonly Pool<Branch> _branchPool;
        private object[] _buffer;
        private object[] _entries;
        private int _offset;
        private Branch _parent;
        private int _subindex;
        private int _useCount;

        static Branch()
        {
            _branchPool = new Pool<Branch>(16, Recycle);
        }

        private Branch(int offset, Branch parent, int subindex)
        {
            _offset = offset;
            _parent = parent;
            _subindex = subindex;
            _entries = ArrayReservoir<object>.GetArray(INT_Capacity);
            _buffer = ArrayReservoir<object>.GetArray(INT_Capacity);
        }

        ~Branch()
        {
            if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                ArrayReservoir<object>.DonateArray(_entries);
                ArrayReservoir<object>.DonateArray(_buffer);
            }
        }

        public static Branch Create(int offset, Branch parent, int subindex)
        {
            Branch result;
            if (_branchPool.TryGet(out result))
            {
                result._offset = offset;
                result._parent = parent;
                result._subindex = subindex;
                result._entries = ArrayReservoir<object>.GetArray(INT_Capacity);
                result._buffer = ArrayReservoir<object>.GetArray(INT_Capacity);
                return result;
            }
            return new Branch(offset, parent, subindex);
        }

        public bool Exchange(uint index, object item, out object previous)
        {
            // Get the target branches
            int resultCount;
            var branches = Map(index, out resultCount);
            // ---
            var branch = branches[resultCount - 1];
            var result = branch.PrivateExchange(index, item, out previous);
            Leave(branches, resultCount);
            return result;
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var child in _entries)
            {
                if (!ReferenceEquals(child, null))
                {
                    var items = child as Branch;
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        yield return child;
                    }
                }
            }
        }

        public bool Insert(uint index, object item, out object previous)
        {
            // Get the target branches
            int resultCount;
            var branches = Map(index, out resultCount);
            // ---
            var branch = branches[resultCount - 1];
            var result = branch.PrivateInsert(index, item, out previous);
            Leave(branches, resultCount);
            return result;
            // if this returns true, the new item was inserted, so there was no previous item
            // if this returns false, something was inserted first... so we get the previous item
        }

        public bool RemoveAt(uint index, out object previous)
        {
            previous = null;
            // Get the target branch  - can be null
            var branch = MapReadonly(index);
            // Check if we got a branch
            if (branch == null)
            {
                // We didn't get a branch, meaning that what we look for is not there
                return false;
            }
            // ---
            if (branch.PrivateRemoveAt(index, out previous))
            {
                branch.Shrink();
                return true;
            }
            return false;
        }

        public void Set(uint index, object value, out bool isNew)
        {
            // Get the target branches
            int resultCount;
            var branches = Map(index, out resultCount);
            // ---
            var branch = branches[resultCount - 1];
            branch.PrivateSet(index, value, out isNew);
            Leave(branches, resultCount);
            // if this returns true, the new item was inserted, so isNew is set to true
            // if this returns false, some other thread inserted first... so isNew is set to false
            // yet we pretend we inserted first and the value was replaced by the other thread
            // So we say the operation was a success regardless
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGet(uint index, out object value)
        {
            value = null;
            // Get the target branch  - can be null
            var branch = MapReadonly(index);
            // Check if we got a branch
            if (branch == null)
            {
                // We didn't get a branch, meaning that what we look for is not there
                return false;
            }
            // ---
            return branch.PrivateTryGet(index, out value);
        }

        public IEnumerable<object> Where(Predicate<object> predicate)
        {
            // Do not convert to foreach - foreach will stop working if the collection is modified, this should not
            for (var index = 0; index < _entries.Length; index++)
            {
                var child = _entries[index];
                if (!ReferenceEquals(child, null))
                {
                    var items = child as Branch;
                    if (items != null)
                    {
                        foreach (var item in items.Where(predicate))
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        if (predicate(child))
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        internal bool TryGetCheckRemoveAt(uint index, Predicate<object> check, out object previous)
        {
            previous = null;
            // Get the target branch  - can be null
            var branch = MapReadonly(index);
            // Check if we got a branch
            if (branch == null)
            {
                // We didn't get a branch, meaning that what we look for is not there
                return false;
            }
            // ---
            return branch.PrivateTryGetCheckRemoveAt(index, check, out previous);
        }

        internal bool TryGetCheckSet(uint index, object item, Predicate<object> check, out bool isNew)
        {
            // Get the target branches
            int resultCount;
            var branches = Map(index, out resultCount);
            // ---
            var branch = branches[resultCount - 1];
            var result = branch.PrivateTryGetCheckSet(index, item, check, out isNew);
            Leave(branches, resultCount);
            return result;
        }

        private static void Leave(Branch[] branches, int resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                var branch = branches[index];
                Interlocked.Decrement(ref branch._useCount);
            }
            ArrayReservoir<Branch>.DonateArray(branches);
        }

        private static void Recycle(Branch branch)
        {
            branch._entries = null;
            branch._buffer = null;
            branch._useCount = 0;
            branch._parent = null;
        }

        private int GetSubindex(uint index)
        {
            return (int)((index >> _offset) & 0xF);
        }

        private Branch Grow(uint index)
        {
            // Grow is never called when _offset == 0
            Interlocked.Increment(ref _useCount); // We are most likely to add - overstatimate count
            var offset = _offset - INT_OffsetStep;
            var subindex = GetSubindex(index);
            var node = _entries[subindex];
            // node can only be Branch or null
            if (node != null)
            {
                // Already grown
                return node as Branch;
            }
            var branch = Create(offset, this, subindex);
            var result = Interlocked.CompareExchange(ref _buffer[subindex], branch, null);
            if (result == null)
            {
                result = branch;
            }
            else
            {
                _branchPool.Donate(branch);
            }
            var found = Interlocked.CompareExchange(ref _entries[subindex], result, null);
            if (found == null)
            {
                Interlocked.Exchange(ref _buffer[subindex], null);
                return result as Branch;
            }
            Interlocked.Decrement(ref _useCount); // We did not add after all
            return this;
        }

        private Branch[] Map(uint index, out int resultCount)
        {
            var result = ArrayReservoir<Branch>.GetArray(16);
            var branch = this;
            resultCount = 0;
            while (true)
            {
                Interlocked.Increment(ref branch._useCount);
                result[resultCount] = branch;
                resultCount++;
                // do we need a leaf?
                if (branch._offset == 0)
                {
                    // It is not responsability of this method to handle leafs
                    return result;
                }
                object found;
                if (branch.PrivateTryGetBranch(index, out found))
                {
                    // if found were null, PrivateTryGetBranch would have returned false
                    // if found cannot be a leaf, because Leaf only appear on _offset == 0
                    // only Branch and Leaf are inserted, ergo... found is Branch
                    branch = (Branch)found;
                    continue;
                }
                branch = branch.Grow(index);
            }
        }

        private Branch MapReadonly(uint index)
        {
            var branch = this;
            while (true)
            {
                // do we need a leaf?
                if (branch._offset == 0)
                {
                    // It is not responsability of this method to handle leafs
                    return branch;
                }
                object found;
                if (branch.PrivateTryGetBranch(index, out found))
                {
                    // if found were null, PrivateTryGetBranch would have returned false
                    // if found cannot be a leaf, because Leaf only appear on _offset == 0
                    // only Branch and Leaf are inserted, ergo... found is Branch
                    branch = (Branch)found;
                    continue;
                }
                return null;
            }
        }

        private bool PrivateExchange(uint index, object item, out object previous)
        {
            Interlocked.Increment(ref _useCount); // We are most likely to add - overstatimate count
            var subindex = GetSubindex(index);
            previous = Interlocked.Exchange(ref _entries[subindex], item ?? BucketHelper.Null);
            if (previous == null)
            {
                return true;
            }
            if (previous == BucketHelper.Null)
            {
                previous = null;
            }
            Interlocked.Decrement(ref _useCount); // We did not add after all
            return false;
        }

        private bool PrivateInsert(uint index, object item, out object previous)
        {
            Interlocked.Increment(ref _useCount); // We are most likely to add - overstatimate count
            var subindex = GetSubindex(index);
            previous = Interlocked.CompareExchange(ref _entries[subindex], item ?? BucketHelper.Null, null);
            if (previous == null)
            {
                return true;
            }
            if (previous == BucketHelper.Null)
            {
                previous = null;
            }
            Interlocked.Decrement(ref _useCount); // We did not add after all
            return false;
        }

        private bool PrivateRemoveAt(uint index, out object previous)
        {
            var subindex = GetSubindex(index);
            try
            {
                previous = Interlocked.Exchange(ref _entries[subindex], null);
                if (previous == null)
                {
                    return false;
                }
                if (previous == BucketHelper.Null)
                {
                    previous = null;
                }
                Interlocked.Decrement(ref _useCount);
                return true;
            }
            catch (NullReferenceException)
            {
                // Eating null reference, the branch has been removed
                previous = null;
                return false;
            }
        }

        private void PrivateSet(uint index, object item, out bool isNew)
        {
            Interlocked.Increment(ref _useCount); // We are most likely to add - overstatimate count
            var subindex = GetSubindex(index);
            isNew = false;
            var previous = Interlocked.Exchange(ref _entries[subindex], item ?? BucketHelper.Null);
            if (previous == null)
            {
                isNew = true;
            }
            else
            {
                Interlocked.Decrement(ref _useCount); // We did not add after all
            }
        }

        private bool PrivateTryGet(uint index, out object previous)
        {
            var subindex = GetSubindex(index);
            try
            {
                previous = Interlocked.CompareExchange(ref _entries[subindex], null, null);
                if (previous == null)
                {
                    return false;
                }
                if (previous == BucketHelper.Null)
                {
                    previous = null;
                }
                return true;
            }
            catch (NullReferenceException)
            {
                // Eating null reference, the branch has been removed
                previous = null;
                return false;
            }
        }

        private bool PrivateTryGetBranch(uint index, out object previous)
        {
            var subindex = GetSubindex(index);
            try
            {
                previous = Interlocked.CompareExchange(ref _entries[subindex], null, null);
                if (previous == null)
                {
                    return false;
                }
                return true;
            }
            catch (NullReferenceException)
            {
                // Eating null reference, the branch has been removed
                previous = null;
                return false;
            }
        }

        private bool PrivateTryGetCheckRemoveAt(uint index, Predicate<object> check, out object previous)
        {
            object found;
            var subindex = GetSubindex(index);
            try
            {
                found = Interlocked.CompareExchange(ref _entries[subindex], null, null);
                if (found == null)
                {
                    previous = null;
                    return false;
                }
                if (found == BucketHelper.Null)
                {
                    found = null;
                }
            }
            catch (NullReferenceException)
            {
                // Eating null reference, the branch has been removed
                previous = null;
                return false;
            }
            // -- Found
            bool checkResult = check(found);
            try
            {
                if (checkResult)
                {
                    // -- Passed
                    previous = Interlocked.Exchange(ref _entries[subindex], null);
                    if (previous == null)
                    {
                        return false;
                    }
                    if (previous == BucketHelper.Null)
                    {
                        previous = null;
                    }
                    Interlocked.Decrement(ref _useCount);
                    return true;
                }
                previous = null;
                return false;
            }
            catch (NullReferenceException)
            {
                // Eating null reference, the branch has been removed
                previous = null;
                return false;
            }
        }

        private bool PrivateTryGetCheckSet(uint index, object item, Predicate<object> check, out bool isNew)
        {
            Interlocked.Increment(ref _useCount); // We are most likely to add - overstatimate count
            isNew = false;
            var subindex = GetSubindex(index);
            var found = Interlocked.CompareExchange(ref _entries[subindex], null, null);
            if (found == null)
            {
                // -- Not found TryAdd
                var previous = Interlocked.CompareExchange(ref _entries[subindex], item ?? BucketHelper.Null, null);
                if (previous == null)
                {
                    isNew = true;
                    return true;
                }
                Interlocked.Decrement(ref _useCount); // We did not add after all
                return false;
            }
            if (found == BucketHelper.Null)
            {
                found = null;
            }
            // -- Found
            bool checkResult;
            try
            {
                checkResult = check(found);
            }
            finally
            {
                Interlocked.Decrement(ref _useCount); // We did not add after all
            }
            if (checkResult)
            {
                // -- Passed
                // This works under the presumption that check will result true to whatever value may have replaced found...
                // That's why we don't use CompareExchange, but simply Exchange instead
                // And also that's why this method is internal, we cannot guarantee the presumption outside internal code.
                var previous = Interlocked.Exchange(ref _entries[subindex], item ?? BucketHelper.Null);
                if (previous == null)
                {
                    isNew = true;
                }
                else
                {
                    Interlocked.Decrement(ref _useCount); // We did not add after all
                }
                return true;
            }
            return false;
        }

        private void Shrink()
        {
            if
                (
                _parent != null
                && Interlocked.CompareExchange(ref _parent._buffer[_subindex], this, null) == null
                && Interlocked.CompareExchange(ref _useCount, 0, 0) == 0
                && Interlocked.CompareExchange(ref _parent._entries[_subindex], null, this) == this // Did --
                )
            {
                if (Interlocked.CompareExchange(ref _useCount, 0, 0) == 0)
                {
                    var found = Interlocked.CompareExchange(ref _parent._buffer[_subindex], null, this);
                    if (found == this)
                    {
                        var parent = _parent;
                        _branchPool.Donate(this);
                        Interlocked.Decrement(ref parent._useCount); // did not undo --
                        parent.Shrink();
                    }
                }
                else
                {
                    var found = Interlocked.CompareExchange(ref _parent._entries[_subindex], _parent._buffer[_subindex], null);
                    if (found != null)
                    {
                        var parent = _parent;
                        _branchPool.Donate(this);
                        Interlocked.Decrement(ref parent._useCount); // did not undo --
                    }
                }
            }
        }
    }
}