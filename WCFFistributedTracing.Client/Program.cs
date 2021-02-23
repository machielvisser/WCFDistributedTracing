using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using Serilog;
using WCFDistributedTracing.Serilog;
using WCFDistributedTracing.WCF;

namespace WCFDistributedTracing.EdgeServer
{
    internal static class Program
    {
        private static async Task Main(string[] _)
        {
            CreateLogger();
            using (var channelFactory = new ChannelFactory<ISimpleEdgeService>(new NetTcpBinding(),
                new EndpointAddress(SimpleEdgeService.BaseAddress)))
            {
                channelFactory.Endpoint.AddBehavior<InspectorBehavior<TracingInspector>>();
                var channel = channelFactory.CreateChannel();
                Log.Information("Host opened");
                var index = 1;
                ConsoleKey? keyPressed = null;
                while (keyPressed != ConsoleKey.Enter && keyPressed != ConsoleKey.Q && keyPressed != ConsoleKey.X)
                {
                    var delay = TimeSpan.FromSeconds(ConsoleKey.D0 <= keyPressed && ConsoleKey.D9 >= keyPressed ? 0 : 5);
                    Log.Information("call delayed for {TimeSpan} seconds", delay);
                    var result = await channel.Echo(Process.GetCurrentProcess()
                        .Id +" echo "+ index, delay);
                    Log.Information("{Result}", result);
                    keyPressed = Console.ReadKey().Key;
                    index++;
                }
            }

            Log.CloseAndFlush();
        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
                // .WriteTo.Seq("http://localhost:5341")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj} {Properties} {NewLine}{Exception}")
                .Enrich.WithProcessName()
                .Enrich.With<WCFTracingEnricher>()
                .CreateLogger();
        }
    }
}
