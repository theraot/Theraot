// Needed for NET35 (TASK)

#pragma warning disable RCS1231 // Make parameter ref read-only.

using System.Runtime.Serialization;
using System.Threading;
using Theraot;

namespace System
{
    [Serializable]
    public partial class OperationCanceledExceptionEx : OperationCanceledException
    {
        public OperationCanceledExceptionEx()
        {
        }

        public OperationCanceledExceptionEx(string message)
            : base(message)
        {
        }

        public OperationCanceledExceptionEx(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET

        protected OperationCanceledExceptionEx(SerializationInfo info, StreamingContext context)
            : base(info, context)

#else
        [Obsolete("This target platform does not support binary serialization.")]
        protected OperationCanceledExceptionEx(SerializationInfo info, StreamingContext context)
#endif
        {
            No.Op(info);
            No.Op(context);
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

        public CancellationToken CancellationToken => _token ?? CancellationToken.None;

#else
        public OperationCanceledExceptionEx(CancellationToken token)
            : base(token)
        {
        }

        public OperationCanceledExceptionEx(string message, CancellationToken token)
            : base(message, token)
        {
        }

        public OperationCanceledExceptionEx(string message, Exception innerException, CancellationToken token)
            : base(message, innerException, token)
        {
        }

#endif

        public void Deconstruct(out CancellationToken token)
        {
            token = CancellationToken;
        }
    }
}