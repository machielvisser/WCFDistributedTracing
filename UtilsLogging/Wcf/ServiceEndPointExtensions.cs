using System.Linq;
using System.ServiceModel.Description;
using UtilsLogging.Wcf.WcfCorrelation;

namespace UtilsLogging.Wcf
{
    public static class ServiceEndPointExtensions
    {
        public static void AddWcfCorrelationBehavior(this ServiceEndpoint endPoint)
        {
            if (!endPoint.Behaviors.OfType<WcfCorrelationBehavior>().Any())
            {
                endPoint.Behaviors.Add(new WcfCorrelationBehavior());
            }

            foreach (var operationDescription in endPoint.Contract.Operations)
            {
                if (operationDescription.Behaviors.OfType<WcfCorrelationBehavior>()
                    .Any())
                {
                    operationDescription.Behaviors.Add(new WcfCorrelationBehavior());
                }
            }
        }
    }
}
