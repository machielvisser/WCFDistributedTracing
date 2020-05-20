using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFDistributedTracing.PlatformServer
{
    [ServiceContract]
    public interface ISimplePlatformService
    {
        [OperationContract]
        Task<string> Echo(string text);
    }
}
