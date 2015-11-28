using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MonoTests.System.Threading.Tasks
{
    internal class TaskContinuationChainLeakTester
    {
        volatile bool m_bStop;
        int counter;
        ManualResetEvent mre = new ManualResetEvent(false);
        WeakReference<Task> headTaskWeakRef;

        public ManualResetEvent TasksPilledUp
        {
            get
            {
                return mre;
            }
        }

        public void Run()
        {
            headTaskWeakRef = new WeakReference<Task>(StartNewTask());
        }

        public Task StartNewTask()
        {
            if (m_bStop)
                return null;

            if (++counter == 50)
                mre.Set();

            return Task.Factory.StartNew(DummyWorker).ContinueWith(task => StartNewTask());
        }

        public void Stop()
        {
            m_bStop = true;
        }

        public void Verify()
        {
            Task task;
            Assert.IsFalse(headTaskWeakRef.TryGetTarget(out task));
        }

        void DummyWorker()
        {
            Thread.Sleep(0);
        }
    }
}