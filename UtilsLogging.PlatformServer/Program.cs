using Serilog;
using System;
using System.ServiceModel;
using UtilsLogging.Serilog;
using UtilsLogging.WCF;

namespace UtilsLogging.PlatformServer
{
    class Program
    {
        static void Main(string[] _)
        {
            CreateLogger();
            var host = new ServiceHost(typeof(SimplePlatformService), new Uri(SimplePlatformService.BaseAddress));
            var endPoint = host.AddServiceEndpoint(typeof(ISimplePlatformService), new BasicHttpBinding(), "");
            endPoint.AddTracingBehavior();
            host.Open();
            Log.Information("Host opened");
            Console.ReadLine();
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
