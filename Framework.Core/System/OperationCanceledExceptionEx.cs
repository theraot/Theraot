// Needed for NET35 (TASK)

using System.Runtime.Serialization;
using System.Threading;

namespace System
{
    [Serializable]
    public partial class OperationCanceledExceptionEx : OperationCanceledException
    {
        public OperationCanceledExceptionEx()
        {
            // Empty
        }

        public OperationCanceledExceptionEx(string message)
            : base(message)
        {
            //Empty
        }

        public OperationCanceledExceptionEx(string message, Exception innerException)
            : base(message, innerException)
        {
            //Empty
        }

#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET
        protected OperationCanceledExceptionEx(SerializationInfo info, StreamingContext context)
            : base(info, context)
#else
        [Obsolete("This target platform does not support binary serialization.")]
        protected OperationCanceledExceptionEx(SerializationInfo info, StreamingContext context)
#endif
        {
            Theraot.No.Op(info);
            Theraot.No.Op(context);
        }
    }

    public partial class OperationCanceledExceptionEx
    {
#if LESSTHAN_NET40

        [NonSerialized]
        private readonly CancellationToken? _token;

        public OperationCanceledExceptionEx(CancellationToken token)
        {
            _token = token;
        }

        public OperationCanceledExceptionEx(string message, CancellationToken token)
            : base(message)
        {
            _token = token;
        }

        public OperationCanceledExceptionEx(string message, Exception innerException, CancellationToken token)
            : base(message, innerException)
        {
            _token = token;
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

        public OperationCanceledExceptionEx(CancellationToken token)
            : base(token)
        {
            // Empty
        }

        public OperationCanceledExceptionEx(string message, CancellationToken token)
            : base(message, token)
        {
            // Empty
        }

        public OperationCanceledExceptionEx(string message, Exception innerException, CancellationToken token)
            : base(message, innerException, token)
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