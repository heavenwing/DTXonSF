using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRemotingInventoryService : IService
    {
        Task<bool> RemoveStockAsync(OrderDto dto);
        //Task<bool> AddStockAsync(int productId, int quantity);
        Task<bool> CompensateStockAsync(OrderDto dto);
    }
}
