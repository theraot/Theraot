#if LESSTHAN_NET45

#pragma warning disable ERP022 // Unobserved exception in generic exception handler

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Sources;

namespace System.Threading.Tasks
{
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder))]
    public readonly struct ValueTask : IEquatable<ValueTask>
    {
        internal readonly bool ContinueOnCapturedContext;
        internal readonly object Obj;
        internal readonly short Token;
        private static readonly Task _canceledTask = TaskExEx.FromCanceled(new CancellationToken(canceled: true));

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public ValueTask(Task task)
        {
            Obj = task ?? throw new ArgumentNullException(nameof(task));
            ContinueOnCapturedContext = true;
            Token = 0;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public ValueTask(IValueTaskSource source, short token)
        {
            Obj = source ?? throw new ArgumentNullException(nameof(source));
            Token = token;
            ContinueOnCapturedContext = true;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private ValueTask(object obj, short token, bool continueOnCapturedContext)
        {
            Obj = obj;
            Token = token;
            ContinueOnCapturedContext = continueOnCapturedContext;
        }

        public bool IsCanceled
        {
            get
            {
                var obj = Obj;
                switch (obj)
                {
                    case null:
                        return false;

                    case Task task:
                        return task.IsCanceled;

                    default:
                        return ((IValueTaskSource)obj).GetStatus(Token) == ValueTaskSourceStatus.Canceled;
                }
            }
        }

        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                var obj = Obj;
                switch (obj)
                {
                    case null:
                        return true;

                    case Task task:
                        return task.IsCompleted;

                    default:
                        return ((IValueTaskSource)obj).GetStatus(Token) != ValueTaskSourceStatus.Pending;
                }
            }
        }

        public bool IsCompletedSuccessfully
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                var obj = Obj;
                switch (obj)
                {
                    case null:
                        return true;

                    case Task task:
                        return task.Status == TaskStatus.RanToCompletion;

                    default:
                        return ((IValueTaskSource)obj).GetStatus(Token) == ValueTaskSourceStatus.Succeeded;
                }
            }
        }

        public bool IsFaulted
        {
            get
            {
                var obj = Obj;
                switch (obj)
                {
                    case null:
                        return false;

                    case Task task:
                        return task.IsFaulted;

                    default:
                        return ((IValueTaskSource)obj).GetStatus(Token) == ValueTaskSourceStatus.Faulted;
                }
            }
        }

        internal static Task CompletedTask { get; } = TaskExEx.CompletedTask;

        /// <summary>Creates a <see cref="ValueTask"/> that has completed due to cancellation with the specified cancellation token.</summary>
        /// <param name="cancellationToken">The cancellation token with which to complete the task.</param>
        /// <returns>The canceled task.</returns>
        public static ValueTask FromCanceled(CancellationToken cancellationToken)
        {
            return new ValueTask(TaskExEx.FromCanceled(cancellationToken));
        }

        /// <summary>Creates a <see cref="ValueTask{TResult}"/> that has completed due to cancellation with the specified cancellation token.</summary>
        /// <param name="cancellationToken">The cancellation token with which to complete the task.</param>
        /// <returns>The canceled task.</returns>
        public static ValueTask<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
        {
            return new ValueTask<TResult>(TaskExEx.FromCanceled<TResult>(cancellationToken));
        }

        /// <summary>Creates a <see cref="ValueTask"/> that has completed with the specified exception.</summary>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>The faulted task.</returns>
        public static ValueTask FromException(Exception exception)
        {
            return new ValueTask(TaskExEx.FromException(exception));
        }

        /// <summary>Creates a <see cref="ValueTask{TResult}"/> that has completed with the specified exception.</summary>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>The faulted task.</returns>
        public static ValueTask<TResult> FromException<TResult>(Exception exception)
        {
            return new ValueTask<TResult>(TaskExEx.FromException<TResult>(exception));
        }

        /// <summary>Creates a <see cref="ValueTask{TResult}"/> that's completed successfully with the specified result.</summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="result">The result to store into the completed task.</param>
        /// <returns>The successfully completed task.</returns>
        public static ValueTask<TResult> FromResult<TResult>(TResult result)
        {
            return new ValueTask<TResult>(result);
        }

        public static bool operator !=(ValueTask left, ValueTask right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(ValueTask left, ValueTask right)
        {
            return left.Equals(right);
        }

        public Task AsTask()
        {
            var obj = Obj;
            switch (obj)
            {
                case null:
                    return CompletedTask;

                case Task task:
                    return task;

                default:
                    return GetTaskForValueTaskSource((IValueTaskSource)obj);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public ConfiguredValueTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredValueTaskAwaitable(new ValueTask(Obj, Token, continueOnCapturedContext));
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueTask task && Equals(task);
        }

        public bool Equals(ValueTask other)
        {
            if (Obj != other.Obj)
            {
                return false;
            }

            return Token == other.Token;
        }

        public ValueTaskAwaiter GetAwaiter()
        {
            return new ValueTaskAwaiter(this);
        }

        public override int GetHashCode()
        {
            return Obj?.GetHashCode() ?? 0;
        }

        public ValueTask Preserve()
        {
            return Obj == null ? this : new ValueTask(AsTask());
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        internal void ThrowIfCompletedUnsuccessfully()
        {
            var obj = Obj;
            if (obj == null)
            {
                return;
            }

            if (obj is Task task)
            {
                task.GetAwaiter().GetResult();
                return;
            }

            ((IValueTaskSource)obj).GetResult(Token);
        }

        private Task GetTaskForValueTaskSource(IValueTaskSource t)
        {
            Task completedTask;
            var status = t.GetStatus(Token);
            if (status == ValueTaskSourceStatus.Pending)
            {
                return new ValueTaskSourceAsTask(t, Token).Task;
            }

            try
            {
                t.GetResult(Token);
                completedTask = CompletedTask;
            }
            catch (Exception caughtException)
            {
                var exception = caughtException;
                if (status != ValueTaskSourceStatus.Canceled)
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();
                    taskCompletionSource.TrySetException(exception);
                    completedTask = taskCompletionSource.Task;
                }
                else
                {
                    completedTask = _canceledTask;
                }
            }

            return completedTask;
        }

        private sealed class ValueTaskSourceAsTask : TaskCompletionSource<bool>
        {
            private static readonly Action<object> _completionAction = CompletionAction;

            private readonly short _token;

            private IValueTaskSource? _source;

            public ValueTaskSourceAsTask(IValueTaskSource source, short token)
            {
                _token = token;
                _source = source;
                source.OnCompleted(_completionAction, this, token, ValueTaskSourceOnCompletedFlags.None);
            }

            private static void CompletionAction(object state)
            {
                if (state is not ValueTaskSourceAsTask valueTaskSourceAsTask)
                {
                    throw new ArgumentOutOfRangeException(nameof(state));
                }

                var valueTaskSource = valueTaskSourceAsTask._source;
                if (valueTaskSource == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(state));
                }

                valueTaskSourceAsTask._source = null;
                var status = valueTaskSource.GetStatus(valueTaskSourceAsTask._token);
                try
                {
                    valueTaskSource.GetResult(valueTaskSourceAsTask._token);
                    valueTaskSourceAsTask.TrySetResult(false);
                }
                catch (Exception exception)
                {
                    if (status != ValueTaskSourceStatus.Canceled)
                    {
                        valueTaskSourceAsTask.TrySetException(exception);
                    }
                    else
                    {
                        valueTaskSourceAsTask.TrySetCanceled();
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Provides a value type that wraps a <see cref="Task{TResult}" /> and a
    ///     <typeparamref name="TResult" />,
    ///     only one of which is used.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <remarks>
    ///     <para>
    ///         Methods may return an instance of this value type when it's likely that the result of their
    ///         operations will be available synchronously and when the method is expected to be invoked so
    ///         frequently that the cost of allocating a new <see cref="Task{TResult}" /> for each call will
    ///         be prohibitive.
    ///     </para>
    ///     <para>
    ///         There are tradeoffs to using a <see cref="ValueTask{TResult}" /> instead of a
    ///         <see cref="Task{TResult}" />.
    ///         For example, while a <see cref="ValueTask{TResult}" /> can help avoid an allocation in the
    ///         case where the
    ///         successful result is available synchronously, it also contains two fields whereas a
    ///         <see cref="Task{TResult}" />
    ///         as a reference type is a single field.  This means that a method call ends up returning two fields worth of
    ///         data instead of one, which is more data to copy.  It also means that if a method that returns one of these
    ///         is awaited within an async method, the state machine for that async method will be larger due to needing
    ///         to store the struct that's two fields instead of a single reference.
    ///     </para>
    ///     <para>
    ///         Further, for uses other than consuming the result of an asynchronous operation via await,
    ///         <see cref="ValueTask{TResult}" /> can lead to a more convoluted programming model, which can
    ///         in turn actually
    ///         lead to more allocations.  For example, consider a method that could return either a
    ///         <see cref="Task{TResult}" />
    ///         with a cached task as a common result or a <see cref="ValueTask{TResult}" />.  If the
    ///         consumer of the result
    ///         wants to use it as a <see cref="Task{TResult}" />, such as to use with in methods like
    ///         Task.WhenAll and Task.WhenAny,
    ///         the <see cref="ValueTask{TResult}" /> would first need to be converted into a
    ///         <see cref="Task{TResult}" /> using
    ///         <see cref="ValueTask{TResult}.AsTask" />, which leads to an allocation that would have been
    ///         avoided if a cached
    ///         <see cref="Task{TResult}" /> had been used in the first place.
    ///     </para>
    ///     <para>
    ///         As such, the default choice for any asynchronous method should be to return a
    ///         <see cref="Task" /> or
    ///         <see cref="Task{TResult}" />. Only if performance analysis proves it worthwhile should a
    ///         <see cref="ValueTask{TResult}" />
    ///         be used instead of <see cref="Task{TResult}" />.  There is no non-generic version of
    ///         <see cref="ValueTask{TResult}" />
    ///         as the Task.CompletedTask property may be used to hand back a successfully completed singleton in the case
    ///         where
    ///         a <see cref="Task" />-returning method completes synchronously and successfully.
    ///     </para>
    /// </remarks>
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder<>))]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct ValueTask<TResult> : IEquatable<ValueTask<TResult>>
    {
        /// <summary>The result to be used if the operation completed successfully synchronously.</summary>
        internal readonly TResult _result;

        /// <summary>
        ///     The task to be used if the operation completed asynchronously or if it completed synchronously but
        ///     non-successfully.
        /// </summary>
        internal readonly Task<TResult>? _task;

        /// <summary>Initialize the <see cref="ValueTask{TResult}" /> with the result of the successful operation.</summary>
        /// <param name="result">The result.</param>
        public ValueTask(TResult result)
        {
            _task = null;
            _result = result;
        }

        /// <summary>
        ///     Initialize the <see cref="ValueTask{TResult}" /> with a <see cref="Task{TResult}" /> that represents the operation.
        /// </summary>
        /// <param name="task">The task.</param>
        public ValueTask(Task<TResult> task)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            _result = default!;
        }

        /// <summary>Gets whether the <see cref="ValueTask{TResult}" /> represents a canceled operation.</summary>
        public bool IsCanceled
        {
            get
            {
                return _task?.IsCanceled == true;
            }
        }

        /// <summary>Gets whether the <see cref="ValueTask{TResult}" /> represents a completed operation.</summary>
        public bool IsCompleted
        {
            get
            {
                return _task?.IsCompleted != false;
            }
        }

        /// <summary>Gets whether the <see cref="ValueTask{TResult}" /> represents a successfully completed operation.</summary>
        public bool IsCompletedSuccessfully
        {
            get
            {
                return _task == null || _task.Status == TaskStatus.RanToCompletion;
            }
        }

        /// <summary>Gets whether the <see cref="ValueTask{TResult}" /> represents a failed operation.</summary>
        public bool IsFaulted
        {
            get
            {
                return _task?.IsFaulted == true;
            }
        }

        /// <summary>Gets the result.</summary>
        public TResult Result
        {
            get
            {
                return _task == null ? _result : _task.GetAwaiter().GetResult();
            }
        }

        /// <summary>Creates a method builder for use with an async method.</summary>
        /// <returns>The created builder.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)] // intended only for compiler consumption
        public static AsyncValueTaskMethodBuilder<TResult> CreateAsyncMethodBuilder()
        {
            return AsyncValueTaskMethodBuilder<TResult>.Create();
        }

        /// <summary>Returns a value indicating whether two <see cref="ValueTask{TResult}" /> values are not equal.</summary>
        public static bool operator !=(ValueTask<TResult> left, ValueTask<TResult> right)
        {
            return !left.Equals(right);
        }

        /// <summary>Returns a value indicating whether two <see cref="ValueTask{TResult}" /> values are equal.</summary>
        public static bool operator ==(ValueTask<TResult> left, ValueTask<TResult> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Gets a <see cref="Task{TResult}" /> object to represent this ValueTask.  It will
        ///     either return the wrapped task object if one exists, or it'll manufacture a new
        ///     task object to represent the result.
        /// </summary>
        public Task<TResult> AsTask()
        {
            // Return the task if we were constructed from one, otherwise manufacture one.  We don't
            // cache the generated task into _task as it would end up changing both equality comparison
            // and the hash code we generate in GetHashCode.
            return _task ?? TaskEx.FromResult(_result);
        }

        /// <summary>Configures an awaiter for this value.</summary>
        /// <param name="continueOnCapturedContext">
        ///     true to attempt to marshal the continuation back to the captured context; otherwise, false.
        /// </param>
        public ConfiguredValueTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredValueTaskAwaitable<TResult>(this, continueOnCapturedContext);
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueTask<TResult> task && Equals(task);
        }

        public bool Equals(ValueTask<TResult> other)
        {
            return _task != null || other._task != null ? _task == other._task : EqualityComparer<TResult>.Default.Equals(_result, other._result);
        }

        /// <summary>Gets an awaiter for this value.</summary>
        public ValueTaskAwaiter<TResult> GetAwaiter()
        {
            return new ValueTaskAwaiter<TResult>(this);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode()
        {
            if (_task != null)
            {
                return _task.GetHashCode();
            }

            if (_result != null)
            {
                return _result.GetHashCode();
            }

            return 0;
        }

        /// <summary>Gets a string-representation of this <see cref="ValueTask{TResult}" />.</summary>
        public override string ToString()
        {
            if (_task != null)
            {
                return _task.Status == TaskStatus.RanToCompletion && _task.Result != null ? _task.Result.ToString()! : string.Empty;
            }

            return _result != null ? _result.ToString()! : string.Empty;
        }
    }
}

#endif