using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Serilog;

namespace UtilsLogging.Wcf.WcfCorrelation
{
    public class WcfCorrelationInspector : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector
    {
        public string HeaderCorrelationId { get; set; }

        public string CorrelationId { get; } = "CorrelationId";

        public virtual object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var correlationId = WcfCorrelationContext.Current?.TraceId;
            HeaderCorrelationId = string.IsNullOrWhiteSpace(correlationId) || !Guid.TryParse(correlationId, out _)
                ? Guid.NewGuid()

                    .ToString()
                : correlationId;
                
            var messageHeader = new MessageHeader<string>(HeaderCorrelationId);
            var untypedMessageHeader = messageHeader.GetUntypedHeader(CorrelationId, string.Empty);
            request.Headers.Add(untypedMessageHeader);
            return HeaderCorrelationId;
        }

        public virtual object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var correlationId = request.Headers.FindHeader(CorrelationId, string.Empty)  >= 0 ? request.Headers.GetHeader<string>(CorrelationId, string.Empty) : string.Empty;

            IDictionary<string, string> messageHeaders =
                !string.IsNullOrWhiteSpace(correlationId) ?
                    new Dictionary<string, string>
                    {
                        { CorrelationId, correlationId }
                    } :
                    new Dictionary<string, string>();

            var context = WcfCorrelationContext.Current;
            if(context != null)
                context.TraceId = correlationId;
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
