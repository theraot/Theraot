namespace Theraot.Threading.Needles
{
    public interface ICacheNeedle<T> : INeedle<T>, IExpected
    {
        bool TryGetValue(out T target);
    }
}