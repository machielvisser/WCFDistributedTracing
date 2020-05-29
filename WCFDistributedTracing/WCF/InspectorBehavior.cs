using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFDistributedTracing.WCF
{
    public class InspectorBehavior<T>: IEndpointBehavior, IServiceBehavior, IOperationBehavior where T : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector, new()
    {
        private readonly T _inspector;

        public InspectorBehavior()
        {
            _inspector = new T();
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.AddIfNotExists(_inspector);
            clientRuntime.MessageInspectors.AddIfNotExists(_inspector);
            clientRuntime.CallbackDispatchRuntime.MessageInspectors.AddIfNotExists(_inspector);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            clientOperation.ParameterInspectors.AddIfNotExists(_inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpointDispatcher.ChannelDispatcher == null) return;
            foreach (var ed in endpointDispatcher.ChannelDispatcher.Endpoints)
            {
                ed.DispatchRuntime.MessageInspectors.AddIfNotExists(_inspector);
                ed.DispatchRuntime.CallbackClientRuntime.MessageInspectors.AddIfNotExists(_inspector);

                foreach (var operation in ed.DispatchRuntime.Operations)
                    operation.ParameterInspectors.AddIfNotExists(_inspector);
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcherBase in serviceHostBase.ChannelDispatchers)
                foreach (var eDispatcher in channelDispatcherBase.Endpoints)
                {
                    eDispatcher.DispatchRuntime.MessageInspectors.AddIfNotExists(_inspector);
                    eDispatcher.DispatchRuntime.CallbackClientRuntime.MessageInspectors.AddIfNotExists(_inspector);

                    foreach (var dispatchOperation in eDispatcher.DispatchRuntime.Operations)
                        dispatchOperation.ParameterInspectors.AddIfNotExists(_inspector);
                }
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.AddIfNotExists(_inspector);
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