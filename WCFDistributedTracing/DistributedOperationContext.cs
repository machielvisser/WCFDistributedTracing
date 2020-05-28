using System;
using System.Linq;
using System.Threading;

namespace WCFDistributedTracing
{
    public class DistributedOperationContext
    {
        private static readonly AsyncLocal<DistributedOperationContext> _current = new AsyncLocal<DistributedOperationContext>();

        public string TraceId { get; set; }
        public string SpanId { get; set; }

        public DistributedOperationContext()
        {
            TraceId = NewTraceId();
            SpanId = NewSpanId();
        }

        public static DistributedOperationContext Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }

        public string NewTraceId() => Guid.NewGuid().ToString("N");
        public string NewSpanId() => Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
