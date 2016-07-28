namespace System.Collections.Generic
{
#if NET40

    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count
        {
            get;
        }
    }

#endif
#if NET35

    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        int Count
        {
            get;
        }
    }

#endif
}