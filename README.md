# WCFDistributedTracing

This project shows how distributed tracing in WCF can be used to do structured logging.
DistributedOperationContext is used to pass context information through the distributed execution path.
Serilog is used as the logging framework, with an output to Seq for central analysis.
![Diagram](./Documentation/Seq.PNG)

Works with:
* Async pattern
* 

Application structure:
![Diagram](./Documentation/Architecture.png)

ToDo:
* Duplex binding
* Multithreading unit test
