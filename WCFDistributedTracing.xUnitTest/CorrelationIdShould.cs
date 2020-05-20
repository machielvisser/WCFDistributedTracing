using System;
using System.Reactive.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Serilog;
using WCFDistributedTracing.EdgeServer;
using WCFDistributedTracing.Serilog;
using WCFDistributedTracing.WCF;
using Xunit;
using Xunit.Abstractions;

namespace WCFDistributedTracing.Test
{
    public class CorrelationIdShould : IDisposable
    {
        private readonly ChannelFactory<ISimpleEdgeService> _channelFactory;

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

            _channelFactory = new ChannelFactory<ISimpleEdgeService>(new BasicHttpBinding(), new EndpointAddress(SimpleEdgeService.BaseAddress));
            _channelFactory.Endpoint.AddTracingBehavior();
        }

        [Fact]
        public async void TestSingleTrace()
        {
            await ExecuteTrace();
        }

        [Fact]
        public async void TestMultiThreadingTraces()
        {
            await Observable
                .Interval(TimeSpan.FromMilliseconds(100))
                .Take(5)
                .Select(_ => ExecuteTrace())
                .Wait();
        }

        private async Task ExecuteTrace()
        {
            var proxy = _channelFactory.CreateChannel();

            Log.Information("TraceId before OperationScope: {TraceId}", DistributedOperationContext.Current?.TraceId);

            using (var scope = new FlowingOperationContextScope(proxy as IContextChannel))
            {
                var traceId = DistributedOperationContext.Current?.TraceId;

                Log.Information("TraceId beginning of OperationScope: {TraceId}", traceId);

                await Task.Delay(200).ContinueOnScope(scope);

                var result = await proxy.Echo($"Hello edge service calling you from operation {traceId}").ContinueOnScope(scope);
                Log.Information("Received: {Answer}", result);
            }

            Log.Information("TraceId after OperationScope: {TraceId}", DistributedOperationContext.Current?.TraceId);

            (proxy as IClientChannel)?.Close();
        }

        public void Dispose()
        {
            _channelFactory.Close();
            Log.CloseAndFlush();
        }
    }
}
