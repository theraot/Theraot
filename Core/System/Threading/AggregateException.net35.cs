#if NET20 || NET30 || NET35

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using Theraot.Collections;
using Theraot.Core;

namespace System.Threading
{
    [SerializableAttribute]
    [DebuggerDisplay("Count = {InnerExceptions.Count}")]
    public class AggregateException : Exception
    {
        private const string STR_BaseMessage = "Exception(s) occurred while inside the Parallel loop. {0}.";

        private readonly ReadOnlyCollection<Exception> _innerExceptions;

        public AggregateException()
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(EmptyList<Exception>.Instance);
        }

        public AggregateException(string message)
            : base(message)
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(EmptyList<Exception>.Instance);
        }

        public AggregateException(string message, Exception exception)
            : base(message, Check.NotNullArgument(exception, "exception"))
        {
            _innerExceptions = new ReadOnlyCollection<Exception>(exception.AsUnaryList());
        }

        public AggregateException(params Exception[] innerExceptions)
            : this
            (
                Check.NotNullArgument
                (
                    innerExceptions as IEnumerable<Exception>,
                    "innerExceptions"
                ),
                string.Empty,
                innerExceptions.FirstOrDefault()
            )
        {
            //Empty
        }

        public AggregateException(string message, params Exception[] innerExceptions)
            : this
            (
                Check.NotNullArgument
                (
                    innerExceptions as IEnumerable<Exception>,
                    "innerExceptions"
                ),
                message,
                System.Linq.Enumerable.FirstOrDefault(innerExceptions)
            )
        {
            //Empty
        }

        public AggregateException(IEnumerable<Exception> innerExceptions)
            : this(Check.NotNullArgument(innerExceptions, "innerExceptions"), string.Empty, System.Linq.Enumerable.FirstOrDefault(innerExceptions))
        {
            //Empty
        }

        public AggregateException(string message, IEnumerable<Exception> innerExceptions)
            : this(Check.NotNullArgument(innerExceptions, "innerExceptions"), message, innerExceptions.FirstOrDefault())
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

        private AggregateException(IEnumerable<Exception> innerExceptions, string message, Exception innerException)
            : base(GetFormattedMessage(message, innerExceptions), innerException)
        {
            _innerExceptions = new ReadOnlyCollection<Exception>
            (
                innerExceptions.Where
                (
                    input => input != null,
                    ActionHelper.GetThrowAction(new ArgumentException("An element of innerExceptions is null."))
                ).ToList()
            );
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
            var queue = new ExtendedQueue<AggregateException>
            {
                this
            };
            AggregateException current;
            while (queue.TryTake(out current))
            {
                foreach (var exception in current._innerExceptions)
                {
                    var aggregatedException = exception as AggregateException;
                    if (aggregatedException != null)
                    {
                        queue.Add(aggregatedException);
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
                if (result._innerExceptions.Count == 0 || ReferenceEquals(null, item = result._innerExceptions[0]))
                {
                    return result;
                }
                else
                {
                    var tmp = item as AggregateException;
                    if (tmp == null)
                    {
                        return item;
                    }
                    else
                    {
                        result = tmp;
                    }
                }
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase", Justification = "Microsoft's Design")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Microsoft's Design")]
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            else
            {
                base.GetObjectData(info, context);
                var exceptionArray = new Exception[_innerExceptions.Count];
                _innerExceptions.CopyTo(exceptionArray, 0);
                info.AddValue("InnerExceptions", exceptionArray, typeof(Exception[]));
            }
        }

        public void Handle(Func<Exception, bool> predicate)
        {
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

        private static string GetFormattedMessage(string customMessage, IEnumerable<Exception> exceptions)
        {
            var result = new System.Text.StringBuilder(string.Format(STR_BaseMessage, customMessage));
            foreach (var exception in exceptions)
            {
                result.Append(Environment.NewLine);
                result.Append("[ ");
                result.Append(exception);
                result.Append(" ]");
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }
    }
}

#endif