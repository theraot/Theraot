using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MonoTests.System.Threading.Tasks
{
    internal class TaskContinuationChainLeakTester
    {
        private volatile bool _stop;
        private int _counter;
        private WeakReference<Task> _headTaskWeakRef;

        public ManualResetEvent TasksPilledUp { get; } = new ManualResetEvent(false);

        public void Run()
        {
            _headTaskWeakRef = new WeakReference<Task>(StartNewTask());
        }

        public Task StartNewTask()
        {
            if (_stop)
            {
                return null;
            }

            if (++_counter == 50)
            {
                TasksPilledUp.Set();
            }

            return Task.Factory.StartNew(DummyWorker).ContinueWith(task => StartNewTask());
        }

        public void Stop()
        {
            _stop = true;
        }

        public void Verify()
        {
            Assert.IsFalse(_headTaskWeakRef.TryGetTarget(out _));
        }

        private void DummyWorker()
        {
            Thread.Sleep(0);
        }
    }
}