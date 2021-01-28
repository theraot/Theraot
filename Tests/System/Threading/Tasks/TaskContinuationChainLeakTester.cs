#pragma warning disable RCS1210 // Return completed task instead of returning null

using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MonoTests.System.Threading.Tasks
{
    internal class TaskContinuationChainLeakTester
    {
        private int _counter;
        private WeakReference<Task> _headTaskWeakRef;
        private volatile bool _stop;
        public ManualResetEvent TasksPiledUp { get; } = new ManualResetEvent(false);

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
                TasksPiledUp.Set();
            }

            return Task.Factory.StartNew(DummyWorker).ContinueWith(_ => StartNewTask());
        }

        public void Stop()
        {
            _stop = true;
        }

        public void Verify()
        {
            Assert.IsFalse(_headTaskWeakRef.TryGetTarget(out _));
        }

        private static void DummyWorker()
        {
            Thread.Sleep(0);
        }
    }
}