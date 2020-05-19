using System.Linq;
using System.ServiceModel.Description;

namespace UtilsLogging.WCF
{
    public static class ServiceEndPointExtensions
    {
        public static void AddTracingBehavior(this ServiceEndpoint endPoint)
        {
            if (!endPoint.Behaviors.OfType<CorrelationBehavior>().Any())
            {
                endPoint.Behaviors.Add(new CorrelationBehavior());
            }

            foreach (var operationDescription in endPoint.Contract.Operations)
            {
                if (operationDescription.Behaviors.OfType<CorrelationBehavior>()
                    .Any())
                {
                    operationDescription.Behaviors.Add(new CorrelationBehavior());
                }
            }
        }
    }
}
