using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Collections.ThreadSafe
{
    internal class BucketCore : IEnumerable<object>
    {
        private const int INT_Capacity = 32;
        private const int INT_Capacity_Final = 2;
        private const int INT_Lvl1 = INT_Capacity;
        private const int INT_Lvl2 = INT_Lvl1 * INT_Capacity;
        private const int INT_Lvl3 = INT_Lvl2 * INT_Capacity;
        private const int INT_Lvl4 = INT_Lvl3 * INT_Capacity;
        private const int INT_Lvl5 = INT_Lvl4 * INT_Capacity;
        private const int INT_Lvl6 = INT_Lvl5 * INT_Capacity;
        private const int INT_Lvl7 = INT_Lvl6 + (INT_Lvl6 - 1);
        private readonly object[] _arrayFirst;
        private readonly object[] _arraySecond;
        private readonly int[] _arrayUse;
        private readonly int _level;

        public BucketCore(int level)
        {
            if (level < 0 || level > 7)
            {
                throw new ArgumentOutOfRangeException("level", "level < 0 || level > 7");
            }
            _level = level;
            if (_level == 7)
            {
                _arrayFirst = new object[INT_Capacity_Final];
                _arraySecond = new object[INT_Capacity_Final];
                _arrayUse = new int[INT_Capacity_Final];
            }
            else
            {
                _arrayFirst = new object[INT_Capacity];
                _arraySecond = new object[INT_Capacity];
                _arrayUse = new int[INT_Capacity];
            }
        }

        public int Length
        {
            get
            {
                switch (_level)
                {
                    case 1:
                        return INT_Lvl1;

                    case 2:
                        return INT_Lvl2;

                    case 3:
                        return INT_Lvl3;

                    case 4:
                        return INT_Lvl4;

                    case 5:
                        return INT_Lvl5;

                    case 6:
                        return INT_Lvl6;

                    case 7:
                        return INT_Lvl7;

                    default:
                        return 0;
                }
            }
        }

        public bool Do(int index, DoAction callback)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index < 0");
            }
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
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index < 0");
            }
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
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index < 0");
            }
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

        public IEnumerable<object> EnumerateFrom(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index < 0");
            }
            var startSubIndex = SubIndex(index);
            for (var subindex = startSubIndex; subindex < (_level == 7 ? INT_Capacity_Final : INT_Capacity); subindex++)
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
                        foreach (var item in ((BucketCore)foundFirst).EnumerateFrom(index))
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

        public IEnumerable<object> EnumerateTo(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index < 0");
            }
            var endSubIndex = SubIndex(index);
            for (var subindex = 0; subindex < endSubIndex - 1; subindex++)
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
                        foreach (var item in ((BucketCore)foundFirst).EnumerateTo(index))
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
            for (var subindex = 0; subindex < (_level == 7 ? INT_Capacity_Final : INT_Capacity); subindex++)
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
            return (index >> (5 * (_level - 1))) & 0x1F;
        }
    }
}