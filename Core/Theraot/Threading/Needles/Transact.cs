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
        private readonly HashBucket<IResource, object> _readLog;
        private readonly HashBucket<IResource, object> _writeLog;

        public Transact()
        {
            _writeLog = new HashBucket<IResource, object>();
            _readLog = new HashBucket<IResource, object>();
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
                        resource.Key.Capture(ref thread);
                    }
                    foreach (var resource in _readLog)
                    {
                        resource.Key.Capture(ref thread);
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
                                    resource.Key.Commit();
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
                if (!resource.Key.Check())
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
                _readLog.Clear();
                _writeLog.Clear();
                Transact parentTransaction = _currentTransaction._parentTransaction;
                _currentTransaction.Dispose();
                _currentTransaction = parentTransaction;
            }
        }
    }
}

#endif
