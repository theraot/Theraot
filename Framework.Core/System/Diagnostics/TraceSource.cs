#if LESSTHAN_NETSTANDARD20

namespace System.Diagnostics
{
    public class TraceSource
    {
        public TraceSource(string name)
        {
            // TODO   
        }

        [Conditional("TRACE")]
        public void TraceData(TraceEventType eventType, int id, object data)
        {
            // TODO
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id)
        {
            // TODO
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id, string message)
        {
            // TODO
        }

        [Conditional("TRACE")]
        public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
        {
            // TODO
        }
    }
}

#endif