using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Serilog;

namespace WCFDistributedTracing.WCF
{
    public class TracingInspector : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector
    {
        public string ContextHeader { get; } = "TraceContext";

        public virtual object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (DistributedOperationContext.Current == null)
            {
                DistributedOperationContext.Current = new DistributedOperationContext();
            }

            var context = DistributedOperationContext.Current;

            var header = new MessageHeader<DistributedOperationContext>(context);
            var untypedHeader = header.GetUntypedHeader(ContextHeader, string.Empty);

            request.Headers.Add(untypedHeader);

            return context;
        }

        public virtual object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var context = request.Headers.FindHeader(ContextHeader, string.Empty) == -1 ?
                DistributedOperationContext.Current ?? new DistributedOperationContext() :
                request.Headers.GetHeader<DistributedOperationContext>(ContextHeader, string.Empty);

            DistributedOperationContext.Current = context;

            return context;
        }

        public virtual void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply == null || !(correlationState is DistributedOperationContext context)) return;
            var header = new MessageHeader<DistributedOperationContext>(context)
                .GetUntypedHeader(ContextHeader, string.Empty);

            reply.Headers.Add(header);
        }

        public virtual void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (correlationState is DistributedOperationContext context)
                DistributedOperationContext.Current = context;
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            var endpoint = OperationContext.Current?.Channel.LocalAddress.Uri;

            Log.Verbose(
                "Operation {OperationName} on {Endpoint} called with inputs {OperationInputs} for {TraceId}",
                operationName,
                endpoint,
                inputs,
                DistributedOperationContext.Current?.TraceId);
        
            return DistributedOperationContext.Current;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            if (correlationState is DistributedOperationContext context)
                DistributedOperationContext.Current = context;

            var endpoint = OperationContext.Current?.Channel.LocalAddress.Uri;

            Log.Verbose(
                "Operation {OperationName} on {Endpoint} returned {ReturnValue} for {TraceId}",
                operationName,
                endpoint,
                returnValue?.ToString(),
                DistributedOperationContext.Current?.TraceId);
        }
    }
}
