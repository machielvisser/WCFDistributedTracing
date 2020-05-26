using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using UtilsLogging.WCF;

namespace WCFDistributedTracing.WCF
{
    public class TracingBehavior: IEndpointBehavior, IServiceBehavior, IOperationBehavior
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
            if(!clientRuntime.ClientMessageInspectors.OfType<TracingInspector>().Any())
                clientRuntime.ClientMessageInspectors.Add(new TracingInspector());

            if (!clientRuntime.MessageInspectors.OfType<TracingInspector>().Any())
                clientRuntime.MessageInspectors.Add(new TracingInspector());

            if (!clientRuntime.CallbackDispatchRuntime.MessageInspectors.OfType<TracingInspector>().Any())
                clientRuntime.CallbackDispatchRuntime.MessageInspectors.Add(new TracingInspector());
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            if(!clientOperation.ParameterInspectors.OfType<TracingInspector>().Any())
                clientOperation.ParameterInspectors.Add(new TracingInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (channelDispatcher == null) return;
            foreach (var ed in channelDispatcher.Endpoints)
            {
                if (!ed.DispatchRuntime.MessageInspectors.OfType<TracingInspector>()
                    .Any())
                    ed.DispatchRuntime.MessageInspectors.Add(new TracingInspector());

                if (!ed.DispatchRuntime.CallbackClientRuntime.MessageInspectors.OfType<TracingInspector>().Any())
                    ed.DispatchRuntime.CallbackClientRuntime.MessageInspectors.Add(new TracingInspector());

                foreach (var operation in ed.DispatchRuntime.Operations)
                {
                    if (!operation.ParameterInspectors.OfType<TracingInspector>().Any())
                        operation.ParameterInspectors.Add(new TracingInspector());
                }
            }
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                foreach (var eDispatcher in channelDispatcherBase.Endpoints)
                {
                    if(!eDispatcher.DispatchRuntime.MessageInspectors.OfType<TracingInspector>().Any())
                        eDispatcher.DispatchRuntime.MessageInspectors.Add(new TracingInspector());

                    if (!eDispatcher.DispatchRuntime.CallbackClientRuntime.MessageInspectors.OfType<TracingInspector>().Any())
                        eDispatcher.DispatchRuntime.CallbackClientRuntime.MessageInspectors.Add(new TracingInspector());

                    foreach (var dispatchOperation in eDispatcher.DispatchRuntime.Operations)
                    {
                        if (!dispatchOperation.ParameterInspectors.OfType<TracingInspector>().Any())
                            dispatchOperation.ParameterInspectors.Add(new TracingInspector());
                    }
                }
            }
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            if (!dispatchOperation.ParameterInspectors.OfType<TracingInspector>().Any())
                dispatchOperation.ParameterInspectors.Add(new TracingInspector());
        }
    }
}
