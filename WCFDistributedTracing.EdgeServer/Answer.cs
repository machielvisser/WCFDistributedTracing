using System;

namespace WCFDistributedTracing.EdgeServer
{
    public class Answer
    {
        public string Message { get; set; }
        public Guid TraceId { get; set; }
    }
}
