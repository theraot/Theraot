using System.Threading;

namespace System
{
    public class Progress<T> : IProgress<T>
    {
        private readonly SendOrPostCallback _callback;
        private readonly SynchronizationContext _context;
        private readonly Action<T> _handler;

        public Progress()
        {
            _context = SynchronizationContext.Current;
            //TODO handle the case where SynchronizationContext.Current is null
        }

        public Progress(Action<T> handler)
            : this()
        {
            if (ReferenceEquals(handler, null))
            {
                throw new ArgumentNullException("handler");
            }
            else
            {
                _handler = handler;
                _callback = Callback;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Microsoft's Design")]
        public event Theraot.Core.NewEventHandler<T> ProgressChanged;

        public void Report(T value)
        {
            OnReport(value);
        }

        protected virtual void OnReport(T value)
        {
            if (_callback != null || ProgressChanged != null)
            {
                _context.Post(_callback, value); //TODO: _context may be null
            }
        }

        private void Callback(object value)
        {
            var _value = (T)value;
            if (_handler != null)
            {
                _handler.Invoke(_value);
            }
            var _ProgressChanged = ProgressChanged;
            if (_ProgressChanged != null)
            {
                _ProgressChanged.Invoke(this, _value);
            }
        }
    }
}