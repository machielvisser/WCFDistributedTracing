using System;
using System.ServiceModel;

namespace UtilsLogging.WCF
{
    public class DistributedOperationContext : IExtension<OperationContext>
    {
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
            get
            {
                var operationContext = OperationContext.Current;
                if (operationContext == null) return null;
                var context = operationContext.Extensions.Find<DistributedOperationContext>();
                if (context != null) return context;
                context = new DistributedOperationContext();
                operationContext.Extensions.Add(context);
                return context;
            }
        }

        public void Attach(OperationContext owner)
        {
            // Method intentionally left empty.
        }
        public void Detach(OperationContext owner)
        {
            // Method intentionally left empty.
        }
    }
}
