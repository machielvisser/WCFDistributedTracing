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
        public static string BaseAddress = $"http://{Environment.MachineName}:8000/Service";

        public async Task<Answer> Echo(string text)
        {
            Log.Information("Received: {Input}", text);

            await Task.Run(() => Log.Information("Some random async operation"));

            var callbackInstance = new InstanceContext(this);
            var factory = new DuplexChannelFactory<ISimplePlatformService>(callbackInstance, new WSDualHttpBinding(), new EndpointAddress(SimplePlatformService.BaseAddress));
            factory.Endpoint.AddTracingBehavior();
            var proxy = factory.CreateChannel(callbackInstance);

            await proxy.Echo($"Hello {nameof(ISimplePlatformService)} here is a message from the client: {text}");

            return new Answer
            {
                Message = $"Forwarded your message to {nameof(ISimplePlatformService)}",
                TraceId = DistributedOperationContext.Current.TraceId
            };
        }
        public async Task EchoClient(string message, Guid traceId)
        {
            await Task.Run(() => Log.Information($"Received from the {nameof(ISimplePlatformService)}: '{{Message}}' with {{TraceId}}", message, traceId));
        }
    }
}
