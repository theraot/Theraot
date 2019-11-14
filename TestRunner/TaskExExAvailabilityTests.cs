using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Theraot;
using TestRunner.AuxiliaryTypes;

namespace TestRunner
{
    public static class TaskExExAvailabilityTests
    {
        public static void MethodAvailability()
        {
            No.Op<Func<CancellationToken, Task>>(TaskExEx.FromCanceled);
            No.Op<Func<CancellationToken, Task<TResult>>>(TaskExEx.FromCanceled<TResult>);
            No.Op<Func<Exception, Task>>(TaskExEx.FromException);
            No.Op<Func<TResult, Task>>(TaskExEx.FromResult);
            No.Op<Func<YieldAwaitable>>(TaskExEx.Yield);
            No.Op<Func<CancellationToken, Task>>(TaskExEx.FromCancellation);
            No.Op<Func<CancellationToken, Task<TResult>>>(TaskExEx.FromCancellation<TResult>);
            No.Op<Func<WaitHandle, Task>>(TaskExEx.FromWaitHandle);
            No.Op<Func<WaitHandle, CancellationToken, Task>>(TaskExEx.FromWaitHandle);
            No.Op<Func<WaitHandle, int, Task>>(TaskExEx.FromWaitHandle);
            No.Op<Func<WaitHandle, int, CancellationToken, Task>>(TaskExEx.FromWaitHandle);
            No.Op<Func<WaitHandle, TimeSpan, Task>>(TaskExEx.FromWaitHandle);
            No.Op<Func<WaitHandle, TimeSpan, CancellationToken, Task>>(TaskExEx.FromWaitHandle);
        }

        public static void PropertyAvailability()
        {
            No.Op(TaskExEx.CompletedTask);
        }
    }
}