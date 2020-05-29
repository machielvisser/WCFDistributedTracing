using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace WCFDistributedTracing.OpenTelemetry
{
    public class OpenTelemetryInspector : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector
    {

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {

        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {

        }
    }
}
