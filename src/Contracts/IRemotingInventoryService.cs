using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRemotingInventoryService:IService
    {
        Task<bool> RemoveStockAsync(int productId, int quantity);
    }
}
