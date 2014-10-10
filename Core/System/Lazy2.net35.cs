#if NET20 || NET30 || NET35

namespace System
{
    public class Lazy<T, TMetadata> : Lazy<T>
    {
        private readonly TMetadata _metadata;

        public Lazy(TMetadata metadata)
        {
            _metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata)
            : base(valueFactory)
        {
            _metadata = metadata;
        }

        public Lazy(TMetadata metadata, bool isThreadSafe)
            : base(isThreadSafe)
        {
            _metadata = metadata;
        }

        public Lazy(TMetadata metadata, LazyThreadSafetyMode mode)
            : base(mode)
        {
            _metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata, bool isThreadSafe)
            : base(valueFactory, isThreadSafe)
        {
            _metadata = metadata;
        }

        public Lazy(Func<T> valueFactory, TMetadata metadata, LazyThreadSafetyMode mode)
            : base(valueFactory, mode)
        {
            _metadata = metadata;
        }

        public TMetadata Metadata
        {
            get
            {
                return _metadata;
            }
        }
    }
}

#endif