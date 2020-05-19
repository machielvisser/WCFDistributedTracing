using System.ServiceModel;
using System.Threading.Tasks;

namespace UtilsLogging.EdgeServer
{
    [ServiceContract]
    public interface ISimpleEdgeService
    {
        [OperationContract]
        Task<string> Echo(string text);
    }
}
