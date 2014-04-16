#if FAT

using Theraot.Core;

namespace Theraot.Factories
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class CustomAggregator<TInput, TOutput> : CustomFactory<TOutput>, IAggregator<TInput, TOutput>
    {
        private readonly Aggregate<TInput, TOutput> _craft;

        private TOutput _result;

        public CustomAggregator(Aggregate<TInput, TOutput> craft)
            : this(craft, default(TOutput))
        {
            //Empty
        }

        public CustomAggregator(Aggregate<TInput, TOutput> craft, TOutput seed)
            : base(input => TypeHelper.As<CustomAggregator<TInput, TOutput>>(input)._result)
        {
            _craft = Check.NotNullArgument(craft, "craft");
            _result = seed;
        }

        public virtual void Process(TInput item)
        {
            _result = _craft.Invoke(_result, item);
        }

        public virtual void Reset()
        {
            _result = default(TOutput);
        }
    }
}

#endif