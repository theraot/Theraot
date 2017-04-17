#if FAT

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections.Specialized
{
    [Serializable]
    public sealed partial class FlagArray
    {
        private readonly IReadOnlyCollection<bool> _asReadOnly;
        private readonly int _capacity;
        private int[] _entries;

        public FlagArray(FlagArray prototype)
        {
            if (ReferenceEquals(prototype, null))
            {
                throw new ArgumentNullException("prototype", "prototype is null.");
            }
            _capacity = prototype._capacity;
            _entries = ArrayReservoir<int>.GetArray(GetLength(_capacity));
            prototype._entries.CopyTo(_entries, 0);
            _asReadOnly = new ExtendedReadOnlyCollection<bool>(this);
        }

        public FlagArray(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "length < 0");
            }
            _capacity = capacity;
            _entries = ArrayReservoir<int>.GetArray(GetLength(_capacity));
            _asReadOnly = new ExtendedReadOnlyCollection<bool>(this);
        }

        public FlagArray(int capacity, bool defaultValue)
            : this(capacity)
        {
            if (defaultValue)
            {
                Fill(true);
            }
        }

        ~FlagArray()
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (!GCMonitor.FinalizingForUnload)
            {
                RecycleExtracted();
            }
        }

        public int Capacity
        {
            get { return _capacity; }
        }

        public int Count
        {
            get
            {
                var count = 0;
                var index = 0;
                var newindex = 0;
                foreach (var entry in _entries)
                {
                    newindex += 32;
                    if (newindex <= _capacity)
                    {
                        count += NumericHelper.PopulationCount(entry);
                        index = newindex;
                    }
                    else
                    {
                        foreach (var bit in entry.BinaryReverse().BitsBinary())
                        {
                            if (bit == 1)
                            {
                                count++;
                            }
                            index++;
                            if (index == _capacity)
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
                return count;
            }
        }

        public IEnumerable<int> Flags
        {
            get
            {
                var index = 0;
                foreach (var entry in _entries)
                {
                    if (entry == 0)
                    {
                        index += 32;
                        if (index >= _capacity)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        foreach (var bit in entry.BinaryReverse().BitsBinary())
                        {
                            if (bit == 1)
                            {
                                yield return index;
                            }
                            index++;
                            if (index == _capacity)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }
        }

        bool ICollection<bool>.IsReadOnly
        {
            get { return false; }
        }

        public bool this[int index]
        {
            get
            {
                var entryIndex = index >> 5;
                var bit = index & 31;
                var mask = 1 << bit;
                return GetBit(entryIndex, mask);
            }
            set
            {
                var entryIndex = index >> 5;
                var bit = index & 31;
                var mask = 1 << bit;
                if (value)
                {
                    SetBit(entryIndex, mask);
                }
                else
                {
                    UnsetBit(entryIndex, mask);
                }
            }
        }

        public FlagArray Clone()
        {
            return new FlagArray(this);
        }

        public bool Contains(bool item)
        {
            var index = 0;
            var newindex = 0;
            var check = item ? 0 : -1;
            foreach (var entry in _entries)
            {
                newindex += 32;
                if (newindex <= _capacity)
                {
                    if (entry != check)
                    {
                        return true;
                    }
                    index = newindex;
                }
                else
                {
                    foreach (var bit in entry.BinaryReverse().BitsBinary())
                    {
                        if ((bit == 1) == item)
                        {
                            return true;
                        }
                        index++;
                        if (index == _capacity)
                        {
                            break;
                        }
                    }
                    break;
                }
            }
            return false;
        }

        public bool Contains(bool item, IEqualityComparer<bool> comparer)
        {
            return System.Linq.Enumerable.Contains(this, item, comparer);
        }

        public void CopyTo(bool[] array, int arrayIndex)
        {
            Extensions.CanCopyTo(_capacity, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public void CopyTo(bool[] array)
        {
            Extensions.CanCopyTo(_capacity, array);
            Extensions.CopyTo(this, array);
        }

        public void CopyTo(bool[] array, int arrayIndex, int countLimit)
        {
            Extensions.CanCopyTo(array, arrayIndex, countLimit);
            Extensions.CopyTo(this, array, arrayIndex, countLimit);
        }

        public IEnumerator<bool> GetEnumerator()
        {
            var index = 0;
            foreach (var entry in _entries)
            {
                foreach (var bit in entry.BinaryReverse().BitsBinary())
                {
                    yield return bit == 1;
                    index++;
                    if (index == _capacity)
                    {
                        yield break;
                    }
                }
            }
        }

#if !NETCOREAPP1_1
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        void ICollection<bool>.Add(bool item)
        {
            throw new NotSupportedException();
        }

        void ICollection<bool>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<bool>.Remove(bool item)
        {
            throw new NotSupportedException();
        }

        void IList<bool>.Insert(int index, bool item)
        {
            throw new NotSupportedException();
        }

        void IList<bool>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public int IndexOf(bool item)
        {
            return Extensions.IndexOf(this, item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Fill(bool value)
        {
            var entryValue = value ? unchecked((int)0xffffffff) : 0;
            for (var index = 0; index < GetLength(_capacity); index++)
            {
                _entries[index] = entryValue;
            }
        }

        private bool GetBit(int index, int mask)
        {
            return (Volatile.Read(ref _entries[index]) & mask) != 0;
        }

        private int GetLength(int length)
        {
            return (length >> 5) + ((length & 31) == 0 ? 0 : 1);
        }

        private void RecycleExtracted()
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var entries = _entries;
            if (entries != null)
            {
                ArrayReservoir<int>.DonateArray(entries);
                _entries = null;
            }
        }

        private void SetBit(int index, int mask)
        {
            again:
            var readed = Volatile.Read(ref _entries[index]);
            if ((readed & mask) == 0)
            {
                if (Interlocked.CompareExchange(ref _entries[index], readed | mask, readed) != readed)
                {
                    goto again;
                }
            }
        }

        private void UnsetBit(int index, int mask)
        {
            again:
            var readed = Volatile.Read(ref _entries[index]);
            if ((readed & mask) != 0)
            {
                if (Interlocked.CompareExchange(ref _entries[index], readed & ~mask, readed) != readed)
                {
                    goto again;
                }
            }
        }
    }

    public sealed partial class FlagArray : IList<bool>, IExtendedCollection<bool>, ICloneable<FlagArray>
    {
        IReadOnlyCollection<bool> IExtendedCollection<bool>.AsReadOnly
        {
            get { return _asReadOnly; }
        }

        bool IExtendedCollection<bool>.Remove(bool item, IEqualityComparer<bool> comparer)
        {
            throw new NotSupportedException();
        }
    }
}

#endif