﻿using System;
using System.Threading;

namespace WCFDistributedTracing.WCF
{
    public class DistributedOperationContext
    {
        private static readonly AsyncLocal<DistributedOperationContext> _current = new AsyncLocal<DistributedOperationContext>();

        public Guid TraceId { get; set; }

        public DistributedOperationContext()
        {
            TraceId = Guid.NewGuid();
        }

        public static DistributedOperationContext Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }
}
