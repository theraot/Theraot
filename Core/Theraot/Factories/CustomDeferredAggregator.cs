#if FAT

using System.Collections.Generic;

namespace Theraot.Factories
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class CustomDeferredAggregator<TInput, TOutput> : CustomAggregator<TInput, TOutput>
    {
        private readonly Queue<TInput> _data;

        public CustomDeferredAggregator(Aggregate<TInput, TOutput> craft, TOutput seed)
            : base(craft, seed)
        {
            _data = new Queue<TInput>();
        }

        public CustomDeferredAggregator(Aggregate<TInput, TOutput> craft)
            : this(craft, default(TOutput))
        {
            //Empty
        }

        public override TOutput Create()
        {
            while (_data.Count > 0)
            {
                var item = _data.Dequeue();
                base.Process(item);
            }
            return base.Create();
        }

        public override void Process(TInput item)
        {
            _data.Enqueue(item);
        }
    }
}

#endif