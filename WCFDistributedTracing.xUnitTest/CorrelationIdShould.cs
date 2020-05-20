using System.ServiceModel;
using Serilog;
using UtilsLogging.WCF;
using WCFDistributedTracing.EdgeServer;
using WCFDistributedTracing.Serilog;
using WCFDistributedTracing.WCF;
using Xunit;
using Xunit.Abstractions;

namespace WCFDistributedTracing.Test
{
    public class CorrelationIdShould
    {
        public CorrelationIdShould(ITestOutputHelper testOutputHelper)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                // add the xunit test output sink to the serilog logger
                // https://github.com/trbenning/serilog-sinks-xunit#serilog-sinks-xunit
                .WriteTo.TestOutput(testOutputHelper, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj} {Properties} {NewLine}{Exception}")
                .Enrich.WithProcessName()
                .Enrich.With<ContextEnricher>()
                .CreateLogger();
        }

        [Fact]
        public async void Test()
        {            
            var factory = new ChannelFactory<ISimpleEdgeService>(new BasicHttpBinding(), new EndpointAddress(SimpleEdgeService.BaseAddress));
            factory.Endpoint.AddTracingBehavior();
            var proxy = factory.CreateChannel();

            Log.Information("TraceId before OperationScope: {TraceId}", DistributedOperationContext.Current?.TraceId);

            using (var scope = new FlowingOperationContextScope(proxy as IContextChannel))
            {
                Log.Information("TraceId beginning of OperationScope: {TraceId}", DistributedOperationContext.Current?.TraceId);

                var result = await proxy.Echo("Hello edge service").ContinueOnScope(scope);
                Log.Information("Received: {Answer}", result);
            }

            Log.Information("TraceId after OperationScope: {TraceId}", DistributedOperationContext.Current?.TraceId);

            (proxy as IClientChannel)?.Close();
            factory.Close();
            Log.CloseAndFlush();
        }
    }
}
