using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCFDistributedTracing
{
    public abstract class Inspector : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector
    {
        public ClientOperation ClientOperation;
        public ClientRuntime ClientRuntime;
        public DispatchOperation DispatchOperation;
        public EndpointDispatcher EndpointDispatcher;
        public OperationDescription OperationDescription;
        public ServiceDescription ServiceDescription;
        public ServiceEndpoint ServiceEndpoint;
        public ServiceHostBase ServiceHostBase;


        // IClientMessageInspector
        public abstract object BeforeSendRequest(ref Message request, IClientChannel channel);
        public abstract void AfterReceiveReply(ref Message reply, object correlationState);

        // IDispatchMessageInspector
        public abstract object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext);
        public abstract void BeforeSendReply(ref Message reply, object correlationState);

        // IParameterInspector
        public abstract void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState);
        public abstract object BeforeCall(string operationName, object[] inputs);
    }
}
