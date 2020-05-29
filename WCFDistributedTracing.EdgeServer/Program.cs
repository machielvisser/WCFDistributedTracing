using System;
using System.ServiceModel;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using Serilog;
using WCFDistributedTracing.OpenTelemetry;
using WCFDistributedTracing.Serilog;
using WCFDistributedTracing.WCF;

namespace WCFDistributedTracing.EdgeServer
{
    class Program
    {
        static void Main(string[] _)
        {
            CreateLogger();

            TracerFactoryBase.SetDefault(TracerFactory.Create(builder =>
            {
                builder
                    .UseJaeger(c =>
                    {
                        c.AgentHost = "localhost";
                        c.AgentPort = 6831;
                    });
            }));

            var host = new TracingEnabledServiceHost(typeof(SimpleEdgeService), new Uri(SimpleEdgeService.BaseAddress));
            var endPoint = host.AddServiceEndpoint(typeof(ISimpleEdgeService), new NetTcpBinding(), "");
            endPoint.AddBehavior<InspectorBehavior<TracingInspector>>();
            endPoint.AddBehavior<InspectorBehavior<OpenTelemetryInspector>>();
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
                .Enrich.With<WCFTracingEnricher>()
                .CreateLogger();
        }
    }
}
