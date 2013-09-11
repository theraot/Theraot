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
        private readonly WeakHashBucket<IResource, object, WeakNeedle<IResource>> _readLog;
        private readonly WeakHashBucket<IResource, object, WeakNeedle<IResource>> _writeLog;

        public Transact()
        {
            _writeLog = new WeakHashBucket<IResource, object, WeakNeedle<IResource>>();
            _readLog = new WeakHashBucket<IResource, object, WeakNeedle<IResource>>();
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
                                    if (!resource.Key.Commit())
                                    {
                                        //unexpected
                                        return false;
                                    }
                                }
                                return true;
                            }
                            else
                            {
                                //the resources has been modified by another thread
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
                        //could not adquire the resources
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
                //the resources has been modified by another thread
                return false;
            }
        }

        private bool Check()
        {
            foreach (var resource in _readLog)
            {
                if (!resource.Key.Check())
                {
                    return false;
                }
            }
            return true;
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
