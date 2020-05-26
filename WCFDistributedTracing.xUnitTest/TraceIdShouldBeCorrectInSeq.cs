using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public class TraceIdShouldBeCorrectInSeq : IDisposable
    {
        private readonly ChannelFactory<ISimpleEdgeService> _channelFactory;
        private readonly List<Process> _services;

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

            var currectDirectory = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar);
            var solutionPath = string.Join(Path.DirectorySeparatorChar.ToString(), currectDirectory.Take(currectDirectory.Length - 4).ToArray());
            var buildPath = string.Join(Path.DirectorySeparatorChar.ToString(), currectDirectory.Skip(currectDirectory.Length - 3).ToArray());
            _services = new List<Process>
            {
                StartService($"{solutionPath}/WCFDistributedTracing.EdgeServer/{buildPath}/WCFDistributedTracing.EdgeServer.exe"),
                StartService($"{solutionPath}/WCFDistributedTracing.PlatformServer/{buildPath}/WCFDistributedTracing.PlatformServer.exe")
            };

            Log.Information(Directory.GetCurrentDirectory());
        }

        private Process StartService(string app)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = app,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            return Process.Start(processStartInfo);
        }

        [Fact]
        // Check that all logging has the same TraceId
        public async void TestSingleTrace()
        {
            await ExecuteAsyncTrace(0, 0);
        }

        [Fact]
        // Check that the call was send with an initialized context
        public async void TestAutoContext()
        {
            var proxy = _channelFactory.CreateChannel();

            Assert.Null(DistributedOperationContext.Current);

            var result = await proxy.Echo($"Hello edge service");
            Log.Information("Received: {Answer}", result);

            Assert.NotEqual(Guid.Empty, result.TraceId);

            (proxy as IClientChannel)?.Close();
        }

        [Fact]
        // Check the correctness of the TraceIds manually
        public void TestMultiThreadingTraces()
        {
            Log.Information("Staring TestMultiThreadingTraces");

            var executions = Enumerable
                .Range(0, 4) // More than 4 results in problems with Reliable Messaging default 4 channels -> can be increased with a custom binding
                .Select(index => ExecuteAsyncTrace(index, 100))
                .ToArray();

            Task.WaitAll(executions);
        }

        private async Task ExecuteAsyncTrace(int index, int delay)
        {
            var proxy = _channelFactory.CreateChannel();

            await Task.Delay(index * delay);

            Assert.Null(DistributedOperationContext.Current);

            // Initialize new ConteDistributedOperationContext
            DistributedOperationContext.Current = new DistributedOperationContext();

            var traceId = DistributedOperationContext.Current?.TraceId;

            // This makes the scope overlap with other scopes in time
            await Task.Delay(delay);

            Assert.Equal(traceId, DistributedOperationContext.Current.TraceId);

            var result = await proxy.Echo($"Hello edge service calling you from operation {traceId}");
            Log.Information("Received: {Answer}", result.Message);

            Assert.Equal(traceId, result.TraceId);

            (proxy as IClientChannel)?.Close();
        }

        public void Dispose()
        {
            _channelFactory.Close();

            foreach (var service in _services)
            {
                service.StandardInput.WriteLine();
                service.WaitForExit();
                service.Close();
            }

            Log.CloseAndFlush();
        }
    }
}
