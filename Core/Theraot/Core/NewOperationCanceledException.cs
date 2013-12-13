using System;
using System.Runtime.Serialization;
using System.Threading;

namespace Theraot.Core
{
    [Serializable]
    public class NewOperationCanceledException : OperationCanceledException
    {
        public NewOperationCanceledException()
        {
            //Empty
        }

#if NET20 || NET30 || NET35

        [NonSerialized]
        private CancellationToken? _token;

        public NewOperationCanceledException(CancellationToken token)
            : base()
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

#else

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