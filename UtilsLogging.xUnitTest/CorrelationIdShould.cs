using System.ServiceModel;
using Serilog;
using UtilsLogging.EdgeServer;
using UtilsLogging.Wcf;
using UtilsLogging.Wcf.WcfCorrelation;
using Xunit;
using Xunit.Abstractions;

namespace UtilsLogging.xUnitTest
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
                .Enrich.With<WcfCorrelationEnricher>()
                .CreateLogger();
        }

        [Fact]
        public async void Test()
        {            
            var factory = new ChannelFactory<ISimpleEdgeService>(new BasicHttpBinding(), new EndpointAddress(SimpleEdgeService.BaseAddress));
            factory.Endpoint.AddWcfCorrelationBehavior();
            var proxy = factory.CreateChannel();

            Log.Information("CorrelationId before OperationScope: {CorrelationId}", WcfCorrelationContext.Current?.TraceId);

            using (var scope = new FlowingOperationContextScope(proxy as IContextChannel))
            {
                Log.Information("CorrelationId beginning of OperationScope: {CorrelationId}", WcfCorrelationContext.Current?.TraceId);

                var result = await proxy.Echo("Hello edge service").ContinueOnScope(scope);
                Log.Information("Received: {Answer}", result);
            }

            Log.Information("CorrelationId after OperationScope: {CorrelationId}", WcfCorrelationContext.Current?.TraceId);

            (proxy as IClientChannel)?.Close();
            factory.Close();
            Log.CloseAndFlush();
        }
    }
}
