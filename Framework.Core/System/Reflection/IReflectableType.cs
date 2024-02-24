#if LESSTHAN_NET45

namespace System.Reflection
{
    public interface IReflectableType
    {
        public TypeInfo GetTypeInfo();
    }
}

#endif