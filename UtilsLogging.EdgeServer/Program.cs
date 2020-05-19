using System;
using System.ServiceModel;
using Serilog;
using UtilsLogging.Wcf;
using UtilsLogging.Wcf.WcfCorrelation;

namespace UtilsLogging.EdgeServer
{
    class Program
    {
        static void Main(string[] _)
        {
            CreateLogger();
            var host = new System.ServiceModel.ServiceHost(typeof(SimpleEdgeService), new Uri(SimpleEdgeService.BaseAddress));
            var endPoint = host.AddServiceEndpoint(typeof(ISimpleEdgeService), new BasicHttpBinding(), "");
            endPoint.AddWcfCorrelationBehavior();
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
                .Enrich.With<WcfCorrelationEnricher>()
                .CreateLogger();
        }
    }
}
