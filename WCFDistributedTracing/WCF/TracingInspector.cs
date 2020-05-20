using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Serilog;
using WCFDistributedTracing.WCF;

namespace UtilsLogging.WCF
{
    public class TracingInspector : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector
    {
        public string TraceId { get; set; }

        public string TraceIdHeader { get; } = "TraceId";

        public virtual object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var correlationId = DistributedOperationContext.Current?.TraceId;
            TraceId = string.IsNullOrWhiteSpace(correlationId) || !Guid.TryParse(correlationId, out _)
                ? Guid.NewGuid()

                    .ToString()
                : correlationId;

            var messageHeader = new MessageHeader<string>(TraceId);
            var untypedMessageHeader = messageHeader.GetUntypedHeader(TraceIdHeader, string.Empty);
            request.Headers.Add(untypedMessageHeader);
            return TraceId;
        }

        public virtual object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var traceId = request.Headers.FindHeader(TraceIdHeader, string.Empty) >= 0 ? request.Headers.GetHeader<string>(TraceIdHeader, string.Empty) : string.Empty;

            IDictionary<string, string> messageHeaders =
                !string.IsNullOrWhiteSpace(traceId) ?
                    new Dictionary<string, string>
                    {
                        [TraceIdHeader] = traceId
                    } :
                    new Dictionary<string, string>();

            var context = DistributedOperationContext.Current;
            if (context != null)
                context.TraceId = traceId;
            return messageHeaders;
        }

        public virtual void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (correlationState is IDictionary<string, string> messageContext && messageContext.Count > 0)
            {
                foreach (var key in messageContext.Keys)
                {
                    var messageHeader = new MessageHeader<string>(messageContext[key]);
                    var untypedMessageHeader = messageHeader.GetUntypedHeader(key, string.Empty);
                    reply.Headers.Add(untypedMessageHeader);
                }
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