// BASEDON: https://github.com/dotnet/corefx/blob/7ae1a252d7e68c5513d2658de7a401c37e9b0504/src/System.Threading.Tasks.Parallel/tests/ParallelForTest.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// ParallelForTest.cs
//
// This file contains functional tests for Parallel.For and Parallel.ForEach
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-

// ReSharper disable CompareOfFloatsByEqualityOperator

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace System.Threading.Tasks.Tests
{
    public enum ActionWithLocal
    {
        // no ParallelLoopState<TLocal>
        None,

        // need ParallelLoopState<TLocal> and Action<TLocal> threadLocalFinally
        HasFinally
    }

    public enum ActionWithState
    {
        // no ParallelLoopState
        None,

        // need ParallelLoopState and will do Stop
        Stop
    }

    // List of APIs being tested
    public enum Api
    {
        For,
        For64,
        ForeachOnArray,
        ForeachOnList,
        Foreach
    }

    public enum DataSourceType
    {
        Partitioner,
        Collection
    }

    /// <summary>
    /// Partitioner types used for ParallelForeach with partitioners
    /// </summary>
    public enum PartitionerType
    {
        // Out of the box List Partitioner
        ListBalancedOob = 0,

        // Out of the box Array partitioner
        ArrayBalancedOob = 1,

        // Out of the box Enumerable partitioner
        EnumerableOob = 2,

        // out of the box range partitioner
        RangePartitioner = 3,

        // partitioner one chunk
        Enumerable1Chunk = 4
    }

    public enum StartIndexBase
    {
        Zero = 0,
        Int16 = short.MaxValue,
        Int32 = int.MaxValue,

        // Enum can't take a Int64.MaxValue
        Int64 = -1
    }

    public enum WithParallelOption
    {
        // no ParallelOptions
        None,

        // ParallelOptions created with DOP
        WithDop
    }

    public enum WorkloadPattern
    {
        Similar,
        Increasing,
        Decreasing,
        Random
    }

    public sealed class ParallelForTest
    {
        private const int _zetaSeedOffset = 10000;
        private readonly TestParameters _parameters;

        private readonly ThreadLocal<Random> _random = new(() => new Random(unchecked((int)(DateTime.Now.Ticks))));
        private readonly double[] _results;
        private readonly List<int>[] _sequences;
        private IList<int> _collection; // the collection used in Foreach

        // global place to store the workload result for verification

        private ParallelOptions _parallelOption;

        private OrderablePartitioner<int> _partitioner;

        // offset to the zeta seed to ensure result converge to the expected
        private OrderablePartitioner<Tuple<int, int>> _rangePartitioner;

        // data structure used with ParallelLoopState<TLocal>
        // each row is the sequence of loop "index" finished in the same thread
        private int _threadCount;

        // Random generator for WorkloadPattern == Random
        public ParallelForTest(TestParameters parameters)
        {
            _parameters = parameters;

            _results = new double[parameters.Count];

            if (parameters.LocalOption == ActionWithLocal.None)
            {
                return;
            }

            _sequences = new List<int>[1024];
            _threadCount = 0;
        }

        public static double ZetaSequence(int n)
        {
            double result = 0;
            for (int i = 1; i < n; i++)
            {
                result += 1.0 / ((double)i * (double)i);
            }

            return result;
        }

        internal void RealRun()
        {
            if (_parameters.Api == Api.For64)
                RunParallelFor64Test();
            else if (_parameters.Api == Api.For)
                RunParallelForTest();
            else
                RunParallelForeachTest();

            // verify result
            for (int i = 0; i < _parameters.Count; i++)
                Verify(i);

            // verify unique  index sequences if run WithLocal
            if (_parameters.LocalOption != ActionWithLocal.None)
                VerifySequences();
        }

        private static List<int> ThreadLocalInit()
        {
            return new List<int>();
        }

        // consolidate all the indexes of the global sequences into one list
        private List<int> Consolidate(out List<int> duplicates)
        {
            duplicates = new List<int>();
            List<int> processedIndexes = new List<int>();
            //foreach (List<int> perThreadSequences in sequences)
            for (int thread = 0; thread < _threadCount; thread++)
            {
                List<int> perThreadSequences = _sequences[thread];
                foreach (int i in perThreadSequences)
                {
                    if (processedIndexes.Contains(i))
                    {
                        duplicates.Add(i);
                    }
                    else
                    {
                        processedIndexes.Add(i);
                    }
                }
            }

            return processedIndexes;
        }

        // Creates an instance of ParallelOptions with an non-default DOP
        private ParallelOptions GetParallelOptions()
        {
            switch (_parameters.ParallelOption)
            {
                case WithParallelOption.WithDop:
                    return new ParallelOptions
                    {
                        TaskScheduler = TaskScheduler.Current,
                        MaxDegreeOfParallelism = _parameters.Count
                    };

                default:
                    throw new ArgumentOutOfRangeException("Test error: Invalid option of " + _parameters.ParallelOption);
            }
        }

        private void InvokeZetaWorkload(int i)
        {
            if (_results[i] == 0)
            {
                int zetaIndex = _zetaSeedOffset;
                switch (_parameters.WorkloadPattern)
                {
                    case WorkloadPattern.Similar:
                        zetaIndex += i;
                        break;

                    case WorkloadPattern.Increasing:
                        zetaIndex += i * _zetaSeedOffset;
                        break;

                    case WorkloadPattern.Decreasing:
                        zetaIndex += (_parameters.Count - i) * _zetaSeedOffset;
                        break;

                    case WorkloadPattern.Random:
                        zetaIndex += _random.Value.Next(0, _parameters.Count) * _zetaSeedOffset;
                        break;
                }

                _results[i] = ZetaSequence(zetaIndex);
            }
            else
            {
                //same index should not be processed twice
                _results[i] = double.MinValue;
            }
        }

        private void ParallelForEach()
        {
            // ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source,Action<TSource> body)
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, Work);
                else
                    Parallel.ForEach(_partitioner, Work);
            }
            else
            {
                Parallel.ForEach(_collection, Work);
            }
        }

        private void ParallelForEachWithIndexAndState()
        {
            // ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source,Action<TSource, ParallelLoopState, Int64> body)
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, WorkWithIndexAndStopPartitioner);
                else
                    Parallel.ForEach(_partitioner, WorkWithIndexAndStopPartitioner);
            }
            else
            {
                Parallel.ForEach(_collection, WorkWithIndexAndStop);
            }
        }

        private void ParallelForeachWithLocal()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
                else
                    Parallel.ForEach(_partitioner, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
            }
            else
            {
                Parallel.ForEach(_collection, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
            }
        }

        private void ParallelForeachWithLocalAndIndex()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, ThreadLocalInit, WorkWithLocalAndIndexPartitioner, ThreadLocalFinally);
                else
                    Parallel.ForEach(_partitioner, ThreadLocalInit, WorkWithLocalAndIndexPartitioner, ThreadLocalFinally);
            }
            else
            {
                Parallel.ForEach(_collection, ThreadLocalInit, WorkWithIndexAndLocal, ThreadLocalFinally);
            }
        }

        private void ParallelForEachWithOptions()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, _parallelOption, Work);
                else
                    Parallel.ForEach(_partitioner, _parallelOption, Work);
            }
            else
            {
                Parallel.ForEach(_collection, _parallelOption, Work);
            }
        }

        private void ParallelForEachWithOptionsAndIndexAndState()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, _parallelOption, WorkWithIndexAndStopPartitioner);
                else
                    Parallel.ForEach(_partitioner, _parallelOption, WorkWithIndexAndStopPartitioner);
            }
            else
            {
                Parallel.ForEach(_collection, _parallelOption, WorkWithIndexAndStop);
            }
        }

        private void ParallelForEachWithOptionsAndLocal()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, _parallelOption, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
                else
                    Parallel.ForEach(_partitioner, _parallelOption, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
            }
            else
            {
                Parallel.ForEach(_collection, _parallelOption, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
            }
        }

        private void ParallelForEachWithOptionsAndLocalAndIndex()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, _parallelOption, ThreadLocalInit, WorkWithLocalAndIndexPartitioner, ThreadLocalFinally);
                else
                    Parallel.ForEach(_partitioner, _parallelOption, ThreadLocalInit, WorkWithLocalAndIndexPartitioner, ThreadLocalFinally);
            }
            else
            {
                Parallel.ForEach(_collection, _parallelOption, ThreadLocalInit, WorkWithIndexAndLocal, ThreadLocalFinally);
            }
        }

        private void ParallelForEachWithOptionsAndState()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, _parallelOption, WorkWithStop);
                else
                    Parallel.ForEach(_partitioner, _parallelOption, WorkWithStop);
            }
            else
            {
                Parallel.ForEach(_collection, _parallelOption, WorkWithStop);
            }
        }

        private void ParallelForEachWithState()
        {
            if (_parameters.ParallelForeachDataSourceType == DataSourceType.Partitioner)
            {
                if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                    Parallel.ForEach(_rangePartitioner, WorkWithStop);
                else
                    Parallel.ForEach(_partitioner, WorkWithStop);
            }
            else
            {
                Parallel.ForEach(_collection, WorkWithStop);
            }
        }

        // Tests Parallel.For version that takes 'long' from and to parameters
        private void RunParallelFor64Test()
        {
            if (_parameters.ParallelOption != WithParallelOption.None)
            {
                ParallelOptions option = GetParallelOptions();

                if (_parameters.LocalOption == ActionWithLocal.None)
                {
                    if (_parameters.StateOption == ActionWithState.None)
                    {
                        // call Parallel.For with ParallelOptions
                        Parallel.For(_parameters.StartIndex64, _parameters.StartIndex64 + _parameters.Count, option, Work);
                    }
                    else if (_parameters.StateOption == ActionWithState.Stop)
                    {
                        // call Parallel.For with ParallelLoopState and ParallelOptions
                        Parallel.For(_parameters.StartIndex64, _parameters.StartIndex64 + _parameters.Count, option, WorkWithStop);
                    }
                }
                else if (_parameters.LocalOption == ActionWithLocal.HasFinally)
                {
                    // call Parallel.For with ParallelLoopState<TLocal>, plus threadLocalFinally, plus ParallelOptions
                    Parallel.For(_parameters.StartIndex64, _parameters.StartIndex64 + _parameters.Count, option, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
                }
            }
            else
            {
                if (_parameters.LocalOption == ActionWithLocal.None)
                {
                    if (_parameters.StateOption == ActionWithState.None)
                    {
                        // call Parallel.For
                        Parallel.For(_parameters.StartIndex64, _parameters.StartIndex64 + _parameters.Count, Work);
                    }
                    else if (_parameters.StateOption == ActionWithState.Stop)
                    {
                        // call Parallel.For with ParallelLoopState
                        Parallel.For(_parameters.StartIndex64, _parameters.StartIndex64 + _parameters.Count, WorkWithStop);
                    }
                }
                else if (_parameters.LocalOption == ActionWithLocal.HasFinally)
                {
                    // call Parallel.For with ParallelLoopState<TLocal>, plus threadLocalFinally
                    Parallel.For(_parameters.StartIndex64, _parameters.StartIndex64 + _parameters.Count, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
                }
            }
        }

        // Tests Parallel.ForEach
        private void RunParallelForeachTest()
        {
            int length = _parameters.Count;
            if (length < 0)
                length = 0;

            int[] arrayCollection = new int[length];
            for (int i = 0; i < length; i++)
                arrayCollection[i] = _parameters.StartIndex + i;

            if (_parameters.Api == Api.ForeachOnArray)
                _collection = arrayCollection;
            else if (_parameters.Api == Api.ForeachOnList)
                _collection = new List<int>(arrayCollection);
            else
                _collection = arrayCollection;

            //if source is partitioner
            if (_parameters.PartitionerType == PartitionerType.RangePartitioner)
                _rangePartitioner = PartitionerFactory<Tuple<int, int>>.Create(_parameters.PartitionerType, _parameters.StartIndex, _parameters.StartIndex + _parameters.Count, _parameters.ChunkSize);
            else
                _partitioner = PartitionerFactory<int>.Create(_parameters.PartitionerType, _collection);

            if (_parameters.ParallelOption != WithParallelOption.None)
            {
                _parallelOption = GetParallelOptions();

                if (_parameters.LocalOption == ActionWithLocal.None)
                {
                    if (_parameters.StateOption == ActionWithState.None)
                    {
                        // call Parallel.Foreach with ParallelOptions
                        ParallelForEachWithOptions();
                    }
                    else if (_parameters.StateOption == ActionWithState.Stop)
                    {
                        // call Parallel.Foreach with ParallelLoopState and ParallelOptions
                        if (_parameters.Api == Api.Foreach)
                            ParallelForEachWithOptionsAndState();
                        else // call indexed version for array / list overloads - to avoid calling too many combinations
                            ParallelForEachWithOptionsAndIndexAndState();
                    }
                }
                else if (_parameters.LocalOption == ActionWithLocal.HasFinally)
                {
                    // call Parallel.Foreach and ParallelLoopState<TLocal>, plus threadLocalFinally, plus ParallelOptions
                    if (_parameters.Api == Api.Foreach)
                        ParallelForEachWithOptionsAndLocal();
                    else // call indexed version for array / list overloads - to avoid calling too many combinations
                        ParallelForEachWithOptionsAndLocalAndIndex();
                }
            }
            else
            {
                if (_parameters.LocalOption == ActionWithLocal.None)
                {
                    if (_parameters.StateOption == ActionWithState.None)
                    {
                        // call Parallel.Foreach
                        ParallelForEach();
                    }
                    else if (_parameters.StateOption == ActionWithState.Stop)
                    {
                        // call Parallel.Foreach with ParallelLoopState
                        if (_parameters.Api == Api.Foreach)
                            ParallelForEachWithState();
                        else // call indexed version for array / list overloads - to avoid calling too many combinations
                            ParallelForEachWithIndexAndState();
                    }
                }
                else if (_parameters.LocalOption == ActionWithLocal.HasFinally)
                {
                    // call Parallel.Foreach and ParallelLoopState<TLocal>, plus threadLocalFinally
                    if (_parameters.Api == Api.Foreach)
                        ParallelForeachWithLocal();
                    else // call indexed version for array / list overloads - to avoid calling too many combinations
                        ParallelForeachWithLocalAndIndex();
                }
            }
        }

        // Tests Parallel.For version that takes 'int' from and to parameters
        private void RunParallelForTest()
        {
            if (_parameters.ParallelOption != WithParallelOption.None)
            {
                ParallelOptions option = GetParallelOptions();

                if (_parameters.LocalOption == ActionWithLocal.None)
                {
                    if (_parameters.StateOption == ActionWithState.None)
                    {
                        // call Parallel.For with ParallelOptions
                        Parallel.For(_parameters.StartIndex, _parameters.StartIndex + _parameters.Count, option, Work);
                    }
                    else if (_parameters.StateOption == ActionWithState.Stop)
                    {
                        // call Parallel.For with ParallelLoopState and ParallelOptions
                        Parallel.For(_parameters.StartIndex, _parameters.StartIndex + _parameters.Count, option, WorkWithStop);
                    }
                }
                else if (_parameters.LocalOption == ActionWithLocal.HasFinally)
                {
                    // call Parallel.For with ParallelLoopState<TLocal>, plus threadLocalFinally, plus ParallelOptions
                    Parallel.For(_parameters.StartIndex, _parameters.StartIndex + _parameters.Count, option, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
                }
            }
            else
            {
                if (_parameters.LocalOption == ActionWithLocal.None)
                {
                    if (_parameters.StateOption == ActionWithState.None)
                    {
                        // call Parallel.For
                        Parallel.For(_parameters.StartIndex, _parameters.StartIndex + _parameters.Count, Work);
                    }
                    else if (_parameters.StateOption == ActionWithState.Stop)
                    {
                        // call Parallel.For with ParallelLoopState
                        Parallel.For(_parameters.StartIndex, _parameters.StartIndex + _parameters.Count, WorkWithStop);
                    }
                }
                else if (_parameters.LocalOption == ActionWithLocal.HasFinally)
                {
                    // call Parallel.For with ParallelLoopState<TLocal>, plus threadLocalFinally
                    Parallel.For(_parameters.StartIndex, _parameters.StartIndex + _parameters.Count, ThreadLocalInit, WorkWithLocal, ThreadLocalFinally);
                }
            }
        }

        private void ThreadLocalFinally(List<int> local)
        {
            //add this row to the global sequences
            int index = Interlocked.Increment(ref _threadCount) - 1;
            _sequences[index] = local;
        }

        /// <summary>
        /// Each Parallel.For loop stores the result of its computation in the 'result' array.
        /// This function checks if result[i] for each i from 0 to _parameters.Count is correct
        /// A result[i] == double[i] means that the body for index i was run more than once
        /// </summary>
        /// <param name="i">index to check</param>
        /// <returns>true if result[i] contains the expected value</returns>
        private void Verify(int i)
        {
            //Function point comparison cant be done by rounding off to nearest decimal points since
            //1.64 could be represented as 1.63999999 or as 1.6499999999. To perform floating point comparisons,
            //a range has to be defined and check to ensure that the result obtained is within the specified range
            const double minLimit = 1.63;
            const double maxLimit = 1.65;

            if (!(_results[i] < minLimit) && !(_results[i] > maxLimit))
            {
                return;
            }

            Assert.False(double.MinValue == _results[i], $"results[{i}] has been revisited");

            Assert.True(_parameters.StateOption == ActionWithState.Stop && _results[i] == 0,
                $"Incorrect results[{i}]. Expected result to lie between {minLimit} and {maxLimit} but got {_results[i]})");
        }

        /// <summary>
        /// <para>
        /// Checks if the ThreadLocal Functions - Init and Locally were run correctly
        /// Init creates a new List. Each body, pushes in a unique index and Finally consolidates
        /// the lists into 'sequences' array
        /// </para>
        /// <para>
        /// Expected: The consolidated list contains all indices that were executed.
        /// Duplicates indicate that the body for a certain index was executed more than once
        /// </para>
        /// </summary>
        /// <returns>true if consolidated list contains indices for all executed loops</returns>
        private void VerifySequences()
        {
            List<int> processedIndexes = Consolidate(out var duplicates);
            Assert.IsEmpty(duplicates);

            // If result[i] != 0 then the body for that index was executed.
            // We expect the ThreadLocal list to also contain the same index
            for (int idx = 0, n = _parameters.Count; idx < n; idx++)
            {
                Assert.AreEqual(processedIndexes.Contains(idx), _results[idx] != 0);
            }
        }

        // workload for normal For
        private void Work(int i)
        {
            InvokeZetaWorkload(i - _parameters.StartIndex);
        }

        // workload for Foreach overload that takes a range partitioner
        private void Work(Tuple<int, int> tuple)
        {
            var (item1, item2) = tuple;
            for (int i = item1; i < item2; i++)
            {
                Work(i);
            }
        }

        // workload for 64-bit For
        private void Work(long i)
        {
            InvokeZetaWorkload((int)(i - _parameters.StartIndex64));
        }

        // workload for Foreach overload that takes a range partitioner
        private List<int> WorkWithIndexAndLocal(Tuple<int, int> tuple, ParallelLoopState state, long index, List<int> threadLocalValue)
        {
            var (item1, item2) = tuple;
            for (int i = item1; i < item2; i++)
            {
                Work(i);
                threadLocalValue.Add((int)index);

                if (_parameters.StateOption != ActionWithState.Stop)
                {
                    continue;
                }

                if (index > (_parameters.StartIndex + (_parameters.Count / 2)))
                    state.Stop();
            }

            return threadLocalValue;
        }

        // workload for Foreach which invokes both Stop and ThreadLocalState from ParallelLoopState
        private List<int> WorkWithIndexAndLocal(int i, ParallelLoopState state, long index, List<int> threadLocalValue)
        {
            Work(i);
            threadLocalValue.Add((int)index);

            if (_parameters.StateOption != ActionWithState.Stop)
            {
                return threadLocalValue;
            }

            if (index > (_parameters.StartIndex + (_parameters.Count / 2)))
                state.Stop();

            return threadLocalValue;
        }

        // workload for Foreach overload that takes a range partitioner
        private void WorkWithIndexAndStop(Tuple<int, int> tuple, ParallelLoopState state, long index)
        {
            var (item1, item2) = tuple;
            for (int i = item1; i < item2; i++)
            {
                WorkWithIndexAndStop(i, state, index);
            }
        }

        // workload for Parallel.Foreach which will possibly invoke ParallelLoopState.Stop
        private void WorkWithIndexAndStop(int i, ParallelLoopState state, long index)
        {
            Work(i);

            if (index > (_parameters.Count / 2))
                state.Stop();
        }

        // workload for Foreach overload that takes a range partitioner
        private void WorkWithIndexAndStopPartitioner(Tuple<int, int> tuple, ParallelLoopState state, long index)
        {
            WorkWithIndexAndStop(tuple, state, index);
        }

        // workload for Parallel.Foreach which will possibly invoke ParallelLoopState.Stop
        private void WorkWithIndexAndStopPartitioner(int i, ParallelLoopState state, long index)
        {
            //index verification
            if (_parameters.PartitionerType == PartitionerType.EnumerableOob)
            {
                int itemAtIndex = _collection[(int)index];
                Assert.AreEqual(i, itemAtIndex);
            }

            WorkWithIndexAndStop(i, state, index);
        }

        // workload for Foreach overload that takes a range partitioner
        private List<int> WorkWithLocal(Tuple<int, int> tuple, ParallelLoopState state, List<int> threadLocalValue)
        {
            var (item1, item2) = tuple;
            for (int i = item1; i < item2; i++)
            {
                WorkWithLocal(i, state, threadLocalValue);
            }

            return threadLocalValue;
        }

        // workload for normal For which uses the ThreadLocalState accessible from ParallelLoopState
        private List<int> WorkWithLocal(int i, ParallelLoopState state, List<int> threadLocalValue)
        {
            Work(i);
            threadLocalValue.Add(i - _parameters.StartIndex);

            if (_parameters.StateOption != ActionWithState.Stop)
            {
                return threadLocalValue;
            }

            if (i > (_parameters.StartIndex + (_parameters.Count / 2)))
                state.Stop();

            return threadLocalValue;
        }

        // workload for 64-bit For which invokes both Stop and ThreadLocalState from ParallelLoopState
        private List<int> WorkWithLocal(long i, ParallelLoopState state, List<int> threadLocalValue)
        {
            Work(i);
            threadLocalValue.Add((int)(i - _parameters.StartIndex64));

            if (_parameters.StateOption != ActionWithState.Stop)
            {
                return threadLocalValue;
            }

            if (i > (_parameters.StartIndex64 + (_parameters.Count / 2)))
                state.Stop();

            return threadLocalValue;
        }

        // workload for Foreach overload that takes a range partitioner
        private List<int> WorkWithLocalAndIndexPartitioner(Tuple<int, int> tuple, ParallelLoopState state, long index, List<int> threadLocalValue)
        {
            for (int i = tuple.Item1; i < tuple.Item2; i++)
            {
                //index verification - only for enumerable
                if (_parameters.PartitionerType != PartitionerType.EnumerableOob)
                {
                    continue;
                }

                int itemAtIndex = _collection[(int)index];
                Assert.AreEqual(i, itemAtIndex);
            }

            return WorkWithIndexAndLocal(tuple, state, index, threadLocalValue);
        }

        // workload for Foreach overload that takes a range partitioner
        private List<int> WorkWithLocalAndIndexPartitioner(int i, ParallelLoopState state, long index, List<int> threadLocalValue)
        {
            //index verification - only for enumerable
            if (_parameters.PartitionerType != PartitionerType.EnumerableOob)
            {
                return WorkWithIndexAndLocal(i, state, index, threadLocalValue);
            }

            int itemAtIndex = _collection[(int)index];
            Assert.AreEqual(i, itemAtIndex);

            return WorkWithIndexAndLocal(i, state, index, threadLocalValue);
        }

        // workload for normal For which will possibly invoke ParallelLoopState.Stop
        private void WorkWithStop(int i, ParallelLoopState state)
        {
            Work(i);
            if (i > (_parameters.StartIndex + (_parameters.Count / 2)))
                state.Stop(); // if the current index is in the second half range, try stop all
        }

        // workload for Foreach overload that takes a range partitioner
        private void WorkWithStop(Tuple<int, int> tuple, ParallelLoopState state)
        {
            var (item1, item2) = tuple;
            for (int i = item1; i < item2; i++)
            {
                WorkWithStop(i, state);
            }
        }

        // workload for 64-bit For which will possibly invoke ParallelLoopState.Stop
        private void WorkWithStop(long i, ParallelLoopState state)
        {
            Work(i);
            if (i > (_parameters.StartIndex64 + (_parameters.Count / 2)))
                state.Stop();
        }
    }

    /// <summary>
    /// used for partitioner creation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PartitionerFactory<T>
    {
        public static OrderablePartitioner<T> Create(PartitionerType partitionerName, IEnumerable<T> dataSource)
        {
            switch (partitionerName)
            {
                case PartitionerType.ListBalancedOob:
                    return Partitioner.Create(new List<T>(dataSource), true);

                case PartitionerType.ArrayBalancedOob:
                    return Partitioner.Create(dataSource.ToArray(), true);

                case PartitionerType.EnumerableOob:
                    return Partitioner.Create(dataSource);

                case PartitionerType.Enumerable1Chunk:
                    return PartitionerEx.Create(dataSource, EnumerablePartitionerOptions.NoBuffering);

                default:
                    break;
            }

            return null;
        }

        public static OrderablePartitioner<Tuple<int, int>> Create(PartitionerType partitionerName, int from, int to, int chunkSize = -1)
        {
            switch (partitionerName)
            {
                case PartitionerType.RangePartitioner:
                    return (chunkSize == -1) ? Partitioner.Create(from, to) : Partitioner.Create(from, to, chunkSize);

                default:
                    break;
            }

            return null;
        }
    }

    public class TestParameters
    {
        public const int Default_StartIndexOffset = 1000;

        public readonly Api Api;

        public readonly StartIndexBase StartIndexBase;

        public int ChunkSize;

        public int Count;

        public ActionWithLocal LocalOption;

        public DataSourceType ParallelForeachDataSourceType;

        // the ParallelLoopState<TLocal> option of the action body
        public WithParallelOption ParallelOption;

        //partitioner
        public PartitionerType PartitionerType;

        public int StartIndex;

        // the real start index (base + offset) for the loop
        public long StartIndex64;

        // the base of the _parameters.StartIndex for boundary testing
        public int StartIndexOffset;

        public ActionWithState StateOption;

        public WorkloadPattern WorkloadPattern;

        public TestParameters(Api api, StartIndexBase startIndexBase, int? startIndexOffset = null)
        {
            Api = api;
            StartIndexBase = startIndexBase;
            StartIndexOffset = startIndexOffset ?? Default_StartIndexOffset;

            if (api == Api.For64)
            {
                // StartIndexBase.Int64 was set to -1 since Enum can't take a Int64.MaxValue. Fixing this below.
                long indexBase64 = (startIndexBase == StartIndexBase.Int64) ? long.MaxValue : (long)startIndexBase;
                StartIndex64 = indexBase64 + StartIndexOffset;
            }
            else
            {
                // startIndexBase must not be StartIndexBase.Int64
                StartIndex = (int)startIndexBase + StartIndexOffset;
            }

            WorkloadPattern = WorkloadPattern.Similar;

            // setting defaults.
            Count = 0;
            ChunkSize = -1;
            StateOption = ActionWithState.None;
            LocalOption = ActionWithLocal.None;
            ParallelOption = WithParallelOption.None;

            //partitioner options
            ParallelForeachDataSourceType = DataSourceType.Collection;
            PartitionerType = PartitionerType.ListBalancedOob;
        }

        // the api to be tested

        // the real start index (base + offset) for the 64 version loop

        // the offset to be added to the base

        // the _parameters.Count of loop range
        // the chunk size to use for the range Partitioner

        // the ParallelLoopState option of the action body
        // the ParallelOptions used in P.For/Foreach

        // the workload pattern used by each workload

        //the partitioner type of the partitioner used - used for Partitioner tests
    }
}