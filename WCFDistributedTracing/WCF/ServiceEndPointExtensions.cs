using System.ServiceModel.Description;

namespace WCFDistributedTracing.WCF
{
    public static class ServiceEndPointExtensions
    {
        public static void AddTracingBehavior(this ServiceEndpoint endPoint)
        {
            var tracingBehavior = new TracingBehavior();

            endPoint.Behaviors.AddIfNotExists(tracingBehavior);

            foreach (var operationDescription in endPoint.Contract.Operations)
                operationDescription.Behaviors.Add(tracingBehavior);
        }
    }
}
