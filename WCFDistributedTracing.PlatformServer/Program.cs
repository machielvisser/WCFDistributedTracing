using Serilog;
using System;
using System.ServiceModel;
using WCFDistributedTracing.Serilog;
using WCFDistributedTracing.WCF;

namespace WCFDistributedTracing.PlatformServer
{
    class Program
    {
        static void Main(string[] _)
        {
            CreateLogger();
            var host = new TracingEnabledServiceHost(typeof(SimplePlatformService), new Uri(SimplePlatformService.BaseAddress));
            var endPoint = host.AddServiceEndpoint(typeof(ISimplePlatformService), new WSDualHttpBinding(), "");
            endPoint.AddTracingBehavior();
            host.Open();
            Log.Information("Host opened");
            Console.ReadLine();
            Log.CloseAndFlush();
            host.Close();
        }

        private static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                // add the xunit test output sink to the serilog logger
                // https://github.com/trbenning/serilog-sinks-xunit#serilog-sinks-xunit
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj} {Properties} {NewLine}{Exception}")
                .Enrich.WithProcessName()
                .Enrich.With<ContextEnricher>()
                .CreateLogger();
        }
    }
}
