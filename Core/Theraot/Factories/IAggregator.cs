#if FAT

namespace Theraot.Factories
{
    public interface IAggregator<in TInput, out TOutput> : IFactory<TOutput>
    {
        void Process(TInput item);

        void Reset();
    }
}

#endif