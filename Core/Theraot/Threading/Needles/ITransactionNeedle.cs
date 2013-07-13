#if FAT

namespace Theraot.Threading.Needles
{
    public interface ITransactionNeedle<T> : ITransactionResource, INeedle<T>
    {
        //Empty
    }
}

#endif