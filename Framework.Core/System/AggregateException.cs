#if LESSTHAN_NET40

#pragma warning disable CA2235 // Mark all non-serializable fields

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System
{
    [Serializable]
    [DebuggerDisplay("Count = {InnerExceptions.Count}")]
    public class AggregateException : Exception
    {
        private const string _baseMessage = "Exception(s) occurred while inside the Parallel loop. {0}.";

        public AggregateException()
            : base(_baseMessage)
        {
            InnerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        public AggregateException(string message)
            : base(message)
        {
            InnerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        public AggregateException(string message, Exception exception)
            : base(message, exception ?? throw new ArgumentNullException(nameof(exception)))
        {
            InnerExceptions = new ReadOnlyCollection<Exception>(new[] { exception });
        }

        public AggregateException(params Exception[] innerExceptions)
            : this(innerExceptions, string.Empty)
        {
            //Empty
        }

        public AggregateException(string message, params Exception[] innerExceptions)
            : this(innerExceptions, message)
        {
            //Empty
        }

        public AggregateException(IEnumerable<Exception> innerExceptions)
            : this(innerExceptions, string.Empty)
        {
            //Empty
        }

        public AggregateException(string message, IEnumerable<Exception> innerExceptions)
            : this(innerExceptions, message)
        {
            //Empty
        }

        [SecurityCritical]
        protected AggregateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (!(info.GetValue(nameof(InnerExceptions), typeof(Exception[])) is Exception[] value))
            {
                throw new SerializationException("Deserialization Failure");
            }
            InnerExceptions = new ReadOnlyCollection<Exception>(value);
        }

        private AggregateException(CreationInfo creationInfo)
            : base(creationInfo.String, creationInfo.Exception)
        {
            InnerExceptions = creationInfo.InnerExceptions;
        }

        private AggregateException(IEnumerable<Exception> innerExceptions, string message)
            : this(GetCreationInfo(message, innerExceptions))
        {
            // Empty
        }

        public ReadOnlyCollection<Exception> InnerExceptions { get; }

        public AggregateException Flatten()
        {
            var inner = new List<Exception>();
            var queue = new Queue<AggregateException>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var exception in current.InnerExceptions)
                {
                    if (exception is AggregateException aggregatedException)
                    {
                        queue.Enqueue(aggregatedException);
                    }
                    else
                    {
                        inner.Add(exception);
                    }
                }
            }
            return new AggregateException(inner);
        }

        public override Exception GetBaseException()
        {
            var result = this;
            while (true)
            {
                Exception item;
                if (result.InnerExceptions.Count != 1 || (item = result.InnerExceptions[0]) == null)
                {
                    return result;
                }
                if (!(item is AggregateException tmp))
                {
                    return item;
                }
                result = tmp;
            }
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            base.GetObjectData(info, context);
            var exceptionArray = new Exception[InnerExceptions.Count];
            InnerExceptions.CopyTo(exceptionArray, 0);
            info.AddValue(nameof(InnerExceptions), exceptionArray, typeof(Exception[]));
        }

        public void Handle(Func<Exception, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            var failed = new List<Exception>();
            foreach (var exception in InnerExceptions)
            {
                try
                {
                    if (!predicate(exception))
                    {
                        failed.Add(exception);
                    }
                }
                catch
                {
                    throw new AggregateException(failed);
                }
            }
            if (failed.Count > 0)
            {
                throw new AggregateException(failed);
            }
        }

        public override string ToString()
        {
            return Message;
        }

        private static CreationInfo GetCreationInfo(string customMessage, IEnumerable<Exception> innerExceptions)
        {
            if (innerExceptions == null)
            {
                throw new ArgumentNullException(nameof(innerExceptions));
            }
            return new CreationInfo(customMessage, innerExceptions);
        }

        private sealed class CreationInfo
        {
            public CreationInfo(string customMessage, IEnumerable<Exception> innerExceptions)
            {
                var exceptions = new List<Exception>();
                var result = new StringBuilder($"Exception(s) occurred while inside the Parallel loop. {customMessage}.");
                var first = true;
                Exception = null;
                foreach (var exception in innerExceptions)
                {
                    if (exception == null)
                    {
                        throw new ArgumentException("An element of innerExceptions is null.");
                    }
                    if (first)
                    {
                        Exception = exception;
                        first = false;
                    }
                    exceptions.Add(exception);
                    result.Append(Environment.NewLine);
                    result.Append("[ ");
                    result.Append(exception);
                    result.Append(" ]");
                    result.Append(Environment.NewLine);
                }
                String = result.ToString();
                InnerExceptions = exceptions.AsReadOnly();
            }

            public Exception Exception { get; }

            public ReadOnlyCollection<Exception> InnerExceptions { get; }

            public string String { get; }
        }
    }
}

#endif