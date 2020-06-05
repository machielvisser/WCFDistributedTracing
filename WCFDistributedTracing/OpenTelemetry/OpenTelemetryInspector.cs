using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;
using Serilog;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCFDistributedTracing.OpenTelemetry
{
    public class OpenTelemetryInspector : Inspector
    {
        private readonly Tracer _tracer;
        private readonly ITextFormat _textFormat = new TraceContextFormat();

        public OpenTelemetryInspector()
        {
            _tracer = TracerFactoryBase.Default.GetTracer(nameof(OpenTelemetryInspector));
        }

        public override object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var action = request.Headers.Action;
            var clientOperation = ClientRuntime?.ClientOperations.FirstOrDefault(o => o.Action == action);
            var isOneWay = clientOperation?.IsOneWay ?? OperationDescription?.IsOneWay ?? false;
            var operationName = clientOperation?.Name ?? OperationDescription?.Name ?? string.Empty;
            var serviceName = ClientRuntime?.ContractClientType.Name ?? OperationDescription?.DeclaringContract.Name;
            var serviceNameSpace = ClientRuntime?.ContractClientType.Namespace ?? OperationDescription?.DeclaringContract.Namespace;
            var fullOperationName = $"{serviceNameSpace}.{serviceName}.{operationName}";

             _tracer.StartActiveSpan(fullOperationName, SpanKind.Client, out var span);
            if (span.IsRecording)
            {
                span.PutPeerNameAttribute(channel.RemoteAddress.Uri.Host);
                span.PutPeerPortAttribute(channel.RemoteAddress.Uri.Port);
                span.PutWcfServiceNamespaceAttribute(serviceNameSpace);
                span.PutWcfServiceAttribute(serviceName);
                span.PutWcfOperationAttribute(operationName);
                span.PutWcfIsOneWayAttribute(isOneWay);
            }
            Log.Information("Started (1): {SpanId}", span.Context.SpanId);

            _textFormat.Inject(span.Context, request, (r, k, v) => r.Headers.Add(MessageHeader.CreateHeader(k, string.Empty, v)));

            if (isOneWay)
                EndSpan(span);
            
            return span;
        }

        public override void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var span = correlationState as TelemetrySpan;
            try
            {
                if (span == null || !span.Context.IsValid)
                {
                    Log.Error($"Span is null in {nameof(OpenTelemetryInspector)}");
                    return;
                }

                if (span.IsRecording)
                {
                    if (reply != null)
                        span.Status = reply.IsFault ? Status.Internal : Status.Ok;
                    else
                        span.Status = Status.Unknown;
                }
            }
            finally
            {
                EndSpan(span);
            }
        }

        private void EndSpan(TelemetrySpan span)
        {
            Log.Information("Ended (3): {SpanId}", span.Context.SpanId);
            span?.End();
        }

        public override object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var parentSpan = _textFormat.Extract(request, (r, k) => r.Headers.YieldHeader<string>(k));
            var contractType = OperationDescription?.DeclaringContract.ContractType;
            var serviceNameSpace = contractType?.Namespace;
            var serviceName = contractType?.Name;
            var operationName = OperationDescription?.Name;
            var fullOperationName = $"{serviceNameSpace}.{serviceName}.{operationName}";

            _tracer.StartActiveSpan(fullOperationName, parentSpan, SpanKind.Server, out TelemetrySpan span);
            if (span.IsRecording)
            {
                span.PutNetHostNameAttribute(channel.LocalAddress.Uri.Host);
                span.PutWcfServiceNamespaceAttribute(serviceNameSpace);
                span.PutWcfServiceAttribute(serviceName);
                span.PutWcfOperationAttribute(operationName);
            }
            Log.Information("Started (4): {SpanId}", span.Context.SpanId);

            return span;
        }

        public override void BeforeSendReply(ref Message reply, object correlationState)
        {
            var span = correlationState as TelemetrySpan;
            try
            {
                if (span == null || !span.Context.IsValid)
                {
                    Log.Error($"Span is null in {nameof(OpenTelemetryInspector)}");
                    return;
                }

                if (span.IsRecording)
                {
                    if (reply != null)
                        span.Status = reply.IsFault ? Status.Internal : Status.Ok;
                    else
                        span.Status = Status.Unknown;
                }
            }
            finally
            {
                Log.Information("Ended (5): {SpanId}", span.Context.SpanId);
                span?.End();
            }
        }

        public override object BeforeCall(string operationName, object[] inputs)
        {
            return null;
        }

        public override void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            var span = _tracer.CurrentSpan;

            Log.Information("Ended (6): {SpanId}", span.Context.SpanId);
            span.End();
        }
    }
}
