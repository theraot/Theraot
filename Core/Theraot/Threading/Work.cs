using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
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
        private PromiseNeedle _result;
        private IPromised _promised;

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
                _result = new PromiseNeedle(out _promised, false);
            }
        }

        public static Work Current
        {
            get
            {
                return _current;
            }
        }

        public bool IsReady
        {
            get
            {
                return _result.IsReady;
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
            _context.ScheduleWork(this);
        }

        public void Wait()
        {
            while (!_result.IsReady)
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
                _promised.OnCompleted();
            }
            catch (Exception exception)
            {
                _promised.OnError(exception);
            }
            finally
            {
                Interlocked.Exchange(ref _current, oldCurrent);
            }
        }

        public Exception Error
        {
            get
            {
                return _result.Error;
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