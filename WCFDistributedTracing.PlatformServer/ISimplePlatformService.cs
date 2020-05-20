using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFDistributedTracing.PlatformServer
{
    [ServiceContract(CallbackContract = typeof(ISimplePlatformServiceCallbackContract))]
    public interface ISimplePlatformService
    {
        [OperationContract(IsOneWay = true)]
        Task Echo(string text);
    }
}
