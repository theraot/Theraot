namespace Theraot.Threading.Needles
{
    public interface ICacheNeedle<T> : INeedle<T>, IExpected
    {
        bool TryGet(out T target);
    }
}