#if FAT

using System;

using Theraot.Core;

namespace Theraot.Factories
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class CustomFactory<TOutput> : IFactory<TOutput>
    {
        private readonly Func<CustomFactory<TOutput>, TOutput> _createCallback;

        public CustomFactory(Func<CustomFactory<TOutput>, TOutput> createCallback)
        {
            _createCallback = Check.NotNullArgument(createCallback, "createCallback");
        }

        public virtual TOutput Create()
        {
            return _createCallback.Invoke(this);
        }
    }
}

#endif