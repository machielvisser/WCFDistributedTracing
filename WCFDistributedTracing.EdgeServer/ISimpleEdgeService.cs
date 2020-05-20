using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFDistributedTracing.EdgeServer
{
    [ServiceContract]
    public interface ISimpleEdgeService
    {
        [OperationContract]
        Task<string> Echo(string text);
    }
}
