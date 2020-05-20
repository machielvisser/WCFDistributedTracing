using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Serilog;
using WCFDistributedTracing.PlatformServer;
using WCFDistributedTracing.WCF;

namespace WCFDistributedTracing.EdgeServer
{
    public class SimpleEdgeService : ISimpleEdgeService
    {
        public static string BaseAddress = $"http://{Environment.MachineName}:8000/Service";

        public async Task<string> Echo(string text)
        {
            Log.Information("Received: {Input}", text);

            await Task.Run(() => Log.Information("Async operation"));

            var factory = new ChannelFactory<ISimplePlatformService>(new BasicHttpBinding(), new EndpointAddress(SimplePlatformService.BaseAddress));
            factory.Endpoint.AddTracingBehavior();
            var proxy = factory.CreateChannel();

            var result = await proxy.Echo("Hello dependency service");
            Log.Information("Received: {Answer}", result);

            return $"The answer of the dependency was {result}";
        }
    }
}
