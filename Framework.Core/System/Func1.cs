#if LESSTHAN_NET35

namespace System
{
    /// <summary>Encapsulates a method that has no parameters and returns a value of the type specified by the <typeparam name="TResult" /> parameter.</summary>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    public delegate TResult Func<out TResult>();
}

#endif