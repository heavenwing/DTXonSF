using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRemotingOrderService : IService
    {
        Task<bool> CreateOrderAsync(OrderDto dto);
    }
}
