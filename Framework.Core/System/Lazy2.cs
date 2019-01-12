#if LESSTHAN_NET40

namespace System
{
    public class Lazy<T, TMetadata> : Lazy<T>
    {
        public Lazy(TMetadata metadata)
        {
            Metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata)
            : base(valueFactory)
        {
            Metadata = metadata;
        }

        public Lazy(TMetadata metadata, bool isThreadSafe)
            : base(isThreadSafe)
        {
            Metadata = metadata;
        }

        public Lazy(TMetadata metadata, LazyThreadSafetyMode mode)
            : base(mode)
        {
            Metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata, bool isThreadSafe)
            : base(valueFactory, isThreadSafe)
        {
            Metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata, LazyThreadSafetyMode mode)
            : base(valueFactory, mode)
        {
            Metadata = metadata;
        }

        public TMetadata Metadata { get; }
    }
}

#endif