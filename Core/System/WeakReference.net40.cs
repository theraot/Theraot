#if NET20 || NET30 || NET35 || NET40

using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System
{
    public sealed class WeakReference<T> : ISerializable
       where T : class
    {
        private GCHandle _handle;
        private bool _trackResurrection;

        public WeakReference(T target)
            : this(target, false)
        {
            //Empty
        }

        public WeakReference(T target, bool trackResurrection)
        {
            _trackResurrection = trackResurrection;
            SetTarget(target);
        }

        internal WeakReference(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            else
            {
                T value = (T)info.GetValue("TrackedObject", typeof(T));
                _trackResurrection = info.GetBoolean("TrackResurrection");
                SetTarget(value);
            }
        }

        public T Target
        {
            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            get
            {
                T value;
                TryGetTarget(out value);
                return value;
            }
            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            set
            {
                SetTarget(value);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            else
            {
                info.AddValue("TrackedObject", this.Target, typeof(T));
                info.AddValue("TrackResurrection", _trackResurrection);
            }
        }

        public void SetTarget(T value)
        {
            var oldHandle = _handle;
            _handle = GetNewHandle(value, _trackResurrection);
            if (oldHandle.IsAllocated)
            {
                oldHandle.Free();
                try
                {
                    oldHandle.Free();
                }
                catch (InvalidOperationException)
                {
                    //Empty
                }
            }
            _handle = GetNewHandle(value, _trackResurrection);
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public bool TryGetTarget(out T target)
        {
            target = default(T);
            if (!_handle.IsAllocated)
            {
                return false;
            }
            else
            {
                try
                {
                    object obj = _handle.Target;
                    if (obj == null)
                    {
                        return false;
                    }
                    else
                    {
                        target = (T)obj;
                        return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private GCHandle GetNewHandle(T value, bool trackResurrection)
        {
            return GCHandle.Alloc(value, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
        }
    }
}

#endif