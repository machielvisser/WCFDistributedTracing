using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using WCFDistributedTracing.EdgeServer;
using WCFDistributedTracing.Serilog;
using WCFDistributedTracing.WCF;
using Xunit;
using Xunit.Abstractions;

namespace WCFDistributedTracing.Test
{
    public class TraceIdShouldBeCorrectInSeq : IDisposable
    {
        private readonly ChannelFactory<ISimpleEdgeService> _channelFactory;

        public TraceIdShouldBeCorrectInSeq(ITestOutputHelper testOutputHelper)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                // add the xunit test output sink to the serilog logger
                // https://github.com/trbenning/serilog-sinks-xunit#serilog-sinks-xunit
                .WriteTo.TestOutput(testOutputHelper, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj} {Properties} {NewLine}{Exception}")
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.With<ContextEnricher>()
                .CreateLogger();

            _channelFactory = new ChannelFactory<ISimpleEdgeService>(new BasicHttpBinding(), new EndpointAddress(SimpleEdgeService.BaseAddress));
            _channelFactory.Endpoint.AddTracingBehavior();
        }

        [Fact]
        public async void TestSingleTrace()
        {
            await ExecuteTrace(0, 0);
        }

        [Fact]
        public void TestMultiThreadingTraces()
        {
            Log.Information("Staring TestMultiThreadingTraces");

            var executions = Enumerable
                .Range(0, 5)
                .Select(index => ExecuteTrace(index, 100))
                .ToArray();

            Task.WaitAll(executions);
        }

        private async Task ExecuteTrace(int index, int delay)
        {
            var proxy = _channelFactory.CreateChannel();

            await Task.Delay(index * delay);

            using (var scope = new DistributedOperationContextScope(proxy as IContextChannel))
            {
                var traceId = DistributedOperationContext.Current?.TraceId;

                Log.Information("Beginning of OperationScope: {TraceId} {ThreadId}", traceId, Thread.CurrentThread.ManagedThreadId);

                // This makes the scope overlap with other scopes
                await Task.Delay(delay).ContinueOnScope(scope);

                var result = await proxy.Echo($"Hello edge service calling you from operation {traceId}").ContinueOnScope(scope);
                Log.Information("Received: {Answer}", result);
            }

            (proxy as IClientChannel)?.Close();
        }

        public void Dispose()
        {
            _channelFactory.Close();
            Log.CloseAndFlush();
        }
    }
}
