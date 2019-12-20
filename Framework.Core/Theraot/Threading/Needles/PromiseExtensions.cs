namespace Theraot.Threading.Needles
{
    public static class PromiseExtensions
    {
        public static IWaitablePromise GetAwaiter(this IWaitablePromise promise)
        {
            return promise;
        }

        public static IWaitablePromise<T> GetAwaiter<T>(this IWaitablePromise<T> promise)
            where T : class
        {
            return promise;
        }
    }
}