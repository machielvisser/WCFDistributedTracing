﻿using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Trace;
using Serilog;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace WCFDistributedTracing.OpenTelemetry
{
    public class OpenTelemetryInspector : IDispatchMessageInspector, IClientMessageInspector, IParameterInspector
    {
        private readonly Tracer _tracer;
        private readonly ITextFormat _textFormat = new TraceContextFormat();

        public OpenTelemetryInspector()
        {
            _tracer = TracerFactoryBase.Default.GetTracer(nameof(OpenTelemetryInspector));
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var context = OperationContext.Current;

            var remoteAddress = channel.RemoteAddress;

            _tracer.StartActiveSpan(request.Headers.Action, SpanKind.Client, out var span);
            if (span.IsRecording)
            {
                span.PutComponentAttribute("grpc");
                span.SetAttribute("rpc.service", channel.RemoteAddress.Uri.LocalPath);
                span.SetAttribute("net.peer.name", channel.RemoteAddress.Uri.Host);
                span.SetAttribute("net.peer.port", channel.RemoteAddress.Uri.Port);
            }

            _textFormat.Inject(span.Context, request, (r, k, v) => r.Headers.Add(MessageHeader.CreateHeader(k, string.Empty, v)));

            return span;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            var context = OperationContext.Current;
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
                    span.Status = reply.IsFault ? Status.Internal : Status.Ok;
                }
            }
            finally
            {
                span?.End();
            }
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
