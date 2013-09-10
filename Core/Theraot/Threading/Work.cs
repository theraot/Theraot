using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading
{
    public sealed class Work : ICloneable
    {
        [ThreadStatic]
        private static Work _current;

        private static int _lastId;
        private readonly Action _action;
        private readonly WorkContext _context;
        private readonly bool _exclusive;
        private int _done;
        private int _id;
        private Exception _resultException;

        internal Work(Action action, bool exclusive, WorkContext context)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            else
            {
                _id = Interlocked.Increment(ref _lastId) - 1;
                _context = context;
                _action = action ?? ActionHelper.GetNoopAction();
                _exclusive = exclusive;
            }
        }

        public static Work Current
        {
            get
            {
                return _current;
            }
        }

        public bool Done
        {
            get
            {
                return Thread.VolatileRead(ref _done) == 1;
            }
        }

        internal bool Exclusive
        {
            get
            {
                return _exclusive;
            }
        }

        public int Id
        {
            get
            {
                return _id;
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
            while (Thread.VolatileRead(ref _done) == 0)
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
            catch (Exception exc)
            {
                _resultException = exc;
            }
            finally
            {
                Thread.VolatileWrite(ref _done, 1);
                Interlocked.Exchange(ref _current, oldCurrent);
            }
        }
    }
}