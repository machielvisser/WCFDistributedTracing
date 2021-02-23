using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Serilog;
using WCFDistributedTracing.PlatformServer;
using WCFDistributedTracing.WCF;

namespace WCFDistributedTracing.EdgeServer
{
    public class SimpleEdgeService : ISimpleEdgeService, ISimplePlatformServiceCallbackContract
    {
        public static readonly string BaseAddress = $"net.tcp://{Environment.MachineName}:8000/Service";

        public async Task<Answer> Echo(string text, TimeSpan? delay = default)
        {
            Log.Information("Received: {Input}", text);

            await Task.Run(() => Log.Information("Some random async operation"));

            await Task.Delay(delay ?? TimeSpan.Zero);

            var callbackInstance = new InstanceContext(this);
            var factory = new DuplexChannelFactory<ISimplePlatformService>(callbackInstance, new WSDualHttpBinding(), new EndpointAddress(SimplePlatformService.BaseAddress));
            factory.Endpoint.AddBehavior<InspectorBehavior<TracingInspector>>();
            var proxy = factory.CreateChannel(callbackInstance);

            await proxy.Echo($"Hello {nameof(ISimplePlatformService)} here is a message from the client: {text}");
            
            return new Answer
            {
                Message = $"Forwarded your message to {nameof(ISimplePlatformService)}",
                TraceId = DistributedOperationContext.Current.TraceId
            };
        }

        public async Task EchoClient(string message, string traceId)
        {
            await Task.Run(() => Log.Information($"Received from the {nameof(ISimplePlatformService)}: '{{Message}}' with {{TraceId}}", message, traceId));
        }
    }
}
