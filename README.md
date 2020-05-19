# WCFDistributedTracing

This project shows how distributed tracing in WCF can be used to do structured logging.
DistributedOperationContext is used to pass context information through the distributed execution path.
Serilog is used as the logging framework, with an output to Seq for central analysis.
![Diagram](./Documentation/Seq.PNG)

## Works with:
* Async pattern
* 

## Application structure:
![Diagram](./Documentation/Architecture.png)

## ToDo:
* Duplex binding
* Multithreading unit test

## Thanks to:
* [noseratio](https://stackoverflow.com/users/1768303/noseratio) for his implementation of [FlowingOperationContextScope]( https://stackoverflow.com/questions/18284998/pattern-for-calling-wcf-service-using-async-await/22753055#22753055)
