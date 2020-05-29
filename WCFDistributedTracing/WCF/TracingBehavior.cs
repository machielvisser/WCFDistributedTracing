using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFDistributedTracing.WCF
{
    public class TracingBehavior: IEndpointBehavior, IServiceBehavior, IOperationBehavior
    {
        private readonly TracingInspector _tracingInspector;

        public TracingBehavior()
        {
            _tracingInspector = new TracingInspector();
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.AddIfNotExists(_tracingInspector);
            clientRuntime.MessageInspectors.AddIfNotExists(_tracingInspector);
            clientRuntime.CallbackDispatchRuntime.MessageInspectors.AddIfNotExists(_tracingInspector);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            clientOperation.ParameterInspectors.AddIfNotExists(_tracingInspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpointDispatcher.ChannelDispatcher == null) return;
            foreach (var ed in endpointDispatcher.ChannelDispatcher.Endpoints)
            {
                ed.DispatchRuntime.MessageInspectors.AddIfNotExists(_tracingInspector);
                ed.DispatchRuntime.CallbackClientRuntime.MessageInspectors.AddIfNotExists(_tracingInspector);

                foreach (var operation in ed.DispatchRuntime.Operations)
                    operation.ParameterInspectors.AddIfNotExists(_tracingInspector);
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcherBase in serviceHostBase.ChannelDispatchers)
                foreach (var eDispatcher in channelDispatcherBase.Endpoints)
                {
                    eDispatcher.DispatchRuntime.MessageInspectors.AddIfNotExists(_tracingInspector);
                    eDispatcher.DispatchRuntime.CallbackClientRuntime.MessageInspectors.AddIfNotExists(_tracingInspector);

                    foreach (var dispatchOperation in eDispatcher.DispatchRuntime.Operations)
                        dispatchOperation.ParameterInspectors.AddIfNotExists(_tracingInspector);
                }
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.AddIfNotExists(_tracingInspector);
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