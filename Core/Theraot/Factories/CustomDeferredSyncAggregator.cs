#if FAT

namespace Theraot.Factories
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class CustomDeferredSyncAggregator<TInput, TOutput> : CustomDeferredAggregator<TInput, TOutput>
    {
        private readonly object _syncroot;

        public CustomDeferredSyncAggregator(Aggregate<TInput, TOutput> craft, TOutput seed)
            : base(craft, seed)
        {
            _syncroot = new object();
        }

        public CustomDeferredSyncAggregator(Aggregate<TInput, TOutput> craft)
            : this(craft, default(TOutput))
        {
            //Empty
        }

        public override TOutput Create()
        {
            lock (_syncroot)
            {
                return base.Create();
            }
        }

        public override void Process(TInput item)
        {
            lock (_syncroot)
            {
                base.Process(item);
            }
        }

        public override void Reset()
        {
            lock (_syncroot)
            {
                base.Reset();
            }
        }
    }
}

#endif