using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Serilog;
using WCFDistributedTracing.WCF;

namespace UtilsLogging.WCF
{
    public class TracingInspector : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector
    {
        public string ContextHeader { get; } = "TraceContext";

        public virtual object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var context = DistributedOperationContext.Current;

            var header = new MessageHeader<DistributedOperationContext>(context)
                .GetUntypedHeader(ContextHeader, string.Empty);

            request.Headers.Add(header);

            return context;
        }

        public virtual object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var context = request.Headers.GetHeader<DistributedOperationContext>(ContextHeader, string.Empty);

            DistributedOperationContext.Current = context;

            return context;
        }

        public virtual void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply != null && correlationState is DistributedOperationContext context)
            {
                var header = new MessageHeader<DistributedOperationContext>(context)
                    .GetUntypedHeader(ContextHeader, string.Empty);

                reply.Headers.Add(header);
            }
        }

        public virtual void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            Log.Information(
                "Wcf Operation {WcfOperationName} called with operation inputs {WcfOperationInputs}.",
                operationName,
                inputs
            );
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            Log.Information(
                "Wcf Operation {WcfOperationName} called with return value {WcfOperationResult}.",
                operationName,
                returnValue.ToString()
            );
        }
    }
}
