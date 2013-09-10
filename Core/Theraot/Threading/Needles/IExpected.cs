namespace Theraot.Threading.Needles
{
    public interface IExpected
    {
        bool IsCanceled
        {
            get;
        }

        bool IsCompleted
        {
            get;
        }

        bool IsFaulted
        {
            get;
        }
    }
}
