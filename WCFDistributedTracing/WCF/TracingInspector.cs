using System.ServiceModel;
using System.ServiceModel.Channels;
using Serilog;

namespace WCFDistributedTracing.WCF
{
    public class TracingInspector : Inspector
    {
        public string ContextHeader { get; } = "TraceContext";

        public override object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var context = DistributedOperationContext.Current ?? new DistributedOperationContext();

            var header = new MessageHeader<DistributedOperationContext>(context);
            var untypedHeader = header.GetUntypedHeader(ContextHeader, string.Empty);

            request.Headers.Add(untypedHeader);

            return context;
        }

        public override object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var context = request.Headers.GetHeader<DistributedOperationContext>(ContextHeader, string.Empty);

            DistributedOperationContext.Current = context;

            return context;
        }

        public override void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply != null && correlationState is DistributedOperationContext context)
            {
                var header = new MessageHeader<DistributedOperationContext>(context)
                    .GetUntypedHeader(ContextHeader, string.Empty);

                reply.Headers.Add(header);
            }
        }

        public override void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (correlationState is DistributedOperationContext context)
                DistributedOperationContext.Current = context;
        }

        public override object BeforeCall(string operationName, object[] inputs)
        {
            if (OperationContext.Current == null)
            {
                Log.Information(
                    "Calling Operation {OperationName} with inputs {OperationInputs}",
                    operationName,
                    inputs);
            }
            else
            {
                var endpoint = OperationContext.Current.Channel.LocalAddress.Uri;

                Log.Information(
                    "Operation {OperationName} on {Endpoint} called with inputs {OperationInputs}",
                    operationName,
                    endpoint,
                    inputs);
            }

            return DistributedOperationContext.Current;
        }

        public override void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            if (correlationState is DistributedOperationContext context)
                DistributedOperationContext.Current = context;

            if (OperationContext.Current == null)
            {
                Log.Information(
                    "Operation {OperationName} returend {ReturnValue}",
                    operationName,
                    returnValue?.ToString());
            }
            else
            {
                var endpoint = OperationContext.Current.Channel.LocalAddress.Uri;

                Log.Information(
                    "Operation {OperationName} on {Endpoint} returned {ReturnValue}",
                    operationName,
                    endpoint,
                    returnValue?.ToString());
            }
        }
    }
}
