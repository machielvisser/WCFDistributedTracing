using Serilog;
using System;
using System.Threading.Tasks;

namespace WCFDistributedTracing.PlatformServer
{
    public class SimplePlatformService : ISimplePlatformService
    {
        public static string BaseAddress = $"http://{Environment.MachineName}:8001/Service";

        public async Task<string> Echo(string text)
        {
            Log.Information("Received: {Input}", text);

            var task = Task.Run(() => Log.Information("Async operation"));

            Task.Delay(500).Wait();

            await task;

            return "Letting you know I received your message!";
        }
    }
}
