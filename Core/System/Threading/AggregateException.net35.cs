#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using Theraot.Core;

namespace System.Threading
{
    [SerializableAttribute]
    [DebuggerDisplay("Count = {InnerExceptions.Count}")]
    public class AggregateException : Exception
    {
        private const string STR_BaseMessage = "Exception(s) occurred while inside the Parallel loop. {0}.";

        [ThreadStatic]
        private static CreationInfo _creationInfo;

        private readonly ReadOnlyCollection<Exception> _innerExceptions;

        public AggregateException()
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        public AggregateException(string message)
            : base(message)
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
        }

        public AggregateException(string message, Exception exception)
            : base(message, Check.NotNullArgument(exception, "exception"))
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(new[] { exception });
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
            if (info != null)
            {
                var value = info.GetValue("InnerExceptions", typeof(Exception[])) as Exception[];
                if (value == null)
                {
                    throw new SerializationException("Deserialization Failure");
                }
                else
                {
                    _innerExceptions = new ReadOnlyCollection<Exception>(value);
                }
            }
            else
            {
                throw new ArgumentNullException("info");
            }
        }

        private AggregateException(IEnumerable<Exception> innerExceptions, string message)
            : base(GetCreationInfo(message, innerExceptions).String, _creationInfo.Exception)
        {
            _innerExceptions = _creationInfo.InnerExceptions;
            _creationInfo = null;
        }

        public ReadOnlyCollection<Exception> InnerExceptions
        {
            get
            {
                return _innerExceptions;
            }
        }

        public AggregateException Flatten()
        {
            var inner = new List<Exception>();
            var queue = new Queue<AggregateException>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                AggregateException current = queue.Dequeue();
                foreach (var exception in current._innerExceptions)
                {
                    var aggregatedException = exception as AggregateException;
                    if (aggregatedException != null)
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
            AggregateException result = this;
            while (true)
            {
                Exception item;
                if (result._innerExceptions.Count != 1 || ReferenceEquals(null, item = result._innerExceptions[0]))
                {
                    return result;
                }
                var tmp = item as AggregateException;
                if (tmp == null)
                {
                    return item;
                }
                result = tmp;
            }
        }

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase", Justification = "Microsoft's Design")]
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Microsoft's Design")]
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            var exceptionArray = new Exception[_innerExceptions.Count];
            _innerExceptions.CopyTo(exceptionArray, 0);
            info.AddValue("InnerExceptions", exceptionArray, typeof(Exception[]));
        }

        public void Handle(Func<Exception, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var failed = new List<Exception>();
            foreach (var exception in _innerExceptions)
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
                throw new ArgumentNullException("innerExceptions");
            }
            return _creationInfo = new CreationInfo(customMessage, innerExceptions);
        }

        private class CreationInfo
        {
            private readonly Exception _exception;

            private readonly ReadOnlyCollection<Exception> _innerExceptions;

            private readonly string _string;

            public CreationInfo(string customMessage, IEnumerable<Exception> innerExceptions)
            {
                var exceptions = new List<Exception>();
                var result = new Text.StringBuilder(string.Format(STR_BaseMessage, customMessage));
                var first = true;
                _exception = null;
                foreach (var exception in innerExceptions)
                {
                    if (exception == null)
                    {
                        throw new ArgumentException("An element of innerExceptions is null.");
                    }
                    if (first)
                    {
                        _exception = exception;
                        first = false;
                    }
                    exceptions.Add(exception);
                    result.Append(Environment.NewLine);
                    result.Append("[ ");
                    result.Append(exception);
                    result.Append(" ]");
                    result.Append(Environment.NewLine);
                }
                _string = result.ToString();
                _innerExceptions = exceptions.AsReadOnly();
            }

            public Exception Exception
            {
                get
                {
                    return _exception;
                }
            }

            public ReadOnlyCollection<Exception> InnerExceptions
            {
                get
                {
                    return _innerExceptions;
                }
            }

            public string String
            {
                get
                {
                    return _string;
                }
            }
        }
    }
}

#endif