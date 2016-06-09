// Needed for NET35 (TASK)

using System;

#if NET35

using System.Threading;

#endif

using System.Runtime.Serialization;

namespace Theraot.Core
{
    [Serializable]
    public partial class NewOperationCanceledException : OperationCanceledException
    {
        public NewOperationCanceledException()
        {
            // Empty
        }
    }

    public partial class NewOperationCanceledException
    {
#if NET35

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
                if (ReferenceEquals(_token, null))
                {
                    return CancellationToken.None;
                }
                return _token.Value;
            }
        }

#endif

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

        protected NewOperationCanceledException(SerializationInfo info, StreamingContext scheduler)
            : base(info, scheduler)
        {
            //Empty
        }
    }
}