namespace Theraot.Collections
{
    internal interface IClosable
    {
        bool IsClosed { get; }

        void Close();
    }
}