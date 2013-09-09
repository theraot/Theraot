#if FAT

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        [ThreadStatic]
        private static Transact _currentTransaction;

        private readonly Transact _parentTransaction;
        private readonly SetBucket<IResource> _readLog;
        private readonly SetBucket<IResource> _writeLog;

        public Transact()
        {
            _writeLog = new SetBucket<IResource>();
            _readLog = new SetBucket<IResource>();
            _parentTransaction = _currentTransaction;
            _currentTransaction = this;
        }

        internal static Transact CurrentTransaction
        {
            get
            {
                return _currentTransaction;
            }
        }

        public static Transact Create()
        {
            return new Transact();
        }

        public bool Commit()
        {
            if (Check())
            {
                Needles.Needle<Thread> thread = null;
                if (_writeLog.Count > 0)
                {
                    foreach (var resource in _writeLog)
                    {
                        resource.Capture(ref thread);
                    }
                    foreach (var resource in _readLog)
                    {
                        resource.Capture(ref thread);
                    }
                    //Should not be null
                    thread = thread.Simplify();
                    if
                    (
                        ReferenceEquals
                        (
                            thread.CompareExchange
                            (
                                new StructNeedle<Thread>(Thread.CurrentThread),
                                null
                            ),
                            null
                        )
                    )
                    {
                        try
                        {
                            if (Check())
                            {
                                foreach (var resource in _writeLog)
                                {
                                    resource.Commit();
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        finally
                        {
                            thread.Value = null;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool Check()
        {
            bool check;
            foreach (var resource in _readLog)
            {
                if (!resource.Check())
                {
                    check = false;
                }
            }
            check = true;
            return check;
        }

        private void Rollback()
        {
            if (!ReferenceEquals(_currentTransaction, null))
            {
                foreach (var resource in _readLog)
                {
                    resource.Rollback();
                }
                foreach (var resource in _writeLog)
                {
                    resource.Rollback();
                }
                _currentTransaction = _currentTransaction._parentTransaction;
                _currentTransaction.Dispose();
                //GC.Collect();
            }
        }
    }
}

#endif
