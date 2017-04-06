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
            if (ReferenceEquals(handler, null))
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
            var _value = (T)value;
            var _progressChanged = ProgressChanged;
            if (_progressChanged != null)
            {
                _progressChanged.Invoke(this, _value);
            }
        }
    }
}

#endif