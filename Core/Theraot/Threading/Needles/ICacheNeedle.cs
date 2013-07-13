namespace Theraot.Threading.Needles
{
    public interface ICacheNeedle<T> : INeedle<T>
    {
        bool IsCached
        {
            get;
        }
    }
}