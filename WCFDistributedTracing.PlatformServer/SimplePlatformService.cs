using Serilog;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFDistributedTracing.PlatformServer
{
    public class SimplePlatformService : ISimplePlatformService
    {
        public static string BaseAddress = $"http://{Environment.MachineName}:8001/Service";

        public async Task Echo(string text)
        {
            Log.Information("Received: {Input}", text);

            await Task.Run(() => Log.Information("Some random async operation"));

            await Callback.EchoClient("Using the duplex channel to let you know I received your message!");
        }

        ISimplePlatformServiceCallbackContract Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<ISimplePlatformServiceCallbackContract>();
            }
        }
    }
}
