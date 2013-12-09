using System;
using System.Threading;

namespace Theraot.Core
{
    [Serializable]
    public class NewOperationCanceledException : OperationCanceledException
    {
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
    }
}