using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
    [Serializable]
    internal class BucketCore : IEnumerable<object>
    {
        private const int _capacityLog2 = 8;
        private const int _capacity = 1 << _capacityLog2;
        private const int _mask = _capacity - 1;
        private const int _maxLevel = 1 + 31 / _capacityLog2;
        private readonly int _level;
        private object[] _arrayFirst;
        private object[] _arraySecond;
        private int[] _arrayUse;

        public BucketCore()
            : this(_maxLevel)
        {
            // Empty
        }

        private BucketCore(int level)
        {
            _level = level;
            _arrayFirst = ArrayReservoir<object>.GetArray(_capacity);
            _arraySecond = ArrayReservoir<object>.GetArray(_capacity);
            _arrayUse = ArrayReservoir<int>.GetArray(_capacity);
        }

        ~BucketCore()
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (!GCMonitor.FinalizingForUnload)
            {
                var arrayFirst = _arrayFirst;
                if (arrayFirst != null)
                {
                    ArrayReservoir<object>.DonateArray(arrayFirst);
                    _arrayFirst = null;
                }
                var arraySecond = _arraySecond;
                if (arraySecond != null)
                {
                    ArrayReservoir<object>.DonateArray(arraySecond);
                    _arraySecond = null;
                }
                var arrayUse = _arrayUse;
                if (arrayUse != null)
                {
                    ArrayReservoir<int>.DonateArray(arrayUse);
                    _arrayUse = null;
                }
            }
        }

        public bool Do(int index, DoAction callback)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var arrayFirst = Volatile.Read(ref _arrayFirst);
            var arraySecond = Volatile.Read(ref _arraySecond);
            var arrayUse = Volatile.Read(ref _arrayUse);
            if (arrayFirst == null || arraySecond == null || arrayUse == null)
            {
                return false;
            }
            if (_level == 1)
            {
                var subIndex = SubIndex(index);
                return Do
                    (
                        ref arrayUse[subIndex],
                        ref arrayFirst[subIndex],
                        ref arraySecond[subIndex],
                        callback
                    );
            }
            else
            {
                var subIndex = SubIndex(index);
                return Do
                    (
                        ref arrayUse[subIndex],
                        ref arrayFirst[subIndex],
                        ref arraySecond[subIndex],
                        (ref object target) => target is BucketCore core && core.Do(index, callback)
                    );
            }
        }

        public bool DoMayDecrement(int index, DoAction callback)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var arrayFirst = Volatile.Read(ref _arrayFirst);
            var arraySecond = Volatile.Read(ref _arraySecond);
            var arrayUse = Volatile.Read(ref _arrayUse);
            if (arrayFirst == null || arraySecond == null || arrayUse == null)
            {
                return false;
            }
            if (_level == 1)
            {
                var subIndex = SubIndex(index);
                return DoMayDecrement
                    (
                        ref arrayUse[subIndex],
                        ref arrayFirst[subIndex],
                        ref arraySecond[subIndex],
                        callback
                    );
            }
            else
            {
                var subIndex = SubIndex(index);
                return DoMayDecrement
                    (
                        ref arrayUse[subIndex],
                        ref arrayFirst[subIndex],
                        ref arraySecond[subIndex],
                        (ref object target) => target is BucketCore core && core.DoMayDecrement(index, callback)
                    );
            }
        }

        public bool DoMayIncrement(int index, DoAction callback)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var arrayFirst = Volatile.Read(ref _arrayFirst);
            var arraySecond = Volatile.Read(ref _arraySecond);
            var arrayUse = Volatile.Read(ref _arrayUse);
            if (arrayFirst == null || arraySecond == null || arrayUse == null)
            {
                return false;
            }
            if (_level == 1)
            {
                var subIndex = SubIndex(index);
                return DoMayIncrement
                    (
                        ref arrayUse[subIndex],
                        ref arrayFirst[subIndex],
                        ref arraySecond[subIndex],
                        FuncHelper.GetDefaultFunc<object>(),
                        callback
                    );
            }
            else
            {
                var subIndex = SubIndex(index);
                return DoMayIncrement
                    (
                        ref arrayUse[subIndex],
                        ref arrayFirst[subIndex],
                        ref arraySecond[subIndex],
                        () => new BucketCore(_level - 1),
                        (ref object target) => target is BucketCore core && core.DoMayIncrement(index, callback)
                    );
            }
        }

#if FAT
        public IEnumerable<object> EnumerateRange(int indexFrom, int indexTo)
        {
            if (indexFrom < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexFrom), "indexFrom < 0");
            }
            if (indexTo < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexTo), "indexTo < 0");
            }
            var startSubIndex = SubIndex(indexFrom);
            var endSubIndex = SubIndex(indexTo);
            return PrivateEnumerableRange(indexFrom, indexTo, startSubIndex, endSubIndex);
        }
#endif

        public IEnumerator<object> GetEnumerator()
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var arrayFirst = Volatile.Read(ref _arrayFirst);
            var arraySecond = Volatile.Read(ref _arraySecond);
            var arrayUse = Volatile.Read(ref _arrayUse);
            if (arrayFirst == null || arraySecond == null || arrayUse == null)
            {
                return Empty();
            }
            return GetEnumeratorExtracted();
            IEnumerator<object> Empty()
            {
                yield break;
            }
            IEnumerator<object> GetEnumeratorExtracted()
            {
                for (var subIndex = 0; subIndex < _capacity; subIndex++)
                {
                    var foundFirst = Interlocked.CompareExchange(ref arrayFirst[subIndex], null, null);
                    if (foundFirst == null)
                    {
                        continue;
                    }
                    try
                    {
                        Interlocked.Increment(ref arrayUse[subIndex]);
                        if (_level == 1)
                        {
                            yield return foundFirst;
                        }
                        else
                        {
                            foreach (var item in (BucketCore)foundFirst)
                            {
                                yield return item;
                            }
                        }
                    }
                    finally
                    {
                        DoLeave(ref arrayUse[subIndex], ref arrayFirst[subIndex], ref arraySecond[subIndex]);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static bool Do(ref int use, ref object first, ref object second, DoAction callback)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
#endif
            var foundFirst = Interlocked.CompareExchange(ref first, null, null);
            if (foundFirst == null)
            {
                return false;
            }
            try
            {
                Interlocked.Increment(ref use);
                return callback(ref first);
            }
            finally
            {
                DoLeave(ref use, ref first, ref second);
            }
        }

        private static void DoEnsureSize(ref int use, ref object first, ref object second, Func<object> factory)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
#endif
            try
            {
                Interlocked.Increment(ref use);
                // May add - make sure second exists
                // Read first
                var foundFirst = Interlocked.CompareExchange(ref first, null, null);
                // Try to restore second
                var foundSecond = Interlocked.CompareExchange(ref second, foundFirst, null);
                if (foundSecond == null)
                {
                    // second was set to first
                    if (foundFirst == null)
                    {
                        // We need to recreate the first
                        var result = factory();
                        // Try to set to first
                        foundFirst = Interlocked.CompareExchange(ref first, result, null);
                        if (foundFirst == null)
                        {
                            // first was set to result
                            if (result != null)
                            {
                                Interlocked.Increment(ref use);
                            }
                            // Try to set to second
                            Interlocked.CompareExchange(ref second, result, null);
                        }
                    }
                }
            }
            finally
            {
                DoLeave(ref use, ref first, ref second);
            }
        }

        private static void DoLeave(ref int use, ref object first, ref object second)
        {
            if (Interlocked.Decrement(ref use) != 0)
            {
                return;
            }
            // Erase second
            Interlocked.Exchange(ref second, null);
            // Erase first - second may have been restored by another thread
            Interlocked.Exchange(ref first, null);
            // Read second
            var foundSecond = Interlocked.CompareExchange(ref second, null, null);
            // Set first to second - either erased or restored
            Interlocked.CompareExchange(ref first, foundSecond, null);
        }

        private static bool DoMayDecrement(ref int use, ref object first, ref object second, DoAction callback)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
#endif
            try
            {
                Interlocked.Increment(ref use);
                // Read first
                var foundFirst = Interlocked.CompareExchange(ref first, null, null);
                // Try to restore second
                Interlocked.CompareExchange(ref second, foundFirst, null);
                if (callback(ref second))
                {
                    Interlocked.Decrement(ref use);
                    return true;
                }
                return false;
            }
            finally
            {
                DoLeave(ref use, ref first, ref second);
            }
        }

        private static bool DoMayIncrement(ref int use, ref object first, ref object second, Func<object> factory, DoAction callback)
        {
#if DEBUG
            // NOTICE this method has no null check in the public build as an optimization, this is just to appease the dragons
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
#endif
            try
            {
                Interlocked.Increment(ref use);
                DoEnsureSize(ref use, ref first, ref second, factory);
                if (callback(ref first))
                {
                    Interlocked.Increment(ref use);
                    return true;
                }
                return false;
            }
            finally
            {
                DoLeave(ref use, ref first, ref second);
            }
        }

#if FAT
        private IEnumerable<object> PrivateEnumerableRange(int indexFrom, int indexTo, int startSubIndex, int endSubIndex)
        {
            var step = endSubIndex - startSubIndex >= 0 ? 1 : -1;
            for (var subIndex = startSubIndex; subIndex < endSubIndex + 1; subIndex += step)
            {
                try
                {
                    Interlocked.Increment(ref _arrayUse[subIndex]);
                    var foundFirst = Interlocked.CompareExchange(ref _arrayFirst[subIndex], null, null);
                    if (_level == 1)
                    {
                        if (foundFirst == null)
                        {
                            continue;
                        }
                        yield return foundFirst;
                    }
                    else
                    {
                        if (!(foundFirst is BucketCore core))
                        {
                            continue;
                        }
                        var subIndexFrom = subIndex == startSubIndex ? core.SubIndex(indexFrom) : 0;
                        var subIndexTo = subIndex == endSubIndex ? core.SubIndex(indexTo) : _capacity - 1;
                        foreach (var item in core.PrivateEnumerableRange(indexFrom, indexTo, subIndexFrom, subIndexTo))
                        {
                            yield return item;
                        }
                    }
                }
                finally
                {
                    DoLeave(ref _arrayUse[subIndex], ref _arrayFirst[subIndex], ref _arraySecond[subIndex]);
                }
            }
        }
#endif

        private int SubIndex(int index)
        {
            var uIndex = unchecked((uint)index);
            return (int)((uIndex >> (_capacityLog2 * (_level - 1))) & _mask);
        }
    }
}