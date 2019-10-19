#if LESSTHAN_NET40 || NETSTANDARD1_0

// BASEDON: https://github.com/dotnet/corefx/blob/e0ba7aa8026280ee3571179cc06431baf1dfaaac/src/System.Threading.Tasks.Parallel/src/System/Threading/Tasks/Parallel.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
// A helper class that contains parallel versions of various looping constructs.  This
// internally uses the task parallel library, but takes care to expose very little
// evidence of this infrastructure being used.
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

namespace System.Threading.Tasks
{
    /// <summary>
    ///     Stores options that configure the operation of methods on the
    ///     <see cref="System.Threading.Tasks.Parallel">Parallel</see> class.
    /// </summary>
    /// <remarks>
    ///     By default, methods on the Parallel class attempt to utilize all available processors, are non-cancelable, and
    ///     target
    ///     the default TaskScheduler (TaskScheduler.Default). <see cref="ParallelOptions" /> enables
    ///     overriding these defaults.
    /// </remarks>
    public class ParallelOptions
    {
        private int _maxDegreeOfParallelism;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParallelOptions" /> class.
        /// </summary>
        /// <remarks>
        ///     This constructor initializes the instance with default values.  <see cref="MaxDegreeOfParallelism" />
        ///     is initialized to -1, signifying that there is no upper bound set on how much parallelism should
        ///     be employed.  <see cref="CancellationToken" /> is initialized to a non-cancelable token,
        ///     and <see cref="TaskScheduler" /> is initialized to the default scheduler (TaskScheduler.Default).
        ///     All of these defaults may be overwritten using the property set accessors on the instance.
        /// </remarks>
        public ParallelOptions()
        {
            TaskScheduler = TaskScheduler.Default;
            _maxDegreeOfParallelism = -1;
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Threading.CancellationToken">CancellationToken</see>
        ///     associated with this <see cref="ParallelOptions" /> instance.
        /// </summary>
        /// <remarks>
        ///     Providing a <see cref="System.Threading.CancellationToken">CancellationToken</see>
        ///     to a <see cref="System.Threading.Tasks.Parallel">Parallel</see> method enables the operation to be
        ///     exited early. Code external to the operation may cancel the token, and if the operation observes the
        ///     token being set, it may exit early by throwing an
        ///     <see cref="System.OperationCanceledExceptionEx" />.
        /// </remarks>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        ///     Gets or sets the maximum degree of parallelism enabled by this ParallelOptions instance.
        /// </summary>
        /// <remarks>
        ///     The <see cref="MaxDegreeOfParallelism" /> limits the number of concurrent operations run by
        ///     <see cref="System.Threading.Tasks.Parallel">Parallel</see> method calls that are passed this
        ///     ParallelOptions instance to the set value, if it is positive.
        ///     If <see cref="MaxDegreeOfParallelism" /> is -1, then there is no limit placed on the number of concurrently
        ///     running operations.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The exception that is thrown when this <see cref="MaxDegreeOfParallelism" /> is set to 0 or some
        ///     value less than -1.
        /// </exception>
        public int MaxDegreeOfParallelism
        {
            get => _maxDegreeOfParallelism;
            set
            {
                if (value == 0 || value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _maxDegreeOfParallelism = value;
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Threading.Tasks.TaskScheduler">TaskScheduler</see>
        ///     associated with this <see cref="ParallelOptions" /> instance. Setting this property to null
        ///     indicates that the current scheduler should be used.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public TaskScheduler TaskScheduler { get; set; }

        internal int EffectiveMaxConcurrencyLevel
        {
            get
            {
                var result = MaxDegreeOfParallelism;
                var schedulerMax = EffectiveTaskScheduler.MaximumConcurrencyLevel;
                if (schedulerMax > 0 && schedulerMax != int.MaxValue)
                {
                    result = result == -1 ? schedulerMax : Math.Min(schedulerMax, result);
                }

                return result;
            }
        }

        // Convenience property used by TPL logic
        internal TaskScheduler EffectiveTaskScheduler => TaskScheduler ?? TaskScheduler.Current;
    }
}

#endif