#if LESSTHAN_NET35

namespace System
{
    /// <summary>Encapsulates a method that has two parameters and returns a value of the type specified by the <typeparam name="TResult" /> parameter.</summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
}

#endif