using System;

namespace WCFDistributedTracing.WCF
{
    public class OperationContextModel
    {
        public string TraceId { get; set; }

        public OperationContextModel()
        {
            TraceId = Guid
                .NewGuid()
                .ToString();
        }
    }
}
