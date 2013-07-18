using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class LazyNeedle<T> : Needle<T>, ICacheNeedle<T>, IEquatable<LazyNeedle<T>>
    {
        private int _isValueCreated;
        private Func<T> _valueFactory;

        public LazyNeedle(Func<T> valueFactory)
            : this(valueFactory, default(T))
        {
            //Empty
        }

        public LazyNeedle(Func<T> valueFactory, T target)
            : base(target)
        {
            Func<T> __valueFactory = valueFactory ?? (() => target);
            Thread thread = null;
            var waitHandle = new ManualResetEvent(false);
            int preIsValueCreated = 0;
            _valueFactory =
                () =>
                {
                    return FullMode(__valueFactory, waitHandle, ref thread, ref preIsValueCreated);
                };
        }

        public bool IsCached
        {
            get
            {
                return Thread.VolatileRead(ref _isValueCreated) == 1;
            }
        }

        public override T Value
        {
            get
            {
                Initialize();
                return base.Value;
            }
            set
            {
                SetTarget(value);
                Interlocked.Exchange(ref _isValueCreated, 1);
            }
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as LazyNeedle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return base.Equals(obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(LazyNeedle<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                return base.Equals(other as Needle<T>);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Initialize()
        {
            _valueFactory.Invoke();
        }

        protected virtual void Initialize(Action beforeInitialize)
        {
            var _beforeInitialize = Check.NotNullArgument(beforeInitialize, "beforeInitialize");
            if (Thread.VolatileRead(ref _isValueCreated) == 0)
            {
                try
                {
                    _beforeInitialize.Invoke();
                }
                finally
                {
                    _valueFactory.Invoke();
                }
            }
        }

        private T FullMode(Func<T> valueFactory, ManualResetEvent waitHandle, ref Thread thread, ref int preIsValueCreated)
        {
        back:
            if (Interlocked.CompareExchange(ref preIsValueCreated, 1, 0) == 0)
            {
                try
                {
                    thread = Thread.CurrentThread;
                    var _target = valueFactory.Invoke();
                    SetTarget(_target);
                    Thread.VolatileWrite(ref _isValueCreated, 1);
                    return _target;
                }
                catch (Exception)
                {
                    Thread.VolatileWrite(ref preIsValueCreated, 0);
                    throw;
                }
                finally
                {
                    waitHandle.Set();
                    thread = null;
                }
            }
            else
            {
                if (ReferenceEquals(thread, Thread.CurrentThread))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    waitHandle.WaitOne();
                    if (Thread.VolatileRead(ref preIsValueCreated) == 1)
                    {
                        return base.Value;
                    }
                    else
                    {
                        goto back;
                    }
                }
            }
        }
    }
}