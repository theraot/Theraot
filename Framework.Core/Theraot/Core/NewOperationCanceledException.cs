// Needed for NET35 (TASK)

using System;
using System.Threading;

namespace Theraot.Core
{
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2

    [Serializable]
#endif
    public partial class NewOperationCanceledException : OperationCanceledException
    {
        public NewOperationCanceledException()
        {
            // Empty
            // This constructor is not redundant
            // If removed, the compiler will not generate the default one...
            // because there are other constructors in the class
        }
    }

    public partial class NewOperationCanceledException
    {
#if NET20 || NET30 || NET35

        [NonSerialized]
        private readonly CancellationToken? _token;

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

        public CancellationToken CancellationToken
        {
            get
            {
                if (_token == null)
                {
                    return CancellationToken.None;
                }
                return _token.Value;
            }
        }

#else

        public NewOperationCanceledException(CancellationToken token)
            : base(token)
        {
            // Empty
        }

#endif

        public void Deconstruct(out CancellationToken token)
        {
            token = CancellationToken;
        }
    }
}