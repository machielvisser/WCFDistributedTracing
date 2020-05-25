using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace WCFDistributedTracing.WCF
{
    public class DistributedOperationContext
    {
        private static readonly AsyncLocal<DistributedOperationContext> _current = new AsyncLocal<DistributedOperationContext>();
        private static readonly PropertyInfo[] _properties;
        public Guid TraceId { get; set; }

        static DistributedOperationContext()
        {
            _properties = typeof(DistributedOperationContext).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

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
