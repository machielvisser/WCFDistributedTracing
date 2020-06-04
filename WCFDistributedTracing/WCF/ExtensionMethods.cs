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

        public static void AddOrMerge<T, U>(this ICollection<U> collection, T element) where T : Inspector, U
        {
            var existing = collection.OfType<T>().FirstOrDefault();
            if (existing == null)
                collection.Add(element);
            else
            {
                existing.ClientOperation = existing.ClientOperation ?? element.ClientOperation;
                existing.ClientRuntime = existing.ClientRuntime ?? element.ClientRuntime;
                existing.DispatchOperation = existing.DispatchOperation ?? element.DispatchOperation;
                existing.EndpointDispatcher = existing.EndpointDispatcher ?? element.EndpointDispatcher;
                existing.OperationDescription = existing.OperationDescription ?? element.OperationDescription;
                existing.ServiceDescription = existing.ServiceDescription ?? element.ServiceDescription;
                existing.ServiceEndpoint = existing.ServiceEndpoint ?? element.ServiceEndpoint;
                existing.ServiceHostBase = existing.ServiceHostBase ?? element.ServiceHostBase;
            }
        }
    }
}
