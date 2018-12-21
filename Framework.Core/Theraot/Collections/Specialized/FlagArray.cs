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
    public sealed class FlagArray : IList<bool>, ICollection<bool>, ICloneable<FlagArray>
    {
        private readonly IReadOnlyCollection<bool> _asReadOnly;
        private int[] _entries;

        public FlagArray(FlagArray prototype)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype), "prototype is null.");
            }
            Capacity = prototype.Capacity;
            _entries = ArrayReservoir<int>.GetArray(GetLength(Capacity));
            prototype._entries.CopyTo(_entries, 0);
            _asReadOnly = Extensions.WrapAsIReadOnlyCollection(this);
        }

        public FlagArray(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "length < 0");
            }
            Capacity = capacity;
            _entries = ArrayReservoir<int>.GetArray(GetLength(Capacity));
            _asReadOnly = Extensions.WrapAsIReadOnlyCollection(this);
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
                var entries = _entries;
                if (entries != null)
                {
                    ArrayReservoir<int>.DonateArray(entries);
                    _entries = null;
                }
            }
        }

        public int Capacity { get; }

        public int Count
        {
            get
            {
                var count = 0;
                var index = 0;
                var newIndex = 0;
                foreach (var entry in _entries)
                {
                    newIndex += 32;
                    if (newIndex <= Capacity)
                    {
                        count += NumericHelper.PopulationCount(entry);
                        index = newIndex;
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
                            if (index == Capacity)
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
                        if (index >= Capacity)
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
                            if (index == Capacity)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }
        }

        bool ICollection<bool>.IsReadOnly => false;

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

        void ICollection<bool>.Add(bool item)
        {
            throw new NotSupportedException();
        }

        void ICollection<bool>.Clear()
        {
            throw new NotSupportedException();
        }

        public FlagArray Clone()
        {
            return new FlagArray(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public bool Contains(bool item)
        {
            var index = 0;
            var newIndex = 0;
            var check = item ? 0 : -1;
            foreach (var entry in _entries)
            {
                newIndex += 32;
                if (newIndex <= Capacity)
                {
                    if (entry != check)
                    {
                        return true;
                    }
                    index = newIndex;
                }
                else
                {
                    foreach (var bit in entry.BinaryReverse().BitsBinary())
                    {
                        if (bit == 1 == item)
                        {
                            return true;
                        }
                        index++;
                        if (index == Capacity)
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
            Extensions.CanCopyTo(Capacity, array, arrayIndex);
            Extensions.CopyTo(this, array, arrayIndex);
        }

        public void CopyTo(bool[] array)
        {
            Extensions.CanCopyTo(Capacity, array);
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
                    if (index == Capacity)
                    {
                        yield break;
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(bool item)
        {
            return Extensions.IndexOf(this, item);
        }

        void IList<bool>.Insert(int index, bool item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<bool>.Remove(bool item)
        {
            throw new NotSupportedException();
        }

        void IList<bool>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        private static int GetLength(int length)
        {
            return (length >> 5) + ((length & 31) == 0 ? 0 : 1);
        }

        private void Fill(bool value)
        {
            var entryValue = value ? unchecked((int)0xffffffff) : 0;
            for (var index = 0; index < GetLength(Capacity); index++)
            {
                _entries[index] = entryValue;
            }
        }

        private bool GetBit(int index, int mask)
        {
            return (Volatile.Read(ref _entries[index]) & mask) != 0;
        }

        private void SetBit(int index, int mask)
        {
        again:
            var read = Volatile.Read(ref _entries[index]);
            if ((read & mask) == 0)
            {
                if (Interlocked.CompareExchange(ref _entries[index], read | mask, read) != read)
                {
                    goto again;
                }
            }
        }

        private void UnsetBit(int index, int mask)
        {
        again:
            var read = Volatile.Read(ref _entries[index]);
            if ((read & mask) != 0)
            {
                if (Interlocked.CompareExchange(ref _entries[index], read & ~mask, read) != read)
                {
                    goto again;
                }
            }
        }
    }
}

#endif