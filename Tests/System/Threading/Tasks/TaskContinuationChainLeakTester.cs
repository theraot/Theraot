using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MonoTests.System.Threading.Tasks
{
    internal class TaskContinuationChainLeakTester
    {
        private volatile bool m_bStop;
        private int counter;
        private ManualResetEvent mre = new ManualResetEvent(false);
        private WeakReference<Task> headTaskWeakRef;

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
            {
                return null;
            }

            if (++counter == 50)
            {
                mre.Set();
            }

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

        private void DummyWorker()
        {
            Thread.Sleep(0);
        }
    }
}