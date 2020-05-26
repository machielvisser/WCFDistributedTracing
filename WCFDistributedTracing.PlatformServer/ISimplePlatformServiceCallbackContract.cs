using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFDistributedTracing.PlatformServer
{
    public interface ISimplePlatformServiceCallbackContract
    {
        [OperationContract(IsOneWay = true)]
        Task EchoClient(string message, Guid traceId);
    }
}
