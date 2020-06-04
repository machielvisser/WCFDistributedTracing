using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFDistributedTracing.WCF
{
    public class InspectorBehavior<T>: IEndpointBehavior, IServiceBehavior, IOperationBehavior where T : Inspector, new()
    {
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspector = new T
            {
                ServiceEndpoint = endpoint,
                ClientRuntime = clientRuntime
            };

            clientRuntime.ClientMessageInspectors.AddOrMerge(inspector);
            clientRuntime.MessageInspectors.AddOrMerge(inspector);
            clientRuntime.CallbackDispatchRuntime.MessageInspectors.AddOrMerge(inspector);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            var inspector = new T
            {
                OperationDescription = operationDescription,
                ClientOperation = clientOperation
            };

            clientOperation.ParameterInspectors.AddOrMerge(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var inspector = new T
            {
                ServiceEndpoint = endpoint,
                EndpointDispatcher = endpointDispatcher
            };

            if (endpointDispatcher.ChannelDispatcher == null) return;
            foreach (var ed in endpointDispatcher.ChannelDispatcher.Endpoints)
            {
                ed.DispatchRuntime.MessageInspectors.AddOrMerge(inspector);
                ed.DispatchRuntime.CallbackClientRuntime.MessageInspectors.AddOrMerge(inspector);

                foreach (var operation in ed.DispatchRuntime.Operations)
                    operation.ParameterInspectors.AddOrMerge(inspector);
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var inspector = new T
            {
                ServiceDescription = serviceDescription,
                ServiceHostBase = serviceHostBase
            };

            foreach (ChannelDispatcher channelDispatcherBase in serviceHostBase.ChannelDispatchers)
                foreach (var eDispatcher in channelDispatcherBase.Endpoints)
                {
                    eDispatcher.DispatchRuntime.MessageInspectors.AddOrMerge(inspector);
                    eDispatcher.DispatchRuntime.CallbackClientRuntime.MessageInspectors.AddOrMerge(inspector);

                    foreach (var dispatchOperation in eDispatcher.DispatchRuntime.Operations)
                        dispatchOperation.ParameterInspectors.AddOrMerge(inspector);
                }
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var inspector = new T
            {
                OperationDescription = operationDescription,
                DispatchOperation = dispatchOperation
            };

            dispatchOperation.ParameterInspectors.AddOrMerge(inspector);
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        { }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void Validate(ServiceEndpoint endpoint) { }

        public void Validate(OperationDescription operationDescription) { }
    }
}