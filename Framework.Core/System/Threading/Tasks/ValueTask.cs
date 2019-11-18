#if LESSTHAN_NET45

using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;

namespace System.Threading.Tasks
{
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder))]
    public readonly struct ValueTask : IEquatable<ValueTask>
    {
        internal readonly bool ContinueOnCapturedContext;
        internal readonly object Obj;
        internal readonly short Token;
        private static readonly Task _task = TaskExEx.FromCanceled(new CancellationToken(true));

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
                if (obj == null)
                {
                    return false;
                }
                if (obj is Task task)
                {
                    return task.IsCanceled;
                }
                return ((IValueTaskSource)obj).GetStatus(Token) == ValueTaskSourceStatus.Canceled;
            }
        }

        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                var obj = Obj;
                if (obj == null)
                {
                    return true;
                }
                if (obj is Task task)
                {
                    return task.IsCompleted;
                }
                return ((IValueTaskSource)obj).GetStatus(Token) != ValueTaskSourceStatus.Pending;
            }
        }

        public bool IsCompletedSuccessfully
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
            get
            {
                var obj = Obj;
                if (obj == null)
                {
                    return true;
                }
                if (obj is Task task)
                {
                    return task.Status == TaskStatus.RanToCompletion;
                }
                return ((IValueTaskSource)obj).GetStatus(Token) == ValueTaskSourceStatus.Succeeded;
            }
        }

        public bool IsFaulted
        {
            get
            {
                var obj = Obj;
                if (obj == null)
                {
                    return false;
                }
                if (obj is Task task)
                {
                    return task.IsFaulted;
                }
                return ((IValueTaskSource)obj).GetStatus(Token) == ValueTaskSourceStatus.Faulted;
            }
        }

        internal static Task CompletedTask { get; } = TaskExEx.CompletedTask;

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
            if (obj == null)
            {
                return CompletedTask;
            }
            if (obj is Task task)
            {
                return task;
            }
            return GetTaskForValueTaskSource((IValueTaskSource)obj);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public ConfiguredValueTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
        {
            return new ConfiguredValueTaskAwaitable(new ValueTask(Obj, Token, continueOnCapturedContext));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ValueTask))
            {
                return false;
            }
            return Equals((ValueTask)obj);
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
            var obj = Obj;
            if (obj != null)
            {
                return obj.GetHashCode();
            }
            return 0;
        }

        public ValueTask Preserve()
        {
            if (Obj == null)
            {
                return this;
            }
            return new ValueTask(AsTask());
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
                return (new ValueTaskSourceAsTask(t, Token)).Task;
            }
            try
            {
                t.GetResult(Token);
                completedTask = CompletedTask;
            }
            catch (Exception exception1)
            {
                var exception = exception1;
                if (status != ValueTaskSourceStatus.Canceled)
                {
                    var taskCompletionSource = new TaskCompletionSource<bool>();
                    taskCompletionSource.TrySetException(exception);
                    completedTask = taskCompletionSource.Task;
                }
                else
                {
                    completedTask = _task;
                }
            }
            return completedTask;
        }

        private sealed class ValueTaskSourceAsTask : TaskCompletionSource<bool>
        {
            private static readonly Action<object> _completionAction = state =>
            {
                if (!(state is ValueTaskSourceAsTask valueTaskSourceAsTask))
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
                catch (Exception exception1)
                {
                    var exception = exception1;
                    if (status != ValueTaskSourceStatus.Canceled)
                    {
                        valueTaskSourceAsTask.TrySetException(exception);
                    }
                    else
                    {
                        valueTaskSourceAsTask.TrySetCanceled();
                    }
                }
            };

            private readonly short _token;
            private IValueTaskSource? _source;

            public ValueTaskSourceAsTask(IValueTaskSource source, short token)
            {
                _token = token;
                _source = source;
                source.OnCompleted(_completionAction, this, token, ValueTaskSourceOnCompletedFlags.None);
            }
        }
    }
}

#endif