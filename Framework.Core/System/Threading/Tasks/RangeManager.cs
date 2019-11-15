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

namespace System.Threading.Tasks
{
    /// <summary>
    /// Represents the entire loop operation, keeping track of workers and ranges.
    /// </summary>
    ///
    /// The usage pattern is:
    ///    1) The Parallel loop entry function (ForWorker) creates an instance of this class
    ///    2) Every thread joining to service the parallel loop calls RegisterWorker to grab a
    ///       RangeWorker struct to wrap the state it will need to find and execute work,
    ///       and they keep interacting with that struct until the end of the loop
    internal class RangeManager
    {
        internal readonly IndexRange[] IndexRanges;
        internal readonly long Step;
        internal int CurrentIndexRangeToAssign;

        /// <summary>
        /// Initializes a RangeManager with the given loop parameters, and the desired number of outer ranges
        /// </summary>
        internal RangeManager(long fromInclusive, long toExclusive, long step, int numExpectedWorkers)
        {
            CurrentIndexRangeToAssign = 0;
            Step = step;

            // Our signed math breaks down w/ nNumExpectedWorkers == 1.  So change it to 2.
            if (numExpectedWorkers == 1)
            {
                numExpectedWorkers = 2;
            }

            //
            // calculate the size of each index range
            //

            var uSpan = (ulong)(toExclusive - fromInclusive);
            var uRangeSize = uSpan / (ulong)numExpectedWorkers; // rough estimate first

            uRangeSize -= uRangeSize % (ulong)step; // snap to multiples of nStep
                                                    // otherwise index range transitions will derail us from nStep

            if (uRangeSize == 0)
            {
                uRangeSize = (ulong)step;
            }

            //
            // find the actual number of index ranges we will need
            //
            Debug.Assert((uSpan / uRangeSize) < int.MaxValue);

            var nNumRanges = (int)(uSpan / uRangeSize);

            if (uSpan % uRangeSize != 0)
            {
                nNumRanges++;
            }

            // Convert to signed so the rest of the logic works.
            // Should be fine so long as uRangeSize < Int64.MaxValue, which we guaranteed by setting #workers >= 2.
            var nRangeSize = (long)uRangeSize;

            // allocate the array of index ranges
            IndexRanges = new IndexRange[nNumRanges];

            var nCurrentIndex = fromInclusive;
            for (var i = 0; i < nNumRanges; i++)
            {
                // the fromInclusive of the new index range is always on nCurrentIndex
                IndexRanges[i].FromInclusive = nCurrentIndex;
                IndexRanges[i].SharedCurrentIndexOffset = null;
                IndexRanges[i].RangeFinished = 0;

                // now increment it to find the toExclusive value for our range
                nCurrentIndex = unchecked(nCurrentIndex + nRangeSize);

                // detect integer overflow or range overage and snap to nToExclusive
                if (nCurrentIndex < unchecked(nCurrentIndex - nRangeSize) || nCurrentIndex > toExclusive)
                {
                    // this should only happen at the last index
                    Debug.Assert(i == nNumRanges - 1);

                    nCurrentIndex = toExclusive;
                }

                // now that the end point of the new range is calculated, assign it.
                IndexRanges[i].ToExclusive = nCurrentIndex;
            }
        }

        /// <summary>
        /// The function that needs to be called by each new worker thread servicing the parallel loop
        /// in order to get a RangeWorker struct that wraps the state for finding and executing indices
        /// </summary>
        internal RangeWorker RegisterNewWorker()
        {
            Debug.Assert(IndexRanges.Length != 0);

            var initialRange = (Interlocked.Increment(ref CurrentIndexRangeToAssign) - 1) % IndexRanges.Length;

            return new RangeWorker(IndexRanges, initialRange, Step);
        }
    }
}

#endif