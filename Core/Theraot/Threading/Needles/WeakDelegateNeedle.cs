using System;
using System.Reflection;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class WeakDelegateNeedle : WeakNeedle<object>, IEquatable<Delegate>, IEquatable<WeakDelegateNeedle>
    {
        private readonly MethodInfo _method;

        private int _hashCode;

        public WeakDelegateNeedle(Delegate handler)
            : base(Check.NotNullArgument(handler, "handler").Target)
        {
            var _handler = Check.NotNullArgument(handler, "handler");
            _hashCode = _handler.Method.GetHashCode();
            _method = _handler.Method;
        }

        public WeakDelegateNeedle(MethodInfo methodInfo, object target)
            : base(target)
        {
            var _target = Check.NotNullArgument(target, "target");
            var _methodInfo = Check.NotNullArgument(methodInfo, "methodInfo");
            if (!(_methodInfo.IsStatic && ReferenceEquals(null, _target)) || (!ReferenceEquals(null, _target)))
            {
                if (_target == null)
                {
                    throw new ArgumentNullException("target", "target is null and the method is not static.");
                }
                else
                {
                    throw new ArgumentException("target is not null and the method is static", "target");
                }
            }
            _hashCode = _methodInfo.GetHashCode();
            _method = _methodInfo;
        }

        public bool IsTargetValid
        {
            get
            {
                object target = Value;
                return VerifyTarget(target);
            }
        }

        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as WeakDelegateNeedle;
            if (ReferenceEquals(null, _obj))
            {
                return Equals(obj as Delegate);
            }
            else
            {
                return Equals(_obj);
            }
        }

        public bool Equals(Delegate other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            else
            {
                return Equals(other.Method, other.Target);
            }
        }

        public bool Equals(MethodInfo method, object target)
        {
            return _method == method && ReferenceEquals(Value, target);
        }

        public bool Equals(WeakDelegateNeedle other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            else
            {
                object target = Value;
                return other._method.Equals(_method) && other.Value.Equals(target);
            }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public void Invoke(object[] args)
        {
            TryInvoke(args);
        }

        public bool TryInvoke(object[] args)
        {
            object target = Value;
            if (VerifyTarget(target))
            {
                Method.Invoke(target, args);
                return true;
            }
            return false;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Generic Version Is Available")]
        public bool TryInvoke(object[] args, out object result)
        {
            object target = Value;
            if (VerifyTarget(target))
            {
                result = Method.Invoke(target, args);
                return true;
            }
            result = null;
            return false;
        }

        public bool TryInvoke<TResult>(object[] args, out TResult result)
        {
            object target = Value;
            if (VerifyTarget(target))
            {
                result = (TResult)Method.Invoke(target, args);
                return true;
            }
            result = default(TResult);
            return false;
        }

        protected bool VerifyTarget(object target)
        {
            return Method.IsStatic == (target == null);
        }
    }
}