// ReSharper disable InvertIf

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;
using Theraot.Threading;

using System.Runtime.Serialization;

namespace Theraot.Collections.ThreadSafe
{
    [Serializable]
    internal sealed class BucketCore : IEnumerable<object>, ISerializable
    {
        private const int _capacity = 1 << _capacityLog2;
        private const int _capacityLog2 = 8;
        private const int _mask = _capacity - 1;
        private const int _maxLevel = 1 + (31 / _capacityLog2);
        private readonly Func<object> _childFactory;
        private readonly int _level;
        private object?[]? _arrayFirst;
        private object?[]? _arraySecond;
        private int[]? _arrayUse;

        public BucketCore()
            : this(_maxLevel)
        {
            // Empty
        }

        private BucketCore(SerializationInfo info, StreamingContext context)
        {
            var _ = context;
            if
            (
                info.GetValue("childFactory", typeof(Func<object>)) is not Func<object> childFactory
                || info.GetValue("level", typeof(int)) is not int level
                || info.GetValue("contents", typeof(object?[])) is not object?[] contents
            )
            {
                throw new SerializationException();
            }

             _childFactory = childFactory;
            _level = level;
            _arrayFirst = ArrayReservoir<object>.GetArray(_capacity);
            _arraySecond = ArrayReservoir<object>.GetArray(_capacity);
            _arrayUse = ArrayReservoir<int>.GetArray(_capacity);
            for (int subIndex = 0; subIndex < Math.Min(_capacity, contents.Length); subIndex++)
            {
                _arrayFirst[subIndex] = contents[subIndex];
                _arraySecond[subIndex] = contents[subIndex];
                _arrayUse[subIndex] = 1;
            }
        }

        private BucketCore(int level)
        {
            _childFactory = level == 1 ? FuncHelper.GetDefaultFunc<object>() : () => new BucketCore(_level - 1);
            _level = level;
            _arrayFirst = ArrayReservoir<object>.GetArray(_capacity);
            _arraySecond = ArrayReservoir<object>.GetArray(_capacity);
            _arrayUse = ArrayReservoir<int>.GetArray(_capacity);
        }

        ~BucketCore()
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            if (GCMonitor.FinalizingForUnload)
            {
                return;
            }

            var arrayFirst = _arrayFirst;
            if (arrayFirst != null)
            {
                ArrayReservoir<object?>.DonateArray(arrayFirst);
                _arrayFirst = null;
            }

            var arraySecond = _arraySecond;
            if (arraySecond != null)
            {
                ArrayReservoir<object?>.DonateArray(arraySecond);
                _arraySecond = null;
            }

            var arrayUse = _arrayUse;
            if (arrayUse != null)
            {
                ArrayReservoir<int>.DonateArray(arrayUse);
                _arrayUse = null;
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

            var subIndex = SubIndex(index);
            return Do
            (
                ref arrayUse[subIndex],
                ref arrayFirst[subIndex],
                ref arraySecond[subIndex],
                _level == 1
                    ? callback
                    : (ref object? target) => target is BucketCore core && core.Do(index, callback)
            );
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

            var subIndex = SubIndex(index);
            return DoMayDecrement
            (
                ref arrayUse[subIndex],
                ref arrayFirst[subIndex],
                ref arraySecond[subIndex],
                _level == 1
                    ? callback
                    : (ref object? target) => target is BucketCore core && core.DoMayDecrement(index, callback)
            );
        }

        public bool DoMayIncrement(int index, DoAction callback)
        {
            // Assume anything could have been set to null, start no sync operation, this could be running during DomainUnload
            var arrayFirst = Volatile.Read(ref _arrayFirst);
            var arraySecond = Volatile.Read(ref _arraySecond);
            var arrayUse = Volatile.Read(ref _arrayUse);
            var childFactory = _childFactory;
            if (arrayFirst == null || arraySecond == null || arrayUse == null || childFactory == null)
            {
                return false;
            }

            var subIndex = SubIndex(index);
            return DoMayIncrement
            (
                ref arrayUse[subIndex],
                ref arrayFirst[subIndex],
                ref arraySecond[subIndex],
                childFactory,
                _level == 1
                    ? callback
                    : (ref object? target) => target is BucketCore core && core.DoMayIncrement(index, callback)
            );
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

            static IEnumerator<object> Empty()
            {
                yield break;
            }

            IEnumerator<object> GetEnumeratorExtracted()
            {
                for (var subIndex = 0; subIndex < _capacity; subIndex++)
                {
                    var foundFirst = Interlocked.CompareExchange(ref arrayFirst![subIndex], null, null);
                    if (foundFirst == null)
                    {
                        continue;
                    }

                    try
                    {
                        Interlocked.Increment(ref arrayUse![subIndex]);
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
                        DoLeave(ref arrayUse![subIndex], ref arrayFirst[subIndex], ref arraySecond![subIndex]);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("level", _level, typeof(int));
            info.AddValue("childFactory", _childFactory, typeof(Func<object>));
            info.AddValue("contents", _arrayFirst, typeof(object?[]));
        }

        private static bool Do(ref int use, ref object? first, ref object? second, DoAction callback)
        {
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

        private static void DoEnsureSize(ref int use, ref object? first, ref object? second, Func<object> factory)
        {
            try
            {
                Interlocked.Increment(ref use);
                // May add - make sure second exists
                // Read first
                var foundFirst = Interlocked.CompareExchange(ref first, null, null);
                // Try to restore second
                var foundSecond = Interlocked.CompareExchange(ref second, foundFirst, null);
                // second was set to first
                if (foundSecond != null || foundFirst != null)
                {
                    return;
                }

                // We need to recreate the first
                var result = factory();
                // Try to set to first
                foundFirst = Interlocked.CompareExchange(ref first, result, null);
                if (foundFirst != null)
                {
                    return;
                }

                // first was set to result
                if (result != null)
                {
                    Interlocked.Increment(ref use);
                }

                // Try to set to second
                Interlocked.CompareExchange(ref second, result, null);
            }
            finally
            {
                DoLeave(ref use, ref first, ref second);
            }
        }

        private static void DoLeave(ref int use, ref object? first, ref object? second)
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

        private static bool DoMayDecrement(ref int use, ref object? first, ref object? second, DoAction callback)
        {
            try
            {
                Interlocked.Increment(ref use);
                // Read first
                var foundFirst = Interlocked.CompareExchange(ref first, null, null);
                // Try to restore second
                Interlocked.CompareExchange(ref second, foundFirst, null);
                if (!callback(ref second))
                {
                    return false;
                }

                Interlocked.Decrement(ref use);
                return true;
            }
            finally
            {
                DoLeave(ref use, ref first, ref second);
            }
        }

        private static bool DoMayIncrement(ref int use, ref object? first, ref object? second, Func<object> factory, DoAction callback)
        {
            try
            {
                Interlocked.Increment(ref use);
                DoEnsureSize(ref use, ref first, ref second, factory);
                if (!callback(ref first))
                {
                    return false;
                }

                Interlocked.Increment(ref use);
                return true;
            }
            finally
            {
                DoLeave(ref use, ref first, ref second);
            }
        }

        private IEnumerable<object> PrivateEnumerableRange(int indexFrom, int indexTo, int startSubIndex, int endSubIndex)
        {
            var arrayFirst = Volatile.Read(ref _arrayFirst);
            var arraySecond = Volatile.Read(ref _arraySecond);
            var arrayUse = Volatile.Read(ref _arrayUse);
            var step = endSubIndex - startSubIndex >= 0 ? 1 : -1;
            for (var subIndex = startSubIndex; subIndex < endSubIndex + 1; subIndex += step)
            {
                try
                {
                    Interlocked.Increment(ref arrayUse![subIndex]);
                    var foundFirst = Interlocked.CompareExchange(ref arrayFirst![subIndex], null, null);
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
                        if (foundFirst is not BucketCore core)
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
                    DoLeave(ref arrayUse![subIndex], ref arrayFirst![subIndex], ref arraySecond![subIndex]);
                }
            }
        }

        private int SubIndex(int index)
        {
            var uIndex = unchecked((uint)index);
            return (int)((uIndex >> (_capacityLog2 * (_level - 1))) & _mask);
        }
    }
}