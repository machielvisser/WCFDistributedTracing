using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;

namespace WCFDistributedTracing.WCF
{
    public static class ExtensionMethods
    {
        public static void AddIfNotExists<T, U>(this ICollection<U> collection, T element) where T : U
        {
            if (!collection.OfType<T>().Any())
                collection.Add(element);
        }

        public static void AddBehavior<T>(this ServiceEndpoint endPoint) where T : IEndpointBehavior, IOperationBehavior, new()
        {
            var tracingBehavior = new T();

            endPoint.Behaviors.AddIfNotExists(tracingBehavior);

            foreach (var operationDescription in endPoint.Contract.Operations)
                operationDescription.Behaviors.AddIfNotExists(tracingBehavior);
        }
    }
}
