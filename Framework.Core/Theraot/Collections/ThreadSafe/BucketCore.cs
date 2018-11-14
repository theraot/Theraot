using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;
using Theraot.Threading;

namespace Theraot.Collections.ThreadSafe
{
#if !NETCOREAPP1_0 && NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6
    [Serializable]
#endif

    internal class BucketCore : IEnumerable<object>
    {
        private const int _capacity = 32;
        private const long _lvl1 = _capacity;
        private const long _lvl2 = _lvl1 * _capacity;
        private const long _lvl3 = _lvl2 * _capacity;
        private const long _lvl4 = _lvl3 * _capacity;
        private const long _lvl5 = _lvl4 * _capacity;
        private const long _lvl6 = _lvl5 * _capacity;
        private const long _lvl7 = _lvl6 * _capacity;
        private object[] _arrayFirst;
        private object[] _arraySecond;
        private int[] _arrayUse;
        private readonly int _level;

        public BucketCore(int level)
        {
            if (level < 0 || level > 7)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "level < 0 || level > 7");
            }
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

        public long Length
        {
            get
            {
                switch (_level)
                {
                    case 1:
                        return _lvl1;

                    case 2:
                        return _lvl2;

                    case 3:
                        return _lvl3;

                    case 4:
                        return _lvl4;

                    case 5:
                        return _lvl5;

                    case 6:
                        return _lvl6;

                    case 7:
                        return _lvl7;

                    default:
                        return 0;
                }
            }
        }

        public bool Do(int index, DoAction callback)
        {
            if (_level == 1)
            {
                var subIndex = SubIndex(index);
                return Do
                    (
                        ref _arrayUse[subIndex],
                        ref _arrayFirst[subIndex],
                        ref _arraySecond[subIndex],
                        callback
                    );
            }
            else
            {
                var subIndex = SubIndex(index);
                return Do
                    (
                        ref _arrayUse[subIndex],
                        ref _arrayFirst[subIndex],
                        ref _arraySecond[subIndex],
                        (ref object target) =>
                        {
                            try
                            {
                                return ((BucketCore)target).Do(index, callback);
                            }
                            catch (NullReferenceException)
                            {
                                return false;
                            }
                        }
                    );
            }
        }

        public bool DoMayDecrement(int index, DoAction callback)
        {
            if (_level == 1)
            {
                var subIndex = SubIndex(index);
                return DoMayDecrement
                    (
                        ref _arrayUse[subIndex],
                        ref _arrayFirst[subIndex],
                        ref _arraySecond[subIndex],
                        callback
                    );
            }
            else
            {
                var subIndex = SubIndex(index);
                return DoMayDecrement
                    (
                        ref _arrayUse[subIndex],
                        ref _arrayFirst[subIndex],
                        ref _arraySecond[subIndex],
                        (ref object target) =>
                        {
                            try
                            {
                                return ((BucketCore)target).DoMayDecrement(index, callback);
                            }
                            catch (NullReferenceException)
                            {
                                return false;
                            }
                        }
                    );
            }
        }

        public bool DoMayIncrement(int index, DoAction callback)
        {
            if (_level == 1)
            {
                var subIndex = SubIndex(index);
                return DoMayIncrement
                    (
                        ref _arrayUse[subIndex],
                        ref _arrayFirst[subIndex],
                        ref _arraySecond[subIndex],
                        FuncHelper.GetDefaultFunc<object>(),
                        callback
                    );
            }
            else
            {
                var subIndex = SubIndex(index);
                return DoMayIncrement
                    (
                        ref _arrayUse[subIndex],
                        ref _arrayFirst[subIndex],
                        ref _arraySecond[subIndex],
                        () => new BucketCore(_level - 1),
                        (ref object target) =>
                        {
                            try
                            {
                                return ((BucketCore)target).DoMayIncrement(index, callback);
                            }
                            catch (NullReferenceException)
                            {
                                return false;
                            }
                        }
                    );
            }
        }

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

        private IEnumerable<object> PrivateEnumerableRange(int indexFrom, int indexTo, int startSubIndex, int endSubIndex)
        {
            var step = endSubIndex - startSubIndex >= 0 ? 1 : -1;
            for (var subindex = startSubIndex; subindex < endSubIndex + 1; subindex += step)
            {
                try
                {
                    Interlocked.Increment(ref _arrayUse[subindex]);
                    var foundFirst = Interlocked.CompareExchange(ref _arrayFirst[subindex], null, null);
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
                        var subIndexFrom = subindex == startSubIndex ? core.SubIndex(indexFrom) : 0;
                        var subIndexTo = subindex == endSubIndex ? core.SubIndex(indexTo) : _capacity - 1;
                        foreach (var item in core.PrivateEnumerableRange(indexFrom, indexTo, subIndexFrom, subIndexTo))
                        {
                            yield return item;
                        }
                    }
                }
                finally
                {
                    DoLeave(ref _arrayUse[subindex], ref _arrayFirst[subindex], ref _arraySecond[subindex]);
                }
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            for (var subindex = 0; subindex < _capacity; subindex++)
            {
                var foundFirst = Interlocked.CompareExchange(ref _arrayFirst[subindex], null, null);
                if (foundFirst == null)
                {
                    continue;
                }
                try
                {
                    Interlocked.Increment(ref _arrayUse[subindex]);
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
                    DoLeave(ref _arrayUse[subindex], ref _arrayFirst[subindex], ref _arraySecond[subindex]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static bool Do(ref int use, ref object first, ref object second, DoAction callback)
        {
#if FAT
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
#if FAT
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
                            // _firstt was set to result
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
#if FAT
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
#if FAT
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

        private int SubIndex(int index)
        {
            var result = (index >> (5 * (_level - 1))) & 0x1F;
            return result;
        }
    }
}