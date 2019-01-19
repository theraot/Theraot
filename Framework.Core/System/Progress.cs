#if LESSTHAN_NET45

#pragma warning disable CA1003 // Use generic event handler instances

using System.Threading;

namespace System
{
    // This class is new in .NET 4.5
    public class Progress<T> : IProgress<T>
    {
        private readonly Action<T> _post;

        public Progress()
        {
            var context = SynchronizationContext.Current;
            switch (context)
            {
                case null:
                    _post = value => ThreadPool.QueueUserWorkItem(Callback, value);
                    break;

                default:
                    _post = value => context.Post(Callback, value);
                    break;
            }
        }

        public Progress(Action<T> handler)
            : this()
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            ProgressChanged += (_, args) => handler(args);
        }

        public event EventHandlerEx<T> ProgressChanged;

        public void Report(T value)
        {
            OnReport(value);
        }

        protected virtual void OnReport(T value)
        {
            if (ProgressChanged != null)
            {
                _post(value);
            }
        }

        private void Callback(object value)
        {
            var valueT = (T)value;
            var progressChanged = ProgressChanged;
            progressChanged?.Invoke(this, valueT);
        }
    }
}

#endif