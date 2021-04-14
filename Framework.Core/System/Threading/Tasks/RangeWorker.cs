#if LESSTHAN_NET40 || NETSTANDARD1_0

// BASEDON: https://github.com/dotnet/corefx/blob/e0ba7aa8026280ee3571179cc06431baf1dfaaac/src/System.Threading.Tasks.Parallel/src/System/Threading/Tasks/ParallelRangeManager.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// Implements the algorithm for distributing loop indices to parallel loop workers
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Threading.Tasks
{
    /// <summary>
    ///     The RangeWorker struct wraps the state needed by a task that services the parallel loop
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    internal struct RangeWorker
    {
        // index of the current index range that this worker is grabbing chunks from
        private int _currentIndexRange;

        // increment value is the current amount that this worker will use
        // to increment the shared index of the range it's working on
        private long _incrementValue;

        // reference to the IndexRange array allocated by the range manager
        private readonly IndexRange[] _indexRanges;

        // the increment value is doubled each time this worker finds work, and is capped at this value
        private readonly long _maxIncrementValue;

        /// <summary>
        ///     Initializes a RangeWorker struct
        /// </summary>
        internal RangeWorker(IndexRange[] ranges, int initialRange, long step)
        {
            _indexRanges = ranges;
            _currentIndexRange = initialRange;
            _incrementValue = step;
            _maxIncrementValue = Parallel.Default_Loop_Stride * step;
        }

        internal readonly bool IsInitialized => _indexRanges != null;

        internal bool FindNewWork(out long fromInclusiveLocal, out long toExclusiveLocal)
        {
            // Implements the core work search algorithm that will be used for this range worker.
            // Usage pattern is:
            //    1) the thread associated with this range worker calls FindNewWork
            //    2) if we return true, the worker uses the fromInclusiveLocal and toExclusiveLocal values
            //       to execute the sequential loop
            //    3) if we return false it means there is no more work left. It's time to quit.
            // since we iterate over index ranges circularly, we will use the
            // count of visited ranges as our exit condition
            var numIndexRangesToVisit = _indexRanges.Length;

            do
            {
                // local snap to save array access bounds checks in places where we only read fields
                var currentRange = _indexRanges[_currentIndexRange];

                if (currentRange.RangeFinished == 0)
                {
                    var found = _indexRanges[_currentIndexRange].SharedCurrentIndexOffset;
                    if (found == null)
                    {
                        var created = new StrongBox<long>(0);
                        found = Interlocked.CompareExchange(ref _indexRanges[_currentIndexRange].SharedCurrentIndexOffset, created, comparand: null) ?? created;
                    }

                    var myOffset = Interlocked.Add(ref found.Value, _incrementValue) - _incrementValue;
                    if (currentRange.ToExclusive - currentRange.FromInclusive > myOffset)
                    {
                        // we found work

                        fromInclusiveLocal = currentRange.FromInclusive + myOffset;
                        toExclusiveLocal = unchecked(fromInclusiveLocal + _incrementValue);

                        // Check for going past end of range, or wrapping
                        if (toExclusiveLocal > currentRange.ToExclusive || toExclusiveLocal < currentRange.FromInclusive)
                        {
                            toExclusiveLocal = currentRange.ToExclusive;
                        }

                        // We will double our unit of increment until it reaches the maximum.
                        if (_incrementValue >= _maxIncrementValue)
                        {
                            return true;
                        }

                        _incrementValue *= 2;
                        if (_incrementValue > _maxIncrementValue)
                        {
                            _incrementValue = _maxIncrementValue;
                        }

                        return true;
                    }

                    // this index range is completed, mark it so that others can skip it quickly
                    Interlocked.Exchange(ref _indexRanges[_currentIndexRange].RangeFinished, 1);
                }

                // move on to the next index range, in circular order.
                _currentIndexRange = (_currentIndexRange + 1) % _indexRanges.Length;
                numIndexRangesToVisit--;
            } while (numIndexRangesToVisit > 0);
            // we've visited all index ranges possible => there's no work remaining

            fromInclusiveLocal = 0;
            toExclusiveLocal = 0;

            return false;
        }

        internal bool FindNewWork32(out int fromInclusiveLocal32, out int toExclusiveLocal32)
        {
            // 32 bit integer version of FindNewWork. Assumes the ranges were initialized with 32 bit values.

            var bRetVal = FindNewWork(out var fromInclusiveLocal, out var toExclusiveLocal);

            Debug.Assert
            (
                fromInclusiveLocal <= int.MaxValue
                && fromInclusiveLocal >= int.MinValue
                && toExclusiveLocal <= int.MaxValue
                && toExclusiveLocal >= int.MinValue
            );

            // convert to 32 bit before returning
            fromInclusiveLocal32 = (int)fromInclusiveLocal;
            toExclusiveLocal32 = (int)toExclusiveLocal;

            return bRetVal;
        }
    }
}

#endif