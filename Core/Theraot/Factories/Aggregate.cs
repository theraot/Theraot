#if FAT
namespace Theraot.Factories
{
    public delegate TOutput Aggregate<TInput, TOutput>(TOutput seed, TInput input);
}
#endif