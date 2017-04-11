#if NET20 || NET30 || NET35 || NET40

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
            if (context == null)
            {
                _post = value =>
                {
                    ThreadPool.QueueUserWorkItem(Callback, value);
                };
            }
            else
            {
                _post = value =>
                {
                    context.Post(Callback, value);
                };
            }
        }

        public Progress(Action<T> handler)
            : this()
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            ProgressChanged += (sender, args) => handler(args);
        }

        public event Theraot.Core.NewEventHandler<T> ProgressChanged;

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
            if (progressChanged != null)
            {
                progressChanged.Invoke(this, valueT);
            }
        }
    }
}

#endif