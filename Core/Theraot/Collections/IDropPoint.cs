namespace Theraot.Collections
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "By Design")]
    public interface IDropPoint<T> : IReadOnlyDropPoint<T>
    {
        bool Add(T item);

        void Clear();

        bool TryTake(out T item);
    }
}