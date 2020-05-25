using System;
using System.Threading;

namespace WCFDistributedTracing.WCF
{
    public class DistributedOperationContext
    {
        private static readonly AsyncLocal<DistributedOperationContext> _current = new AsyncLocal<DistributedOperationContext>();

        public DistributedOperationContext()
        {
            ResetCorrelationId();
        }

        public string TraceId { get; set; }

        public void ResetCorrelationId()
        {
            TraceId = Guid
                .NewGuid()
                .ToString();
        }

        public static DistributedOperationContext Current
        {
            get => _current.Value;
        }
    }
}
