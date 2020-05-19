using System;
using System.ServiceModel;

namespace UtilsLogging.Wcf.WcfCorrelation
{
    public class WcfCorrelationContext : IExtension<OperationContext>
    {
        public WcfCorrelationContext()
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

        public static WcfCorrelationContext Current
        {
            get
            {
                var operationContext = OperationContext.Current;
                if (operationContext == null) return null;
                var context = operationContext.Extensions.Find<WcfCorrelationContext>();
                if (context != null) return context;
                context = new WcfCorrelationContext();
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
