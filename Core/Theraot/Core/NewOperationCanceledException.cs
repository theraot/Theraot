using System;

#if NET20 || NET30 || NET35

using System.Threading;

#endif

#if NET40 || NET45

using System.Runtime.Serialization;

#endif

namespace Theraot.Core
{
    [Serializable]
    public partial class NewOperationCanceledException : OperationCanceledException
    {
        public NewOperationCanceledException()
        {
            //Empty
        }
    }

    public partial class NewOperationCanceledException : OperationCanceledException
    {
#if NET20 || NET30 || NET35

        [NonSerialized]
        private CancellationToken? _token;

        public NewOperationCanceledException(CancellationToken token)
        {
            _token = token;
        }

        public NewOperationCanceledException(string message, CancellationToken token)
            : base(message)
        {
            _token = token;
        }

        public NewOperationCanceledException(string message, Exception innerException, CancellationToken token)
            : base(message, innerException)
        {
            _token = token;
        }

        public CancellationToken CancellationToken
        {
            get
            {
                if (object.ReferenceEquals(_token, null))
                {
                    return CancellationToken.None;
                }
                else
                {
                    return _token.Value;
                }
            }
        }

#endif
#if NET40 || NET45

        public NewOperationCanceledException(string message)
            : base(message)
        {
            //Empty
        }

        public NewOperationCanceledException(string message, Exception innerException)
            : base(message, innerException)
        {
            //Empty
        }

        protected NewOperationCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //Empty
        }

#endif
    }
}