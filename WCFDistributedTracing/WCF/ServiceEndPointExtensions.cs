using System.Linq;
using System.ServiceModel.Description;

namespace WCFDistributedTracing.WCF
{
    public static class ServiceEndPointExtensions
    {
        public static void AddTracingBehavior(this ServiceEndpoint endPoint)
        {
            if (!endPoint.Behaviors.OfType<TracingBehavior>().Any())
                endPoint.Behaviors.Add(new TracingBehavior());

            foreach (var operationDescription in endPoint.Contract.Operations)
            {
                if (operationDescription.Behaviors.OfType<TracingBehavior>().Any())
                    operationDescription.Behaviors.Add(new TracingBehavior());
            }
        }
    }
}
