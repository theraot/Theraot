// Needed for Workaround

using System;
using System.Reflection;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class WeakDelegateNeedle : WeakNeedle<Delegate>, IEquatable<Delegate>, IEquatable<WeakDelegateNeedle>
    {
        public WeakDelegateNeedle(Delegate handler)
            : base(Check.NotNullArgument(handler, "handler"))
        {
            Check.NotNullArgument(handler, "handler");
        }

        public WeakDelegateNeedle(MethodInfo methodInfo, object target)
            : base(BuildDelegate(methodInfo, target))
        {
            // Empty
        }

        public MethodInfo Method
        {
            get
            {
                var value = Value;
                if (IsAlive)
                {
                    return value.Method;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool Equals(Delegate other)
        {
            return !ReferenceEquals(null, other) && Equals(other.Method, other.Target);
        }

        public bool Equals(MethodInfo method, object target)
        {
            var value = Value;
            if (IsAlive)
            {
                return value.Method.Equals(method) && ReferenceEquals(value.Target, target);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(WeakDelegateNeedle other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            else
            {
                var value = Value;
                if (IsAlive)
                {
                    var otherValue = other.Value;
                    if (other.IsAlive)
                    {
                        return value.Method.Equals(otherValue.Method) && ReferenceEquals(value.Target, otherValue.Target);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return !other.IsAlive;
                }
            }
        }

        public void Invoke(object[] args)
        {
            TryInvoke(args);
        }

        public bool TryInvoke(object[] args)
        {
            var value = Value;
            if (IsAlive)
            {
                value.DynamicInvoke(args); // Throws TargetInvocationException
                return true;
            }
            else
            {
                return false;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Generic Version Is Available")]
        public bool TryInvoke(object[] args, out object result)
        {
            var value = Value;
            if (IsAlive)
            {
                result = value.DynamicInvoke(args);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public bool TryInvoke<TResult>(object[] args, out TResult result)
        {
            var value = Value;
            if (IsAlive)
            {
                result = (TResult)value.DynamicInvoke(args);
                return true;
            }
            else
            {
                result = default(TResult);
                return false;
            }
        }

        private static Delegate BuildDelegate(MethodInfo methodInfo, object target)
        {
            if (ReferenceEquals(methodInfo, null))
            {
                throw new ArgumentNullException("methodInfo");
            }
            else
            {
                if (methodInfo.IsStatic != ReferenceEquals(null, target))
                {
                    if (ReferenceEquals(target, null))
                    {
                        throw new ArgumentNullException("target", "target is null and the method is not static.");
                    }
                    else
                    {
                        throw new ArgumentException("target is not null and the method is static", "target");
                    }
                }
                else
                {
                    var type = methodInfo.DeclaringType;
                    if (ReferenceEquals(type, null))
                    {
                        throw new ArgumentException("methodInfo.DeclaringType is null", "methodInfo");
                    }
                    else
                    {
                        return Delegate.CreateDelegate(type, target, methodInfo);
                    }
                }
            }
        }
    }
}