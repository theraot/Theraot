using System;
using System.Threading;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public sealed partial class Work : ICloneable, IPromise
    {
        [ThreadStatic]
        private static Work _current;

        private readonly Action _action;
        private readonly WorkContext _context;
        private readonly bool _exclusive;
        private Exception _error;
        private int _isCompleted;

        //Leaking ManualResetEvent
        private StructNeedle<ManualResetEvent> _waitHandle;

        internal Work(Action action, bool exclusive, WorkContext context)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            else
            {
                _context = context;
                _action = action ?? ActionHelper.GetNoopAction();
                _exclusive = exclusive;
                _waitHandle = new ManualResetEvent(false);
            }
        }

        public static Work Current
        {
            get
            {
                return _current;
            }
        }

        public Exception Error
        {
            get
            {
                return _error;
            }
        }

        public bool IsCanceled
        {
            get
            {
                return false;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Thread.VolatileRead(ref _isCompleted) == 1;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return !ReferenceEquals(_error, null);
            }
        }

        internal bool Exclusive
        {
            get
            {
                return _exclusive;
            }
        }

        public Work Clone()
        {
            return _context.AddWork(_action, _exclusive);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public void Start()
        {
            //Fail on AppDomain Unload
            if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                _context.ScheduleWork(this);
            }
        }

        public void Wait()
        {
            while (Thread.VolatileRead(ref _isCompleted) != 1)
            {
                _context.DoOneWork();
            }
        }

        internal void Execute()
        {
            var oldCurrent = Interlocked.Exchange(ref _current, this);
            try
            {
                _action.Invoke();
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle.Value.Set();
            }
            catch (Exception exception)
            {
                _error = exception;
                Thread.VolatileWrite(ref _isCompleted, 1);
                _waitHandle.Value.Set();
            }
            finally
            {
                Interlocked.Exchange(ref _current, oldCurrent);
            }
        }
    }

    public sealed partial class Work : ICloneable
    {
#if DEBUG || FAT
        private static int _lastId;
        private int _id = Interlocked.Increment(ref _lastId) - 1;

        public int Id
        {
            get
            {
                return _id;
            }
        }

#endif
    }
}