# WCFDistributedTracing

This project shows how distributed tracing in WCF can be used to do structured logging.
DistributedOperationContext is used to pass context information through the distributed execution path.
Serilog is used as the logging framework, with an output to Seq for central analysis.

This will result in the following output in Seq:
![Diagram](./Documentation/Seq.PNG)

## Details

### Application structure:
![Diagram](./Documentation/Architecture.png)

### Async pattern
Since .Net 4.6.2 the OperationContext is maintained in Async scenarios. However this is disabled by default for backwards compatibility reasons. To disable disabling this feature add the following to the appSettings:
```
<add key="wcf:disableOperationContextAsyncFlow" value="false" />
```

At the beginning of a trace a new OperationContext is created using an OperationContextScope. The default OperationContextScope cannot handle the async scenario in its Dispose logic. Therefore we included the implementation of [FlowingOperationContextScope]( https://stackoverflow.com/questions/18284998/pattern-for-calling-wcf-service-using-async-await/22753055#22753055) created by [noseratio](https://stackoverflow.com/users/1768303/noseratio)


## ToDo:
* Duplex binding
* Multithreading unit test
