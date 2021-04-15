#if LESSTHAN_NET45

using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    internal static class AsyncMethodTaskCache
    {
        static AsyncMethodTaskCache()
        {
            Singleton<bool>.SetInstance(new CacheBool());
            Singleton<int>.SetInstance(new CacheInt32());
        }

        /// <summary>
        ///     Creates a non-disposable task.
        /// </summary>
        /// <param name="result">The result for the task.</param>
        /// <returns>
        ///     The cacheable task.
        /// </returns>
        internal static TaskCompletionSource<TResult> CreateCompleted<TResult>(TResult result)
        {
            var singleton = Singleton<TResult>.GetInstance();
            if (singleton == null)
            {
                return CacheGeneric<TResult>.FromResultStatic(result);
            }

            return singleton.FromResult(result);
        }

        private static class Singleton<TResult>
        {
            private static CacheGeneric<TResult>? _instance;

            public static CacheGeneric<TResult>? GetInstance()
            {
                return _instance;
            }

            public static void SetInstance(CacheGeneric<TResult>? value)
            {
                _instance = value;
            }
        }

        private sealed class CacheBool : CacheGeneric<bool>
        {
            private readonly TaskCompletionSource<bool> _false = FromResultStatic(result: false);
            private readonly TaskCompletionSource<bool> _true = FromResultStatic(result: true);

            public override TaskCompletionSource<bool> FromResult(bool result)
            {
                return result ? _true : _false;
            }
        }

        private abstract class CacheGeneric<TResult>
        {
            public static TaskCompletionSource<TResult> FromResultStatic(TResult result)
            {
                var completionSource = new TaskCompletionSource<TResult>();
                completionSource.TrySetResult(result);
                return completionSource;
            }

            public abstract TaskCompletionSource<TResult> FromResult(TResult result);
        }

        private sealed class CacheInt32 : CacheGeneric<int>
        {
            private const int _maxInt32ValueExclusive = 9;
            private const int _minInt32ValueInclusive = -1;
            private static readonly TaskCompletionSource<int>[] _int32Tasks = CreateInt32Tasks();

            public override TaskCompletionSource<int> FromResult(int result)
            {
                if (result < _minInt32ValueInclusive || result >= _maxInt32ValueExclusive)
                {
                    return FromResultStatic(result);
                }

                return _int32Tasks[result - -1];
            }

            private static TaskCompletionSource<int>[] CreateInt32Tasks()
            {
                var completionSourceArray = new TaskCompletionSource<int>[10];
                for (var index = 0; index < completionSourceArray.Length; ++index)
                {
                    completionSourceArray[index] = FromResultStatic(index - 1);
                }

                return completionSourceArray;
            }
        }
    }
}

#endif