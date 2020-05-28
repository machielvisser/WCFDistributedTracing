using System;
using System.Diagnostics;

namespace WCFDistributedTracing.EdgeServer
{
    public class Answer
    {
        public string Message { get; set; }
        public string TraceId { get; set; }
    }
}
