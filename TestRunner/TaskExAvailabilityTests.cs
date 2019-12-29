using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Theraot;
using TestRunner.AuxiliaryTypes;

#if LESSTHAN_NET45 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

using System.Runtime.CompilerServices;

#else

using Microsoft.Runtime.CompilerServices;

#endif

namespace TestRunner
{
    public static class TaskExAvailabilityTests
    {
        public static void MethodAvailability()
        {
            No.Op<Func<int, Task>>(TaskEx.Delay);
            No.Op<Func<TimeSpan, Task>>(TaskEx.Delay);
            No.Op<Func<TimeSpan, CancellationToken, Task>>(TaskEx.Delay);
            No.Op<Func<int, CancellationToken, Task>>(TaskEx.Delay);
            No.Op<Func<TResult, Task<TResult>>>(TaskEx.FromResult);
            No.Op<Func<Action, Task>>(TaskEx.Run);
            No.Op<Func<Action, CancellationToken, Task>>(TaskEx.Run);
            No.Op<Func<Func<TResult>, Task<TResult>>>(TaskEx.Run);
            No.Op<Func<Func<TResult>, CancellationToken, Task<TResult>>>(TaskEx.Run);
            No.Op<Func<Func<Task>, Task>>(TaskEx.Run);
            No.Op<Func<Func<Task>, CancellationToken, Task>>(TaskEx.Run);
            No.Op<Func<Func<Task<TResult>>, Task<TResult>>>(TaskEx.Run);
            No.Op<Func<Func<Task<TResult>>, CancellationToken, Task<TResult>>>(TaskEx.Run);
            No.Op<Func<Task[], Task>>(TaskEx.WhenAll);
            No.Op<Func<Task<TResult>[], Task<TResult[]>>>(TaskEx.WhenAll);
            No.Op<Func<IEnumerable<Task>, Task>>(TaskEx.WhenAll);
            No.Op<Func<IEnumerable<Task<TResult>>, Task<TResult[]>>>(TaskEx.WhenAll);
            No.Op<Func<Task[], Task>>(TaskEx.WhenAny);
            No.Op<Func<Task<TResult>[], Task<Task<TResult>>>>(TaskEx.WhenAny);
            No.Op<Func<IEnumerable<Task>, Task>>(TaskEx.WhenAny);
            No.Op<Func<IEnumerable<Task<TResult>>, Task<Task<TResult>>>>(TaskEx.WhenAny);
            No.Op<Func<YieldAwaitable>>(TaskEx.Yield);
        }
    }
}