using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class Needle<T> : INeedle<T>, IUnifiableNeedle<T>
    {
        private int _hashCode;
        private INeedle<T> _target;

        public Needle()
        {
            _target = null;
            _hashCode = base.GetHashCode();
        }

        public Needle(T target)
        {
            _target = new StructNeedle<T>(target);
            if (IsAlive)
            {
                _hashCode = target.GetHashCode();
            }
            else
            {
                _hashCode = base.GetHashCode();
            }
        }

        public Needle(INeedle<T> target)
        {
            _target = target;
            if (IsAlive)
            {
                _hashCode = target.GetHashCode();
            }
            else
            {
                _hashCode = base.GetHashCode();
            }
        }

        public bool IsAlive
        {
            get
            {
                var target = _target;
                return !ReferenceEquals(_target, null) && target.IsAlive;
            }
        }

        public virtual T Value
        {
            get
            {
                Thread.MemoryBarrier();
                return (T)_target;
            }
            set
            {
                SetTarget(value);
            }
        }

        public static explicit operator T(Needle<T> needle)
        {
            if (ReferenceEquals(needle, null))
            {
                throw new ArgumentNullException("needle");
            }
            else
            {
                return needle.Value;
            }
        }

        public static implicit operator Needle<T>(T field)
        {
            return new Needle<T>(field);
        }

        public static bool operator !=(Needle<T> left, Needle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(Needle<T> left, Needle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as Needle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return EqualsExtracted(this, _obj);
            }
            else
            {
                return _target.Equals(obj);
            }
        }

        public bool Equals(Needle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public void Release()
        {
            OnRelease();
        }

        public override string ToString()
        {
            var target = Value;
            if (IsAlive)
            {
                return target.ToString();
            }
            else
            {
                return "<Dead Needle>";
            }
        }

        private bool TryUnifyExtracted<TNeedle>(ref TNeedle value)
            where TNeedle : INeedle<T>
        {
            if (_target is TNeedle)
            {
                value = (TNeedle)_target;
                return true;
            }
            else
            {
                if (NeedleHelper.CanCreateNestedNeedle<T, TNeedle>())
                {
                    value = NeedleHelper.CreateNestedNeedle<T, TNeedle>(this);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool TryUnify<TNeedle>(ref TNeedle value)
            where TNeedle : INeedle<T>
        {
            if (ReferenceEquals(value, null))
            {
                if (ReferenceEquals(_target, null))
                {
                    Func<TNeedle> tmp;
                    if (TypeHelper.TryGetCreate(out tmp))
                    {
                        value = tmp.Invoke();
                        _target = value;
                        Thread.MemoryBarrier();
                        return true;
                    }
                    else
                    {
                        _target = new Needle<T>();
                        Thread.MemoryBarrier();
                        return TryUnifyExtracted<TNeedle>(ref value);
                    }
                }
                else
                {
                    IUnifiableNeedle<T> tmp = value as IUnifiableNeedle<T>;
                    if (ReferenceEquals(tmp, null) && tmp.TryUnify(ref _target))
                    {
                        return true;
                    }
                    else
                    {
                        return TryUnifyExtracted<TNeedle>(ref value);
                    }
                }
            }
            else
            {
                if (ReferenceEquals(_target, value))
                {
                    return true;
                }
                else
                {
                    IUnifiableNeedle<T> tmp = value as IUnifiableNeedle<T>;
                    if (ReferenceEquals(tmp, null) || !tmp.TryUnify(ref _target))
                    {
                        tmp = _target as IUnifiableNeedle<T>;
                        if (ReferenceEquals(tmp, null) && tmp.TryUnify(ref value))
                        {
                            return true;
                        }
                        else
                        {
                            return TryUnifyExtracted<TNeedle>(ref value);
                        }
                    }
                    else
                    {
                        Thread.MemoryBarrier();
                        return true;
                    }
                }
            }
        }

        public bool TryUnify(ref INeedle<T> value)
        {
            if (ReferenceEquals(value, null))
            {
                if (ReferenceEquals(_target, null))
                {
                    _target = new Needle<T>();
                    Thread.MemoryBarrier();
                    value = _target;
                }
                else
                {
                    IUnifiableNeedle<T> tmp = value as IUnifiableNeedle<T>;
                    if (!ReferenceEquals(tmp, null) || !tmp.TryUnify(ref _target))
                    {
                        value = _target;
                    }
                }
            }
            else
            {
                if (!ReferenceEquals(_target, value))
                {
                    IUnifiableNeedle<T> tmp = value as IUnifiableNeedle<T>;
                    if (ReferenceEquals(tmp, null) || !tmp.TryUnify(ref _target))
                    {
                        tmp = _target as IUnifiableNeedle<T>;
                        if (!ReferenceEquals(tmp, null) || !tmp.TryUnify(ref value))
                        {
                            value = _target;
                        }
                    }
                    else
                    {
                        Thread.MemoryBarrier();
                    }
                }
            }
            return true;
        }

        public bool TryUnify(ref Needle<T> value)
        {
            if (ReferenceEquals(value, null))
            {
                if (ReferenceEquals(_target, null))
                {
                    value = new Needle<T>();
                    _target = value;
                    Thread.MemoryBarrier();
                }
                else
                {
                    if (!value.TryUnify(ref _target))
                    {
                        value = new Needle<T>(_target);
                    }
                }
            }
            else
            {
                if (!ReferenceEquals(_target, value))
                {
                    if (value.TryUnify(ref _target))
                    {
                        Thread.MemoryBarrier();
                    }
                    else
                    {
                        Needle<T> tmp = _target as Needle<T>;
                        if (!ReferenceEquals(tmp, null) || !tmp.TryUnify(ref value))
                        {
                            value = new Needle<T>(_target);
                        }
                    }
                }
            }
            return true;
        }

        protected virtual void OnRelease()
        {
            _target = null;
        }

        protected void SetTarget(T value)
        {
            if (ReferenceEquals(_target, null))
            {
                _target = new StructNeedle<T>(value);
                Thread.MemoryBarrier();
            }
            else
            {
                _target.Value = value;
            }
        }

        private static bool EqualsExtracted(Needle<T> left, Needle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return left._target.Equals(left._target);
            }
        }

        private static bool NotEqualsExtracted(Needle<T> left, Needle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return !left._target.Equals(left._target);
            }
        }
    }
}