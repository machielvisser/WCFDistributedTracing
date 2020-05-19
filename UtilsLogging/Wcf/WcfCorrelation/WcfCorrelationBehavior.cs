using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace UtilsLogging.Wcf.WcfCorrelation
{
    public class WcfCorrelationBehavior: IEndpointBehavior, IServiceBehavior, IOperationBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // Method intentionally left empty.
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
            // Method intentionally left empty.
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
            // Method intentionally left empty.
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // Method intentionally left empty.
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // Method intentionally left empty.
        }

        public void Validate(OperationDescription operationDescription)
        {
            // Method intentionally left empty.
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if(!clientRuntime.ClientMessageInspectors.OfType<WcfCorrelationInspector>().Any())
                clientRuntime.ClientMessageInspectors.Add(CreateClientMessageInspector());
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            if(!clientOperation.ParameterInspectors.OfType<WcfCorrelationInspector>().Any())
                clientOperation.ParameterInspectors.Add(new WcfCorrelationInspector());

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (channelDispatcher == null) return;
            foreach (var ed in channelDispatcher.Endpoints)
            {
                if (!ed.DispatchRuntime.MessageInspectors.OfType<WcfCorrelationInspector>()
                    .Any())
                    ed.DispatchRuntime.MessageInspectors.Add(CreateDispatchMessageInspector());
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var cDispatcher = (ChannelDispatcher) channelDispatcherBase;
                foreach (var eDispatcher in cDispatcher.Endpoints)
                {
                    if(!eDispatcher.DispatchRuntime.MessageInspectors.OfType<WcfCorrelationInspector>().Any())
                        eDispatcher.DispatchRuntime.MessageInspectors.Add(CreateDispatchMessageInspector());
                }
            }
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(new WcfCorrelationInspector());
        }

        protected virtual IClientMessageInspector CreateClientMessageInspector() => new WcfCorrelationInspector();

        protected virtual IDispatchMessageInspector CreateDispatchMessageInspector() => new WcfCorrelationInspector();
    }
}
