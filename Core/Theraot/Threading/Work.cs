#if FAT

using System;
using System.Threading;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    public sealed partial class Work : ICloneable, IPromise, ICloneable<Work>
    {
        private const int INT_StatusCompleted = 2;
        private const int INT_StatusNew = 0;
        private const int INT_StatusRunning = 1;

        [ThreadStatic]
        private static Work _current;

        private readonly Action _action;
        private readonly WorkContext _context;
        private readonly bool _exclusive;
        private Exception _error;
        private int _status = INT_StatusNew;

        private StructNeedle<ManualResetEventSlim> _waitHandle;

        internal Work(Action action, bool exclusive, WorkContext context)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            _context = context;
            _action = action ?? ActionHelper.GetNoopAction();
            _exclusive = exclusive;
            _waitHandle = new ManualResetEventSlim(false);
        }

        ~Work()
        {
            var waitHandle = _waitHandle.Value;
            if (!ReferenceEquals(waitHandle, null))
            {
                waitHandle.Dispose();
            }
            _waitHandle.Value = null;
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
                return Thread.VolatileRead(ref _status) == INT_StatusCompleted;
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

        public bool Start()
        {
            var check = Interlocked.Exchange(ref _status, INT_StatusRunning);
            if (check != INT_StatusRunning && !GCMonitor.FinalizingForUnload)
            {
                _error = null;
                _context.ScheduleWork(this);
                return true;
            }
            return false;
        }

        public bool StartAndWait()
        {
            if (Start())
            {
                Wait();
                return true;
            }
            return false;
        }

        public void Wait()
        {
            while (Thread.VolatileRead(ref _status) != INT_StatusCompleted)
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
            }
            catch (Exception exception)
            {
                _error = exception;
            }
            finally
            {
                Thread.VolatileWrite(ref _status, INT_StatusCompleted);
                _waitHandle.Value.Set();
                Interlocked.Exchange(ref _current, oldCurrent);
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public sealed partial class Work
    {
#if DEBUG || FAT
        private static int _lastId;
        private readonly int _id = Interlocked.Increment(ref _lastId) - 1;

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

#endif