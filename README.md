# WCFDistributedTracing
![Publish NuGet](https://github.com/machielvisser/WCFDistributedTracing/workflows/Publish%20NuGet/badge.svg)

This project shows how distributed tracing in WCF can be used to do structured logging.
DistributedOperationContext is used to pass context information through the distributed execution path.
Serilog is used as the logging framework, with an output to Seq for central analysis.

This will result in the following output in Seq:
![Diagram](https://raw.githubusercontent.com/machielvisser/WCFDistributedTracing/master/Documentation/Seq.PNG)

## How to use

### Seq
Download and install [Seq](https://datalust.co/seq)

### Add the WCFDistributedTracing package
```powershell
Install-Package WCFDistributedTracing
```

### Add the TracingBehavior
```csharp
var host = new TracingEnabledServiceHost(typeof(SimpleEdgeService), new Uri(SimpleEdgeService.BaseAddress));
var endPoint = host.AddServiceEndpoint(typeof(ISimpleEdgeService), new BasicHttpBinding(), "");
endPoint.AddBehavior<InspectorBehavior<TracingInspector>>();
host.Open();

var factory = new DuplexChannelFactory<ISimplePlatformService>(callbackInstance, new WSDualHttpBinding(), new EndpointAddress(SimplePlatformService.BaseAddress));
factory.Endpoint.AddBehavior<InspectorBehavior<TracingInspector>>();
var proxy = factory.CreateChannel(callbackInstance);
```

### Add the Serilog ContextEnricher
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    // add the xunit test output sink to the serilog logger
    // https://github.com/trbenning/serilog-sinks-xunit#serilog-sinks-xunit
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj} {Properties} {NewLine}{Exception}")
    .Enrich.WithProcessName()
    .Enrich.With<WCFTracingEnricher>()
    .CreateLogger();
```

### Initiate new trace (not required)
When there is not a current DistributedOperationContext during a WCF call a new one will be created with a new TraceId.
It is also possible to force a new DistributedOperationContext when it is necessary to run multiple parralel WCF calls with the same TraceId:
```csharp
var channelFactory = new ChannelFactory<ISimpleEdgeService>(new BasicHttpBinding(), new EndpointAddress(SimpleEdgeService.BaseAddress));
channelFactory.Endpoint.AddBehavior<InspectorBehavior<TracingInspector>>();
var proxy = channelFactory.CreateChannel();

// All WCF operations and logging statements will have the same TraceId after this initialization
DistributedOperationContext.Current = new DistributedOperationContext();

var result = await proxy.Echo($"Hello edge service calling you from operation {traceId}");
Log.Information("Received: {Answer}", result.Message);
```


## Details

### Application structure
![Diagram](https://raw.githubusercontent.com/machielvisser/WCFDistributedTracing/master/Documentation/Architecture.png)

### Async pattern
The DistributedOperationContext is scoped using AsyncLocal. That means it flows with async operations downstream, but not upstream if changed downstream.

### Duplex channel
Duplex channels are supported. The interaction between the EdgeServer and the PlatformServer shows this.

### Binding
The implementation works with different types of bindings, BasicHttpBinding, WSDUalHttpBinding and NetTcpBinding have been tested. 
The sample services demonstrate the use of a NetTcpBinding for the EdgeService and a WSDualHttpBinding for the PlatormService.

### Standards
For the trace context we try to follow the [W3C standard](https://www.w3.org/TR/trace-context/) as much as possible.


## ToDo:
* 
