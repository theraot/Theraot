#if LESSTHAN_NETSTANDARD13

namespace System.Runtime.Serialization
{
    public interface IDeserializationCallback
    {
        void OnDeserialization(object sender);
    }
}

#endif